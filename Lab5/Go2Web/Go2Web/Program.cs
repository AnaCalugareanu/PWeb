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
        // Placeholder for now
        Console.WriteLine($"Would fetch URL: {url}");
    }

    private static void PerformSearch(string searchTerm)
    {
        // Placeholder for now
        Console.WriteLine($"Would search for: {searchTerm}");
    }
}