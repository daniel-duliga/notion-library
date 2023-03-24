namespace IGDBNotionSync.IGDB
{
    internal class GenericObject
    {
        public int id { get; set; }
        public string name { get; set; }

        public static string ToStringList(List<GenericObject>? genericObjects, int[]? genericObjectIds)
        {
            string result = "";
            if (genericObjectIds != null && genericObjectIds.Length > 0)
            {
                foreach (var id in genericObjectIds)
                {
                    var match = genericObjects?.SingleOrDefault(x => x.id == id);
                    if (match != null)
                    {
                        result += $"{match.name}, ";
                    }
                }
                if (result.EndsWith(", "))
                {
                    result = result[..^2];
                }
            }
            return result;
        }
    }
}
