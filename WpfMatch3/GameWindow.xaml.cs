using System;
using System.Windows;

namespace WpfMatch3
{
    
    public partial class GameWindow : Window
    {
        private GameField game;

        public GameWindow()
        {
            InitializeComponent();
            FillGAmeField();
            //game.CheckForCombinations();
        }
        
        private void FillGAmeField()
        {
            game = new GameField(GameCanvas);
        }
        
    }
}
