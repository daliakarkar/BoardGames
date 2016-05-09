using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Data_structure_XO.GameEngines;

namespace Data_structure_XO.GuiStrategies
{
    internal class ConnectFourStrategy : GuiStrategy
    {
        private readonly Image Chip = new Image();

        public ConnectFourStrategy(GameWindow window, int mode) : base(window, mode)
        {
            GameEngine = new GameConnect4();
            UpdateStatusBar();

            GameCanvas.MouseMove += GameCanvasOnMouseMove;
        }

        protected override int NumOfRows => 6;
        protected override int NumOfColumns => 7;

        protected override double OriginalBoardWidth
        {
            get
            {
                var originalBoardImg = GamesResourceDictionary["Connect4-Board"] as BitmapImage;
                return originalBoardImg.Width;
            }
        }

        protected override void ChangeTurn()
        {
            base.ChangeTurn();
            SetChip(GameEngine.CurrentPlayer);
        }

        protected override KeyValuePair<double, double> CalculateSymbolOffset(int row, int column, Image symbol)
        {
            var rowSize = GameBoard.ActualHeight/NumOfRows;
            var columnSize = GameBoard.ActualWidth/NumOfColumns;

            var topOffset = Canvas.GetTop(GameBoard) + (NumOfRows - row - 1)*rowSize + (rowSize - symbol.Height)/2 +
                            2*GetSizeRatio()*(NumOfRows - row - 1);
            var leftOffset = Canvas.GetLeft(GameBoard) + column*columnSize + (columnSize - symbol.Width)/2 +
                             2*GetSizeRatio()*column;
            return new KeyValuePair<double, double>(topOffset, leftOffset);
        }

        private void SetChip(GameEngine.Token t)
        {
            if (t == GameEngine.Token.One)
                Chip.Source = GamesResourceDictionary["Connect4-Red-Circle"] as BitmapImage;
            else
                Chip.Source = GamesResourceDictionary["Connect4-Yellow-Circle"] as BitmapImage;
        }

        private void GameCanvasOnMouseMove(object sender, MouseEventArgs e)
        {
            if (GameFinished)
                return;
            var mousePoistion = e.GetPosition(GameBoard);
            if (mousePoistion.X > 0 && mousePoistion.X < GameBoard.ActualWidth)
            {
                //Calculate row & column
                var columnSize = GameBoard.ActualWidth/NumOfColumns;
                var column = (int) (mousePoistion.X/columnSize);

                var leftOffset = Canvas.GetLeft(GameBoard) + column*columnSize + (columnSize - Chip.Width)/2.0;
                Canvas.SetLeft(Chip, leftOffset);
            }
        }


        public override void InitializeBoard()
        {
            var srcImage = GamesResourceDictionary["Connect4-Board"] as BitmapImage;
            var RedChip = GamesResourceDictionary["Connect4-Red-Circle"] as BitmapImage;


            var board = new Image {Source = srcImage};
            board.Height = GameCanvas.ActualHeight - RedChip.Height - 10;
            board.Width = (double) NumOfColumns/NumOfRows*board.Height;

            GameCanvas.Children.Add(board);

            Panel.SetZIndex(board, BoardZIndex);

            var topOffset = 5 + RedChip.Height;
            Canvas.SetTop(board, topOffset);

            var leftOffSet = (GameCanvas.ActualWidth - board.Width)/2.0;
            if (leftOffSet < 0)
                leftOffSet = 0;
            Canvas.SetLeft(board, leftOffSet);

            //Give a global access to the board
            GameBoard = board;


            Chip.Source = RedChip;

            Chip.Width = RedChip.Width*GetSizeRatio();
            Chip.Height = RedChip.Height*GetSizeRatio();

                GameCanvas.Children.Add(Chip);
            var ChipLeftOffset = leftOffSet + (board.Width/NumOfColumns - Chip.Width)/2;
            Canvas.SetLeft(Chip, ChipLeftOffset);
        }

        protected override BitmapImage getPlayerSymbol(GameEngine.Token t)
        {
            BitmapImage symbolImage;

            //Selet X or O
            if (t == GameEngine.Token.One)
                symbolImage = GamesResourceDictionary["Connect4-Red-Circle"] as BitmapImage;
            else
                symbolImage = GamesResourceDictionary["Connect4-Yellow-Circle"] as BitmapImage;

            return symbolImage;
        }


        protected override void GameCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //If the user can't play,because the game is finished
            if (GameFinished)
                return;
            var mousePoistion = e.GetPosition(GameBoard);
            if (mousePoistion.X > 0 && mousePoistion.X < GameBoard.ActualWidth
                && mousePoistion.Y > 0 && mousePoistion.Y < GameBoard.ActualHeight)
            {
                //Calculate row & column
                var columnSize = GameBoard.ActualWidth/NumOfColumns;
                var column = (int) (mousePoistion.X/columnSize);
                var row = getEmptyRow(column);
                //If there is no empty row just return
                if (row == -1)
                    return;
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

        private int getEmptyRow(int col)
        {
            for (var i = 0; i < NumOfRows; i++)
            {
                var tile = GameEngine.GetTileValue(i, col);
                if (tile == GameEngine.Token.Empty)
                    return i;
            }
            return -1;
        }


        public override void LoadGame(FileStream fs)
        {
            base.LoadGame(fs);
            SetChip(GameEngine.CurrentPlayer);
        }
    }
}