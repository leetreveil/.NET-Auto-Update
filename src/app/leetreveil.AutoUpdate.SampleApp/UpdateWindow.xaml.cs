using System;
using System.Windows;
using leetreveil.AutoUpdate.Framework;

namespace leetreveil.AutoUpdate.SampleApp
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private readonly Update _newUpdate;

        public UpdateWindow(Update newUpdate)
        {
            _newUpdate = newUpdate;
            InitializeComponent();

            this.DataContext = this;
        }

        public Update Update
        {
            get { return _newUpdate; }
        }

        private void InstallNow_Click(object sender, RoutedEventArgs e)
        {   
            UpdateManager.Instance.ApplyUpdate();
        }
    }
}