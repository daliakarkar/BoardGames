using System;
using System.IO;
using System.Threading.Tasks;
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

        public GameWindow(int type,int mode) // 0 for XO and 1 for Connect4
        {
            InitializeComponent();
            if (type == 0)
                strategy = new TicTacToeGuiStrategy(this,mode);
            else if (type == 1)
                strategy = new ConnectFourStrategy(this,mode);
            else
            {
                return;
            }
            SizeChanged += strategy.OnSizeChanged;
            Loaded += strategy.OnLoaded;
            MinWidth = strategy.MinWidth;
            MinHeight = strategy.MinHeight;
        }

        public GameWindow(FileStream fs, int type,int mode) : this(type,mode)
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

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            strategy.Undo();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new OptionsWindow();
            optionsWindow.Show();
            Close();
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            strategy.Redo();
        }
    }
}