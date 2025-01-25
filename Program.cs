using HTML;
using System.Xml.Serialization;

var serializer = new HtmlSerializer();

var html = await serializer.Load("https://claude.ai");

var rootElement = serializer.Serialize(html);

var selector = Selector.Parse("div.content #main");
var elements = rootElement.FindElements(selector);

foreach (var descendant in rootElement.Descendants())
{
    Console.WriteLine(descendant.Name);
}