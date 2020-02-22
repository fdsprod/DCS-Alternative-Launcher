using System;
using System.Diagnostics;
using System.IO;

namespace AutoUpdate
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Debugger.Launch();

            var directory = Directory.GetCurrentDirectory();
            var updateDirectory = Path.Combine(directory, "_update");
            var exePath = Path.Combine(directory, "DCS Alternative Launcher.exe");

            if (Directory.Exists(updateDirectory))
            {
                RecursiveUpdate(updateDirectory, directory);
                Directory.Delete(updateDirectory, true);
            }

            Process.Start(exePath);
        }
        
        private static void RecursiveUpdate(string sourceDirectory, string destDirectory)
        {
            foreach (var file in Directory.GetFiles(sourceDirectory))
            {
                var destFile = Path.Combine(destDirectory, Path.GetFileName(file));
                var updatingPath = destFile + ".updating";

                try
                {
                    if (File.Exists(destFile))
                    {
                        File.Move(destFile, updatingPath);
                    }

                    File.Move(file, destFile);
                }
                catch (Exception e)
                {
                    if (File.Exists(updatingPath))
                    {
                        File.Move(updatingPath, destFile);
                    }
                }
                finally
                {
                    try
                    {
                        if (File.Exists(updatingPath))
                        {
                            File.Delete(updatingPath);
                        }

                        File.Delete(file);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            foreach (var folder in Directory.GetDirectories(sourceDirectory))
            {
                var name = Path.GetDirectoryName(folder);

                RecursiveUpdate(folder, Path.Combine(destDirectory, name));
            }
        }

    }
}
