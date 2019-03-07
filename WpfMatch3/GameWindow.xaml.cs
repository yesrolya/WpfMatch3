using System;
using System.Windows;

namespace WpfMatch3
{
    
    public partial class GameWindow : Window
    {
        private GameField game;
        private MyTimer timer;

        public GameWindow()
        {
            InitializeComponent();
            StartGame();
        }
        
        private void StartGame()
        {
            int maxTime = 60;
            game = new GameField(GameCanvas, ScoreLabel);
            timer = new MyTimer(game, TimeBar, TimeLabel, maxTime);
        }
    }
}
