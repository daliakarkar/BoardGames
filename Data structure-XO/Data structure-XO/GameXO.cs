using System;
using System.Windows;

namespace Data_structure_XO
{
   
    public class GameXO : GameEngine
    {
        public GameXO() 
        {
            CurrentPlayer = Token.One;
            Game = new Token[3,3];
            Count = 0;
        }

        public override bool IsGameWon(int row, int column)
        {
            int count;
            //check col
            for (count = 0; count < 3 && Game[count, column] == CurrentPlayer; count++) { }
            if (count == 3) return true;
            //check row
            for (count = 0; count < 3 && Game[row, count] == CurrentPlayer; count++){}
            if (count == 3) return true;
            //check diag
            if (row == column)
            {
                for (count = 0; count < 3 && Game[count, count] == CurrentPlayer; count++){}
                if (count == 3) return true;
            }
            //check anti diag 
            for (count = 0; count < 3 && Game[count, 2 - count] == CurrentPlayer; count++){}
            return count == 3;
        }

        public override bool IsGameDraw()
        {
            return Count == 9;
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
            var board = "###############" + Environment.NewLine;
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    board += "#  " + TokenToString(Game[i, j]) + "  #";
                }
                board += Environment.NewLine;
            }
            board += "###############" + Environment.NewLine;
            return board;
        }

        public override void Restart()
        {
            CurrentPlayer = Token.One;
            Game = new Token[3, 3];
            Count = 0;
        }

        protected override bool IsValidInsertion(int row, int column) 
        {
            if (row > 2 || column > 2)
            {
               
                return false;
            }
            if (Game[row, column] == Token.Empty) return true;
           
            return false;
        }

        private static string TokenToString(Token token)
        {
            string result;
            switch (token)
            {
                case Token.One:
                    result = "X";
                    break;
                case Token.Two:
                    result = "O";
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
