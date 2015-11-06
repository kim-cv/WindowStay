using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WindowStay.Interfaces;
using WindowStay.Model;

namespace WindowStay.Controller
{
    public class WindowListController : ISubject
    {
        // A Collection to keep track of all Registered Observers
        private readonly List<IObserver> _observers = new List<IObserver>();
        private readonly List<GetWindow> _windows = new List<GetWindow>();

        public WindowListController()
        {
            LoadAllWindows();
        }

        private void AddWindowElement(GetWindow window)
        {
            XElement el = new XElement("window");
            el.Add(new XAttribute("title", window.WindowTitle));
            el.Add(new XAttribute("top", window.WindowRect.Top));
            el.Add(new XAttribute("right", window.WindowRect.Right));
            el.Add(new XAttribute("bottom", window.WindowRect.Bottom));
            el.Add(new XAttribute("left", window.WindowRect.Left));

            XmlController.Instance.AddWindowElement(el);
        }

        private void UpdateWindowElement(GetWindow window)
        {
            XElement el = new XElement("window");
            el.Add(new XAttribute("title", window.WindowTitle));
            el.Add(new XAttribute("top", window.WindowRect.Top));
            el.Add(new XAttribute("right", window.WindowRect.Right));
            el.Add(new XAttribute("bottom", window.WindowRect.Bottom));
            el.Add(new XAttribute("left", window.WindowRect.Left));

            XmlController.Instance.UpdateWindowElementOnAttributeNameAndValue("title", window.WindowTitle, el);
        }

        private bool DoesWindowElementExist(string windowTitle)
        {
            return XmlController.Instance.ExistsWindowElementOnAttributeNameAndValue("title", windowTitle);
        }

        private void DeleteElement(string windowTitle)
        {
            XmlController.Instance.DeleteWindowElementOnAttributeNameAndValue("title", windowTitle);
        }

        public void LoadAllWindows()
        {
            _windows.Clear();

            foreach (XElement element in XmlController.Instance.GetAllWindowElements())
            {
                string title = element.Attribute("title").Value;
                InvokeStructs.RECT rect = new InvokeStructs.RECT
                {
                    Top = Convert.ToInt16(element.Attribute("top").Value),
                    Right = Convert.ToInt16(element.Attribute("right").Value),
                    Bottom = Convert.ToInt16(element.Attribute("bottom").Value),
                    Left = Convert.ToInt16(element.Attribute("left").Value)
                };

                _windows.Add(new GetWindow(title, rect));
            }
            Notify();
        }

        public void SaveWindow(GetWindow window)
        {
            if (DoesWindowElementExist(window.WindowTitle))
            {
                UpdateWindowElement(window);

                //Find window we updated in list
                foreach (GetWindow item in _windows)
                {
                    if (item.WindowTitle == window.WindowTitle)
                    {
                        int index = _windows.IndexOf(item);
                        _windows[index] = window;
                    }
                }
            }
            else
            {
                AddWindowElement(window);
                _windows.Add(window);
            }
            Notify();
        }

        public void DeleteWindows(List<GetWindow> windows)
        {
            foreach (GetWindow window in windows.Where(window => DoesWindowElementExist(window.WindowTitle)))
            {
                //Delete XML element
                DeleteElement(window.WindowTitle);
            }

            LoadAllWindows();
        }

        public void PositionWindows(List<GetWindow> windows)
        {
            windows.ForEach(window => window.PositionWindow());
        }
        public void PositionAllWindows()
        {
            _windows.ForEach(window => window.PositionWindow());
        }

        #region Observer
        public void Register(IObserver o)
        {
            _observers.Add(o);
            
            //When new observers registers, tell the observer what we currently got
            LoadAllWindows();
        }

        public void Unregister(IObserver o)
        {
            _observers.Remove(o);
        }

        // Telling all observers we got updated data
        public void Notify()
        {
            _observers.ForEach(observer => observer.Update(_windows));
        }
        #endregion
    }
}