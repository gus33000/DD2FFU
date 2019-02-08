using System.Xml.Serialization;

namespace DD2FFU
{
    [XmlRoot(ElementName = "OEMDevicePlatform",
        Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
    public class OEMDevicePlatform
    {
        [XmlElement(ElementName = "MinSectorCount",
            Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public string MinSectorCount { get; set; }

        [XmlElement(ElementName = "DevicePlatformID",
            Namespace = "http://schemas.microsoft.com/embedded/2004/10/ImageUpdate")]
        public string DevicePlatformID { get; set; }

        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}