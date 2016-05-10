using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Data_structure_XO.GameEngines;

namespace Data_structure_XO.GuiStrategies
{
    internal class TicTacToeGuiStrategy : GuiStrategy
    {
        public TicTacToeGuiStrategy(GameWindow window, int mode) : base(window, mode)
        {
            GameEngine = new GameXO();
            UpdateStatusBar();
            GameEngine.Mode = mode;
        }

        protected override int NumOfRows => 3;
        protected override int NumOfColumns => 3;


        protected override double OriginalBoardWidth
        {
            get
            {
                var srcImage = GamesResourceDictionary["XO-Board"] as BitmapImage;
                return srcImage?.Width ?? 0;
            }
        }


        public override void InitializeBoard()
        {
            var srcImage = GamesResourceDictionary["XO-Board"] as BitmapImage;
            var board = new Image {Source = srcImage};
            GameCanvas.Children.Add(board);

            Panel.SetZIndex(board, BoardZIndex);
            board.Height = GameCanvas.ActualHeight - 20;
            board.Width = board.Height + 20;

            var offset = (GameCanvas.ActualWidth - board.Width)/2.0;
            if (offset < 0)
                offset = 0;
            Canvas.SetLeft(board, offset);

            Canvas.SetTop(board, 10);
            //Give a global access to the board
            GameBoard = board;
        }

        protected override BitmapImage GetPlayerSymbol(GameEngine.Token t)
        {
            BitmapImage symbolImage;

            //Selet X or O
            if (t == GameEngine.Token.One)
                symbolImage = GamesResourceDictionary["XO-X-Mark"] as BitmapImage;
            else
                symbolImage = GamesResourceDictionary["XO-O-Mark"] as BitmapImage;

            return symbolImage;
        }


        protected override KeyValuePair<double, double> CalculateSymbolOffset(int row, int column, Image symbol)
        {
//Calculate offset
            var rowSize = GameBoard.ActualHeight/NumOfRows;
            var columnSize = GameBoard.ActualWidth/NumOfColumns;

            var topOffset = row*rowSize + (rowSize - symbol.Height)/2 + 5*(row + 1);
            var leftOffset = Canvas.GetLeft(GameBoard) + column*columnSize + (columnSize - symbol.Width)/2 +
                             5*(column + 1);
            return new KeyValuePair<double, double>(topOffset, leftOffset);
        }


        protected override async void GameCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //If the user can't play,because the game is finished
            if (GameFinished)
                return;
            var mousePoistion = e.GetPosition(GameBoard);
            if (!(mousePoistion.X > 0) || !(mousePoistion.X < GameBoard.ActualWidth) || !(mousePoistion.Y > 0) ||
                !(mousePoistion.Y < GameBoard.ActualHeight)) return;
            //Calculate row & column
            var rowSize = GameBoard.ActualHeight/3.0;
            var columnSize = GameBoard.ActualWidth/3.0;
            var row = (int) (mousePoistion.Y/rowSize);
            var column = (int) (mousePoistion.X/columnSize);


            var insert = GameEngine.InsertSymbol(row, column);
            // If insertion fails don't continue
            if (!insert) return;

            InsertSymbol(row, column);
            CheckForWinning(row, column);

            //If you play versus computer it's his time
            if (Mode != 0 || GameFinished) return;
            var coordinates = GameEngine.PlayComputer();
            if (coordinates == null) return;
            await Task.Delay(1000);
            InsertSymbol(coordinates.Value.Key, coordinates.Value.Value);
            CheckForWinning(coordinates.Value.Key, coordinates.Value.Value);
        }
    }
}