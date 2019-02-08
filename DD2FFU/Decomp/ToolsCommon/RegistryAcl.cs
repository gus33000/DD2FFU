// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.RegistryAcl
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public class RegistryAcl : ResourceAcl
    {
        private NativeObjectSecurity m_objectSecurity;
        private readonly RegistrySecurity m_registrySecurity;

        public RegistryAcl()
        {
        }

        public RegistryAcl(ORRegistryKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            m_registrySecurity = key.RegistrySecurity;
            Initialize(key.RegistrySecurity, key.FullName, key.FullName, null, null);
        }

        [XmlAttribute("SACL")]
        public override string MandatoryIntegrityLabel
        {
            get
            {
                if (Nos != null)
                {
                    MacLabel = null;
                    var stringSd = SecurityUtils.ConvertSDToStringSD(Nos.GetSecurityDescriptorBinaryForm(),
                        SecurityInformationFlags.SACL_SECURITY_INFORMATION |
                        SecurityInformationFlags.MANDATORY_ACCESS_LABEL);
                    if (!string.IsNullOrEmpty(stringSd))
                    {
                        var match = regexExtractMIL.Match(stringSd);
                        if (match.Success)
                        {
                            var group = match.Groups["MIL"];
                            if (group != null)
                                MacLabel = SddlNormalizer.FixAceSddl(group.Value);
                        }
                    }
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
                        var registrySecurity = new RegistrySecurity();
                        registrySecurity.SetSecurityDescriptorBinaryForm(Nos.GetSecurityDescriptorBinaryForm());
                        m_objectSecurity = registrySecurity;
                    }

                return m_objectSecurity;
            }
        }

        public override string TypeString => "RegKey";

        protected override string ComputeExplicitDACL()
        {
            var accessRules = m_registrySecurity.GetAccessRules(true, false, typeof(NTAccount));
            var count = accessRules.Count;
            foreach (RegistryAccessRule rule in accessRules)
                if (rule.IsInherited)
                {
                    m_registrySecurity.RemoveAccessRule(rule);
                    --count;
                }

            if (DACLProtected && m_registrySecurity.AreAccessRulesCanonical)
                m_registrySecurity.SetAccessRuleProtection(true, PreserveInheritance);
            var str = (string) null;
            if (DACLProtected || count > 0)
            {
                str = m_registrySecurity.GetSecurityDescriptorSddlForm(AccessControlSections.Access);
                if (!string.IsNullOrEmpty(str))
                    str = regexStripDacl.Replace(str, string.Empty);
            }

            return SddlNormalizer.FixAceSddl(str);
        }
    }
}