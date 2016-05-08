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
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {

        public GameWindow(int type) // 0 for XO and 1 for Connect4
        {
            InitializeComponent();
            GameEngine gameEngine;
            switch (type) 
            {
                case 0:
                    gameEngine = new GameXO();
                    break;
                case 1:
                    gameEngine = new GameConnect4();
                    break;
            }
        }
    }
}
