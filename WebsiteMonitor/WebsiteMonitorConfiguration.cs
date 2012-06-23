using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace WebsiteMonitor
{
    class WebsiteMonitorConfiguration: ConfigurationSection
    {
        [ConfigurationProperty("websites")]
        public WebsiteCollection Websites
        {
            get
            { return (WebsiteCollection)this["websites"]; }
            set
            { this["websites"] = value; }
        }

        [ConfigurationProperty("alertEmailAddress", IsRequired = true)]
        public String AlertEmailAddress
        {
            get
            { return (String)this["alertEmailAddress"]; }
            set
            { this["alertEmailAddress"] = value; }
        }

        [ConfigurationProperty("fromEmailAddress", IsRequired = true)]
        public String FromEmailAddress
        {
            get
            { return (String)this["fromEmailAddress"]; }
            set
            { this["fromEmailAddress"] = value; }
        }

        [ConfigurationProperty("emailUseSsl", IsRequired = true)]
        public bool EmailuseSsl
        {
            get
            { return (bool)this["emailUseSsl"]; }
            set
            { this["emailUseSsl"] = value; }
        }

        public sealed class WebsiteCollection : ConfigurationElementCollection
        {
            protected override ConfigurationElement CreateNewElement()
            {
                return new WebsiteElement();
            }
            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((WebsiteElement)element).Url;
            }
            public override ConfigurationElementCollectionType CollectionType
            {
                get
                {
                    return ConfigurationElementCollectionType.BasicMap;
                }
            }
            protected override string ElementName
            {
                get
                {
                    return "website";
                }
            }
        }

        public class WebsiteElement : ConfigurationElement
        {
            [ConfigurationProperty("requiredContent", IsRequired = false)]
            public String RequiredContent
            {
                get
                { return (String)this["requiredContent"]; }
                set
                { this["requiredContent"] = value; }
            }

            [ConfigurationProperty("forbiddenContent", IsRequired = false)]
            public String ForbiddenContent
            {
                get
                { return (String)this["forbiddenContent"]; }
                set
                { this["forbiddenContent"] = value; }
            }

            [ConfigurationProperty("url", IsRequired = true)]
            public String Url
            {
                get
                { return (String)this["url"]; }
                set
                { this["url"] = value; }
            }

            [ConfigurationProperty("name", IsRequired = true)]
            public String Name
            {
                get
                { return (String)this["name"]; }
                set
                { this["name"] = value; }
            }
        }
    }
}
