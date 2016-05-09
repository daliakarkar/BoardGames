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

        

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseGame.SelectedItem != null && ChooseMode.SelectedItem != null)
            {
                if (ChooseMode.SelectedIndex == 0)
                {
                    
                        var gameWindow = new GameWindow(ChooseGame.SelectedIndex,0);
                        gameWindow.Show();
                        Close();
                    
                   
                }
                else
                {
                    var gameWindow = new GameWindow(ChooseGame.SelectedIndex,1);
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
