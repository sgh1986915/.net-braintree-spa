using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySitterHub.Host.Console.Init
{
    public class ExampleSection : ConfigurationSection
    {
        #region Constructors
        /// <summary>
        /// Predefines the valid properties and prepares
        /// the property collection.
        /// </summary>
        static ExampleSection()
        {
            // Predefine properties here
            s_propString = new ConfigurationProperty(
                "stringValue",
                typeof(string),
                null,
                ConfigurationPropertyOptions.IsRequired
            );

            s_propBool = new ConfigurationProperty(
                "boolValue",
                typeof(bool),
                false,
                ConfigurationPropertyOptions.None
            );

            s_propTimeSpan = new ConfigurationProperty(
                "timeSpanValue",
                typeof(TimeSpan),
                null,
                ConfigurationPropertyOptions.None
            );

            s_properties = new ConfigurationPropertyCollection();

            s_properties.Add(s_propString);
            s_properties.Add(s_propBool);
            s_properties.Add(s_propTimeSpan);
        }
        #endregion

        #region Static Fields
        private static ConfigurationProperty s_propString;
        private static ConfigurationProperty s_propBool;
        private static ConfigurationProperty s_propTimeSpan;

        private static ConfigurationPropertyCollection s_properties;
        #endregion


        #region Properties
        /// <summary>
        /// Gets the StringValue setting.
        /// </summary>
        [ConfigurationProperty("stringValue", IsRequired = true)]
        public string StringValue
        {
            get { return (string)base[s_propString]; }
        }

        /// <summary>
        /// Gets the BooleanValue setting.
        /// </summary>
        [ConfigurationProperty("boolValue")]
        public bool BooleanValue
        {
            get { return (bool)base[s_propBool]; }
        }

        /// <summary>
        /// Gets the TimeSpanValue setting.
        /// </summary>
        [ConfigurationProperty("timeSpanValue")]
        public TimeSpan TimeSpanValue
        {
            get { return (TimeSpan)base[s_propTimeSpan]; }
        }

        /// <summary>
        /// Override the Properties collection and return our custom one.
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return s_properties; }
        }
        #endregion
    }
   
}
