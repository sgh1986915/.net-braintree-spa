using System;
using System.Collections.Generic;

namespace MySitterHub.Model.Common
{

    /// <summary>
    /// Localizable Item, used as an item in Options or as a constant label
    /// </summary>
    public class LocItem
    {
        public LocItem(string key, string display = null)
        {
            Key = key;
            Display = display ?? key;
            Translations = new Dictionary<string, string>();
        }

        public string Key { get; private set; }

        /// <summary>
        /// Default Display Value (English)
        /// </summary>
        public string Display { get; set; }

        /// <summary>
        /// Key: Locale, for example "es" or "zh"
        /// Value: Translation
        /// </summary>
        public Dictionary<string, string> Translations { get; set; }

        /// <summary>
        /// If a value is not found for the locale, return default Display
        /// </summary>
        public string DisplayLocalized(string locale)
        {
            string t;
            if (!Translations.TryGetValue(locale, out t) || string.IsNullOrEmpty(t))
            {
                return Display;
            }

            return Translations[locale];
        }

        public override string ToString()
        {
            return Display;
        }

    }

    /// <summary>
    /// LocItem with underlying Enum
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //public class LocItem<T>
    //{
    //    public LocItem(T key, string display = null)
    //    {
    //        Key = key;
    //        Display = display ?? key.ToString();

    //        Translations = new Dictionary<string, string>();
    //    }

    //    public T Key { get; private set; }

    //    /// <summary>
    //    /// Default Display Value (English)
    //    /// </summary>
    //    public string Display { get; set; }

    //    /// <summary>
    //    /// Key: Locale, for example "es" or "zh"
    //    /// Value: Translation, for example "El Unknown"
    //    /// </summary>
    //    public Dictionary<string, string> Translations { get; set; }

    //    /// <summary>
    //    /// If a value is not found for the locale, return default Display
    //    /// </summary>
    //    public string DisplayLocalized(string locale)
    //    {
    //        string t;
    //        if (!Translations.TryGetValue(locale, out t) || string.IsNullOrEmpty(t))
    //        {
    //            return Display;
    //        }

    //        return Translations[locale];
    //    }

    //    public override string ToString()
    //    {
    //        return Display;
    //    }

    //}

    /// <summary>
    /// Dropdown Item.
    /// The display item should be a localized value
    /// </summary>
    public struct DdItem
    {
        public DdItem(string key, string display)
        {
            _key = key;
            _display = display;
        }

        /// <summary>
        /// Key and display are the same
        /// </summary>
        public DdItem(string key)
        {
            _key = key;
            _display = key;
        }

        private string _key;
        private string _display;

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public string Display
        {
            get { return _display; }
            set { _display = value; }
        }

        /// <summary>
        /// Helper to create a List of DdItem.
        /// keysAndValues contanain alternating key and display.
        /// keysAndValues.Length must be an even number.
        /// </summary>
        public static List<DdItem> CreateList(params string[] keysAndValues)
        {
            if (keysAndValues.Length % 2 != 0)
                throw new Exception("keysAndValues.Length must be an even number");

            List<DdItem> items = new List<DdItem>();
            for (int i = 0; i < keysAndValues.Length - 1; i = i + 2)
            {
                items.Add(new DdItem(keysAndValues[i], keysAndValues[i + 1]));
            }
            return items;
        }

        public static List<DdItem> CreateListKeys(params string[] keys)
        {
            List<DdItem> items = new List<DdItem>();
            foreach (var key in keys)
            {
                items.Add(new DdItem(key));
            }
            return items;
        }
    }
}
