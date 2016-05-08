using System;
using System.Windows;

namespace Data_structure_XO
{
    public class GameConnect4 : GameEngine
    {
        public GameConnect4() 
        {
            CurrentPlayer = Token.One;
            Game = new Token[6,7];
            Count = 0;
        }

        public override bool IsGameWon(int row, int column) 
        {
            var count = 1;
            const int maxCol = 7;
            const int maxRow = 6;
            //check col
            if (row >= 3)
            {
                for (var i = row - 1;i >= 0 && Game[i, column] == CurrentPlayer; i--,count++) { }
                if (count == 4) return true;
            }
            //check row
            count = 1;
            for (var i = column + 1; i < maxCol && Game[row, i] == CurrentPlayer; i++,count++) { }
            if (column > 0)
                for (var i = column - 1; i >= 0 && Game[row, i] == CurrentPlayer; i--,count++) { }
            if (count == 4) return true;
            //check diag
            count = 1;
            if(row > 0)
                for (int i = row - 1, j = column + 1; i >= 0 && j < maxCol 
                     && Game[i, j] == CurrentPlayer; i--,j++, count++) { }
            if(column > 0)
                for (int i = row + 1, j = column - 1; i < maxRow && j >= 0
                     && Game[i, j] == CurrentPlayer; i++, j--, count++) { }
            if (count == 4) return true;
            //check anti diag 
            count = 1;
            for (int i = row + 1, j = column + 1; i < maxRow && j < maxCol
                     && Game[i, j] == CurrentPlayer; i++, j++, count++) { }
            if (row <= 0 || column <= 0) return count == 4;
            {
                for (int i = row - 1, j = column - 1; i >= 0 && j >= 0
                     && Game[i, j] == CurrentPlayer; i--, j--, count++) { }
            }
            return count == 4;
        }

        public override bool IsGameDraw()
        {
            return Count == 42;
        }

        public override bool InsertSymbol(int row, int column)
        {
            if (!IsValidInsertion(row, column)) return false;
            Game[row, column] = CurrentPlayer;
            Count++;
            return true;
        }

        public override string DisplayBoard()
        {
            var board = "###################################" + Environment.NewLine;
            for (var i = 5; i >= 0; i--)
            {
                for (var j = 0; j < 7; j++)
                {
                    board += "#  " + TokenToString(Game[i, j]) + "  #";
                }
                board += Environment.NewLine;
            }
            board += "###################################" + Environment.NewLine;
            return board;
        }

        public override void Restart()
        {
            CurrentPlayer = Token.One;
            Game = new Token[6, 7];
            Count = 0;
        }

        protected override bool IsValidInsertion(int row, int column) 
        {
            if (row > 5 || column > 6 || (row != 0 && Game[row - 1, column] == Token.Empty))
            {
                MessageBox.Show("You must enter  a valid position.", "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (Game[row, column] == Token.Empty) return true;
            MessageBox.Show("Please choose an empty slot.", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        private static string TokenToString(Token token)
        {
            string result;
            switch (token)
            {
                case Token.One:
                    result = "Y";
                    break;
                case Token.Two:
                    result = "R";
                    break;
                case Token.Empty:
                    result = " ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }

    }
}
