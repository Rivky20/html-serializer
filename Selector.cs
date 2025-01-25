using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML
{
    // מחלקה המייצגת CSS Selector

    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public HashSet<string> Classes { get; set; } = new();
        public Selector Child { get; set; }
        public static Selector Parse(string query)
        {
            var parts = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Selector root = null;
            Selector current = null;

            foreach (var part in parts)
            {
                var selector = new Selector();

                foreach (var component in SplitSelectorComponents(part))
                {
                    if (component.StartsWith("#"))
                    {
                        selector.Id = component[1..];
                    }
                    else if (component.StartsWith("."))
                    {
                        selector.Classes.Add(component[1..]);
                    }
                    else if (HtmlHelper.Instance.HtmlTags.Contains(component.ToLower()))
                    {
                        selector.TagName = component.ToLower();
                    }
                }

                if (root == null)
                {
                    root = selector;
                    current = root;
                }
                else
                {
                    current.Child = selector;
                    current = selector;
                }
            }

            return root;
        }

        private static List<string> SplitSelectorComponents(string selector)
        {
            var components = new List<string>();
            var current = "";

            foreach (var c in selector)
            {
                if (c == '#' || c == '.')
                {
                    if (!string.IsNullOrEmpty(current))
                    {
                        components.Add(current);
                    }
                    current = c.ToString();
                }
                else
                {
                    current += c;
                }
            }

            if (!string.IsNullOrEmpty(current))
            {
                components.Add(current);
            }

            return components;
        }

        public bool Matches(HtmlElement element)
        {
            if (!string.IsNullOrEmpty(TagName) && TagName != element.Name)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(Id) && Id != element.Id)
            {
                return false;
            }

            if (Classes.Count > 0 && !Classes.All(c => element.Classes.Contains(c)))
            {
                return false;
            }
            return true;
        }
    }
}
