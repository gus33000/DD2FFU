// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.DirectoryAcl
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public class DirectoryAcl : ResourceAcl
    {
        private DirectoryInfo m_di;
        private bool m_isRoot;
        private NativeObjectSecurity m_objectSecurity;

        public DirectoryAcl()
        {
        }

        public DirectoryAcl(string path, string owner, string explicitDacl)
        {
            Initialize(null, path, null, explicitDacl, owner);
        }

        public DirectoryAcl(string directory, string rootPath)
        {
            if (!LongPathDirectory.Exists(directory))
                throw new DirectoryNotFoundException(string.Format("Folder {0} cannot be found", directory));
            Initialize(new DirectoryInfo(directory), rootPath);
        }

        public DirectoryAcl(DirectoryInfo di, string rootPath)
        {
            if (di == null)
                throw new ArgumentNullException(nameof(di));
            Initialize(di, rootPath);
        }

        [XmlAttribute("SACL")]
        public override string MandatoryIntegrityLabel
        {
            get
            {
                if (Nos != null)
                {
                    MacLabel = SecurityUtils.GetFileSystemMandatoryLevel(FullPath);
                    if (string.IsNullOrEmpty(MacLabel))
                        MacLabel = null;
                    else
                        MacLabel = SddlNormalizer.FixAceSddl(MacLabel);
                }

                return MacLabel;
            }
            set => MacLabel = value;
        }

        public override NativeObjectSecurity ObjectSecurity
        {
            get
            {
                if (m_objectSecurity == null)
                {
                    var directorySecurity = (DirectorySecurity) null;
                    if (Nos != null)
                    {
                        directorySecurity = new DirectorySecurity();
                        directorySecurity.SetSecurityDescriptorBinaryForm(Nos.GetSecurityDescriptorBinaryForm());
                    }

                    m_objectSecurity = directorySecurity;
                }

                return m_objectSecurity;
            }
        }

        public override string TypeString => "Directory";

        protected override string ComputeExplicitDACL()
        {
            var str = (string) null;
            if (m_isRoot)
            {
                str = Nos.GetSecurityDescriptorSddlForm(AccessControlSections.Access);
            }
            else
            {
                var accessControl = m_di.GetAccessControl(AccessControlSections.All);
                var accessRules = accessControl.GetAccessRules(true, false, typeof(NTAccount));
                var count = accessRules.Count;
                foreach (FileSystemAccessRule rule in accessRules)
                    if (rule.IsInherited)
                    {
                        accessControl.RemoveAccessRule(rule);
                        --count;
                    }

                if (DACLProtected && accessControl.AreAccessRulesCanonical)
                    accessControl.SetAccessRuleProtection(true, PreserveInheritance);
                if (DACLProtected || count > 0)
                    str = accessControl.GetSecurityDescriptorSddlForm(AccessControlSections.Access);
            }

            if (!string.IsNullOrEmpty(str))
                str = regexStripDacl.Replace(str, string.Empty);
            return SddlNormalizer.FixAceSddl(str);
        }

        private void Initialize(DirectoryInfo di, string rootPath)
        {
            if (di == null)
                throw new ArgumentNullException(nameof(di));
            m_di = di;
            m_isRoot = string.Equals(di.FullName, rootPath, StringComparison.OrdinalIgnoreCase);
            var upper = System.IO.Path.Combine("\\", di.FullName.Remove(0, rootPath.Length))
                .ToUpper(CultureInfo.InvariantCulture);
            Initialize(di.GetAccessControl(AccessControlSections.All), upper, di.FullName, null, null);
        }
    }
}