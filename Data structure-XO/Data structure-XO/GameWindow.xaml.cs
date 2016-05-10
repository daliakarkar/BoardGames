using System.IO;
using System.Windows;
using Data_structure_XO.GuiStrategies;

namespace Data_structure_XO
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow
    {
        private readonly GuiStrategy _strategy;

        public GameWindow(int type,int mode) // 0 for XO and 1 for Connect4
        {
            InitializeComponent();
            switch (type)
            {
                case 0:
                    _strategy = new TicTacToeGuiStrategy(this,mode);
                    break;
                case 1:
                    _strategy = new ConnectFourStrategy(this,mode);
                    break;
                default:
                    return;
            }
            SizeChanged += _strategy.OnSizeChanged;
            Loaded += _strategy.OnLoaded;
            
        }

        public GameWindow(FileStream fs, int type,int mode) : this(type,mode)
        {
            _strategy.LoadGame(fs);
            fs.Close();
        }

        private void OpenGame_Click(object sender, RoutedEventArgs e)
        {
            _strategy.OpenGame();
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            _strategy.SaveGame();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            _strategy.Undo();
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            _strategy.Redo();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new OptionsWindow();
            optionsWindow.Show();
            Close();
        }

        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            _strategy.RestartGame();
        }


    }
}