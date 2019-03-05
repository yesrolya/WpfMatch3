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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        //создает случайный элемент обычного типа
        //привязывает его к контейнеру и устанавливает на соответствующую позицию
        public Candy(Random rand, Canvas container, int row, int column, int size, int colors = 6)
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
            Image currentObject = (Image)sender;
            //анимация выделения элемента

            int resize = 10;
            double duration = 0.2;
            
            DoubleAnimation widthAnimation = new DoubleAnimation();
            DoubleAnimation heightAnimation = new DoubleAnimation();
            ThicknessAnimation marginAnimation = new ThicknessAnimation();
            heightAnimation.AutoReverse = widthAnimation.AutoReverse = marginAnimation.AutoReverse = true;
            heightAnimation.Duration = widthAnimation.Duration = marginAnimation.Duration = TimeSpan.FromSeconds(duration);

            heightAnimation.From = widthAnimation.From = size;
            heightAnimation.To = widthAnimation.To = size - resize;
            marginAnimation.From = new Thickness(0);
            marginAnimation.To = new Thickness(resize / 2);

            currentObject.BeginAnimation(Image.HeightProperty, heightAnimation);
            currentObject.BeginAnimation(Image.WidthProperty, widthAnimation);
            currentObject.BeginAnimation(Image.MarginProperty, marginAnimation);
        }

        public bool Compare (Candy c)
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

    class GameField
    {
        int rows;
        int columns;
        Candy[,] candies;
        public int size;
        Canvas gameField;
        Random rand;

        public GameField (Canvas GF, int rows = 8, int columns = 8, int colors = 6)
        {
            this.gameField = GF;
            GF.Height = 500;
            this.rows = rows;
            this.columns = columns;
            this.size = (int) 500 / rows; //они же квадратные
            candies = new Candy[rows, columns];
            
            rand = new Random();
            //upper left corner
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    candies[i, j] = new Candy(rand, GF, i, j, size, colors);
                    candies[i,j].img.MouseLeftButtonUp += ElementSelected;
                }
        }
        
        private int GetRow(Image im)
        {
            return (int)Canvas.GetTop(im) / size;
        }
        private int GetColumn(Image im)
        {
            return (int)Canvas.GetLeft(im) / size;
        }
        private Candy GetCandy(Image im)
        {
            return candies[GetRow(im), GetColumn(im)];
        }
        

        bool selected = false;
        int row1, col1;


        public void ElementSelected(object sender, RoutedEventArgs e)
        {
            Image currentObject = (Image)sender;
            //какие объекты выделены
            if (!selected)
            {
                selected = true;
                row1 = GetRow(currentObject);
                col1 = GetColumn(currentObject);
            }
            else
            {
                selected = false;
                int row2 = GetRow(currentObject);
                int col2 = GetColumn(currentObject);
                //проверка того, кликнуты ли соседние элементы или тот же
                int difference = Math.Abs(row1 - row2) + Math.Abs(col1 - col2);
                Console.WriteLine("РАЗНИЦА = " + difference);
                if (difference == 1)
                    //выделены соседние элементы
                    SwapElements(row1, col1, row2, col2);
            }
            
        }

        private void ChangeIndexes(int row1, int col1, int row2, int col2)
        {
            Candy temp = new Candy(candies[row2, col2]);
            candies[row2, col2] = candies[row1, col1];
            candies[row1, col1] = temp;
        }

        private void SwapElements(int row1, int col1, int row2, int col2)
        {
            ChangeIndexes(row1, col1, row2, col2);

            DoubleAnimation move1 = new DoubleAnimation();
            DoubleAnimation move2 = new DoubleAnimation();
            move1.AutoReverse = move2.AutoReverse = false;

            move1.BeginTime = move2.BeginTime = TimeSpan.FromSeconds(0.5);
            move1.Duration = move2.Duration = TimeSpan.FromSeconds(0.5);

            //Console.WriteLine("c1 start index: " + c1.row + " " + c1.column + " position: " + Canvas.GetTop(c1.img) + " " + Canvas.GetLeft(c1.img));
            //Console.WriteLine("c2 start index: " + c2.row + " " + c2.column + " position: " + Canvas.GetTop(c2.img) + " " + Canvas.GetLeft(c2.img));
            //движение по горизонтали
            if (row1 == row2)
            {
                Console.WriteLine("HORIZONTAL");
                move1.From = move2.To = Canvas.GetLeft(candies[row1, col1].img);
                move1.To = move2.From = Canvas.GetLeft(candies[row2, col2].img);

                candies[row1, col1].img.BeginAnimation(Canvas.LeftProperty, move1);
                candies[row2, col2].img.BeginAnimation(Canvas.LeftProperty, move2);
            }
            //движение по вертикали
            else if (col1 == col2)
            {
                Console.WriteLine("VERTICAL");
                move1.From = move2.To = Canvas.GetTop(candies[row1, col1].img);
                move1.To = move2.From = Canvas.GetTop(candies[row2, col2].img);

                candies[row1, col1].img.BeginAnimation(Canvas.TopProperty, move1);
                candies[row2, col2].img.BeginAnimation(Canvas.TopProperty, move2);
            }
            Console.WriteLine("SWAP (" + row1 + "," + col1 + ") and ( " + row2 + "," + col2 + ") ");

            CheckForOneElementCombination(row1, col1);
            CheckForOneElementCombination(row2, col2);
        }

        List<int[]> combinations; //список элементов, которые нужно удалить
        List<int[]> new_candies; //список элементов, которые будут созданы

        public void CheckForCombinations()
        {
            combinations = new List<int[]>();
            new_candies = new List<int[]>();
            //check for horizontal lines
            for (int i = 0; i < rows; i++)
            {
                int line = 0, startj = 0; ;
                for (int j = 0; j < columns; j++)
                {
                    if (line == 0)
                    {
                        line++;
                        startj = j;
                    }
                    else
                    {
                        //цвета элементов соврадают - продолжаем просматривать
                        if (candies[i, j].Compare(candies[i, startj]))
                            line++;
                        //если не совпадают
                        else
                        {
                            if (line >= 3)
                            {
                                for (int k = startj; k < j; k++)
                                    combinations.Add(new int[] { i, k });
                            }
                            line = 0;
                        }
                    }
                }
            }

            //check for vertical lines
            for (int j = 0; j < columns; j++)
            {
                int line = 0, starti = 0; 
                for (int i = 0; i < rows; i++)
                {
                    if (line == 0)
                    {
                        line++;
                        starti = i;
                    }
                    else
                    {
                        //цвета элементов соврадают - продолжаем просматривать
                        if (candies[i, j].Compare(candies[starti, j]))
                            line++;
                        //если не совпадают
                        else
                        {
                            if (line >= 3)
                            {
                                for (int k = starti; k < i; k++)
                                {
                                    int[] newEl = new int[] { k, j };
                                    combinations.Add(newEl);                                    
                                }
                            }
                            line = 0;
                        }
                    }
                }
            }
            //delete them
            DeleteElements();

            //put down elements above
        }

        public int[] CheckForOneElementCombination (int row, int column)
        {
            int[] result = new int[5]; //top, right, bottom, left
            int count = 0;
            for (int i = row - 1; (i >= 0) && (candies[row, column].Compare(candies[i, column])); i--) 
                count++;
            result[0] = count;

            count = 0;
            for (int j = column + 1; (j < columns) && (candies[row, column].Compare(candies[row, j])); j++)
                count++;
            result[1] = count;

            count = 0;
            for (int i = row + 1; (i < rows) && (candies[row, column].Compare(candies[i, column])); i++)
                count++;
            result[2] = count;

            count = 0;
            for (int j = column - 1; (j >= 0) && (candies[row, column].Compare(candies[row, j])); j--)
                count++;
            result[3] = count;

            result[4] = (result[0] + result[2] >= 2 || result[1] + result[3] >= 2)?  1 : 0;
            Console.WriteLine((result[4] == 0)? "NO COMBOS": "CAN BE DELETED");
            return result;
        }

        void DeleteElements ()
        {
            Console.WriteLine("QUANTITY " + combinations.Count());
            for (int i = 0; i < combinations.Count(); i++)
            {
                DoubleAnimation opacity = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    BeginTime = TimeSpan.FromSeconds(0.5),
                    Duration = TimeSpan.FromSeconds(0.5)
                };

                candies[combinations[i][0], combinations[i][1]].img.BeginAnimation(Image.OpacityProperty , opacity);
            }
        }
    }

    public partial class GameWindow : Window
    {
        private GameField game;

        public GameWindow()
        {
            InitializeComponent();
            FillGAmeField();
            game.CheckForCombinations();
        }
        
        private void FillGAmeField()
        {
            game = new GameField(GameCanvas);
        }
        
    }
}
