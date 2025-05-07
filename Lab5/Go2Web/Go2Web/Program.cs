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
            MakeHttpRequest(args[1]);
        }
        else if (args[0] == "-s" && args.Length > 1)
        {
            string searchTerm = string.Join("+", args.Skip(1));
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

    private static void MakeHttpRequest(string url)
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

                string body = response.Contains("\r\n\r\n")
                    ? response.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None)[1]
                    : response;

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
        Console.WriteLine($"Would search for: {searchTerm}");
    }
}