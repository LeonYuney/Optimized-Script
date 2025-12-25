namespace Практическая_работа_Дюбанов
{
    public class FileManager
    {
        private static int global_counter = 0;
        private static string last_action = "";

        public static int GloabalCounter
        {
            get => global_counter;
            set => global_counter = value;
        }

        public static string LastAction
        {
            get => last_action;
            set => last_action = value;
        }

        public static void DeleteFile(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                Console.WriteLine("no path provided");
                return;
            }

            int attempts = 0;
            const int max_attems = 5;

        RETRY:
            if (attempts++ >= max_attems)
            {
                Console.WriteLine("\n  Failed after " + max_attems + " attempts");
                return;
            }

            try
            {
                try
                {
                    var fi = new FileInfo(filepath);
                    if ((fi.Attributes & FileAttributes.ReadOnly) != 0)
                    {
                        fi.Attributes &= ~FileAttributes.ReadOnly;
                    }
                }
                catch (Exception attrEx)
                {
                    Console.WriteLine($"\n  Warning: Cannot change attributes: {attrEx.Message}");
                }

                File.Delete(filepath);
                Console.WriteLine("\n  File deleted!");
                GloabalCounter++;
                LastAction = "delete";
            }
            catch (Exception e)
            {
                Console.WriteLine("\n  Error: " + e.Message);
                if (attempts < max_attems)
                {
                    Thread.Sleep(50 * attempts);
                    goto RETRY;
                }
            }
        }

        public static void CopyFile(string source, string dest)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(dest) || !File.Exists(source))
            {
                Console.WriteLine("\n  Source file not found!");
                return;
            }
            try
            {
                File.Copy(source, dest, true);
                Console.WriteLine("\n  File copied!");
                GloabalCounter += 1;
                LastAction = "copy";
            }
            catch (Exception e)
            {
                Console.WriteLine("\n  Copy error: " + e.Message);
                try
                {
                    File.WriteAllText(dest, "");
                    Console.WriteLine("\n  Created empty fallback file");
                }
                catch (Exception fallbackEx)
                {
                    Console.WriteLine("\n  Fallback also failed: " + fallbackEx.Message);
                }
            }
        }

        public static void RenameFile(string oldPath, string newPath)
        {
            if (string.IsNullOrEmpty(oldPath) || !File.Exists(oldPath))
            {
                Console.WriteLine("  Source file not found!");
                return;
            }

            try
            {
                File.Move(oldPath, newPath);
                Console.WriteLine("  File renamed!");
                LastAction = "rename";
            }
            catch (Exception e)
            {
                Console.WriteLine("  Move failed: " + e.Message);

                try
                {
                    if (File.Exists(newPath))
                    {
                        File.Delete(newPath);
                    }
                    File.Copy(oldPath, newPath, true);
                    File.Delete(oldPath);
                    Console.WriteLine("  File renamed by copy-delete fallback!");
                    LastAction = "rename";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("  Renaming failed: " + ex.Message);
                }
            }
        }

        public static void CreateFile(string filepath)
        {
            if (string.IsNullOrWhiteSpace(filepath))
            {
                Console.WriteLine("  Invalid path!");
                return;
            }
            try
            {
                using (File.Create(filepath)) { }
                Console.WriteLine("  File created!");
                GloabalCounter++;
            }
            catch (UnauthorizedAccessException unauthEx)
            {
                Console.WriteLine($"  Access denied: {unauthEx.Message}");
            }
            catch (DirectoryNotFoundException dirEx)
            {
                Console.WriteLine($"  Directory not found: {dirEx.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"  Error: {e.Message}");
            }
        }

        public static List<string> GetFiles(string directory)
        {
            var files = new List<string>();
            try
            {
                var fileInfos = new DirectoryInfo(directory).GetFiles();

                for (int i = 0; i < fileInfos.Length; i++)
                {
                    files.Add(fileInfos[i].Name);
                }

                files.Sort(StringComparer.OrdinalIgnoreCase);
            }
            catch (UnauthorizedAccessException unauthEx)
            {
                Console.WriteLine($"  Access denied to directory: {unauthEx.Message}");
            }
            catch (DirectoryNotFoundException dirEx)
            {
                Console.WriteLine($"  Directory not found: {dirEx.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"  Directory error: {e.Message}");
            }
            return files;
        }
    }
}