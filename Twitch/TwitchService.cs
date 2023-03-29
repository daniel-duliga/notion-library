using Newtonsoft.Json;

namespace NotionLibrary.Twitch
{
    internal class TwitchService
    {
        private readonly string ClientId;
        private readonly string ClientSecret;

        public TwitchService(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public async Task<string> GetAccessToken()
        {
            using var twitchHttpClient = new HttpClient() { BaseAddress = new Uri("https://id.twitch.tv") };
            var response = await twitchHttpClient.PostAsync(
                $"/oauth2/token?client_id={ClientId}&client_secret={ClientSecret}&grant_type=client_credentials", null
            );
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<TwitchOAuthResponse>(responseContent);
            return responseData?.access_token ?? string.Empty;
        }

        internal class TwitchOAuthResponse
        {
            public string? access_token { get; set; }
            public int expires_in { get; set; }
            public string? token_type { get; set; }
        }
    }
}
