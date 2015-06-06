using System;
using System.Configuration;

namespace SwaggerWcf.Configuration
{
    [ConfigurationCollection(typeof(TagElement), AddItemName = "setting")]
    public class SettingCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SettingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentException("element");
            }
            return ((SettingElement) element).Name;
        }
    }
}
