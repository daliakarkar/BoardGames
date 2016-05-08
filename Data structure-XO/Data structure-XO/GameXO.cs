﻿using System;
using System.IO;
using System.Text;
using System.Windows;

namespace Data_structure_XO
{
   
    public class GameXO : GameEngine
    {
        public GameXO() 
        {
            Game = new Token[3,3];
        }

        public override bool IsGameWon(int row, int column)
        {
            int count;
            //check col
            for (count = 0; count < 3 && Game[count, column] == CurrentPlayer; count++) { }
            if (count == 3)
            {
                MessageBox.Show("Player " + CurrentPlayer + " Won !", "Result",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            //check row
            for (count = 0; count < 3 && Game[row, count] == CurrentPlayer; count++){}
            if (count == 3)
            {
                MessageBox.Show("Player " + CurrentPlayer + " Won !", "Result",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            //check diag
            if (row == column)
            {
                for (count = 0; count < 3 && Game[count, count] == CurrentPlayer; count++){}
                if (count == 3)
                {
                    MessageBox.Show("Player " + CurrentPlayer + " Won !", "Result",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
            }
            //check anti diag 
            for (count = 0; count < 3 && Game[count, 2 - count] == CurrentPlayer; count++){}
            if (count != 3) return false;
            MessageBox.Show("Player " + CurrentPlayer + " Won !", "Result",
                            MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }

        public override bool IsGameDraw()
        {
            if(Count == 9)
                MessageBox.Show("Draw !", "Result",
                           MessageBoxButton.OK, MessageBoxImage.Information);
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
            CurrentPlayer = Token.One;
            Game = new Token[3, 3];
            Count = 0;
        }

        public override void SaveGame(FileStream fs)
        {
            var game = TokenToString(CurrentPlayer);
            game += Mode;
            game += Level;
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
                if (streamReader.ReadToEnd().Length != 26) throw new FileLoadException();
                streamReader.DiscardBufferedData();
                streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                CurrentPlayer = StringToToken(ReadClean(streamReader));
                if (!int.TryParse(ReadClean(streamReader), out Mode))
                    throw new ArgumentException();
                if (!int.TryParse(ReadClean(streamReader), out Level))
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

        public override bool PlayLow()
        {
            var rnd = new Random();
            var row = rnd.Next(0, 3);
            var col = rnd.Next(0, 3);
            while (Game[row, col] != Token.Empty)
            {
                row = rnd.Next(0, 3);
                col = rnd.Next(0, 3);
            }
            if (!InsertSymbol(row, col)) return false;
            return IsGameWon(row, col) || IsGameDraw();
        }

        public override bool PlayHigh()
        {
            return PlayLow();
        }

        protected override bool IsValidInsertion(int row, int column) 
        {
            if (row > 2 || column > 2)
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
