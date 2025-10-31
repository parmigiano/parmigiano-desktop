using Parmigiano.Core;
using Parmigiano.Interface;
using Parmigiano.Services;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Parmigiano.Repository
{
    public class UserConfigRepository : IUserConfigRepository
    {
        private const string ROOT_ELEMENT = "userConf";
        private const string SETTING_ELEMENT = "settings";
        private const string NAME_ATTRIBUTE = "name";
        private const string VALUE_ELEMENT = "value";

        private readonly string _configPath;

        public UserConfigRepository()
        {
            this._configPath = Config.Current.CONFIG_USER_PATH;
        }

        private XDocument LoadOrCreateDocument()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    return XDocument.Load(_configPath);
                }
                else
                {
                    return new XDocument(new XElement(ROOT_ELEMENT));
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка чтения user.config: {ex.Message}");
                return new XDocument(new XElement(ROOT_ELEMENT));
            }
        }

        private void SaveDocument(XDocument doc)
        {
            try
            {
                string folder = Path.GetDirectoryName(_configPath);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                doc.Save(_configPath);
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка записи user.config: {ex.Message}");
            }
        }

        public string Get(string key)
        {
            try
            {
                var doc = this.LoadOrCreateDocument();
                var element = doc.Root?
                    .Elements(SETTING_ELEMENT)
                    .FirstOrDefault(e => e.Attribute(NAME_ATTRIBUTE)?.Value == key);

                return element?.Element(VALUE_ELEMENT)?.Value;
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка при получении ключа '{key}': {ex.Message}");
                return null;
            }
        }

        public void Set(string key, string value)
        {
            try
            {
                var doc = this.LoadOrCreateDocument();

                var existing = doc.Root?
                    .Elements(SETTING_ELEMENT)
                    .FirstOrDefault(e => e.Attribute(NAME_ATTRIBUTE)?.Value == key);

                if (existing == null)
                {
                    var newElement = new XElement(SETTING_ELEMENT,
                        new XAttribute(NAME_ATTRIBUTE, key),
                        new XAttribute("serializeAs", "String"),
                        new XElement(VALUE_ELEMENT, value));

                    doc.Root?.Add(newElement);
                }
                else
                {
                    var valueElement = existing.Element(VALUE_ELEMENT);
                    if (valueElement == null)
                    {
                        existing.Add(new XElement(VALUE_ELEMENT, value));
                    }
                    else
                    {
                        valueElement.Value = value;
                    }
                }

                this.SaveDocument(doc);
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка при добавлении/обновлении '{key}': {ex.Message}");
            }
        }

        public void DeleteKey(string key)
        {
            try
            {
                var doc = this.LoadOrCreateDocument();

                var element = doc.Root?
                    .Elements(SETTING_ELEMENT)
                    .FirstOrDefault(e => e.Attribute(NAME_ATTRIBUTE)?.Value == key);

                if (element != null)
                {
                    element.Remove();
                    SaveDocument(doc);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка при удалении ключа '{key}': {ex.Message}");
            }
        }
    }
}
