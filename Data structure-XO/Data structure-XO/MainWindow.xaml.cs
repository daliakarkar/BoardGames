﻿using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Data_structure_XO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void load_game_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Board Game File|*.bgf",
                Title = "Open Board Game File",
                Multiselect = false
            };
            var userClickedOk = openFileDialog.ShowDialog();
            if (userClickedOk != true) return;
            var fs = (FileStream)openFileDialog.OpenFile();
            var token = (char)fs.ReadByte();
            var mode = fs.ReadByte();
            int type;
            switch (token)
            {
                case 'X':
                case 'O':
                    type = 0;
                    break;
                case 'Y':
                case 'R':
                    type = 1;
                    break;
                default:
                    throw new FileLoadException();
            }
            var gameWindow = new GameWindow(fs, type, mode);
            gameWindow.Show();
            Close();
        }

        private void new_game_Click(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new OptionsWindow();
            optionsWindow.Show();
            Close();
        }
    }
}
