using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Data_structure_XO
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
        }

        private void chooseMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (chooseMode.SelectedIndex == 0)
            {
                difficultyLabel.Visibility = Visibility.Hidden;
                chooseDifficulty.Visibility = Visibility.Hidden;
                Grid.SetRow(startGame, 2);
            }
            else
            {
                difficultyLabel.Visibility = Visibility.Visible;
                chooseDifficulty.Visibility = Visibility.Visible;
                Grid.SetRow(startGame, 3);
            }
        }

        private void startGame_Click(object sender, RoutedEventArgs e)
        {
            var gameWindow = new GameWindow(0);
            gameWindow.Show();
            Close();
        }
    }
}
