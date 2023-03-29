using CsvHelper;
using System.Globalization;

namespace NotionLibrary.CSV
{
    internal class CsvService
    {
        public static List<VideoGame> LoadGamesFromCsv(string csvFilePath)
        {
            using var reader = new StreamReader(csvFilePath);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csvReader.GetRecords<VideoGame>().ToList();
        }
    }

    internal class VideoGame
    {
        public string? Name { get; set; }
        public string? Platforms { get; set; }
        public string? Status { get; set; }
        public string? IGDB_URL { get; set; }
        public string? Released { get; set; }
        public string? Genres { get; set; }
        public string? Modes { get; set; }
        public string? Themes { get; set; }
        public string? Series { get; set; }
        public string? Perspectives { get; set; }
        public string? Description { get; set; }
        public bool? Matched { get; set; }
        public string? ArtworkUrl { get; set; }
    }
}
