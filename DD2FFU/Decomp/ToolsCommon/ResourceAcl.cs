// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.ImageUpdate.Tools.Common.ResourceAcl
// Assembly: ToolsCommon, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: 8A4E8FCA-4522-42C3-A670-4E93952F2307
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\ToolsCommon.dll

using System;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common
{
    public abstract class ResourceAcl
    {
        protected static readonly Regex regexExtractMIL =
            new("(?<MIL>\\(ML[^\\)]*\\))", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        protected static readonly Regex regexStripDacl =
            new("^D:", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        protected static readonly Regex regexStripDriveLetter =
            new("^[A-Z]:", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly HashAlgorithm Sha256Algorithm = HashAlgorithm.Create("SHA256");
        private string m_attributeHash;
        private string m_elementId;
        private string m_explicitDacl;
        private string m_owner;
        private string m_path;
        private string m_protected;

        protected AuthorizationRuleCollection AccessRules
        {
            get; private set;
        }

        protected NativeObjectSecurity Nos
        {
            get; private set;
        }

        public abstract string TypeString
        {
            get;
        }

        protected string MacLabel
        {
            get; set;
        }

        protected string FullPath { get; private set; } = string.Empty;

        [XmlAttribute("DACL")]
        public string ExplicitDACL
        {
            get => m_explicitDacl;
            set
            {
                if (m_explicitDacl != null)
                {
                    return;
                }

                m_explicitDacl = value;
            }
        }

        [XmlAttribute("SACL")]
        public abstract string MandatoryIntegrityLabel
        {
            get; set;
        }

        [XmlAttribute("Owner")]
        public string Owner
        {
            get => m_owner;
            set
            {
                if (m_owner != null)
                {
                    return;
                }

                m_owner = value;
            }
        }

        [XmlAttribute]
        public string ElementID
        {
            get => m_elementId;
            set
            {
                if (m_elementId != null)
                {
                    return;
                }

                m_elementId = value;
            }
        }

        [XmlAttribute]
        public virtual string AttributeHash
        {
            get => m_attributeHash;
            set
            {
                if (m_attributeHash != null)
                {
                    return;
                }

                m_attributeHash = value;
            }
        }

        [XmlAttribute]
        public string Path
        {
            get => m_path;
            set
            {
                if (m_path != null)
                {
                    return;
                }

                m_path = value;
            }
        }

        [XmlIgnore]
        public string Protected
        {
            get => m_protected;
            set
            {
                if (m_protected != null)
                {
                    return;
                }

                m_protected = value;
            }
        }

        public string DACL
        {
            get; private set;
        }

        public bool DACLProtected
        {
            get; private set;
        }

        public string FullACL
        {
            get; private set;
        }

        public bool PreserveInheritance
        {
            get; private set;
        }

        public bool IsEmpty
        {
            get; private set;
        }

        public static ResourceAclComparer Comparer { get; } = new ResourceAclComparer();

        public abstract NativeObjectSecurity ObjectSecurity
        {
            get;
        }

        protected void Initialize(NativeObjectSecurity nos, string path, string fullPath, string explicitDacl,
            string owner)
        {
            Nos = nos;
            FullPath = fullPath;
            m_path = path;
            m_protected = StrProtected(Nos);
            m_elementId = StrElementId(m_path, TypeString);
            m_owner = owner ?? StrOwner(Nos);
            DACL = StrDacl(Nos);
            AccessRules = RuleCollection(Nos);
            DACLProtected = IsDaclProtected(Nos);
            FullACL = StrFullAcl(Nos);
            PreserveInheritance = IsInheritancePreserved(Nos);
            m_explicitDacl = explicitDacl ?? ComputeExplicitDACL();
            IsEmpty = IsDaclEmpty(DACLProtected, MandatoryIntegrityLabel, m_explicitDacl);
            m_attributeHash = StrAttributeHash(TypeString, m_path, m_protected, m_owner, m_explicitDacl,
                MandatoryIntegrityLabel);
        }

        private string GetSha256Hash(byte[] buffer)
        {
            return BitConverter.ToString(Sha256Algorithm.ComputeHash(buffer)).Replace("-", string.Empty);
        }

        private string StrProtected(NativeObjectSecurity nos)
        {
            string str = "No";
            if (nos != null)
            {
                str = !nos.AreAccessRulesProtected ? "No" : "Yes";
            }

            return str;
        }

        private string StrOwner(NativeObjectSecurity nos)
        {
            string str = null;
            if (nos != null)
            {
                str = SddlNormalizer.FixOwnerSddl(
                                nos.GetSecurityDescriptorSddlForm(AccessControlSections.Owner | AccessControlSections.Group));
            }

            return str;
        }

        private string StrDacl(NativeObjectSecurity nos)
        {
            string str = string.Empty;
            if (nos != null)
            {
                str = nos.GetSecurityDescriptorSddlForm(AccessControlSections.Access);
                if (!string.IsNullOrEmpty(str))
                {
                    str = regexStripDacl.Replace(str, string.Empty);
                }
            }

            return SddlNormalizer.FixAceSddl(str);
        }

        private string StrElementId(string path, string typeString)
        {
            string str = null;
            if (!string.IsNullOrEmpty(path))
            {
                StringBuilder stringBuilder = new();
                _ = stringBuilder.Append(typeString);
                _ = stringBuilder.Append(path.ToUpper(new CultureInfo("en-US", false)));
                str = GetSha256Hash(Encoding.Unicode.GetBytes(stringBuilder.ToString()));
            }

            return str;
        }

        private AuthorizationRuleCollection RuleCollection(NativeObjectSecurity nos)
        {
            AuthorizationRuleCollection authorizationRuleCollection = null;
            if (nos != null)
            {
                authorizationRuleCollection = nos.GetAccessRules(true, false, typeof(NTAccount));
            }

            return authorizationRuleCollection;
        }

        private bool IsDaclProtected(NativeObjectSecurity nos)
        {
            return nos != null && nos.AreAccessRulesProtected;
        }

        private string StrFullAcl(NativeObjectSecurity nos)
        {
            string str = string.Empty;
            if (nos != null)
            {
                str = nos.GetSecurityDescriptorSddlForm(AccessControlSections.All);
            }

            return str;
        }

        private bool IsInheritancePreserved(NativeObjectSecurity nos)
        {
            return nos != null && nos.GetAccessRules(false, true, typeof(NTAccount)).Count > 0;
        }

        private bool IsDaclEmpty(bool isDaclProtected, string mandatoryIntegrityLabel, string explicitDacl)
        {
            return string.IsNullOrEmpty(explicitDacl) && string.IsNullOrEmpty(mandatoryIntegrityLabel) && !isDaclProtected;
        }

        private string StrAttributeHash(string strType, string strPath, string strProtected, string strOwner,
            string strExplicitDacl, string strMandatoryIntegrityLabel)
        {
            StringBuilder stringBuilder = new();
            _ = stringBuilder.Append(strType);
            if (!string.IsNullOrEmpty(strPath))
            {
                _ = stringBuilder.Append(strPath.ToUpper(new CultureInfo("en-US", false)));
            }

            if (!string.IsNullOrEmpty(strProtected))
            {
                _ = stringBuilder.Append(strProtected);
            }

            if (!string.IsNullOrEmpty(strOwner))
            {
                _ = stringBuilder.Append(strOwner);
            }

            if (!string.IsNullOrEmpty(strExplicitDacl))
            {
                _ = stringBuilder.Append(strExplicitDacl);
            }

            if (!string.IsNullOrEmpty(strMandatoryIntegrityLabel))
            {
                _ = stringBuilder.Append(strMandatoryIntegrityLabel);
            }

            return GetSha256Hash(Encoding.Unicode.GetBytes(stringBuilder.ToString()));
        }

        protected abstract string ComputeExplicitDACL();
    }
}