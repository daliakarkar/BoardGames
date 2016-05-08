using System.Windows;
using System.Windows.Controls;

namespace Data_structure_XO
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow
    {
        public OptionsWindow()
        {
            InitializeComponent();
        }

        private void chooseMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChooseMode.SelectedIndex == 1)
            {
                DifficultyLabel.Visibility = Visibility.Hidden;
                ChooseDifficulty.Visibility = Visibility.Hidden;
                Grid.SetRow(StartGame, 2);
            }
            else
            {
                DifficultyLabel.Visibility = Visibility.Visible;
                ChooseDifficulty.Visibility = Visibility.Visible;
                Grid.SetRow(StartGame, 3);
            }
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseGame.SelectedItem != null && ChooseMode.SelectedItem != null)
            {
                if (ChooseMode.SelectedIndex == 0)
                {
                    if (ChooseDifficulty.SelectedItem != null)
                    {
                        var gameWindow = new GameWindow(ChooseGame.SelectedIndex);
                        gameWindow.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("You must select all game options.", "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
                    } 
                }
                else
                {
                    var gameWindow = new GameWindow(ChooseGame.SelectedIndex);
                    gameWindow.Show();
                    Close();
                }
            }
            else
            {
                MessageBox.Show("You must select all game options.", "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
