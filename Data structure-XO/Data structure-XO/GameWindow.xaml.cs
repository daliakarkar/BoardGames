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
        private readonly int _mode;
        private readonly int _level;

        public GameWindow(int type, int mode, int level = 3) // 0 for XO and 1 for Connect4
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
            _mode = mode;
            _level = level;
            _gameEngine.Mode = _mode;
            _gameEngine.Level = level;
            UpdateBoard();
        }

        public GameWindow(FileStream fs,int type, int mode, int level = 3) : this(type, mode, level)
        {
            try
            {
                _gameEngine.LoadGame(fs);
            }
            catch (FileLoadException)
            {
                MessageBox.Show("Could not load file.", "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
            }
            fs.Close();
            UpdateBoard();
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            var row = int.Parse(PositionTextbox.Text[0].ToString());
            var col = int.Parse(PositionTextbox.Text[2].ToString());
            var insert = _gameEngine.InsertSymbol(row,col);
            if (!insert) return;
            _gameEngine.RedoStack.Clear();
            BoardLabel.Content = _gameEngine.DisplayBoard();
            CheckGame(row, col, 1);
        }

        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            _gameEngine.Restart();
            PositionTextbox.IsEnabled = true;
            EnterButton.IsEnabled = true;
            UpdateBoard();
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
            var token = (char)fs.ReadByte();
            var mode = fs.ReadByte();
            var level = fs.ReadByte();
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
            var gameWindow = new GameWindow(fs, type, mode, level);
            gameWindow.Show();
            Close();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new OptionsWindow();
            optionsWindow.Show();
            Close();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                try
                {
                    _gameEngine.Undo();
                    if (PositionTextbox.IsEnabled)
                        _gameEngine.ChangeTurn();
                    UpdateBoard();
                    PositionTextbox.IsEnabled = true;
                    EnterButton.IsEnabled = true;
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Nothing to undo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if (_mode == 0 && _gameEngine.CurrentPlayer == GameEngine.Token.Two)
                {
                    continue;
                }
                break;
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                try
                {
                    _gameEngine.Redo();
                    BoardLabel.Content = _gameEngine.DisplayBoard();
                    var col = _gameEngine.UndoStack.Pop();
                    var row = _gameEngine.UndoStack.Peek();
                    _gameEngine.UndoStack.Push(col);
                    CheckGame(row, col, 0);
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Nothing to redo.", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if (_mode == 0 && _gameEngine.CurrentPlayer == GameEngine.Token.Two)
                {
                    continue;
                }
                break;
            }
        }

        private void UpdateBoard()
        {
            PositionTextbox.Clear();
            PlayerLabel.Content = "Player " + _gameEngine.CurrentPlayer + " turn";
            BoardLabel.Content = _gameEngine.DisplayBoard();
            UndoItem.IsEnabled = _gameEngine.UndoStack.Count >= 2;
            RedoItem.IsEnabled = _gameEngine.RedoStack.Count >= 2;
        }

        private void CheckGame(int row, int col,int from) //from: 0 for Redo and 1 for Enter
        {
            if (_gameEngine.IsGameWon(row, col) || _gameEngine.IsGameDraw())
            {
                PositionTextbox.IsEnabled = false;
                EnterButton.IsEnabled = false;
            }
            else
            {
                _gameEngine.ChangeTurn();
                UpdateBoard();
                if (from != 1 || _mode != 0) return;
                var result = _level == 0 ? _gameEngine.PlayLow() : _gameEngine.PlayHigh();
                if (result)
                {
                    PositionTextbox.IsEnabled = false;
                    EnterButton.IsEnabled = false;
                }
                else
                {
                    _gameEngine.ChangeTurn();
                    UpdateBoard();
                }
            }
        }
    }
}
