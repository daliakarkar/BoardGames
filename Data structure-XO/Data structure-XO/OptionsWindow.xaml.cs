using System.Windows;

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
                var gameWindow = new GameWindow(ChooseGame.SelectedIndex, ChooseMode.SelectedIndex);
                gameWindow.Show();
                Close();               
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
