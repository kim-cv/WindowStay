using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using WindowStay.Model;

namespace WindowStay.Controller
{
    public class XmlController
    {
        //Singleton
        private static XmlController _instance = new XmlController();
        public static XmlController Instance
        {
            get
            {
                return _instance;
            }
        }

        private string _appDataPath;
        private string _appFolder = "WindowStay";
        private string _fullAppDataPath;
        private string _dataFileName = "data.dat";
        private string _fullAppDataPathWithFilename;

        private XmlController()
        {
            _appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _fullAppDataPath = Path.Combine(_appDataPath, _appFolder);
            _fullAppDataPathWithFilename = Path.Combine(_fullAppDataPath, _dataFileName);

            if (!DoesAppFolderExist())
            {
                CreateAppFolder();
            }

            if (!DoesDataFileExist())
            {
                CreateDataFile();
            }
        }

        private bool DoesAppFolderExist()
        {
            if (_fullAppDataPath != null)
            {
                return Directory.Exists(_fullAppDataPath);
            }

            throw new NullReferenceException();
        }

        private void CreateAppFolder()
        {
            if (_fullAppDataPath != null)
            {
                Directory.CreateDirectory(_fullAppDataPath);
            }
        }

        private bool DoesDataFileExist()
        {
            return File.Exists(_fullAppDataPathWithFilename);
        }

        private void CreateDataFile()
        {
            try
            {
                XDocument doc = new XDocument(
                         new XDeclaration("1.0", "utf-8", "yes"),
                         new XElement("WindowStay",
                             new XElement("settings"),
                             new XElement("windows")
                             ));

                SaveDocument(doc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public XDocument LoadDocument()
        {
            return XDocument.Load(_fullAppDataPathWithFilename);
        }
        public void SaveDocument(XDocument doc)
        {
            doc.Save(_fullAppDataPathWithFilename);
        }

        #region Setting Elements
        public void AddSettingsElement(XElement element)
        {
            XDocument doc = LoadDocument();
            XElement windowsElement = doc.Element("WindowStay").Element("settings");

            windowsElement?.Add(element);

            SaveDocument(doc);
        }
        public void DeleteAllSettings()
        {
            XDocument doc = LoadDocument();
            XElement windowsElement = doc.Element("WindowStay").Element("settings");

            windowsElement.Elements().Remove();

            SaveDocument(doc);
        }

        public Settings GetSettings()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            XDocument doc = LoadDocument();
            XElement settingsElement = doc.Element("WindowStay").Element("settings");
            
            try
            {
                return (Settings)serializer.Deserialize(settingsElement.CreateReader());
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }
        #endregion

        #region Window Elements
        public void AddWindowElement(XElement element)
        {
            XDocument doc = LoadDocument();
            XElement windowsElement = doc.Element("WindowStay").Element("windows");

            windowsElement?.Add(element);

            SaveDocument(doc);
        }

        public void UpdateWindowElementOnAttributeNameAndValue(string attributeName, string attributeValue, XElement el)
        {
            XDocument doc = LoadDocument();
            XElement windowsElement = doc.Element("WindowStay").Element("windows");
            XElement windowElement = windowsElement.Elements("window").First(element => element.Attribute(attributeName).Value == attributeValue);

            windowElement = el;

            SaveDocument(doc);
        }

        public bool ExistsWindowElementOnAttributeNameAndValue(string attributeName, string attributeValue)
        {
            XDocument doc = LoadDocument();
            return doc.Element("WindowStay").Element("windows").Elements("window").Any(element => element.Attribute(attributeName).Value == attributeValue);
        }

        public void DeleteWindowElementOnAttributeNameAndValue(string attributeName, string attributeValue)
        {
            if (ExistsWindowElementOnAttributeNameAndValue(attributeName, attributeValue))
            {
                XDocument doc = LoadDocument();
                doc.Element("WindowStay").Element("windows").Elements("window").First(element => element.Attribute(attributeName).Value == attributeValue).Remove();
                SaveDocument(doc);
            }
        }

        public List<XElement> GetAllWindowElements()
        {
            XDocument doc = LoadDocument();
            return doc.Element("WindowStay").Element("windows").Elements("window").ToList();
        }
        #endregion
    }
}