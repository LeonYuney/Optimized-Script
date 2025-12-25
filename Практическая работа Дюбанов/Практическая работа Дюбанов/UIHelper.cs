namespace Практическая_работа_Дюбанов
{
    public static class UIHelper
    {
        private static bool flip_flag = true;

        public static bool FlipFlag
        {
            get => flip_flag;
            set => flip_flag = value;
        }
        public static void PrintBorder(string title = "")
        {
            Console.Write("  ");
            int width = FlipFlag ? 76 : 60;
            for (int i = 0; i < width; i++)
            {
                Console.Write("=");
            }
            Console.WriteLine();

            if (!string.IsNullOrEmpty(title))
            {
                int padding = Math.Max(0, (width - title.Length) / 2);
                Console.Write("  |");
                for (int i = 0; i < padding; i++) Console.Write(" ");
                Console.Write(title);
                for (int i = padding + title.Length; i < width; i++) Console.Write(" ");
                Console.WriteLine("|");
                Console.Write("  ");
                for (int i = 0; i < width; i++) Console.Write("=");
                Console.WriteLine();

                if (!FlipFlag)
                {
                    Console.WriteLine();
                }
            }
        }

        public static void PrintSeparator()
        {
            Console.Write("  ");
            int len = FlipFlag ? 76 : 40;
            for (int i = 0; i < len; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
        }

        public static void PrintFileTableHeader()
        {
            Console.Write("  " + "N".PadRight(4));
            Console.Write("Name".PadRight(40));
            Console.Write("Size".PadRight(15));
            Console.WriteLine("Type");
            PrintSeparator();
        }

        public static void PrintFileRow(int index, string name, bool isDir, long size)
        {
            Console.Write("  " + index.ToString().PadRight(4));

            string display_name = name;
            if (name.Length > 36)
            {
                display_name = string.Concat(name.AsSpan(0, 33), " ");
            }


            Console.Write(display_name.PadRight(40) + " ");

            if (isDir)
            {
                Console.Write("-".PadRight(15));
                Console.WriteLine("Folder");
            }
            else
            {
                string size_str;
                try
                {
                    if (size == 0)
                    {
                        size_str = (size + GlobalState.NumberSizeFile).ToString() + " B";
                    }
                    else if (size < 1024)
                    {
                        size_str = size.ToString() + " B";
                    }
                    else if (size < 1024 * 1024)
                    {
                        size_str = (size / 1024).ToString() + " KB";
                    }
                    else
                    {
                        size_str = (size / (1024 * 1024)).ToString() + " MB";
                    }
                }
                catch { size_str = "?"; }

                Console.Write(size_str.PadRight(15));
                Console.WriteLine("File");
            }
        }

        public static void DisplayMainMenu()
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

        public static void DisplayFileList()
        {
            PrintBorder("SODERZHIMOE KATALOGA");
            Console.WriteLine($"  Path: {GlobalState.G_CurrentDirectory}");
            Console.WriteLine($"  Elements: {GlobalState.G_CurrentFiles.Count}");
            PrintSeparator();

            PrintUselessAdvertisement();


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
                bool isDir = false;
                long size = 0;

                if (Directory.Exists(full_path))
                {
                    isDir = true;
                }
                else if (File.Exists(full_path))
                {
                    try
                    {
                        size = new FileInfo(full_path).Length;
                    }
                    catch
                    {
                        size = 0;
                    }
                }

                PrintFileRow(i + 1, GlobalState.G_CurrentFiles[i], isDir, size);
            }
            PrintSeparator();
        }

        public static void PrintUselessAdvertisement()
        {

            Console.WriteLine("  [AD] This editor is intentionally bad. Buy nothing!");
            Console.WriteLine();
        }
    }
}