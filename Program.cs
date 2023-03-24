using IGDBNotionSync.IGDB;
using IGDBNotionSync.Notion;
using IGDBNotionSync.Twitch;
using Sharprompt;

namespace IGDBNotionSync
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Notion <-> IGDB");

            var notionService = new NotionService("");
            var twitchClientId = "";
            var twitchService = new TwitchService("", "");
            var twitchAccessToken = await twitchService.GetAccessToken();
            var igdbService = new IgdbService(twitchClientId, twitchAccessToken);

            var notionGames = await notionService.GetAllPages("");
            if (notionGames == null || notionGames.Count == 0)
            {
                Console.WriteLine("No games to process");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine($"Processing {notionGames.Count} games...");
            }

            var allGameModes = await igdbService.GetAllGameModes();
            var allGenres = await igdbService.GetAllGenres();
            var allPerspectives = await igdbService.GetAllPerspectives();
            var allThemes = await igdbService.GetAllThemes();

            for (int i = 0; i < notionGames.Count; i++)
            {
                var notionGame = notionGames[i];
                var name = notionGame["properties"]["Name"]["title"][0]["plain_text"].ToString();
                var id = notionGame["id"].ToString();

                var matchingGames = await igdbService.SearchVideoGame(name);
                if (matchingGames == null || matchingGames.Count == 0)
                {
                    Console.WriteLine($"{i + 1}. {name}: No matching games found in IGDB");
                    Console.ReadKey();
                    continue;
                }

                var matchingGame = Prompt.Select($"{i + 1}. {name}", matchingGames, textSelector: x => x.NameWithInfo);
                if (matchingGame == null)
                    continue;

                var series = await igdbService.GetCollection(matchingGame.collection);

                var gameModes = allGameModes?
                    .Where(x => matchingGame.game_modes != null && matchingGame.game_modes.Contains(x.id))
                    .Select(x => x.name.Replace(",", " /"));
                
                var genres = allGenres?
                    .Where(x => matchingGame.genres != null && matchingGame.genres.Contains(x.id))
                    .Select(x => x.name.Replace(",", " /"));
                
                var perspectives = allPerspectives?
                    .Where(x => matchingGame.player_perspectives != null && matchingGame.player_perspectives.Contains(x.id))
                    .Select(x => x.name.Replace(",", " /"));
                
                var themes = allThemes?
                    .Where(x => matchingGame.themes != null && matchingGame.themes.Contains(x.id))
                    .Select(x => x.name.Replace(",", " /"));

                await notionService.UpdatePage(
                    id,
                    matchingGame.name,
                    matchingGame.FirstReleaseYear,
                    series,
                    await igdbService.GetCoverUrl(matchingGame.id),
                    gameModes,
                    genres,
                    perspectives,
                    themes
                );
            }
        }
    }
}