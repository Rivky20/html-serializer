using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HTML
{

    public class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;

        public HashSet<string> HtmlTags { get; private set; }
        public HashSet<string> VoidTags { get; private set; }

        private HtmlHelper()
        {
            try
            {
                HtmlTags = new HashSet<string>(
                    JsonSerializer.Deserialize<string[]>(File.ReadAllText("HtmlTags.json")) ?? Array.Empty<string>());
                VoidTags = new HashSet<string>(
                    JsonSerializer.Deserialize<string[]>(File.ReadAllText("HtmlVoidTags.json")) ?? Array.Empty<string>());
            }
            catch (Exception ex)
            {
                throw new FileLoadException("Error loading HTML tags files", ex);
            }
        }
    }

}

