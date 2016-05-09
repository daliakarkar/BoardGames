using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Data_structure_XO.GameEngines
{
    public class GameXO : GameEngines.GameEngine
    {
        public GameXO()
        {
            Game = new Token[3, 3];
        }

        public override bool IsGameWon(int row, int column)
        {
            int count;
            //check col
            for (count = 0; count < 3 && Game[count, column] == CurrentPlayer; count++)
            {
            }
            if (count == 3)
            {
                return true;
            }
            //check row
            for (count = 0; count < 3 && Game[row, count] == CurrentPlayer; count++)
            {
            }
            if (count == 3)
            {
                return true;
            }
            //check diag
            if (row == column)
            {
                for (count = 0; count < 3 && Game[count, count] == CurrentPlayer; count++)
                {
                }
                if (count == 3)
                {
                    return true;
                }
            }
            //check anti diag 
            for (count = 0; count < 3 && Game[count, 2 - count] == CurrentPlayer; count++)
            {
            }
            if (count != 3) return false;

            return true;
        }

        public override bool IsGameDraw()
        {
            if (Count == 9)
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
            return board.Replace('E', ' ');
        }

        public override void Restart()
        {
            base.Restart();
            Game = new Token[3, 3];
        }

        public override void SaveGame(FileStream fs)
        {
            var game = TokenToString(CurrentPlayer);
            game += Mode;
            game += Count;
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
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
                if (streamReader.ReadToEnd().Length != 24) throw new FileLoadException();
                streamReader.DiscardBufferedData();
                streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                CurrentPlayer = StringToToken(ReadClean(streamReader));
                if (!int.TryParse(ReadClean(streamReader), out Mode))
                    throw new ArgumentException();

                if (!int.TryParse(ReadClean(streamReader), out Count))
                    throw new ArgumentException();
                for (var i = 0; i < 3; i++)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        Game[i, j] = StringToToken(ReadClean(streamReader));
                    }
                }
            }
        }

        public override KeyValuePair<int, int>? PlayComputer()
        {
            var rnd = new Random();
            var row = rnd.Next(0, 3);
            var col = rnd.Next(0, 3);
            while (Game[row, col] != Token.Empty)
            {
                row = rnd.Next(0, 3);
                col = rnd.Next(0, 3);
            }
            if (InsertSymbol(row, col))
                return new KeyValuePair<int, int>(row, col);
            else return null;
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

        protected override Token StringToToken(string s)
        {
            Token result;
            switch (s)
            {
                case "X":
                    result = Token.One;
                    break;
                case "O":
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
                    result = "X";
                    break;
                case Token.Two:
                    result = "O";
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