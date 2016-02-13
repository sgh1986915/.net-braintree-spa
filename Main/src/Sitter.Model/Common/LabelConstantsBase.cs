using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MySitterHub.Model.Common
{

    public class LabelConstantsBase
    {
        public const string DefaultLocale = "en";
        public const string SpanishLocale = "es";
        public const string ChineseLocale = "zh";

        public void SetMyLocItem<T>(Expression<Func<T, object>> fieldExpression, string defaultDisplay)
        {
            var memberSelectorExpression = fieldExpression.Body as MemberExpression;
            if (memberSelectorExpression != null)
            {
                var fieldInfo = memberSelectorExpression.Member as FieldInfo;
                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(this, new LocItem(fieldInfo.Name, defaultDisplay));
                }
            }
        }

        public string FormatKey<T>(Expression<Func<T, object>> fieldExpression)
        {
            var memberSelectorExpression = (MemberExpression)fieldExpression.Body;
            var fieldInfo = (FieldInfo)memberSelectorExpression.Member;
            return typeof(T).Name + "." + fieldInfo.Name;
        }

        private Dictionary<string, Dictionary<string, string>> _cache = new Dictionary<string, Dictionary<string, string>>();
        private volatile object lockObj = new object();

        /// <summary>
        /// Get Dictionary of key= locItem.Key and value= translated value for locale
        /// </summary>
        /// <param name="locale"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetLabelConstantsForUI(string locale)
        {
            if (locale == null)
                locale = DefaultLocale;

            if (!_cache.ContainsKey(locale)) //double lock check Threadsafe
            {
                lock (lockObj)
                {
                    if (!_cache.ContainsKey(locale))
                    {
                        Dictionary<string, string> ret = new Dictionary<string, string>();

                        foreach (FieldInfo locItemFieldInfo in this.GetType().GetFields())
                        {
                            if (locItemFieldInfo.FieldType != typeof(LocItem))
                                continue;

                            LocItem li = (LocItem)locItemFieldInfo.GetValue(this);

                            ret.Add(li.Key, li.DisplayLocalized(locale));
                        }

                        _cache.Add(locale, ret);
                    }
                }
            }
            return _cache[locale];
        }
    }
}
