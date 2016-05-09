using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace Data_structure_XO
{
    public abstract class GuiStrategy
    {
        protected ResourceDictionary GamesResourceDictionary;
        protected GameEngine GameEngine;
        protected Image GameBoard;
        protected const int BoardZIndex = 1;
        protected const int SymbolZIndex = 2;
        protected bool GameFinished = false;
        protected Canvas GameCanvas;
        private bool _loadGameLater = false;


        public GuiStrategy(GameWindow window, int mode)
        {
            Window = window;
            GameCanvas = Window.GameCanvas;
            GameCanvas.MouseLeftButtonUp += GameCanvas_OnMouseLeftButtonUp;
            GamesResourceDictionary = new ResourceDictionary
            {
                Source = new Uri("resources/Resources.xaml",
                    UriKind.Relative)
            };
            Mode = mode;
        }


        protected int Mode { get; set; }

        public GameWindow Window { get; }

        public abstract int MinWidth { get; }
        public abstract int MinHeight { get; }
        public abstract void InitializeBoard();

        public virtual void InsertSymbol(int row, int column, GameEngine.Token token = GameEngine.Token.Empty,
            bool clearRedoStack = true)
        {
            if (clearRedoStack)
                GameEngine.RedoStack.Clear();
     
            Window.UndoItem.IsEnabled = GameEngine.UndoStack.Count >= 2;
            Window.RedoItem.IsEnabled = GameEngine.RedoStack.Count >= 2;
        }

        protected abstract void GameCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e);


        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeBoard();
            if (_loadGameLater)
            {
                _loadGameLater = false;
                GameBoard.UpdateLayout();

                DrawSymbolsFromGameEngine();
            }
        }

        protected abstract void DrawSymbolsFromGameEngine();

        public void RestartGame()
        {
            GameFinished = false;
            Window.RedoItem.IsEnabled = false;
            Window.UndoItem.IsEnabled = false;

            GameEngine.Restart();
            GameCanvas.Children.Clear();
            
            InitializeBoard();
        }

        public void SaveGame()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Board Game File|*.bgf",
                Title = "Save Board Game File"
            };
            saveFileDialog.ShowDialog();
            // If the file name is not an empty string open it for saving.
            if (saveFileDialog.FileName == "") return;
            // Saves the File via a FileStream created by the OpenFile method.
            var fs = (FileStream) saveFileDialog.OpenFile();
            GameEngine.SaveGame(fs);
            fs.Close();
        }

        public void OpenGame()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Board Game File|*.bgf",
                Title = "Open Board Game File",
                Multiselect = false
            };
            var userClickedOk = openFileDialog.ShowDialog();
            // Process input if the user clicked OK.
            if (userClickedOk != true) return;
            // Open the selected file to read.
            var fs = (FileStream) openFileDialog.OpenFile();
            var token = (char)fs.ReadByte();
            var mode = fs.ReadByte();
            int type;
            switch (token)
            {
                case 'X':
                case 'O':
                    type = 0;
                    break;
                case 'Y':
                case 'R':
                    type = 1;
                    break;
                default:
                    throw new FileLoadException();
            }
            var gameWindow = new GameWindow(fs, type, mode);
            gameWindow.Show();
            Window.Close();
        }

        public virtual void LoadGame(FileStream fs)
        {
            try
            {
                GameEngine.LoadGame(fs);
                fs.Close();
                if (Window.IsLoaded)
                {
                    GameCanvas.Children.Clear();
                    InitializeBoard();
                    GameBoard.UpdateLayout();
                    DrawSymbolsFromGameEngine();
                    UpdateStatusBar();

                }
                else
                {
                    _loadGameLater = true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(Window, "You have opened invalid save file", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public void CheckForWinning(int row, int column)
        {
            //Check if it was a won
            if (GameEngine.IsGameWon(row, column))
            {
                GameFinished = true;
                MessageBox.Show("Player " + GameEngine.CurrentPlayer + " Won !", "Result",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (GameEngine.IsGameDraw())
            {
                GameFinished = true;
                MessageBox.Show("Draw !", "Result",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ChangeTurn();
            }
        }

        protected virtual void ChangeTurn()
        {
            GameEngine.ChangeTurn();
            UpdateStatusBar();
        }

        protected void UpdateStatusBar()
        {
            if (GameEngine.CurrentPlayer == GameEngine.Token.One)
                Window.Statusbar.Content = "Turn : Player 1";
            else if (GameEngine.CurrentPlayer == GameEngine.Token.Two)
                Window.Statusbar.Content = "Turn : Player 2";
        }

        protected double GetSizeRatio()
        {
            double ratio;
            GameBoard.UpdateLayout();
            ratio = GameBoard.ActualWidth/OriginalBoardWidth;
            return ratio;
        }

        protected abstract double OriginalBoardWidth { get; }

        private bool LoadGameLater
        {
            get { return _loadGameLater; }
            set { _loadGameLater = value; }
        }

        public void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            //this is a bad solution if used in a heavy duty app
            GameCanvas.Children.Clear();
            InitializeBoard();
            GameBoard.UpdateLayout();
            DrawSymbolsFromGameEngine();
        }

        public void Undo()
        {
            if (GameFinished)
                return;
            if (Mode == 0)
            {
                GameEngine.Undo();
                GameCanvas.Children.RemoveAt(GameCanvas.Children.Count - 1);
                GameEngine.Undo();
                GameCanvas.Children.RemoveAt(GameCanvas.Children.Count - 1);
            }
            else
            {
                GameEngine.Undo();
                GameCanvas.Children.RemoveAt(GameCanvas.Children.Count - 1);
                ChangeTurn();
            }
            //Enable Redo Menu
            Window.RedoItem.IsEnabled = true;


            //Disable  Undo Menu if stack reaches 0
            if (GameEngine.UndoStack.Count == 0)
                Window.UndoItem.IsEnabled = false;
        }

        public void Redo()
        {
            if (GameFinished)
                return;


            if (Mode == 0)
            {
                GameEngine.Redo();
                var column = GameEngine.UndoStack.Pop();
                var row = GameEngine.UndoStack.Peek();

                GameEngine.UndoStack.Push(column);
                InsertSymbol(row, column, clearRedoStack: false);
                ChangeTurn();

                GameEngine.Redo();

                 column = GameEngine.UndoStack.Pop();
                row = GameEngine.UndoStack.Peek();

                GameEngine.UndoStack.Push(column);
                InsertSymbol(row, column, clearRedoStack: false);
                ChangeTurn();

            }
            else
            {
                GameEngine.Redo();
                var column = GameEngine.UndoStack.Pop();
                var row = GameEngine.UndoStack.Peek();

                GameEngine.UndoStack.Push(column);
                InsertSymbol(row, column, clearRedoStack: false);
                ChangeTurn();

            }
        }
    }
}