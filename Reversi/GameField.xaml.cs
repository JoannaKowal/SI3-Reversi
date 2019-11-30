using ReversiBackend;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reversi
{
    /// <summary>
    /// Logika interakcji dla klasy GameField.xaml
    /// </summary>
    public partial class GameField : UserControl
    {
        int fieldId;
        MainWindow controller;
        public GameField()
        {
            InitializeComponent();
        }
        public void Initialize(MainWindow UIController, int fieldId)
        {
            this.controller = UIController;
            this.fieldId = fieldId;
        }
        public void RefreshImage(PlayerNumber playerNumber)
        {
            if (playerNumber == PlayerNumber.None)
            {
                VisualState.Source = new BitmapImage(new Uri("Resources/pawn_empty.png", UriKind.Relative));
            }
            else if (playerNumber == PlayerNumber.FirstPlayer)
            {
                VisualState.Source = new BitmapImage(new Uri("Resources/pawn_white.png", UriKind.Relative));
               
            }
            else
            {
                VisualState.Source = new BitmapImage(new Uri("Resources/pawn_Black.png", UriKind.Relative));
            }
        }
        private List<GameState> GetNextPossibleStates(PlayerNumber playerNumber)
        {
            List<GameState> gameStates = new List<GameState>();
            return gameStates;
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.controller.HandleButtonClick(this.fieldId);
        }
    }
}
