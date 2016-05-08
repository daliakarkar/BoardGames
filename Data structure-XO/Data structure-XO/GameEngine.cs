using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_structure_XO
{
    public abstract class GameEngine
    {
        public enum Token { ONE, TWO, EMPTY =0 }
        protected Token[][] game;
        protected Token currentPlayer;
        protected virtual bool GameWon(int row, int column) { return false; }
        protected virtual bool IsValidInsertion(int row, int column) { return false; }
        public virtual void InsertSymbol(int row, int column) { }
        public virtual string DisplayBoard() { return ""; }
        public void ChangeTurn()
        {
            if (currentPlayer == Token.ONE) { currentPlayer= Token.TWO; }
            else if (currentPlayer == Token.TWO) { currentPlayer = Token.ONE; }
        }
    }

    
}
