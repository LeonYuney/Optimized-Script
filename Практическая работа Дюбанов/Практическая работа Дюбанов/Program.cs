using System.Text;

namespace Практическая_работа_Дюбанов
{
    public static class GlobalState
    {
        private static List<string> g_current_files = [];
        private static string g_current_directory = ".";
        private static string g_current_edit_file = "";
        private static string g_editor_buffer = "";
        private static List<string> g_search_results = [];
        private static int g_total_file_reads = 0;
        private static int g_total_file_writes = 0;
        private static bool g_unsaved_changes = false;
        private static Random bad_random = new();
        private static object random_lock = new();
        private static int number_size_file = 1337;
        public static int NumberSizeFile
        {
            get => number_size_file;
            set => number_size_file = value;
        }
        public static List<string> G_CurrentFiles
        {
            get => g_current_files;
            set => g_current_files = value ?? [];
        }
        public static string G_CurrentDirectory
        {
            get => g_current_directory;
            set => g_current_directory = value;
        }
        public static string G_CurrentEditFile
        {
            get => g_current_edit_file;
            set => g_current_edit_file = value;
        }
        public static string G_EditorBuffer
        {
            get => g_editor_buffer;
            set => g_editor_buffer = value;
        }
        public static List<string> G_SearchResult
        {
            get => g_search_results;
            set => g_search_results = value ?? [];
        }
        public static int G_TotalFileReads
        {
            get => g_total_file_reads;
            set => g_total_file_reads = value;
        }
        public static int G_TotalFileWrites
        {
            get => g_total_file_writes;
            set => g_total_file_writes = value;
        }
        public static bool G_UnsavedChanges
        {
            get => g_unsaved_changes;
            set => g_unsaved_changes = value;
        }
        public static Random BadRandom
        {
            get => bad_random;
            set => bad_random = value ?? new Random();
        }
        public static object RandomLock
        {
            get => random_lock;
            set => random_lock = value ?? new object();
        }
    }

    class Program
    {
        static void Main()
        {
            int choice = 0;
            bool first_run = true;

            while (choice != 99)
            {
                ClearScreen();
                ListFilesWithQuickSort();

                if (first_run)
                {
                    first_run = false;
                }

                DisplayMainMenu();
                Console.Write("\n  Ваш выбор: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    choice = -1;
                }

                switch (choice)
                {
                    case 1: ReadFile(); Console.Write("\n  Нажмите Enter для продолжения..."); Console.ReadLine(); break;
                    case 2: EditFile(); break;
                    case 3: SaveFile(); Console.Write("\n  Нажмите Enter для продолжения..."); Console.ReadLine(); break;
                    case 4: SearchInFile(); Console.Write("\n  Нажмите Enter для продолжения..."); Console.ReadLine(); break;
                    case 5: DeleteFile(); Console.Write("\n  Нажмите Enter для продолжения..."); Console.ReadLine(); break;
                    case 6: CreateNewFile(); Console.Write("\n  Нажмите Enter для продолжения..."); Console.ReadLine(); break;
                    case 7: NavigateDirectory(); break;
                    case 8: CopyFile(); Console.Write("\n  Нажмите Enter для продолжения..."); Console.ReadLine(); break;
                    case 9: RenameFile(); Console.Write("\n  Нажмите Enter для продолжения..."); Console.ReadLine(); break;
                    case 10: AnalyzeFileProperties(); Console.Write("\n  Нажмите Enter для продолжения..."); Console.ReadLine(); break;
                    case 11: CreateBackup(); Console.Write("\n  Нажмите Enter для продолжения..."); Console.ReadLine(); break;
                    case 99:
                        {
                            ClearScreen();
                            Console.WriteLine("\n\n");
                            PrintBorder("ВЫХОД");
                            Console.WriteLine("  Спасибо за использование Грязного Редактора!");
                            Console.WriteLine("  До свидания!");
                            PrintBorder();
                            Console.WriteLine("\n\n");
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("\n  ❌ Неверный выбор! Попробуйте снова.");
                            Console.Write("\n  Нажмите Enter для продолжения...");
                            Console.ReadLine();
                            break;
                        }
                }
            }
        }
        static void ClearScreen() => Console.Clear();

        static void PrintBorder(string title = "")
        {
            Console.Write("  ");
            for (int i = 0; i < 76; i++) Console.Write("=");
            Console.WriteLine();

            if (!string.IsNullOrEmpty(title))
            {
                int padding = (76 - title.Length) / 2;
                Console.Write("  |");
                for (int i = 0; i < padding; i++) Console.Write(" ");
                Console.Write(title);
                for (int i = padding + title.Length; i < 76; i++) Console.Write(" ");
                Console.WriteLine("|");
                Console.Write("  ");
                for (int i = 0; i < 76; i++) Console.Write("=");
                Console.WriteLine();
            }
        }

        static void PrintSeparator()
        {
            Console.Write("  ");
            for (int i = 0; i < 76; i++) Console.Write("-");
            Console.WriteLine();
        }

        static void DisplayFileList()
        {
            PrintBorder("SODERZHIMOE KATALOGA");
            Console.WriteLine($"  Path: {GlobalState.G_CurrentDirectory}");
            Console.WriteLine($"  Elements: {GlobalState.G_CurrentFiles.Count}");
            PrintSeparator();

            if (GlobalState.G_CurrentFiles.Count == 0)
            {
                Console.WriteLine("  Folder is empty");
                PrintSeparator();
                return;
            }

            PrintFileTableHeader();

            for (int i = 0; i < GlobalState.G_CurrentFiles.Count; i++)
            {
                string full_path = Path.Combine(GlobalState.G_CurrentDirectory, GlobalState.G_CurrentFiles[i]);
                bool isDir = Directory.Exists(full_path);
                long size = 0;

                if (!isDir && File.Exists(full_path))
                {
                    try { size = new FileInfo(full_path).Length; }
                    catch { size = 0; }
                }

                PrintFileRow(i + 1, GlobalState.G_CurrentFiles[i], isDir, size);
            }

            PrintSeparator();
        }

        static void PrintFileTableHeader()
        {
            Console.Write("  " + "N".PadRight(4));
            Console.Write("Name".PadRight(40));
            Console.Write("Size".PadRight(15));
            Console.WriteLine("Type");
            PrintSeparator();
        }

        static void PrintFileRow(int index, string name, bool isDir, long size)
        {
            Console.Write("  " + index.ToString().PadRight(4));

            string display_name = name.Length > 36 ? string.Concat(name.AsSpan(0, 33), "...") : name;
            Console.Write(display_name.PadRight(40));

            if (isDir)
            {
                Console.Write("-".PadRight(15));
                Console.WriteLine("Folder");
            }
            else
            {
                string size_str = size switch
                {
                    < 1024 => $"{size} B",
                    < 1024 * 1024 => $"{size / 1024} KB",
                    >= 1024 * 1024 => $"{size / (1024 * 1024)} MB" 
                };
                Console.Write(size_str.PadRight(15));
                Console.WriteLine("File");
            }

        }

        static void DisplayMainMenu()
        {
            PrintBorder("MAIN MENU");
            Console.WriteLine("  1.  Read file");
            Console.WriteLine("  2.  Edit file");
            Console.WriteLine("  3.  Save file");
            Console.WriteLine("  4.  Search in file");
            Console.WriteLine("  5.  Delete file");
            Console.WriteLine("  6.  Create new file");
            Console.WriteLine("  7.  Go to folder");
            Console.WriteLine("  8.  Copy file");
            Console.WriteLine("  9.  Rename file");
            Console.WriteLine("  10. File analysis");
            Console.WriteLine("  11. Backup");
            PrintSeparator();
            Console.WriteLine("  99. Exit");
            PrintBorder();
        }

        // Быстрая сортировка
        static void QuickSortIterative(List<string> arr, int low, int high)
        {
            if (low >= high) return;

            var stack = new Stack<(int l, int h)>();
            stack.Push((low, high));

            while (stack.Count > 0)
            {
                var (l, h) = stack.Pop();
                if (l >= h) continue;

                int pivotIndex = Partition(arr, l, h);

                // Оптимизация порядка push для лучшего кэш-использования
                if (pivotIndex - l < h - pivotIndex)
                {
                    stack.Push((pivotIndex + 1, h));
                    stack.Push((l, pivotIndex - 1));
                }
                else
                {
                    stack.Push((l, pivotIndex - 1));
                    stack.Push((pivotIndex + 1, h));
                }
            }
        }

        static int Partition(List<string> arr, int low, int high)
        {
            string pivot = arr[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (string.CompareOrdinal(arr[j], pivot) <= 0) //используем CompareOrdinal вместо StringComparer
                {
                    i++;
                    (arr[i], arr[j]) = (arr[j], arr[i]);
                }
            }

            (arr[i + 1], arr[high]) = (arr[high], arr[i + 1]);
            return i + 1;
        }

        static void ListFilesWithQuickSort()
        {
            try
            {
                GlobalState.G_CurrentFiles.Clear();

                var dirInfo = new DirectoryInfo(GlobalState.G_CurrentDirectory);
                var fileSystemEntries = dirInfo.GetFileSystemInfos();

                //резервируем память заранее
                GlobalState.G_CurrentFiles.Capacity = fileSystemEntries.Length;

                foreach (var entry in fileSystemEntries)
                {
                    GlobalState.G_CurrentFiles.Add(Path.GetFileName(entry.FullName));
                }

                //убрали случайную перестановку - она бессмысленна для сортировки
                QuickSortIterative(GlobalState.G_CurrentFiles, 0, GlobalState.G_CurrentFiles.Count - 1);
                DisplayFileList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в ListFilesWithBubbleSort: {ex.Message}");
            }
        }

        static void ReadFile()
        {
            var files = GlobalState.G_CurrentFiles;
            if (files.Count == 0)
            {
                Console.WriteLine("\n  Folder is empty!");
                return;
            }

            Console.Write("\n  Choose file number: ");
            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice) ||
                choice < 1 || choice > files.Count)
            {
                Console.WriteLine("  Invalid choice!");
                return;
            }

            string selectedName = files[choice - 1];
            string fullPath = Path.Combine(GlobalState.G_CurrentDirectory, selectedName);

            if (Directory.Exists(fullPath))
            {
                Console.WriteLine("  This is a folder!");
                return;
            }

            try
            {
                string content = File.ReadAllText(fullPath, Encoding.UTF8);

                GlobalState.G_CurrentEditFile = fullPath;
                GlobalState.G_EditorBuffer = content;
                GlobalState.G_UnsavedChanges = false;
                GlobalState.G_TotalFileReads++;

                ClearScreen();
                PrintBorder("FILE CONTENT");
                Console.WriteLine($"  File: {fullPath}");
                Console.WriteLine($"  Size: {content.Length} bytes");
                PrintSeparator();
                Console.WriteLine(content);
                PrintSeparator();
                Console.WriteLine("\n  File loaded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n  Error reading file: {ex.Message}\n");
            }
        }

        static void EditFile()
        {
            if (string.IsNullOrEmpty(GlobalState.G_CurrentEditFile))
            {
                Console.WriteLine("\n  Open file first!");
                return;
            }

            ClearScreen();
            PrintBorder("EDIT FILE");
            Console.WriteLine($"  File: {GlobalState.G_CurrentEditFile}");
            Console.WriteLine($"  Size: {GlobalState.G_EditorBuffer.Length} bytes");
            PrintSeparator();
            Console.WriteLine("\n  Enter new content. Type 'EOF' on a new line to finish:");
            PrintSeparator();

            var lines = new List<string>();
            while (true)
            {
                string? rawInput = Console.ReadLine();
                if (rawInput == null || rawInput.Trim() == "EOF") break;
                lines.Add(rawInput);
            }

            GlobalState.G_EditorBuffer = string.Join("\n", lines) + "\n";
            GlobalState.G_UnsavedChanges = true;
            Console.WriteLine("\n  Content modified (not saved)!");
        }

        static void SaveFile()
        {
            string filePath = GlobalState.G_CurrentEditFile;
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("\n  No open file!");
                return;
            }

            ClearScreen();
            PrintBorder("SAVE FILE");
            Console.WriteLine($"  File: {filePath}");
            Console.WriteLine($"  Size: {GlobalState.G_EditorBuffer.Length} bytes");
            PrintSeparator();

            try
            {
                File.WriteAllText(filePath, GlobalState.G_EditorBuffer, Encoding.UTF8);
                GlobalState.G_TotalFileWrites++;
                GlobalState.G_UnsavedChanges = false;
                Console.WriteLine("  File saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Failed to save file: {ex.Message}");
            }
        }

        static void SearchInFile()
        {
            if (string.IsNullOrEmpty(GlobalState.G_EditorBuffer))
            {
                Console.WriteLine("\n  Open file first!");
                return;
            }

            ClearScreen();
            PrintBorder("SEARCH");
            Console.Write("\n  Enter search query: ");
            string query = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(query))
            {
                Console.WriteLine("\n  Empty query!");
                return;
            }

            GlobalState.G_SearchResult.Clear();

            int index = 0;
            const int MAX_RESULTS = 10000;
            while (GlobalState.G_SearchResult.Count < MAX_RESULTS &&
                   (index = GlobalState.G_EditorBuffer.IndexOf(query, index,
                    StringComparison.OrdinalIgnoreCase)) != -1)
            {
                GlobalState.G_SearchResult.Add(index.ToString());
                index += query.Length;
            }

            ClearScreen();
            PrintBorder("SEARCH RESULTS");
            Console.WriteLine($"  Query: '{query}'");
            Console.WriteLine($"  Found: {GlobalState.G_SearchResult.Count}");
            PrintSeparator();

            if (GlobalState.G_SearchResult.Count == 0)
            {
                Console.WriteLine("  Not found");
            }
            else
            {
                int limit = Math.Min(20, GlobalState.G_SearchResult.Count);
                for (int pos = 0; pos < limit; pos++)
                {
                    Console.WriteLine($"  {pos + 1}. Position {GlobalState.G_SearchResult[pos]}");
                }

                if (GlobalState.G_SearchResult.Count > 20)
                {
                    Console.WriteLine($"  ... and {GlobalState.G_SearchResult.Count - 20} more");
                }
            }
            PrintSeparator();
        }

        static void DeleteFile()
        {
            if (GlobalState.G_CurrentFiles.Count == 0)
            {
                Console.WriteLine("\n  No files!");
                return;
            }

            Console.Write("\n  Choose file number: ");
            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice) ||
                choice < 1 || choice > GlobalState.G_CurrentFiles.Count)
            {
                Console.WriteLine("  Invalid choice!");
                return;
            }

            string filepath = Path.Combine(GlobalState.G_CurrentDirectory, GlobalState.G_CurrentFiles[choice - 1]);

            if (Directory.Exists(filepath))
            {
                Console.WriteLine("  This is a folder!");
                return;
            }

            try
            {
                File.Delete(filepath);
                Console.WriteLine("\n  File deleted!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n  Error: {e.Message}\n");
            }
        }

        static void CreateNewFile()
        {
            Console.Write("\n  Enter filename: ");
            string filename = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(filename) || filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                Console.WriteLine("  Invalid filename!");
                return;
            }

            if (Path.GetExtension(filename) == "") filename += ".txt";

            string filepath = Path.Combine(GlobalState.G_CurrentDirectory, filename);

            if (File.Exists(filepath))
            {
                Console.Write("  File exists. Overwrite? (y/N): ");
                if (Console.ReadLine()?.Trim().ToLower() != "y")
                {
                    Console.WriteLine("  Cancelled.");
                    return;
                }
            }

            try
            {
                File.WriteAllText(filepath, "");
                Console.WriteLine("  File created!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"  Error: {e.Message}");
            }
        }

        static void NavigateDirectory()
        {
            Console.Write("\n  Enter path (.. for parent): ");
            string newdir = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(newdir)) return;

            if (newdir == "..")
            {
                var parent = Directory.GetParent(GlobalState.G_CurrentDirectory);
                if (parent != null)
                {
                    GlobalState.G_CurrentDirectory = parent.FullName;
                    Console.WriteLine($"  Changed to: {GlobalState.G_CurrentDirectory}");
                }
                else
                {
                    Console.WriteLine("  Already at root!");
                }
                return;
            }

            string test_path = Path.IsPathRooted(newdir) ? newdir : Path.Combine(GlobalState.G_CurrentDirectory, newdir);

            if (Directory.Exists(test_path))
            {
                GlobalState.G_CurrentDirectory = Path.GetFullPath(test_path);
                Console.WriteLine($"  Changed to: {GlobalState.G_CurrentDirectory}");
            }
            else
            {
                Console.WriteLine("  Folder not found!");
            }
        }

        static void CopyFile()
        {
            if (GlobalState.G_CurrentFiles.Count == 0)
            {
                Console.WriteLine("\n  No files!");
                return;
            }

            Console.Write("\n  Choose file number: ");
            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice) ||
                choice < 1 || choice > GlobalState.G_CurrentFiles.Count)
            {
                Console.WriteLine("  Invalid choice!");
                return;
            }

            string source = Path.Combine(GlobalState.G_CurrentDirectory, GlobalState.G_CurrentFiles[choice - 1]);

            Console.Write("  Enter copy name (or press Enter for 'filename.copy'): ");
            string copyName = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrEmpty(copyName))
                copyName = Path.GetFileNameWithoutExtension(source) + ".copy";

            string dest = Path.Combine(GlobalState.G_CurrentDirectory, copyName);

            try
            {
                File.Copy(source, dest, true);
                Console.WriteLine("\n  File copied!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n  Error: {e.Message}\n");
            }
        }

        static void RenameFile()
        {
            if (GlobalState.G_CurrentFiles.Count == 0)
            {
                Console.WriteLine("\n  No files!");
                return;
            }

            Console.Write("\n  Choose file number: ");
            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice) ||
                choice < 1 || choice > GlobalState.G_CurrentFiles.Count)
            {
                Console.WriteLine("  Invalid choice!");
                return;
            }

            Console.Write("  Enter new name: ");
            string new_name = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrEmpty(new_name) || new_name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                Console.WriteLine("  Invalid filename!");
                return;
            }

            string old_name = GlobalState.G_CurrentFiles[choice - 1];
            string old_ext = Path.GetExtension(old_name);
            if (string.IsNullOrEmpty(Path.GetExtension(new_name)) && !string.IsNullOrEmpty(old_ext))
                new_name += old_ext;

            string old_path = Path.Combine(GlobalState.G_CurrentDirectory, old_name);
            string new_path = Path.Combine(GlobalState.G_CurrentDirectory, new_name);

            if (File.Exists(new_path))
            {
                Console.Write("  File exists. Overwrite? (y/N): ");
                if (Console.ReadLine()?.Trim().ToLower() != "y")
                {
                    Console.WriteLine("  Cancelled.");
                    return;
                }
            }

            try
            {
                File.Move(old_path, new_path, overwrite: true);
                Console.WriteLine("  File renamed!");
                GlobalState.G_CurrentFiles[choice - 1] = new_name;
            }
            catch (Exception e)
            {
                Console.WriteLine($"  Error: {e.Message}");
            }
        }

        internal static readonly char[] separator = [' ', '\t'];

        static void AnalyzeFileProperties()
        {
            if (GlobalState.G_CurrentFiles.Count == 0)
            {
                Console.WriteLine("\n  No files!");
                return;
            }

            Console.Write("\n  Choose file number: ");
            if (!int.TryParse(Console.ReadLine()?.Trim(), out int choice) ||
                choice < 1 || choice > GlobalState.G_CurrentFiles.Count)
            {
                Console.WriteLine("  Invalid choice!");
                return;
            }

            string filepath = Path.Combine(GlobalState.G_CurrentDirectory, GlobalState.G_CurrentFiles[choice - 1]);

            try
            {
                ClearScreen();
                PrintBorder("FILE ANALYSIS");

                var fi = new FileInfo(filepath);
                long size = fi.Length;

                int lines = 0, words = 0;
                long chars = 0; //long вместо int для больших файлов

                using var reader = new StreamReader(filepath, Encoding.UTF8);
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    string trimmed = line.Trim().ToLowerInvariant(); //ОПТИМИЗАЦИЯ: вынесли Trim+ToLower
                    lines++;
                    chars += line.Length + 1; // Считаем все символы, включая пробелы
                    words += trimmed.Split(separator, StringSplitOptions.RemoveEmptyEntries).Length;
                }

                Console.WriteLine($"  Name: {GlobalState.G_CurrentFiles[choice - 1]}");
                Console.WriteLine($"  Size: {size:N0} bytes ({size / 1024:N1} KB)");
                Console.WriteLine($"  Lines: {lines:N0}");
                Console.WriteLine($"  Words: {words:N0}");
                Console.WriteLine($"  Chars: {chars:N0}");
                PrintSeparator();
            }
            catch (Exception e)
            {
                Console.WriteLine($"  Error: {e.Message}");
            }
        }

        static void CreateBackup()
        {
            if (string.IsNullOrEmpty(GlobalState.G_CurrentEditFile))
            {
                Console.WriteLine("\n  No open file!");
                return;
            }

            string fullEditPath = Path.Combine(GlobalState.G_CurrentDirectory, GlobalState.G_CurrentEditFile);
            if (!File.Exists(fullEditPath))
            {
                Console.WriteLine("\n  Edit file not found!");
                return;
            }

            ClearScreen();
            PrintBorder("CREATE BACKUP");
            Console.WriteLine($"  File: {GlobalState.G_CurrentEditFile}");
            PrintSeparator();

            try
            {
                string backup_name = Path.Combine(GlobalState.G_CurrentDirectory,
                    Path.GetFileNameWithoutExtension(GlobalState.G_CurrentEditFile) + ".backup");
                File.WriteAllText(backup_name, GlobalState.G_EditorBuffer, Encoding.UTF8);
                Console.WriteLine($"  Backup created: {Path.GetFileName(backup_name)}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"  Error: {e.Message}");
            }
        }
    }
}
