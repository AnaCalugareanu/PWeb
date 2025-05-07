using System.Net.Sockets;
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0 || args[0] == "-ch")
        {
            ShowHelp();
        }
        else if (args[0] == "-u" && args.Length > 1)
        {
            MakeHttpRequest(args[1], isSearch: true);
        }
        else if (args[0] == "-s" && args.Length > 1)
        {
            string searchTerm = string.Join(" ", args.Skip(1));
            PerformSearch(searchTerm);
        }
        else
        {
            Console.WriteLine("Invalid arguments. Use -h for help.");
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("go2web -u <URL>         # Make HTTP request to URL and show response");
        Console.WriteLine("go2web -s <search-term> # Search the term and show top 10 results");
        Console.WriteLine("go2web -h               # Show help");
    }

    private static void MakeHttpRequest(string url, bool isSearch = false)
    {
        try
        {
            Uri uri = new Uri(url);
            string host = uri.Host;
            string path = string.IsNullOrEmpty(uri.PathAndQuery) ? "/" : uri.PathAndQuery;

            using (TcpClient client = new TcpClient(host, 80))
            using (NetworkStream stream = client.GetStream())
            using (StreamWriter writer = new StreamWriter(stream))
            using (StreamReader reader = new StreamReader(stream))
            {
                writer.Write(
                    $"GET {path} HTTP/1.1\r\n" +
                    $"Host: {host}\r\n" +
                    $"User-Agent: go2web/1.0\r\n" +
                    $"Connection: close\r\n\r\n");
                writer.Flush();

                string response = reader.ReadToEnd();
                Console.WriteLine("=== RAW RESPONSE HEADERS AND BODY ===");
                Console.WriteLine(response);

                if (response.StartsWith("HTTP/1.1 301") || response.StartsWith("HTTP/1.1 302"))
                {
                    var match = Regex.Match(response, "Location: (.+?)\r\n");
                    if (match.Success)
                    {
                        string newUrl = match.Groups[1].Value.Trim();
                        Console.WriteLine($"Redirecting to: {newUrl}");

                        if (newUrl.StartsWith("https://"))
                        {
                            Console.WriteLine("Redirected to HTTPS. HTTPS is not supported yet.");
                            return;
                        }

                        MakeHttpRequest(newUrl, isSearch);
                        return;
                    }
                }

                string[] parts = response.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);
                string body = parts.Length > 1 ? parts[1] : response;

                if (isSearch)
                {
                    var matches = Regex.Matches(body, "<a[^>]*class=\"result__a\"[^>]*href=\"([^\"]+)\"[^>]*>");
                    int count = 0;
                    foreach (Match match in matches)
                    {
                        string link = match.Groups[1].Value;
                        Console.WriteLine($"{++count}. {link}");
                        if (count >= 10) break;
                    }

                    if (count == 0)
                    {
                        Console.WriteLine("No results found or parsing failed.");
                    }

                    return;
                }

                string cleanText = Regex.Replace(body, "<.*?>", string.Empty);
                Console.WriteLine(cleanText);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void PerformSearch(string searchTerm)
    {
        string encodedQuery = Uri.EscapeDataString(searchTerm);
        string url = $"http://html.duckduckgo.com/html/?q={encodedQuery}";
        MakeHttpRequest(url);
    }
}