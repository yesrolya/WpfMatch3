using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace WpfMatch3
{
    static class MyAnimation
    {
        const double SWAP_DURATION = 0.5;
        const double SWAP_DELAY = 0.5;
        const double RESIZE_DURATION = 0.5;
        const double RESIZE_PX = 10;
        const double OPACITY_DURATION = 0.5;

        public static void ResizeElement (object sender, int size)
        {
            Image currentObject = (Image)sender;
            DoubleAnimation widthAnimation = new DoubleAnimation {
                From = size,
                To = size - RESIZE_PX,
                Duration = TimeSpan.FromSeconds(RESIZE_DURATION),
                AutoReverse = false
            };

            DoubleAnimation heightAnimation = new DoubleAnimation
            {
                From = size,
                To = size - RESIZE_PX,
                Duration = TimeSpan.FromSeconds(RESIZE_DURATION),
                AutoReverse = false
            };
            ThicknessAnimation marginAnimation = new ThicknessAnimation
            {
                From = new Thickness(0),
                To = new Thickness(RESIZE_PX / 2),
                Duration = TimeSpan.FromSeconds(RESIZE_DURATION),
                AutoReverse = false
            };
            
            currentObject.BeginAnimation(Image.HeightProperty, heightAnimation);
            currentObject.BeginAnimation(Image.WidthProperty, widthAnimation);
            currentObject.BeginAnimation(Image.MarginProperty, marginAnimation);
        }

        public static void SwapElements(Image img1, Image img2, bool horizontal, int quantity = 1)
        {
            DoubleAnimation move1 = new DoubleAnimation();
            DoubleAnimation move2 = new DoubleAnimation();
            move1.AutoReverse = move2.AutoReverse = false;

            move1.BeginTime = move2.BeginTime = TimeSpan.FromSeconds(SWAP_DURATION);
            move1.Duration = move2.Duration = TimeSpan.FromSeconds(SWAP_DURATION * quantity);
            
            if (horizontal)
            {
                move1.From = move2.To = Canvas.GetLeft(img1);
                move1.To = move2.From = Canvas.GetLeft(img2);

                img1.BeginAnimation(Canvas.LeftProperty, move1);
                img2.BeginAnimation(Canvas.LeftProperty, move2);
            }
            else 
            {
                move1.From = move2.To = Canvas.GetTop(img1);
                move1.To = move2.From = Canvas.GetTop(img2);

                img1.BeginAnimation(Canvas.TopProperty, move1);
                img2.BeginAnimation(Canvas.TopProperty, move2);
            }
        }

        public static void OpacityAnimation(Image img, bool hide)
        {
            DoubleAnimation opacity = new DoubleAnimation
            {
                From = (hide? 1: 0),
                To = (hide? 0: 1),
                BeginTime = TimeSpan.FromSeconds(0.5),
                Duration = TimeSpan.FromSeconds(0.5)
            };
            img.BeginAnimation(Image.OpacityProperty, opacity);
        }
    }
}
