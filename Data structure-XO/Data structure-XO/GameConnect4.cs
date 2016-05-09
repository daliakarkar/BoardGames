using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Data_structure_XO
{
    public class GameConnect4 : GameEngine
    {
        private const int MaxRow = 6;
        private const int MaxCol = 7;


        public GameConnect4() 
        {
            Game = new Token[MaxRow,MaxCol];
        }

        public override bool IsGameWon(int row, int column) 
        {
            var count = 1;
            //check col
            if (row >= 3)
            {
                for (var i = row - 1;i >= 0 && Game[i, column] == CurrentPlayer; i--,count++) { }
                if (count == 4)
                {
       
                    return true;
                }
            }
            //check row
            count = 1;
            for (var i = column + 1; i < MaxCol && Game[row, i] == CurrentPlayer; i++,count++) { }
            if (column > 0)
                for (var i = column - 1; i >= 0 && Game[row, i] == CurrentPlayer; i--,count++) { }
            if (count == 4)
            {

                return true;
            }
            //check diag
            count = 1;
            if(row > 0)
                for (int i = row - 1, j = column + 1; i >= 0 && j < MaxCol 
                     && Game[i, j] == CurrentPlayer; i--,j++, count++) { }
            if(column > 0)
                for (int i = row + 1, j = column - 1; i < MaxRow && j >= 0
                     && Game[i, j] == CurrentPlayer; i++, j--, count++) { }
            if (count == 4)
            {
               
                return true;
            }
            //check anti diag 
            count = 1;
            for (int i = row + 1, j = column + 1; i < MaxRow && j < MaxCol
                     && Game[i, j] == CurrentPlayer; i++, j++, count++) { }
            if (row == 0 || column == 0)
            {
                if (count == 4)
                {
                    return true;
                }
            }
            else
            {
                for (int i = row - 1, j = column - 1; i >= 0 && j >= 0
                     && Game[i, j] == CurrentPlayer; i--, j--, count++) { }
            }
            if (count != 4) return false;

            return true;
        }

        public override bool IsGameDraw()
        {
            if (Count == MaxRow*MaxCol)
                return true;
            return false;
        }

        public override bool InsertSymbol(int row, int column)
        {
            if (!IsValidInsertion(row, column)) return false;
            Game[row, column] = CurrentPlayer;
            UndoStack.Push(row);
            UndoStack.Push(column);
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
            return board.Replace('E', ' ');
        }

        public override void Restart()
        {
            CurrentPlayer = Token.One;
            Game = new Token[6, 7];
            Count = 0;
        }

        public override void SaveGame(FileStream fs)
        {
            var game = TokenToString(CurrentPlayer);
            game += Mode;
            game += Count;
            for (var i = 0; i < MaxRow; i++)
            {
                for (var j = 0; j < MaxCol; j++)
                {
                    game += TokenToString(Game[i, j]);
                }
            }
            var uniEncoding = new UnicodeEncoding();
            fs.Write(uniEncoding.GetBytes(game), 0, uniEncoding.GetByteCount(game));
        }

        public override void LoadGame(FileStream fs)
        {
            using (var streamReader = new StreamReader(fs, Encoding.UTF8))
            {
                streamReader.DiscardBufferedData();
                streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                if (streamReader.ReadToEnd().Length != 90) throw new FileLoadException();
                streamReader.DiscardBufferedData();
                streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                CurrentPlayer = StringToToken(ReadClean(streamReader));
                if (!int.TryParse(ReadClean(streamReader), out Mode))
                    throw new ArgumentException();
                if (!int.TryParse(ReadClean(streamReader), out Count))
                    throw new ArgumentException();
                for (var i = 0; i < MaxRow; i++)
                {
                    for (var j = 0; j < MaxCol; j++)
                    {
                        Game[i, j] = StringToToken(ReadClean(streamReader));
                    }
                }
            }
        }

        public override KeyValuePair<int, int>? PlayComputer()
        {
            var rnd = new Random();
            var row = rnd.Next(0, MaxRow);
            var col = rnd.Next(0, MaxCol);
            while (Game[row, col] != Token.Empty || (row != 0 && Game[row - 1, col] == Token.Empty))
            {
                row = rnd.Next(0, MaxRow);
                col = rnd.Next(0, MaxCol);
            }
            if (InsertSymbol(row, col))
                return new KeyValuePair<int, int>(row, col);
            else return null;
        }


        protected override bool IsValidInsertion(int row, int column) 
        {
            if (row > 5 || column > 6 || (row != 0 && Game[row - 1, column] == Token.Empty))
            {
         
                return false;
            }
            if (Game[row, column] == Token.Empty) return true;
     
            return false;
        }

        protected override Token StringToToken(string s)
        {
            Token result;
            switch (s)
            {
                case "Y":
                    result = Token.One;
                    break;
                case "R":
                    result = Token.Two;
                    break;
                case "E":
                    result = Token.Empty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }

        protected override string TokenToString(Token token)
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
                    result = "E";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
    }
}
