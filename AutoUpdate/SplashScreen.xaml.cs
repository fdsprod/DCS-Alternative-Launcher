using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Path = System.Windows.Shapes.Path;

namespace AutoUpdate
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Update();
            });
        }

        public void Update()
        {
            var directory = Directory.GetCurrentDirectory();
            var updateDirectory = System.IO.Path.Combine(directory, "_update");
            var exePath = System.IO.Path.Combine(directory, "DCS Alternative Launcher.exe");

            updateStatus("Gathering data...");

            Dispatcher.Invoke(() => { return progress.Maximum = CountFiles(updateDirectory); });

            updateStatus("Applying update...");

            if (Directory.Exists(updateDirectory))
            {
                RecursiveUpdate(updateDirectory, directory);
            }

            Directory.Delete(updateDirectory, true);

            Process.Start(exePath);
            Process.GetCurrentProcess().Kill();
        }

        private static int CountFiles(string sourceDirectory)
        {
            return Directory.GetFiles(sourceDirectory).Length + Directory.GetDirectories(sourceDirectory).Sum(CountFiles);
        }

        private void RecursiveUpdate(string sourceDirectory, string destDirectory, int count = 0)
        {
            foreach (var file in Directory.GetFiles(sourceDirectory))
            {
                var destFile = System.IO.Path.Combine(destDirectory, System.IO.Path.GetFileName(file));
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

                    count++;
                    updateProgress(count);
                }
            }

            foreach (var folder in Directory.GetDirectories(sourceDirectory))
            {
                var name = System.IO.Path.GetDirectoryName(folder);

                RecursiveUpdate(folder, System.IO.Path.Combine(destDirectory, name), count);
            }
        }


        private void updateStatus(string value)
        {
            Dispatcher.Invoke(() => lblStatus.Text = value);
        }


        private void updateProgress(int count)
        {
               Dispatcher.Invoke(() => progress.Value = count);
        }
    }
}
