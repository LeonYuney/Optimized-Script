namespace Практическая_работа_Дюбанов
{
    public class Utils
    {
        public static void PerformQuickSort(List<string> arr)
        {
            if (arr == null || arr.Count < 2) return;

            QuickSortRecursive(arr, 0, arr.Count - 1);
        }

        private static void QuickSortRecursive(List<string> arr, int low, int high)
        {
            if (low < high)
            {
                int pi = Partition(arr, low, high);
                QuickSortRecursive(arr, low, pi - 1);
                QuickSortRecursive(arr, pi + 1, high);
            }
        }

        private static int Partition(List<string> arr, int low, int high)
        {
            string pivot = arr[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (string.CompareOrdinal(arr[j], pivot) <= 0)
                {
                    i++;
                    (arr[i], arr[j]) = (arr[j], arr[i]);
                }
            }

            (arr[i + 1], arr[high]) = (arr[high], arr[i + 1]);
            return i + 1;
        }


        public static void AnalyzeFileProperties(string filepath)
        {
            try
            {
                Console.Clear();
                UIHelper.PrintBorder("FILE ANALYSIS");

                var fi = new FileInfo(filepath);
                long size = fi.Length;

                // Стриминг — НУЛЬ утечек!
                int lines = 0, words = 0;
                using (var reader = new StreamReader(filepath))
                {
                    string line;
                    while ((line = reader.ReadLine()?.Trim() ?? "") != null)
                    {
                        lines++;
                        words += line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
                    }
                }

                Console.WriteLine($"  Name: {fi.Name}");
                Console.WriteLine($"  Size: {size:N0} bytes ({size / 1024:N1} KB)");
                Console.WriteLine($"  Lines: {lines:N0}");
                Console.WriteLine($"  Words: {words:N0}");
                UIHelper.PrintSeparator();
            }
            catch (Exception e)
            {
                Console.WriteLine($"  Error: {e.Message}");
            }
        }

        public static void DangerouslyTouchFile(string path)
        {
            // С подтверждением — не портить файлы случайно
            Console.Write($"  Touch '{path}'? (y/N): ");
            if (Console.ReadLine()?.Trim().ToLower() != "y")
            {
                return;
            }
            try
            {
                File.AppendAllText(path, "\n#touched#");
                Console.WriteLine("  File touched!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"  Error: {e.Message}");
            }
        }
    }
}