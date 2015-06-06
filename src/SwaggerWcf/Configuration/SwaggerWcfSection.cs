using System.Configuration;

namespace SwaggerWcf.Configuration
{
    public class SwaggerWcfSection : ConfigurationSection
    {
        [ConfigurationProperty("tags", IsRequired = true)]
        public TagCollection Tags
        {
            get
            {
                return (TagCollection) this["tags"];
            }
            set
            {
                this["tags"] = value;
            }
        }

        [ConfigurationProperty("settings", IsRequired = false)]
        public SettingCollection Settings
        {
            get
            {
                return (SettingCollection) this["settings"];
            }
            set
            {
                this["settings"] = value;
            }
        }
    }
}
