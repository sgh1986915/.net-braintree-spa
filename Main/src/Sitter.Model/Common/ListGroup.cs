using System;
using System.Collections.Generic;

namespace MySitterHub.Model.Common
{
    public class ListGroup
    {

        public ListGroup(string name, params LocItem[] items)
        {
            Name = name;
            EnumType = null;
            Items = items;
        }

        public ListGroup(Type enumType,Dictionary<string,string> overrideDefault = null )
        {
            Name = enumType.Name;
            EnumType = enumType;

            var items = new List<LocItem>();
            foreach (var item in Enum.GetValues(enumType))
            {
                if (overrideDefault != null && overrideDefault.ContainsKey(item.ToString()))
                {
                    items.Add(new LocItem(item.ToString(), overrideDefault[item.ToString()]));
                }
                else
                {
                    items.Add(new LocItem(item.ToString()));    
                }
                
            }
            
            Items = items.ToArray();
        }

        public string Name { get; set; }

        /// <summary>
        /// If Underlying type on data object is string, set to null, otherwise set to enum type.
        /// </summary>
        public Type EnumType { get; set; }

        public LocItem[] Items { get; set; }
    }

    /// <summary>
    /// Place this attribute on string properties which contain values corresponding to ListGroup.Items
    /// </summary>
    public class ListGroupAttribute : Attribute
    {
        public ListGroupAttribute(string listGroupName)
        {
            ListGroupName = listGroupName;
            EnumType = null;
        }

        public ListGroupAttribute(Type enumType)
        {
            ListGroupName = enumType.Name;
            EnumType = enumType;
        }

        public string ListGroupName { get; set; }
        public Type EnumType { get; set; }
    }

}