// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.FileAcl
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
    public class FileAcl : ResourceAcl
    {
        private FileInfo m_fi;
        private NativeObjectSecurity m_objectSecurity;

        public FileAcl()
        {
        }

        public FileAcl(string file, string rootPath)
        {
            if (!LongPathFile.Exists(file))
                throw new FileNotFoundException("Specified file cannot be found", file);
            Initialize(new FileInfo(file), rootPath);
        }

        public FileAcl(FileInfo fi, string rootPath)
        {
            if (fi == null)
                throw new ArgumentNullException(nameof(fi));
            Initialize(fi, rootPath);
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
                    if (Nos != null)
                    {
                        var fileSecurity = new FileSecurity();
                        fileSecurity.SetSecurityDescriptorBinaryForm(Nos.GetSecurityDescriptorBinaryForm());
                        m_objectSecurity = fileSecurity;
                    }

                return m_objectSecurity;
            }
        }

        public override string TypeString => "File";

        protected override string ComputeExplicitDACL()
        {
            var accessControl = m_fi.GetAccessControl(AccessControlSections.All);
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
            var str = (string) null;
            if (DACLProtected || count > 0)
            {
                str = accessControl.GetSecurityDescriptorSddlForm(AccessControlSections.Access);
                if (!string.IsNullOrEmpty(str))
                    str = regexStripDacl.Replace(str, string.Empty);
            }

            return SddlNormalizer.FixAceSddl(str);
        }

        private void Initialize(FileInfo fi, string rootPath)
        {
            if (fi == null)
                throw new ArgumentNullException(nameof(fi));
            m_fi = fi;
            Initialize(fi.GetAccessControl(AccessControlSections.All),
                System.IO.Path.Combine("\\", fi.FullName.Remove(0, rootPath.Length))
                    .ToUpper(CultureInfo.InvariantCulture), fi.FullName, null, null);
        }
    }
}