if (!GetUrl().StartsWith("http://mikanani.me/RSS/")) return Item;

Info("Mikanani.cs");

var magnet = $"magnet:?xt=urn:btih:{Item.Links[0].Uri.LocalPath.Split('/')[^1]}&tr=http%3a%2f%2ft.nyaatracker.com%2fannounce&tr=http%3a%2f%2ftracker.kamigami.org%3a2710%2fannounce&tr=http%3a%2f%2fshare.camoe.cn%3a8080%2fannounce&tr=http%3a%2f%2fopentracker.acgnx.se%2fannounce&tr=http%3a%2f%2fanidex.moe%3a6969%2fannounce&tr=http%3a%2f%2ft.acg.rip%3a6699%2fannounce&tr=https%3a%2f%2ftr.bangumi.moe%3a9696%2fannounce&tr=udp%3a%2f%2ftr.bangumi.moe%3a6969%2fannounce&tr=http%3a%2f%2fopen.acgtracker.com%3a1096%2fannounce&tr=udp%3a%2f%2ftracker.opentrackr.org%3a1337%2fannounce";

for (int i = 0; i < Item.Links.Count; i++)
{
    var link = Item.Links[i];
    if (link.RelationshipType is "enclosure")
    {
        Info($"Replace with {magnet}");
        link.Uri = new(magnet);
        link.MediaType = null;
        link.Length = 0;
        return Item;
    }
}

Warn("No enclosure found");
return Item;
