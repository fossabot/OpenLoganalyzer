﻿using Microsoft.Win32;
using OpenLoganalyzer.Core.Extensions;
using OpenLoganalyzer.Core.Interfaces;
using OpenLoganalyzer.Core.Settings;
using OpenLoganalyzer.Core.Style;
using OpenLoganalyzer.Windows;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace OpenLoganalyzer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ThemeManager themeManager;

        private readonly ISettingsManager settingsManager;

        private ISettings settings;

        public MainWindow()
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string themeFolder = appdata + @"\OpenLoganalyzer\Themes\";
            appdata += @"\OpenLoganalyzer\settings.json";
            settingsManager = new SettingsManager(appdata);

            LoadSettings();
            
            InitializeComponent();

            themeManager = new ThemeManager();
            themeManager.ScanFolder(themeFolder);
            this.ChangeStyle(settings, themeManager);

            settingsManager.Save(settings);

            BuildMenu();
        }

        private void LoadSettings()
        {
            ISettings newSettings = settingsManager.Load();
            if (newSettings == null)
            {
                newSettings = new Settings();
            }
            settings = newSettings;
            
        }

        private void BuildMenu()
        {
            MI_Style.Items.Clear();
            foreach (StyleDict style in themeManager.Styles)
            {
                MenuItem item = new MenuItem()
                {
                    Style = Resources["SubMenuItem"] as Style,
                    Name = "MI_"+style.Name,
                    Header = style.Name,
                    IsCheckable = true,
                    Tag = style
                };
                if (item.Header.ToString() == settings.GetSetting("theme"))
                {
                    item.IsChecked = true;
                }
                item.Click += Style_Click;
                MI_Style.Items.Add(item);
            }
        }

        private void Style_Click(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() != typeof(MenuItem))
            {
                return;
            }
            MenuItem item = (MenuItem)sender;
            if (item.Tag == null || item.Tag.GetType() != typeof(StyleDict))
            {
                return;
            }

            StyleDict style = (StyleDict)item.Tag;
            this.ChangeStyle(style.GetDictionary());
            settings.AddSetting("theme", style.Name);
            settingsManager.Save(settings);
            BuildMenu();
        }

        private void MI_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
        }

        private void MI_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CB_FilterBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender.GetType() != typeof(ComboBox))
            {
                return;
            }
            ComboBox box = (ComboBox)sender;
            if (box.Items.Count == 0)
            {
                box.Visibility = Visibility.Hidden;
                L_Filter.Visibility = Visibility.Visible;
                return;
            }
            
            box.Visibility = Visibility.Visible;
            L_Filter.Visibility = Visibility.Visible;
        }

        private void MI_Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(settingsManager, settings, themeManager);
            settingsWindow.ShowDialog();
        }
    }
}
