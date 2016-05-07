using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_structure_XO
{
    public class GameEngine
    {
        public enum Token { ONE, TWO, EMPTY }
        protected Token[][] game;
        protected Token currentPlayer;
        protected bool GameWon(int row, int column) { return false; }
        protected bool IsValidInsertion(int row, int column) { return false; }
        public void InsertSymbol(int row, int column) { }
        public string DisplayBoard() { return ""; }
        public void ChangeTurn()
        {
            if (currentPlayer == Token.ONE) { currentPlayer= Token.TWO; }
            else if (currentPlayer == Token.TWO) { currentPlayer = Token.ONE; }
        }
    }

    
}
