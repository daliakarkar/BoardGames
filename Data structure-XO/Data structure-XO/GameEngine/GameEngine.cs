using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Data_structure_XO
{
    public class GameEngine
    {
        protected GameEngine()
        {
            CurrentPlayer = Token.One;
            Count = 0;
            UndoStack = new Stack<int>();
            RedoStack = new Stack<int>();
            Mode = 1;
        }

        public enum Token { Empty, One, Two}

        public Token CurrentPlayer { get; set; }
        public Stack<int> UndoStack { get; set; }
        public Stack<int> RedoStack { get; set; }
        public int Mode;

        public virtual bool IsGameWon(int row, int column)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsGameDraw()
        {
            throw new NotImplementedException();
        }

        public virtual bool InsertSymbol(int row, int column)
        {
            throw new NotImplementedException();
        }

        public virtual string DisplayBoard()
        {
            throw new NotImplementedException();
        }

        public virtual void Restart()
        {
            UndoStack.Clear();
            RedoStack.Clear();
            CurrentPlayer = Token.One;
            Count = 0;
        }

        public virtual void SaveGame(FileStream fs)
        {
            throw new NotImplementedException();
        }

        public virtual void LoadGame(FileStream fs)
        {
            throw new NotImplementedException();
        }

        public virtual KeyValuePair<int, int>? PlayComputer()
        {
            throw new NotImplementedException();
        }


        public void ChangeTurn()
        {
            switch (CurrentPlayer)
            {
                case Token.One:
                    CurrentPlayer = Token.Two;
                    break;
                case Token.Two:
                    CurrentPlayer = Token.One;
                    break;
                case Token.Empty:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Undo()
        {
            if(UndoStack.Count < 2) throw new InvalidOperationException();
            var column = UndoStack.Pop();
            var row = UndoStack.Pop();
            Game[row, column] = Token.Empty;
            RedoStack.Push(row);
            RedoStack.Push(column);
            Count--;

        }

        public bool Redo()
        {
            if (RedoStack.Count < 2) throw new InvalidOperationException();
            var column = RedoStack.Pop();
            var row = RedoStack.Pop();
            return InsertSymbol(row, column);
        }

        protected virtual bool IsValidInsertion(int row, int column)
        {
            throw new NotImplementedException();
        }

        protected virtual Token StringToToken(string s)
        {
            throw new NotImplementedException();
        }

        protected virtual string TokenToString(Token token)
        {
            throw new NotImplementedException();
        }

        protected static string ReadClean(TextReader streamReader)
        {
            var read = ((char)streamReader.Read()).ToString();
            if (read == "\0")
                read = ((char)streamReader.Read()).ToString();
            return read;
        }

        public Token GetTileValue(int row, int column)
        {
            return Game[row, column];
        }

        protected Token[,] Game;
        protected int Count;

    }
}
