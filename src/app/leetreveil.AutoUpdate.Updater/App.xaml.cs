using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Reflection;
using System.Diagnostics;

namespace leetreveil.AutoUpdate.Updater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //load this if update is available
            string[] strings = Environment.GetCommandLineArgs();
            //string pathToExeWithResourceDictionary = strings[1];

            //Assembly loadedAssembly = Assembly.LoadFrom(pathToExeWithResourceDictionary);

            //string loadThisStr = String.Format(@"/{0};component/{1}", loadedAssembly.GetName().Name, "LoadThis.xaml");

            //Uri loadThisUri = new Uri(loadThisStr, UriKind.Relative);

            //this.StartupUri = loadThisUri;
        }
    }
}
