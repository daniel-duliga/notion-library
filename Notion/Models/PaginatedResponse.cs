using Newtonsoft.Json.Linq;

namespace IGDBNotionSync.Notion.Models
{
    internal class PaginatedResponse
    {
        public List<JObject>? results { get; set; }
        public bool has_more { get; set; }
        public string? next_cursor { get; set; }
    }
}
