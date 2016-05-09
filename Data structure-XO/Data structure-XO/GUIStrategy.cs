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
        protected ResourceDictionary gamesResourceDictionary;
        protected GameEngine gameEngine;
        protected Image gameBoard;
        protected const int BoardZIndex = 1;
        protected const int SymbolZIndex = 2;
        protected bool gameFinished = false;
        protected Canvas GameCanvas;
        private bool LoadGameLater = false;


        public GuiStrategy(GameWindow window, int mode)
        {
            Window = window;
            GameCanvas = Window.GameCanvas;
            GameCanvas.MouseLeftButtonUp += GameCanvas_OnMouseLeftButtonUp;
            gamesResourceDictionary = new ResourceDictionary
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
                gameEngine.RedoStack.Clear();
            if (gameEngine.RedoStack.Count == 0)
                Window.RedoItem.IsEnabled = false;
            Window.UndoItem.IsEnabled = true;
        }

        protected abstract void GameCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e);


        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeBoard();
            if (LoadGameLater)
            {
                LoadGameLater = false;
                gameBoard.UpdateLayout();

                DrawSymbolsFromGameEngine();
            }
        }

        protected abstract void DrawSymbolsFromGameEngine();

        public void RestartGame()
        {
            gameFinished = false;
            Window.RedoItem.IsEnabled = false;
            Window.UndoItem.IsEnabled = false;

            gameEngine.Restart();
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
            gameEngine.SaveGame(fs);
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
                gameEngine.LoadGame(fs);
                fs.Close();
                if (Window.IsLoaded)
                {
                    GameCanvas.Children.Clear();
                    InitializeBoard();
                    gameBoard.UpdateLayout();
                    DrawSymbolsFromGameEngine();
                }
                else
                {
                    LoadGameLater = true;
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
            if (gameEngine.IsGameWon(row, column))
            {
                gameFinished = true;
                MessageBox.Show("Player " + gameEngine.CurrentPlayer + " Won !", "Result",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (gameEngine.IsGameDraw())
            {
                gameFinished = true;
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
            gameEngine.ChangeTurn();
        }

        protected double GetSizeRatio()
        {
            double ratio;
            gameBoard.UpdateLayout();
            ratio = gameBoard.ActualWidth/OriginalBoardWidth;
            return ratio;
        }

        protected abstract double OriginalBoardWidth { get; }

        public void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            //this is a bad solution if used in a heavy duty app
            GameCanvas.Children.Clear();
            InitializeBoard();
            gameBoard.UpdateLayout();
            DrawSymbolsFromGameEngine();
        }

        public void Undo()
        {
            if (gameFinished)
                return;
            gameEngine.Undo();
            GameCanvas.Children.RemoveAt(GameCanvas.Children.Count - 1);
            ChangeTurn();
            //Enable Redo Menu
            Window.RedoItem.IsEnabled = true;


            //Disable  Undo Menu if stack reaches 0
            if (gameEngine.UndoStack.Count == 0)
                Window.UndoItem.IsEnabled = false;
        }

        public void Redo()
        {
            if (gameFinished)
                return;
            gameEngine.Redo();
            var column = gameEngine.UndoStack.Pop();
            var row = gameEngine.UndoStack.Peek();

            gameEngine.UndoStack.Push(column);
            InsertSymbol(row, column, clearRedoStack: false);
            ChangeTurn();
        }
    }
}