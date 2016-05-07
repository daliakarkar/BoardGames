using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_structure_XO
{
   
    public class GameXO : GameEngine
    {
        public GameXO() 
        {
            currentPlayer = Token.ONE;
            game = new Token[3][];
            for (int i = 0; i < 3; i++)
                game[i] = new Token[3];
        }
        protected override bool GameWon(int row,int column) 
        {
            //check col
            for (int i = 0; i < 3; i++)
            {
                if (game[row][i] != currentPlayer)
                    break;
                if (i == 2)
                {
                    return true;//win
                }
            }

            //check row
            for (int i = 0; i < 3; i++)
            {
                if (game[i][column] != currentPlayer)
                    break;
                if (i == 2)
                {
                    return true;//win
                }
            }

            //check diag
            if (row == column)
            {
                
                for (int i = 0; i < 3; i++)
                {
                    if (game[i][i] != currentPlayer)
                        break;
                    if (i == 2)
                    {
                        return true;//win
                    }
                }
            }

            //check anti diag 
            for (int i = 0; i < 3; i++)
            {
                if (game[i][(2) - i] != currentPlayer)
                    break;
                if (i == 2)
                {
                    return true;//win
                }
            }
            return false;
        }
        protected override bool IsValidInsertion(int row, int column) 
        {
            
            if (row >2|| column>2)
            {
                //Show("You must enter  a valid position");
                return false;

            }
           
            if (game[row][column] != Token.EMPTY)
            {
                //Show("Please choose an empty slot");
                return false;
            }
           
            
            //check function
            InsertSymbol(row,column);
            GameWon(row, column);
            ChangeTurn();
            return false;
        }
        public override void InsertSymbol(int row, int column) 
        {
            game[row][column] = currentPlayer;
        }
        public override string DisplayBoard() 
        {
            return ""; 
        }

    }
}
