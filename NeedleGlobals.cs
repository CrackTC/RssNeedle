using System.ServiceModel.Syndication;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace RssNeedle;

public class NeedleGlobals(SyndicationFeed feed, SyndicationItem item)
{
    public SyndicationFeed Feed => feed;
    public SyndicationItem Item { get; set; } = item;

    public string GetUrl() =>
        (feed.Links.FirstOrDefault(link => link.RelationshipType is "alternate") ??
         feed.Links.First()).Uri.ToString();

    private static readonly HtmlParser _parser = new();
    private IHtmlDocument _document = null!;
    public IHtmlCollection<IElement> QueryAll(string selector)
    {
        _document ??= _parser.ParseDocument(Item.Summary.Text);
        return _document.QuerySelectorAll(selector);
    }
    public IHtmlCollection<IElement> QueryAll(string html, string selector)
    {
        var document = _parser.ParseDocument(html);
        return document.QuerySelectorAll(selector);
    }
    public IElement? Query(string selector)
    {
        _document ??= _parser.ParseDocument(Item.Summary.Text);
        return _document.QuerySelector(selector);
    }
    public IElement? Query(string html, string selector)
    {
        var document = _parser.ParseDocument(html);
        return document.QuerySelector(selector);
    }

    private static readonly HttpClient _client = new();
    public static async Task<string> Get(Uri url) => await _client.GetStringAsync(url);

    public static void Info(string message) => Console.WriteLine($"[info] | {message}");
    public static void Warn(string message) {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[warn] | {message}");
        Console.ResetColor();
    }
    public static void Error(string message) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[error] | {message}");
        Console.ResetColor();
    }
    public static void Error(Exception e) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[error] | {e.Message}\n{e.StackTrace}");
        Console.ResetColor();
    }
}
