using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using AngleSharp.Html.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace RssNeedle;

internal static class RequestHandler
{

    private static readonly ScriptOptions _scriptOptions = ScriptOptions.Default
        .AddReferences([
            typeof(SyndicationFeed).Assembly,
            typeof(HtmlParser).Assembly
        ])
        .WithImports([
            "System",
            "System.Collections.Generic",
            "System.IO",
            "System.Linq",
            "System.Net.Http",
            "System.Text.RegularExpressions",
            "System.Threading",
            "System.Threading.Tasks",
        ])
        .WithOptimizationLevel(OptimizationLevel.Release);

    private static readonly IEnumerable<ScriptRunner<SyndicationItem>> _needles = Directory
        .GetFiles("Needles", "*.cs")
        .Select(File.ReadAllText)
        .Select(code => CSharpScript.Create<SyndicationItem>(code, options: _scriptOptions, globalsType: typeof(NeedleGlobals)).CreateDelegate())
        .ToList();

    public static async void HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        using var response = context.Response;

        if (request.HttpMethod != HttpMethod.Get.ToString())
        {
            response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            return;
        }

        if (request.QueryString["url"] is not { } url)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        Console.WriteLine("[info] <- " + url);

        SyndicationFeed feed;
        try
        {
            using var reader = XmlReader.Create(url);
            feed = SyndicationFeed.Load(reader);
        }
        catch (Exception e)
        {
            NeedleGlobals.Error(e);
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return;
        }

        feed.Items = await Task.WhenAll(feed.Items.Select(async item =>
        {
            var globals = new NeedleGlobals(feed, item);

            foreach (var needle in _needles)
            {
                try
                {
                    globals.Item = await needle(globals);
                }
                catch (Exception e)
                {
                    NeedleGlobals.Error(e);
                }
            }

            return globals.Item;
        }));

        response.ContentType = "application/rss+xml";

        try
        {
            using var writer = XmlWriter.Create(response.OutputStream);
            feed.SaveAsAtom10(writer);
        }
        catch (HttpListenerException e)
        {
            NeedleGlobals.Error(e);
            return;
        }

        Console.WriteLine("[info] -> " + response.StatusCode);
    }
}
