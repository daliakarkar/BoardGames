using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Data_structure_XO
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow
    {
        private readonly GameEngine _gameEngine;

        public GameWindow(int type) // 0 for XO and 1 for Connect4
        {
            InitializeComponent();
            switch (type)
            {
                case 0:
                    _gameEngine = new GameXO();
                    break;
                case 1:
                    _gameEngine = new GameConnect4();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            PlayerLabel.Content = "Player " + _gameEngine.CurrentPlayer + " turn";
            BoardLabel.Content = _gameEngine.DisplayBoard();

        }

        public GameWindow(FileStream fs,int type) : this(type)
        {
            _gameEngine.LoadGame(fs);
            fs.Close();
            PlayerLabel.Content = "Player " + _gameEngine.CurrentPlayer + " turn";
            BoardLabel.Content = _gameEngine.DisplayBoard();
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            var row = int.Parse(PositionTextbox.Text[0].ToString());
            var col = int.Parse(PositionTextbox.Text[2].ToString());
            var insert = _gameEngine.InsertSymbol(row,col);
            if (!insert) return;
            BoardLabel.Content = _gameEngine.DisplayBoard();
            if (_gameEngine.IsGameWon(row, col))
            {
                PositionTextbox.IsEnabled = false;
                EnterButton.IsEnabled = false;
                MessageBox.Show("Player "+_gameEngine.CurrentPlayer+" Won !", "Result",
                           MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (_gameEngine.IsGameDraw())
            {
                PositionTextbox.IsEnabled = false;
                EnterButton.IsEnabled = false;
                MessageBox.Show("Draw !", "Result",
                           MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _gameEngine.ChangeTurn();
                PlayerLabel.Content = "Player " + _gameEngine.CurrentPlayer + " turn";
            }
        }

        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            _gameEngine.Restart();
            PositionTextbox.IsEnabled = true;
            EnterButton.IsEnabled = true;
            PositionTextbox.Clear();
            PlayerLabel.Content = "Player " + _gameEngine.CurrentPlayer + " turn";
            BoardLabel.Content = _gameEngine.DisplayBoard();
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
