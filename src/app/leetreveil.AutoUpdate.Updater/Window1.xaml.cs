using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Resources;
using System.Diagnostics;
using leetreveil.AutoUpdate.Updater.Zip;

namespace leetreveil.AutoUpdate.Updater
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();

            var compressedUpdateFile = args[2];
            var appPath = args[1];

            if (!File.Exists(compressedUpdateFile))
                throw new FileNotFoundException();

            ExtractAndStartApplication(compressedUpdateFile,appPath);

            //TODO: clean up update file after extraction

            if (File.Exists(compressedUpdateFile))
                File.Delete(compressedUpdateFile);
        }

        private void ExtractAndStartApplication(string updateFilePath, string applicationFilePath)
        {
            var extractor = new ZipFileExtractor(updateFilePath);
            extractor.ExtractTo(Environment.CurrentDirectory);

            Process.Start(applicationFilePath);
        }
    }
}
