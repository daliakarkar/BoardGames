using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Data_structure_XO
{
    public abstract class GuiStrategy
    {
        protected ResourceDictionary gamesResourceDictionary;
        protected GameEngine gameEngine;
        protected Image gameBoard;
        protected const int BoardZIndex = 1;
        protected const int SymbolZIndex = 2;
        protected bool gameFinished = false;
        protected Canvas GameCanvas;

        public GuiStrategy(Canvas gameCanvas)
        {
            GameCanvas = gameCanvas;
            GameCanvas.MouseLeftButtonUp += GameCanvas_OnMouseLeftButtonUp;
            gamesResourceDictionary = new ResourceDictionary
            {
                Source = new Uri("resources/Resources.xaml",
                    UriKind.Relative)
            };
        }

        public abstract int MinWidth { get; }
        public abstract int MinHeight { get; }
        public abstract void InitializeBoard();
        public abstract void InsertSymbol(int row, int column, GameEngine.Token token = GameEngine.Token.Empty);
        public abstract void CheckForWinning(int row, int column);

        protected abstract void GameCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e);


        public abstract void OnSizeChanged(object sender, SizeChangedEventArgs e);

        public abstract void OnLoaded(object sender, RoutedEventArgs e);
        protected abstract void DrawSymbolsFromGameEngine();

        public  void RestartGame()
        {
            gameEngine.Restart();
            GameCanvas.Children.Clear();
            InitializeBoard();

        }
    }
}
