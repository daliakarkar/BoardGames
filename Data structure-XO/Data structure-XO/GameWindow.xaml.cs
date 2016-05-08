using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Data_structure_XO
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow
    {
        private ResourceDictionary gamesResourceDictionary;
        private GameEngine gameEngine;
        private Image gameBoard;
        private const int BoardZIndex = 1;
        private const int SymbolZIndex = 2;
        private bool gameFinished = false;
        private GuiStrategy strategy;

        public GameWindow(int game) // 0 for XO and 1 for Connect4
        {
            InitializeComponent();
            if(game == 0)
            strategy = new TicTacToeGuiStrategy(GameCanvas);
            else
            {
                return;
            }
            SizeChanged += strategy.OnSizeChanged;
            Loaded += strategy.OnLoaded;
            MinWidth = strategy.MinWidth;
            MinHeight = strategy.MinHeight;
        }


        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            strategy.RestartGame();
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Board Game File|*.bgf",
                Title = "Save Board Game File"
            };
            saveFileDialog.ShowDialog();
            // If the file name is not an empty string open it for saving.
            if (saveFileDialog.FileName == "") return;
            // Saves the File via a FileStream created by the OpenFile method.
            var fs = (FileStream) saveFileDialog.OpenFile();
            _gameEngine.SaveGame(fs);
            fs.Close();
        }

        private void OpenGame_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Board Game File|*.bgf",
                Title = "Open Board Game File",
                Multiselect = false
            };
            var userClickedOk = openFileDialog.ShowDialog();
            // Process input if the user clicked OK.
            if (userClickedOk != true) return;
            // Open the selected file to read.
            var fs = (FileStream) openFileDialog.OpenFile();
            _gameEngine.LoadGame(fs);
            fs.Close();
            PositionTextbox.IsEnabled = true;
            EnterButton.IsEnabled = true;
            PositionTextbox.Clear();
            PlayerLabel.Content = "Player " + _gameEngine.CurrentPlayer + " turn";
            BoardLabel.Content = _gameEngine.DisplayBoard();
        }
    }
}