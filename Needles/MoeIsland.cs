if (GetUrl() is not "https://t.me/s/moeisland") return Item;

Info("MoeIsland.cs");

string[] patterns = [
    @"^https://(?:x|twitter)\.com/[^/]+/status/",
    @"^https://www\.pixiv\.net/artworks/",
];

var links = QueryAll("a").Select(x => x.GetAttribute("href"));

foreach (var link in links)
{
    if (patterns.Any(pattern => Regex.IsMatch(link, pattern)))
    {
        Info($"Replace with {link}");
        Item.Links[0].Uri = new(link);
        Item.Summary = new(Item.Summary.Text, TextSyndicationContentKind.Html);
        return Item;
    }
}

Warn("No match found");
return Item;
