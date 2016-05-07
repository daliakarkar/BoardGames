using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_structure_XO
{
    public class GameConnect4 : GameEngine
    {
        public GameConnect4() 
        {
            currentPlayer = Token.ONE;
            game = new Token[6][];
            for (int i = 0; i < 6; i++)
                game[i] = new Token[7];
        }
        protected override bool GameWon(int row, int col) 
        {
      
            int count=0,maxCol=7,maxRow=6;


            // Horizontal check
            for (int i=0;i<maxCol;i++)
            {
                if (game[row][i]==currentPlayer)
                    count++;
                else
                    count=0;

                    if (count>=4)
                          return true;
            }
            //Vertical check
            for (int i=0;i<maxRow;i++)
            {
                if (game[i][col]==currentPlayer)
                    count++;
                else
                    count=0;

                if (count>=4)
                    return true;
            } 
           //Diagonal check  
            int rowStart,colStart;
            //top left to bottom right
            for (rowStart = 0; rowStart < maxRow - 4; rowStart++)
            {


                for (row = rowStart, col = 0; row < maxRow && col < maxCol; row++, col++)
                {
                    if (game[row][col] == currentPlayer)
                    {
                        count++;
                        if (count >= 4) return true;
                    }
                    else
                    {
                        count = 0;
                    }
                }
            }

            
            for (colStart = 1; colStart < maxCol - 4; rowStart++)
            {
                
                for (row = 0, col = colStart; row < maxRow && col < maxCol ; row++, col++)
                {
                    if (game[row][col] == currentPlayer)
                    {
                        count++;
                        if (count >= 4) return true;
                    }
                    else
                    {
                        count = 0;
                    }
                }
            }
            //Anti Diagonal check  
      
            //top left to bottom right
            for (rowStart = maxRow-1; rowStart >2; rowStart--)
            {


                for (row = rowStart, col = 0; row > 2 && col < maxCol; row--, col++)
                {
                    if (game[row][col] == currentPlayer)
                    {
                        count++;
                        if (count >= 4) return true;
                    }
                    else
                    {
                        count = 0;
                    }
                }
            }

            //bottom left to top right
            for (colStart = 1; colStart < maxCol - 4; rowStart--)
            {

                for (row = rowStart, col = colStart; row >=0 && col < maxCol; row--, col++)
                {
                    if (game[row][col] == currentPlayer)
                    {
                        count++;
                        if (count >= 4) return true;
                    }
                    else
                    {
                        count = 0;
                    }
                }
            }
            return false;
        }
        protected override bool IsValidInsertion(int row, int column) 
        {
            if (row > 5 || column > 6)
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
            InsertSymbol(row, column);
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
