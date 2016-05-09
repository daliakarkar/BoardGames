using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Data_structure_XO.GameEngines;

namespace Data_structure_XO.GuiStrategies
{
    class TicTacToeGuiStrategy : GuiStrategy
    {
        protected override int NumOfRows => 3;
        protected override int NumOfColumns => 3;
        

        public override void InitializeBoard()
        {
            var srcImage = GamesResourceDictionary["XO-Board"] as BitmapImage;
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
            GameBoard = board;
        }

        public override void InsertSymbol(int row, int column, GameEngine.Token token = GameEngine.Token.Empty,
            bool ClearRedoStack = true)
        {
            base.InsertSymbol(row, column, token, ClearRedoStack);
            BitmapImage symbolImage;
            if (token == GameEngine.Token.Empty)
                token = GameEngine.CurrentPlayer;
            //Selet X or O
            if (token == GameEngine.Token.One)
                symbolImage = GamesResourceDictionary["XO-X-Mark"] as BitmapImage;
            else
                symbolImage = GamesResourceDictionary["XO-O-Mark"] as BitmapImage;
            Image symbol = new Image()
            {
                Source = symbolImage
            };

            GameCanvas.Children.Add(symbol);
            Canvas.SetZIndex(symbol, SymbolZIndex);
            //Calculate offset
            double rowSize = GameBoard.ActualHeight/NumOfRows;
            double columnSize = GameBoard.ActualWidth/NumOfColumns;

            double topOffset = row*rowSize + (rowSize - symbolImage.Height)/2 + 5*(row + 1);
            double leftOffset = Canvas.GetLeft(GameBoard) + column*columnSize + (columnSize - symbolImage.Width)/2 +
                                5*(column + 1);

            //Move it    
            Canvas.SetTop(symbol, topOffset);
            Canvas.SetLeft(symbol, leftOffset);
        }


        protected override void GameCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //If the user can't play,because the game is finished
            if (GameFinished)
                return;
            Point mousePoistion = e.GetPosition(GameBoard);
            if (mousePoistion.X > 0 && mousePoistion.X < GameBoard.ActualWidth
                && mousePoistion.Y > 0 && mousePoistion.Y < GameBoard.ActualHeight)
            {
                //Calculate row & column
                double rowSize = GameBoard.ActualHeight/3.0;
                double columnSize = GameBoard.ActualWidth/3.0;
                int row = (int) (mousePoistion.Y/rowSize);
                int column = (int) (mousePoistion.X/(columnSize));


                var insert = GameEngine.InsertSymbol(row, column);
                // If insertion fails don't continue
                if (!insert) return;

                InsertSymbol(row, column);
                CheckForWinning(row, column);

                //If you play versus computer it's his time
                if (Mode == 0 && !GameFinished)
                {
                    var coordinates = GameEngine.PlayComputer();
                    if (coordinates != null)
                    {
                        InsertSymbol(coordinates.Value.Key, coordinates.Value.Value);
                        CheckForWinning(coordinates.Value.Key, coordinates.Value.Value);
                    }
                }
               
            }
        }

        public TicTacToeGuiStrategy(GameWindow window, int mode) : base(window, mode)
        {
            GameEngine = new GameXO();
            UpdateStatusBar();
            GameEngine.Mode = mode;
        }


      

        protected override double OriginalBoardWidth
        {
            get
            {
                var srcImage = GamesResourceDictionary["XO-Board"] as BitmapImage;
                return srcImage.Width;
            }
        }
    }
}