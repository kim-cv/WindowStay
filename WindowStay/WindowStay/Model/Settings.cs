using System;
using System.Xml.Serialization;

namespace WindowStay.Model
{
    [Serializable, XmlRoot("settings")]
    public class Settings
    {
        [System.Xml.Serialization.XmlElement("runatstartup")]
        public bool RunAtStartup { get; set; }
    }
}
