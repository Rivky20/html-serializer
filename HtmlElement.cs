using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML
{
    // מחלקה המייצגת אלמנט HTML בודד
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public HashSet<string> Attributes { get; set; } = new();
        public HashSet<string> Classes { get; set; } = new();
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new();

        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();
            var visited = new HashSet<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (visited.Add(current))
                {
                    yield return current;
                    foreach (var child in current.Children)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            var current = Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        public IEnumerable<HtmlElement> FindElements(Selector selector)
        {
            var result = new HashSet<HtmlElement>();
            FindElementsRecursive(this, selector, result);
            return result;
        }

        private void FindElementsRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
        {
            if (selector == null) return;

            foreach (var descendant in element.Descendants())
                if (selector.Matches(descendant))
                {
                    if (selector.Child == null)
                    {
                        result.Add(descendant);
                    }
                    else
                    {
                        FindElementsRecursive(descendant, selector.Child, result);
                    }
                }
        }
    }
}
