using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

internal class Program
{
    private static Dictionary<string, string> cache = new Dictionary<string, string>();

    private static void Main(string[] args)
    {
        if (args.Length == 0 || args[0] == "-h")
        {
            ShowHelp();
        }
        else if (args[0] == "-u" && args.Length > 1)
        {
            MakeHttpRequest(args[1], isSearch: false);
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

            string fileSafeUrl = Convert.ToBase64String(Encoding.UTF8.GetBytes(url))
                                .Replace('+', '-')
                                .Replace('/', '_')
                                .Replace('=', '.');
            string cachePath = Path.Combine("cache", fileSafeUrl + ".txt");

            if (File.Exists(cachePath))
            {
                Console.WriteLine("=== [CACHED RESPONSE] ===");
                string cachedResponse = File.ReadAllText(cachePath);
                ProcessHttpResponse(cachedResponse, isSearch);
                return;
            }
            bool isHttps = url.ToLower().StartsWith("https://");
            var port = isHttps ? 443 : 80;

            using (TcpClient client = new TcpClient(host, port))
            {
                Stream baseStream = client.GetStream();

                if (isHttps)
                {
                    var sslStream = new SslStream(baseStream, false);
                    sslStream.AuthenticateAsClient(host);
                    baseStream = sslStream; // overwrite with SSL stream
                }

                using (var writer = new StreamWriter(baseStream))
                using (var reader = new StreamReader(baseStream))
                {
                    writer.Write(
                            $"GET {path} HTTP/1.1\r\n" +
                            $"Host: {host}\r\n" +
                            $"User-Agent: go2web/1.0\r\n" +
                            $"Connection: close\r\n\r\n");
                    writer.Flush();

                    string response = reader.ReadToEnd();
                    Directory.CreateDirectory("cache");
                    File.WriteAllText(cachePath, response);
                    /*Console.WriteLine("=== RAW RESPONSE HEADERS AND BODY ===");
                    Console.WriteLine(response);*/

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

                    ProcessHttpResponse(response, isSearch);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void ProcessHttpResponse(string response, bool isSearch)
    {
        string[] parts = response.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);
        string body = parts.Length > 1 ? parts[1] : response;

        if (isSearch)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(body);

            var urlNodes = doc.DocumentNode.SelectNodes("//a[contains(@class, 'result__url')]");

            if (urlNodes == null || urlNodes.Count == 0)
            {
                Console.WriteLine("No valid links found.");
                return;
            }

            int count = 0;
            foreach (var node in urlNodes)
            {
                string urlText = node.InnerText.Trim();
                if (!string.IsNullOrEmpty(urlText))
                {
                    string cleanedUrl = urlText.Trim();

                    // Always start with https:// and remove any starting www.
                    cleanedUrl = cleanedUrl.Replace("www.", "");
                    if (!cleanedUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        cleanedUrl = "https://" + cleanedUrl;
                    }

                    Console.WriteLine(cleanedUrl);

                    count++;
                    if (count >= 10) break;
                }
            }

            return;
        }

        string cleanText = Regex.Replace(body, "<.*?>", string.Empty);
        Console.WriteLine(cleanText);
    }

    private static void PerformSearch(string searchTerm)
    {
        string encodedQuery = Uri.EscapeDataString(searchTerm);
        string url = $"https://html.duckduckgo.com/html/?q={encodedQuery}";
        MakeHttpRequest(url, true);
    }
}