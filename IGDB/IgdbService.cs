using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace NotionLibrary.IGDB
{
    internal class IgdbService
    {
        private HttpClient IgdbHttpClient { get; set; }

        public IgdbService(string twitchClientId, string twitchAccessToken)
        {
            IgdbHttpClient = new HttpClient() { BaseAddress = new Uri("https://api.igdb.com") };
            IgdbHttpClient.DefaultRequestHeaders.Add("Client-ID", twitchClientId);
            IgdbHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", twitchAccessToken);
        }

        public Task<List<GenericObject>?> GetAllGameModes() => GetGenericObjects("/v4/game_modes", 10);
        public Task<List<GenericObject>?> GetAllGenres() => GetGenericObjects("/v4/genres", 25);
        public Task<List<GenericObject>?> GetAllPerspectives() => GetGenericObjects("/v4/player_perspectives", 10);
        public Task<List<GenericObject>?> GetAllThemes() => GetGenericObjects("/v4/themes", 25);
        public async Task<List<VideoGame>> SearchVideoGame(string search)
        {
            var query = "limit 500; ";
            query += "fields name,first_release_date,summary,url,game_modes,genres,player_perspectives,themes,collection; ";
            query += $"search \"{search}\"; where platforms = (6);";

            var res = await IgdbHttpClient.PostAsync("/v4/games", new StringContent(query));
            var matchingGames = JsonConvert.DeserializeObject<List<VideoGame>>(await res.Content.ReadAsStringAsync());

            if (matchingGames != null)
            {
                return matchingGames.OrderBy(x => x.FirstReleaseYear).ToList();
            }
            else
            {
                return new List<VideoGame>();
            }
        }
        public async Task<string?> GetCoverUrl(int videoGameId)
        {
            var res = await IgdbHttpClient.PostAsync("/v4/covers", new StringContent($"fields url; where game = {videoGameId};"));
            var artworks = JsonConvert.DeserializeObject<List<Artwork>>(await res.Content.ReadAsStringAsync());
            if (artworks != null && artworks.Count > 0)
            {
                return $"https://{artworks[0].url?[2..].Replace("/t_thumb/", "/t_cover_big/")}";
            }
            else
            {
                return null;
            }
        }
        public async Task<string?> GetCollection(int? collectionId)
        {
            var res = await IgdbHttpClient.PostAsync("/v4/collections", new StringContent($"fields name; where id = {collectionId};"));
            var collections = JsonConvert.DeserializeObject<List<GenericObject>>(await res.Content.ReadAsStringAsync());
            if (collections != null && collections.Count > 0)
            {
                return collections[0].name;
            }
            else
            {
                return null;
            }
        }

        private async Task<List<GenericObject>?> GetGenericObjects(string path, int limit)
        {
            var res = await IgdbHttpClient.PostAsync(path, new StringContent($"fields name; limit {limit};"));
            return JsonConvert.DeserializeObject<List<GenericObject>>(await res.Content.ReadAsStringAsync());
        }

    }
}
