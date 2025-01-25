using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HTML
{
        public class HtmlSerializer
        {
            private readonly HtmlHelper _helper = HtmlHelper.Instance;

            public async Task<string> Load(string url)
            {
                using var client = new HttpClient();
                try
                {
                    return await client.GetStringAsync(url);
                }
                catch (Exception ex)
                {
                    throw new HttpRequestException($"Error loading URL {url}: {ex.Message}");
                }
            }

            public HtmlElement Serialize(string html)
            {
                var cleanHtml = CleanHtml(html);
                var tokens = TokenizeHtml(cleanHtml);
                return BuildTree(tokens);
            }

            private string CleanHtml(string html)
            {
                return Regex.Replace(html, @"\s+", " ").Trim();
            }

            private List<string> TokenizeHtml(string html)
            {
                var pattern = @"<[^>]+>|[^<>]+";
                return Regex.Matches(html, pattern)
                           .Select(m => m.Value.Trim())
                           .Where(s => !string.IsNullOrEmpty(s))
                           .ToList();
            }

            private HtmlElement BuildTree(List<string> tokens)
            {
                var root = new HtmlElement { Name = "root" };
                var current = root;

                var tagPattern = @"</?([^\s>]+).*?>";
                var attributePattern = @"(\w+)=""([^""]*)""|(\w+)='([^']*)'";

                foreach (var token in tokens)
                {
                    if (!token.StartsWith("<"))
                    {
                        if (current != null)
                        {
                            current.InnerHtml = token;
                        }
                        continue;
                    }

                    var tagMatch = Regex.Match(token, tagPattern);
                    if (!tagMatch.Success) continue;

                    var tagName = tagMatch.Groups[1].Value.ToLower();

                    if (token.StartsWith("</"))
                    {
                        current = current?.Parent;
                        continue;
                    }

                    if (_helper.HtmlTags.Contains(tagName))
                    {
                        var element = new HtmlElement
                        {
                            Name = tagName,
                            Parent = current
                        };

                        foreach (Match attr in Regex.Matches(token, attributePattern))
                        {
                            var name = attr.Groups[1].Value;
                            var value = attr.Groups[2].Value;

                            if (name.Equals("class", StringComparison.OrdinalIgnoreCase))
                            {
                                foreach (var className in value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                                {
                                    element.Classes.Add(className);
                                }
                            }
                            else if (name.Equals("id", StringComparison.OrdinalIgnoreCase))
                            {
                                element.Id = value;
                            }
                            else
                            {
                                element.Attributes.Add($"{name}=\"{value}\"");
                            }
                        }

                        current?.Children.Add(element);

                        if (!token.EndsWith("/>") && !_helper.VoidTags.Contains(tagName))
                        {
                            current = element;
                        }
                    }
                }

                return root;
            }
        }
    
}
