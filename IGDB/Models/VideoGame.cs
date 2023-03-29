namespace NotionLibrary.IGDB
{
    internal class VideoGame
    {
        public int id { get; set; }
        public string name { get; set; }
        public string? first_release_date { get; set; }
        public string? summary { get; set; }
        public string? url { get; set; }
        public int? collection { get; set; }
        public int[]? game_modes { get; set; }
        public int[]? genres { get; set; }
        public int[]? player_perspectives { get; set; }
        public int[]? themes { get; set; }

        public int FirstReleaseYear { get { return DateTimeOffset.FromUnixTimeSeconds(long.Parse(first_release_date ?? "0")).Year; } }
        public string NameWithInfo { get { return $"{name} ({FirstReleaseYear})"; } }
    }
}
