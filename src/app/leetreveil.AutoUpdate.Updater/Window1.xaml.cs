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
using leetreveil.AutoUpdate.Core.Zip;
using System.Diagnostics;

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

            ExtractAndStartApplication(args[2],args[1]);

            //TODO: clean up update files after extraction
        }

        private void ExtractAndStartApplication(string updateFilePath, string applicationFilePath)
        {
            var extractor = new ZipFileExtractor(updateFilePath);
            extractor.ExtractTo(Environment.CurrentDirectory);

            Process.Start(applicationFilePath);
        }
    }
}
