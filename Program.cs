using System.Net;

namespace RssNeedle;

internal class Program
{
    public static async Task Main()
    {
        string port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://*:{port}/");
        listener.Start();

        Console.WriteLine($"Listening on http://0.0.0.0:{port}/");

        while (true)
        {
            var context = await listener.GetContextAsync();
            RequestHandler.HandleRequest(context);
        }
    }
}

