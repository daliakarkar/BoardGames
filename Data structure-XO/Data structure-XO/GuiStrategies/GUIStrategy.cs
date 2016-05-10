using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Data_structure_XO.GameEngines;
using Microsoft.Win32;

namespace Data_structure_XO.GuiStrategies
{
    public abstract class GuiStrategy
    {
        protected const int BoardZIndex = 1;
        protected const int SymbolZIndex = 2;
        protected Image GameBoard;
        protected Canvas GameCanvas;
        protected GameEngine GameEngine;
        protected bool GameFinished;
        protected ResourceDictionary GamesResourceDictionary;


        protected GuiStrategy(GameWindow window, int mode)
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

        protected abstract int NumOfRows { get; }
        protected abstract int NumOfColumns { get; }


        protected int Mode { get; set; }

        public GameWindow Window { get; }

        protected abstract double OriginalBoardWidth { get; }

        public abstract void InitializeBoard();
        protected abstract BitmapImage GetPlayerSymbol(GameEngine.Token t);

        public void InsertSymbol(int row, int column, GameEngine.Token token = GameEngine.Token.Empty,
            bool clearRedoStack = true)
        {
            if (clearRedoStack)
                GameEngine.RedoStack.Clear();

            Window.UndoItem.IsEnabled = GameEngine.UndoStack.Count >= 2;
            Window.RedoItem.IsEnabled = GameEngine.RedoStack.Count >= 2;
            if (token == GameEngine.Token.Empty)
                token = GameEngine.CurrentPlayer;
            var symbolImage = GetPlayerSymbol(token);
            var symbol = new Image
            {
                Source = symbolImage,
                Width = symbolImage.Width*GetSizeRatio(),
                Height = symbolImage.Height*GetSizeRatio()
            };

            GameCanvas.Children.Add(symbol);
            Panel.SetZIndex(symbol, SymbolZIndex);
            var offset = CalculateSymbolOffset(row, column, symbol);

            //Move it    
            Canvas.SetTop(symbol, offset.Key);
            Canvas.SetLeft(symbol, offset.Value);
        }

        protected abstract void GameCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e);


        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            RedrawGame();
        }

        protected void DrawSymbolsFromGameEngine()
        {
            for (var i = 0; i < NumOfRows; i++)
            {
                for (var j = 0; j < NumOfColumns; j++)
                {
                    var t = GameEngine.GetTileValue(i, j);
                    if (t != GameEngine.Token.Empty)
                        InsertSymbol(i, j, t);
                }
            }
        }

        public void RestartGame()
        {
            GameFinished = false;
            Window.RedoItem.IsEnabled = false;
            Window.UndoItem.IsEnabled = false;

            GameEngine.Restart();

            RedrawGame();
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
            var token = (char) fs.ReadByte();
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
                    RedrawGame();
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
            if (Mode == 0 && GameEngine.CurrentPlayer == GameEngine.Token.Two)
            {
                GameCanvas.MouseLeftButtonUp -= GameCanvas_OnMouseLeftButtonUp;
            }
            else
            {
                GameCanvas.MouseLeftButtonUp += GameCanvas_OnMouseLeftButtonUp;
            }
        }

        protected void UpdateStatusBar()
        {
            switch (GameEngine.CurrentPlayer)
            {
                case GameEngine.Token.One:
                    Window.Statusbar.Content = "Turn : Player 1";
                    break;
                case GameEngine.Token.Two:
                    Window.Statusbar.Content = "Turn : Player 2";
                    break;
                case GameEngine.Token.Empty:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected double GetSizeRatio()
        {
            GameBoard.UpdateLayout();
            var ratio = GameBoard.ActualWidth/OriginalBoardWidth;
            return ratio;
        }

        public void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            RedrawGame();
        }

        protected void RedrawGame()
        {
            GameCanvas.Children.Clear();
            InitializeBoard();
            GameBoard.UpdateLayout();
            DrawSymbolsFromGameEngine();
            UpdateStatusBar();
        }

        public void Undo()
        {
            if (GameFinished)
                return;
            var undoCount = Mode == 0 ? 2 : 1;
            for (var i = 0; i < undoCount; i++)
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

            var redoCount = Mode == 0 ? 2 : 1;
            for (var i = 0; i < redoCount; i++)
            {
                GameEngine.Redo();
                var column = GameEngine.UndoStack.Pop();
                var row = GameEngine.UndoStack.Peek();

                GameEngine.UndoStack.Push(column);
                InsertSymbol(row, column, clearRedoStack: false);
                ChangeTurn();
            }
        }

        protected abstract KeyValuePair<double, double> CalculateSymbolOffset(int row, int column, Image symbolImage);
    }
}