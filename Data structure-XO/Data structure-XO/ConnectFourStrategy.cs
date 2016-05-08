using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Data_structure_XO
{
    class ConnectFourStrategy : GuiStrategy
    {
        private const int NumOfRows = 6;
        private const int NumOfColumns = 7;
        private Image Chip = new Image();
        public ConnectFourStrategy(GameWindow Window) : base(Window)

        {
            gameEngine = new GameConnect4();
            GameCanvas.MouseMove+=GameCanvasOnMouseMove;
        
        }

        protected override void ChangeTurn()
        {
            base.ChangeTurn();
            SetChip(gameEngine.CurrentPlayer);
        }

        private void SetChip(GameEngine.Token t)
        {
            if (  t == GameEngine.Token.One)
                Chip.Source = gamesResourceDictionary["Connect4-Red-Circle"] as BitmapImage;
            else
                Chip.Source = gamesResourceDictionary["Connect4-Yellow-Circle"] as BitmapImage;
        }

        private void GameCanvasOnMouseMove(object sender, MouseEventArgs e)
        {
            if (gameFinished)
                return;
            Point mousePoistion = e.GetPosition(gameBoard);
            if (mousePoistion.X > 0 && mousePoistion.X < gameBoard.ActualWidth)
            {
                //Calculate row & column
                double columnSize = gameBoard.ActualWidth / NumOfColumns;
                int column = (int)(mousePoistion.X / (columnSize));

                double leftOffset = Canvas.GetLeft(gameBoard) + column*columnSize + (columnSize - Chip.Width)/2.0;
                Canvas.SetLeft(Chip,leftOffset);

            }
        }

        public override int MinWidth => 600;
        public override int MinHeight => 600;

        public override void InitializeBoard()
        {
            var srcImage = gamesResourceDictionary["Connect4-Board"] as BitmapImage;
            var RedChip = gamesResourceDictionary["Connect4-Red-Circle"] as BitmapImage;

        

            Image board = new Image {Source = srcImage};
            board.Height = GameCanvas.ActualHeight - RedChip.Height -10;
            board.Width = (double) NumOfColumns/NumOfRows*board.Height;

            GameCanvas.Children.Add(board);

            Canvas.SetZIndex(board, BoardZIndex);

            double topOffset = 5+ RedChip.Height;
            Canvas.SetTop(board, topOffset);

            double leftOffSet = (GameCanvas.ActualWidth - board.Width)/2.0;
            if (leftOffSet < 0)
                leftOffSet = 0;
            Canvas.SetLeft(board, leftOffSet);

            //Give a global access to the board
            gameBoard = board;


            Chip.Source = RedChip;
            
            Chip.Width = RedChip.Width*GetSizeRatio();
            Chip.Height = RedChip.Height*GetSizeRatio();
            GameCanvas.Children.Add(Chip);
            double ChipLeftOffset = leftOffSet + (board.Width/NumOfColumns - Chip.Width)/2; 
            Canvas.SetLeft(Chip, ChipLeftOffset);
        }

        public override void InsertSymbol(int row, int column, GameEngine.Token token = GameEngine.Token.Empty,bool clearRedoStack = true)
        {
            base.InsertSymbol(row, column, token, clearRedoStack);

            BitmapImage symbolImage;
            if (token == GameEngine.Token.Empty)
                token = gameEngine.CurrentPlayer;
            //Selet X or O
            if (token == GameEngine.Token.One)
                symbolImage = gamesResourceDictionary["Connect4-Red-Circle"] as BitmapImage;
            else
                symbolImage = gamesResourceDictionary["Connect4-Yellow-Circle"] as BitmapImage;
            Image symbol = new Image()
            {
                Source = symbolImage
            };
            
            //Set Size
            symbol.Width = symbolImage.Width*GetSizeRatio();
            symbol.Height = symbolImage.Height*GetSizeRatio();

            GameCanvas.Children.Add(symbol);
            Canvas.SetZIndex(symbol, SymbolZIndex);
            //Calculate offset
            double rowSize = gameBoard.ActualHeight/NumOfRows;
            double columnSize = gameBoard.ActualWidth/NumOfColumns;

            double topOffset = Canvas.GetTop(gameBoard) + (NumOfRows - row - 1)*rowSize + (rowSize - symbol.Height)/2 + 2*GetSizeRatio()*(NumOfRows - row-1 );
            double leftOffset = Canvas.GetLeft(gameBoard) + column*columnSize + (columnSize - symbol.Width)/2 +
                                2*GetSizeRatio()*(column );

            //Move it    
            Canvas.SetTop(symbol, topOffset);
            Canvas.SetLeft(symbol, leftOffset);
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
                double columnSize = gameBoard.ActualWidth/NumOfColumns;
                int column = (int) (mousePoistion.X/(columnSize));
                int row = getEmptyRow(column);
                //If there is no empty row just return
                if (row == -1)
                    return;
                var insert = gameEngine.InsertSymbol(row, column);
                // If insertion fails don't continue
                if (!insert) return;

                InsertSymbol(row, column);
                CheckForWinning(row, column);
            }
        }

        private int getEmptyRow(int col)
        {
            for (int i = 0; i < NumOfRows; i++)
            {
                var tile = gameEngine.GetTileValue(i, col);
                if (tile == GameEngine.Token.Empty)
                    return i;
            }
            return -1;
        }


        protected override void DrawSymbolsFromGameEngine()
        {
            for (int i = 0; i < NumOfRows; i++)
            {
                for (int j = 0; j < NumOfColumns; j++)
                {
                    GameEngine.Token t = gameEngine.GetTileValue(i, j);
                    if (t != GameEngine.Token.Empty)
                        InsertSymbol(i, j, t);
                }
            }
        }

        public override void LoadGame(FileStream fs)
        {
           
                base.LoadGame(fs);
                SetChip(gameEngine.CurrentPlayer);
            
           
            
        }

        protected override double OriginalBoardWidth {
            get
            {
                var originalBoardImg = gamesResourceDictionary["Connect4-Board"] as BitmapImage;
                return originalBoardImg.Width;
            }
        }
    }
}