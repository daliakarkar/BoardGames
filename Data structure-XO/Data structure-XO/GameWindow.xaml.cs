using System;
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
    }
}