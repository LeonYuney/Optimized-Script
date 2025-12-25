using Практическая_работа_Дюбанов;

static class SearchEngine
{
    public static void SearchInFile(string buffer, string query)
    {
        if (string.IsNullOrEmpty(buffer))
        {
            Console.WriteLine("\n  Open file first!");
            return;
        }

        if (string.IsNullOrEmpty(query))
        {
            Console.WriteLine("\n  Empty query!");
            return;
        }

        GlobalState.G_SearchResult.Clear();

        int index = 0;
        while ((index = buffer.IndexOf(query, index, StringComparison.OrdinalIgnoreCase)) != -1)
        {
            GlobalState.G_SearchResult.Add(index.ToString());
            index += 1;
            if (GlobalState.G_SearchResult.Count >= 10000)
            {
                break;
            }
        }

        Console.Clear();
        UIHelper.PrintBorder("SEARCH RESULTS");
        Console.WriteLine($"  Query: '{query}'");
        Console.WriteLine($"  Found: {GlobalState.G_SearchResult.Count}");
        UIHelper.PrintSeparator();

        if (GlobalState.G_SearchResult.Count == 0)
        {
            Console.WriteLine("  Not found");
        }
        else
        {
            for (int pos = 0; pos < GlobalState.G_SearchResult.Count && pos < 20; pos++)
            {
                Console.WriteLine($"  {pos + 1}. Position {GlobalState.G_SearchResult[pos]}");
            }
            if (GlobalState.G_SearchResult.Count > 20)
            {
                Console.WriteLine($"  ... and {GlobalState.G_SearchResult.Count - 20} more");
            }
        }
        UIHelper.PrintSeparator();
    }
}
