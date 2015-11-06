using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using WindowStay.Model;

namespace WindowStay.Controller
{
    public class SettingsController
    {
        public void UpdateSettings(Settings settings)
        {
            XDocument doc = new XDocument();

            //Serialize settings obj to XML
            using (XmlWriter xmlWriter = doc.CreateWriter())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                serializer.Serialize(xmlWriter, settings);
            }

            if (doc.Root != null)
            {
                //Overwrite settings with new values
                XmlController.Instance.DeleteAllSettings();
                doc.Root.Elements().ToList().ForEach(XmlController.Instance.AddSettingsElement);
            }
        }
    }
}