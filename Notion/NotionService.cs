using IGDBNotionSync.Notion.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Net.Http.Headers;
using System.Text;

namespace IGDBNotionSync.Notion
{
    internal class NotionService
    {
        private HttpClient NotionHttpClient { get; set; }

        public NotionService(string notionAccessToken)
        {
            NotionHttpClient = new HttpClient() { BaseAddress = new Uri("https://api.notion.com") };
            NotionHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", notionAccessToken);
            NotionHttpClient.DefaultRequestHeaders.Add("Notion-Version", "2022-06-28");
        }

        public async Task<List<JObject>> GetAllPages(string databaseId)
        {
            var result = new List<JObject>();

            bool moreDataAvailable = true;
            string? nextCursor = null;
            while (moreDataAvailable)
            {
                moreDataAvailable = false;

                PaginatedResponse? resultsPage = await GetResults(databaseId, nextCursor);
                if (resultsPage != null)
                {
                    moreDataAvailable = resultsPage.has_more;
                    nextCursor = resultsPage.next_cursor;

                    if (resultsPage.results != null)
                    {
                        result.AddRange(resultsPage.results);
                    }
                }
            }

            return result;

            async Task<PaginatedResponse?> GetResults(string databaseId, string? nextCursor)
            {
                dynamic body = new ExpandoObject();
                body.sorts = new dynamic[1]
                {
                   new
                   {
                       property = "Name",
                       direction = "ascending"
                   }
                };
                body.filter = new
                {
                    property = "Matched",
                    checkbox = new
                    {
                        equals = false
                    }
                };

                if (nextCursor != null)
                {
                    body.start_cursor = nextCursor;
                }
                
                var responseRaw = await NotionHttpClient.PostAsync(
                    $"/v1/databases/{databaseId}/query",
                    new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
                );
                
                return JsonConvert.DeserializeObject<PaginatedResponse>(await responseRaw.Content.ReadAsStringAsync());
            }
        }
        public async Task UpdatePage(
            string pageId,
            string name,
            int released,
            string? series,
            string? coverUrl,
            IEnumerable<string>? gameModes,
            IEnumerable<string>? genres,
            IEnumerable<string>? perspectives,
            IEnumerable<string>? themes
        )
        {
            dynamic body = new ExpandoObject();
            body.cover = new { external = new { url = coverUrl } };
            body.properties = new ExpandoObject();
            body.properties.Matched = new { checkbox = true };
            body.properties.Name = new { title = new[] { new { text = new { content = name } } } };
            body.properties.Released = new { number = released };
            if (series != null) { body.properties.Series = new { select = new { name = series } }; }
            if (gameModes != null) { body.properties.Modes = new { multi_select = gameModes.Select(x => new { name = x }) }; }
            if (genres != null) { body.properties.Genres = new { multi_select = genres.Select(x => new { name = x }) }; }
            if (perspectives != null) { body.properties.Perspectives = new { multi_select = perspectives.Select(x => new { name = x }) }; }
            if (themes != null) { body.properties.Themes = new { multi_select = themes.Select(x => new { name = x }) }; }

            var response = await NotionHttpClient.PatchAsync(
                $"/v1/pages/{pageId}",
                new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
            );
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Response: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
        }
    }
}
