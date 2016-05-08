using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Data_structure_XO
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow
    {
        private GuiStrategy strategy;

        public GameWindow(int game) // 0 for XO and 1 for Connect4
        {
            InitializeComponent();
            if(game == 0)
            strategy = new TicTacToeGuiStrategy(this);
            else
            {
                return;
            }
            SizeChanged += strategy.OnSizeChanged;
            Loaded += strategy.OnLoaded;
            MinWidth = strategy.MinWidth;
            MinHeight = strategy.MinHeight;
        }

        public GameWindow(FileStream fs, int type) : this(type)
        {

            strategy.LoadGame(fs);
            fs.Close();
            
        }

        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            strategy.RestartGame();
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            strategy.SaveGame();
           
        }

        private void OpenGame_Click(object sender, RoutedEventArgs e)
        {
            strategy.OpenGame();

            
           
        }
    }
}