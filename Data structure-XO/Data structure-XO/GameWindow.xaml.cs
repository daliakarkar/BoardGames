using System;
using System.Windows;

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
    }
}
