using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Data_structure_XO
{
    class TicTacToeGuiStrategy : GuiStrategy
    {
        public override int MinWidth => 500;
        public override int MinHeight => 500;

        public override void InitializeBoard()
        {
            var srcImage = gamesResourceDictionary["XO-Board"] as BitmapImage;
            Image board = new Image {Source = srcImage};
            GameCanvas.Children.Add(board);

            Canvas.SetZIndex(board, BoardZIndex);
            board.Height = GameCanvas.ActualHeight - 20;
            board.Width = board.Height + 20;

            double offset = (GameCanvas.ActualWidth - board.Width)/2.0;
            if (offset < 0)
                offset = 0;
            Canvas.SetLeft(board, offset);

            Canvas.SetTop(board, 10);
            //Give a global access to the board
            gameBoard = board;
        }

        public override void InsertSymbol(int row, int column, GameEngine.Token token = GameEngine.Token.Empty)
        {
            BitmapImage symbolImage;
            if (token == GameEngine.Token.Empty)
                token = gameEngine.CurrentPlayer;
            //Selet X or O
            if (token == GameEngine.Token.One)
                symbolImage = gamesResourceDictionary["XO-X-Mark"] as BitmapImage;
            else
                symbolImage = gamesResourceDictionary["XO-O-Mark"] as BitmapImage;
            Image symbol = new Image()
            {
                Source = symbolImage
            };

            GameCanvas.Children.Add(symbol);
            Canvas.SetZIndex(symbol, SymbolZIndex);
            //Calculate offset
            double rowSize = gameBoard.ActualHeight/3.0;
            double columnSize = gameBoard.ActualWidth/3.0;

            double topOffset = row*rowSize + (rowSize - symbolImage.Height)/2 + 5*(row + 1);
            double leftOffset = Canvas.GetLeft(gameBoard) + column*columnSize + (columnSize - symbolImage.Width)/2 +
                                5*(column + 1);

            //Move it    
            Canvas.SetTop(symbol, topOffset);
            Canvas.SetLeft(symbol, leftOffset);
        }

        public override void CheckForWinning(int row, int column)
        {
            //Check if it was a won
            if (gameEngine.IsGameWon(row, column))
            {
                gameFinished = true;
                MessageBox.Show("Player " + gameEngine.CurrentPlayer + " Won !", "Result",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (gameEngine.IsGameDraw())
            {
                gameFinished = true;
                MessageBox.Show("Draw !", "Result",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                gameEngine.ChangeTurn();
            }
        }

        protected override void GameCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //If the user can't play,because the game is finished
            if (gameFinished)
                return;
            Point mousePoistion = e.GetPosition(gameBoard);
            if (mousePoistion.X > 0 && mousePoistion.X < gameBoard.ActualWidth
                && mousePoistion.Y > 0 && mousePoistion.Y < gameBoard.ActualHeight)
            {
                //Calculate row & column
                double rowSize = gameBoard.ActualHeight/3.0;
                double columnSize = gameBoard.ActualWidth/3.0;
                int row = (int) (mousePoistion.Y/rowSize);
                int column = (int) (mousePoistion.X/(columnSize));


                var insert = gameEngine.InsertSymbol(row, column);
                // If insertion fails don't continue
                if (!insert) return;

                InsertSymbol(row, column);
                CheckForWinning(row, column);
            }
        }

        public TicTacToeGuiStrategy(Canvas gameCanvas) : base(gameCanvas)
        {
            gameEngine = new GameXO();
        }

        public override void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            InitializeBoard();
        }

        public override void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            //this is a bad solution if used in a heavy duty app
            GameCanvas.Children.Clear();
            InitializeBoard();
            gameBoard.UpdateLayout();
            DrawSymbolsFromGameEngine();
        }

        protected override void DrawSymbolsFromGameEngine()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    GameEngine.Token t = gameEngine.GetTileValue(i, j);
                    if (t != GameEngine.Token.Empty)
                        InsertSymbol(i, j, t);
                }
            }
        }
    }
}