using System;
using System.Configuration;

namespace SwaggerWcf.Configuration
{
    [ConfigurationCollection(typeof(TagElement), AddItemName = "tag")]
    public class TagCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TagElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return ((TagElement) element).Name;
        }
    }
}
