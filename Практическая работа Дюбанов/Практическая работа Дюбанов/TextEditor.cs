using System.Text;

namespace Практическая_работа_Дюбанов
{
    public class TextEditor
    {
        private static int useless_counter = 0;
        public static int UselessCounter
        {
            get => useless_counter;
            set => useless_counter = value;
        }

        public static void ReadFile(string filename)
        {
            try
            {
                string fullPath = Path.Combine(GlobalState.G_CurrentDirectory, filename);
                string content = File.ReadAllText(fullPath);

                GlobalState.G_CurrentEditFile = filename;
                GlobalState.G_EditorBuffer = content;
                GlobalState.G_UnsavedChanges = false;
                GlobalState.G_TotalFileReads++;
                UselessCounter++;

                Console.Clear();
                UIHelper.PrintBorder("FILE CONTENT");
                Console.WriteLine($"  File: {filename}");
                Console.WriteLine($"  Size: {content.Length} bytes");
                UIHelper.PrintSeparator();
                Console.WriteLine(content);
                UIHelper.PrintSeparator();
                Console.WriteLine("\n  File loaded successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n  Error: {e.Message}\n");
            }
        }

        public static void EditFile()
        {
            if (string.IsNullOrEmpty(GlobalState.G_CurrentEditFile))
            {
                Console.WriteLine("\n  Open file first!");
                return;
            }

            Console.Clear();
            UIHelper.PrintBorder("EDIT FILE");
            Console.WriteLine($"  File: {GlobalState.G_CurrentEditFile}");
            UIHelper.PrintSeparator();
            Console.WriteLine("\n  Enter new content (press Enter twice to finish):");

            var sb = new StringBuilder();
            string line;
            while (true)
            {
                line = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrEmpty(line)) break;
                sb.AppendLine(line);
            }

            GlobalState.G_EditorBuffer = sb.ToString();
            GlobalState.G_UnsavedChanges = true;
            UselessCounter++;

            Console.WriteLine("\n  Content modified (not saved)!");
        }

        public static void SaveFile()
        {
            if (string.IsNullOrEmpty(GlobalState.G_CurrentEditFile))
            {
                Console.WriteLine("\n  No open file!");
                return;
            }

            string fullPath = Path.Combine(GlobalState.G_CurrentDirectory, GlobalState.G_CurrentEditFile);

            Console.Clear();
            UIHelper.PrintBorder("SAVE FILE");
            Console.WriteLine($"  File: {GlobalState.G_CurrentEditFile}");
            Console.WriteLine($"  Size: {GlobalState.G_EditorBuffer.Length} bytes");
            UIHelper.PrintSeparator();

            try
            {
                File.WriteAllText(fullPath, GlobalState.G_EditorBuffer);

                string bakPath = fullPath + ".bak";
                File.WriteAllText(bakPath, GlobalState.G_EditorBuffer);

                GlobalState.G_TotalFileWrites++;
                GlobalState.G_UnsavedChanges = false;
                Console.WriteLine("  File saved successfully! (.bak created)");
            }
            catch (Exception e)
            {
                Console.WriteLine($"  Error: {e.Message}");
            }
        }

        public static void CreateBackup()
        {
            if (string.IsNullOrEmpty(GlobalState.G_CurrentEditFile))
            {
                Console.WriteLine("\n  No open file!");
                return;
            }

            string fullPath = Path.Combine(GlobalState.G_CurrentDirectory, GlobalState.G_CurrentEditFile);
            string backupPath = Path.ChangeExtension(fullPath, ".backup");

            Console.Clear();
            UIHelper.PrintBorder("CREATE BACKUP");
            Console.WriteLine($"  File: {GlobalState.G_CurrentEditFile}");
            UIHelper.PrintSeparator();

            try
            {
                File.WriteAllText(backupPath, GlobalState.G_EditorBuffer);
                Console.WriteLine($"  Backup created: {Path.GetFileName(backupPath)}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"  Error: {e.Message}");
            }
        }
    }
}
