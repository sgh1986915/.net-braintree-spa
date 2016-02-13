using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Sitter.Toolbox.Utility
{
    public static class UIControlsHelper
    {
        public static Window GetCurrentActiveWindow()
        {
            Window activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
            return activeWindow;
        }

        public static T FindVisualParent<T>(this DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null)
                return null;
            if (parent is T)
                return parent as T;
            return FindVisualParent<T>(parent);
        }

        /// <summary>
        /// Get a list of descendant dependencyobjects of the specified type and which meet the criteria
        /// as specified by the predicate
        /// </summary>
        /// <typeparam name="T">The type of child you want to find</typeparam>
        /// <param name="parent">The dependency object whose children you wish to scan</param>
        /// <param name="predicate">The child object is selected if the predicate evaluates to true</param>
        /// <returns>The first matching descendant of the specified type</returns>
        /// <remarks> usage: myWindow.FindChildren<StackPanel>( child => child.Name == "myPanel" ) </StackPanel></remarks>
        public static List<T> FindChildren<T>(this DependencyObject parent, Func<T, bool> predicate = null)
            where T : DependencyObject
        {
            List<T> foundList = new List<T>();
            FindChildren(parent, predicate, foundList);
            return foundList;
        }

        private static void FindChildren<T>(this DependencyObject parent, Func<T, bool> predicate, List<T> foundList)
            where T : DependencyObject
        {
            var children = new List<DependencyObject>();

            if ((parent is Visual))
            {
                var visualChildrenCount = VisualTreeHelper.GetChildrenCount(parent);
                for (int childIndex = 0; childIndex < visualChildrenCount; childIndex++)
                    children.Add(VisualTreeHelper.GetChild(parent, childIndex));
            }
            foreach (var logicalChild in LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>())
                if (!children.Contains(logicalChild))
                    children.Add(logicalChild);

            foreach (var child in children)
            {
                var typedChild = child as T;
                if ((typedChild != null) && (predicate == null || predicate.Invoke(typedChild)) && !foundList.Contains(typedChild))
                    foundList.Add(typedChild);

                FindChildren(child, predicate, foundList);
            }
        }

        public static string ToAlphabet(this int columnIndex)
        {
            var res = new List<char>();
            if (columnIndex > 0)
            {
                columnIndex--;
                if (columnIndex < 26)
                {
                    res.Add((char)('A' + columnIndex));
                }
                else
                {
                    columnIndex -= 26;
                    do
                    {
                        res.Add((char)('A' + columnIndex % 26));
                        columnIndex = columnIndex / 26;
                    } while (columnIndex > 0);
                    if (res.Count == 1)
                        res.Add('A');
                }
            }
            return new string(res.Reverse<char>().ToArray());
        }
        
        public static bool IsCtrlDown()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }

        public static bool IsShiftDown()
        {
            return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
        }
        
    }

}