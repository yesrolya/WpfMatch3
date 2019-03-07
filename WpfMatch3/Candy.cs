using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfMatch3
{
    enum Color { Red = 0, Orange, Yellow, Green, Blue, Purple }
    enum TypeCandy { Usual = 0, Vertical, Horizontal, Bomb }

    class Candy
    {
        public Color color;
        public TypeCandy type;
        public Image img;
        public int size;

        public Candy(Candy original)
        {
            color = original.color;
            type = original.type;
            img = original.img;
            size = original.size;
        }
        public void Change(Random rand, int colors = 5)
        {
            color = (Color)rand.Next(0, colors);
            type = TypeCandy.Usual;
            var path = @"pack://application:,,,/Resources/" + type.ToString("F") + "/" + color.ToString("F") + ".png";
            img.Source = new BitmapImage(new Uri(path));
        }
        //создает случайный элемент обычного типа
        //привязывает его к контейнеру и устанавливает на соответствующую позицию
        public Candy(Random rand, Canvas container, int row, int column, int size, int colors = 5)
        {
            color = (Color)rand.Next(0, colors);
            type = TypeCandy.Usual;
            var path = @"pack://application:,,,/Resources/" + type.ToString("F") + "/" + color.ToString("F") + ".png";
            this.size = size;
            //изображение
            img = new Image
            {
                Width = size,
                Height = size,
                Margin = new Thickness(0),
                Source = new BitmapImage(new Uri(path)),
                Stretch = Stretch.Fill,
            };
            //обработчик нажатия на элемент
            img.MouseLeftButtonUp += ElementClicked;
            //привязка к контейнеру
            container.Children.Add(img);
            Canvas.SetLeft(img, size * column);
            Canvas.SetTop(img, size * row);
            Canvas.SetZIndex(img, 1);
        }

        public void ElementClicked(object sender, RoutedEventArgs e)
        {
            //анимация выделения элемента
            MyAnimation.ResizeElement(sender, size);
        }

        public bool Compare(Candy c)
        {
            return (this.color == c.color);
        }
        public bool IsHorizontal()
        {
            return this.type == TypeCandy.Horizontal;
        }
        public bool IsVertical()
        {
            return this.type == TypeCandy.Vertical;
        }
        public bool IsBomb()
        {
            return this.type == TypeCandy.Bomb;
        }
    }
}
