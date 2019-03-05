using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfMatch3
{
    class GameField
    {
        int rows;
        int columns;
        Candy[,] candies;
        public int size;
        Canvas gameField;
        Random rand;

        public GameField(Canvas GF, int rows = 8, int columns = 8, int colors = 6)
        {
            this.gameField = GF;
            GF.Height = 500;
            this.rows = rows;
            this.columns = columns;
            this.size = (int)500 / rows; //они же квадратные
            candies = new Candy[rows, columns];

            rand = new Random();
            //upper left corner
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    candies[i, j] = new Candy(rand, GF, i, j, size, colors);
                    candies[i, j].img.MouseLeftButtonUp += ElementSelected;
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
                int difference = Math.Abs(row1 - row2) + Math.Abs(col1 - col2);
                Console.WriteLine("РАЗНИЦА = " + difference);
                if (difference == 1)
                    SwapElements(row1, col1, row2, col2);
            }
        }
        
        private bool SwapElements(int row1, int col1, int row2, int col2, int quantity = 1)
        {
            Candy temp = new Candy(candies[row2, col2]);
            candies[row2, col2] = candies[row1, col1];
            candies[row1, col1] = temp;

            if (row1 == row2)
            {
                Console.WriteLine("HORIZONTAL");
                MyAnimation.SwapElements(candies[row1, col1].img, candies[row2, col2].img, true);
            }
            else if (col1 == col2)
            {
                Console.WriteLine("VERTICAL");
                MyAnimation.SwapElements(candies[row1, col1].img, candies[row2, col2].img, false);
            }
            Console.WriteLine("SWAP (" + row1 + "," + col1 + ") and ( " + row2 + "," + col2 + ") ");

            if (CheckForOneElementCombination(row1, col1)[4] != 0 ||
            CheckForOneElementCombination(row2, col2)[4] != 0)
                return true;
            else
                return false;
        }

        List<int[]> combinations; //список элементов, которые нужно удалить

        public void FindCombinations()
        {
            combinations = new List<int[]>();
            //check for horizontal lines
            for (int i = 0; i < rows; i++)
            {
                int line = 0, startj = 0;
                for (int j = 0; j < columns; j++)
                {
                    if (line == 0)
                    {
                        line++;
                        startj = j;
                    }
                    else
                    {
                        if (candies[i, j].Compare(candies[i, startj]))
                        {   //цвета элементов соврадают - продолжаем просматривать
                            line++;
                            if (j == columns - 1)
                            {
                                Console.WriteLine("HORIZONTALLINE " + i + " " + startj + " " + line);
                                if (line >= 3)
                                    for (int k = startj; k <= j; k++)
                                        combinations.Add(new int[] { i, k });
                            }
                        }
                        else
                        {   //если не совпадают
                            if (j == columns - 1)
                                Console.WriteLine("HORIZONTALLINE " + i + " " + startj + " " + line);
                            Console.WriteLine("HORIZONTALLINE " + i + " " + startj + " " + line);
                            if (line >= 3)
                                for (int k = startj; k < j; k++)
                                    combinations.Add(new int[] { i, k });
                            line = 1;
                            startj = j;
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
                        if (candies[i, j].Compare(candies[starti, j]))
                        {
                            line++;
                            if (i == rows - 1)
                            {
                                Console.WriteLine("VERTICALLINE " + starti + " " + j + " " + line);
                                if (line >= 3)
                                    for (int k = starti; k <= i; k++)
                                        combinations.Add(new int[] { k, j });
                            }
                        }
                        else
                        {
                            Console.WriteLine("VERTICALLINE " + starti + " " + j + " " + line);
                            if (line >= 3)
                                for (int k = starti; k < i; k++)
                                    combinations.Add(new int[] { k, j });
                            line = 1;
                            starti = i;
                        }
                    }
                }
            }

        }

        public int[] CheckForOneElementCombination(int row, int column)
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

            result[4] = (result[0] + result[2] >= 2 || result[1] + result[3] >= 2) ? 1 : 0;
            Console.WriteLine((result[4] == 0) ? "NO COMBOS" : "CAN BE DELETED");
            return result;
        }

        void RemovingElementsAnimation()
        {
            Console.WriteLine("QUANTITY " + combinations.Count());
            for (int i = 0; i < combinations.Count(); i++)
            {
                MyAnimation.OpacityAnimation(
                    candies[combinations[i][0], combinations[i][1]].img, true);
            }
        }

        void PutDownAfterRemoving()
        {
            //снизу вверх по колонкам
            for (int j = 0; j < columns; j++)
            {
                int qDown = 0;
                for (int i = rows - 1; i >= 0; i--)
                {
                    if (combinations.IndexOf(new int[] { i, j }) != -1)
                    {
                        qDown++;
                    }
                    else if (qDown != 0)
                    {
                        SwapElements(i, j, i - qDown, j, qDown);
                        //PutDownOneElement(i, j, qDown);
                    }
                }

                for (int q = 0; q < qDown; q++)
                {
                    CreateNewCandy(q, j);
                }
            }
        }

        void CreateNewCandy(int row, int column)
        {
            candies[row, column].Change(rand);
        }

        //void PutDownOneElement(int row, int column, int quantity)
        //{
        //    DoubleAnimation down = new DoubleAnimation
        //    {
        //        From = Canvas.GetTop(candies[row, column].img),
        //        To = Canvas.GetTop(candies[row, column].img) - (size * quantity),
        //        BeginTime = TimeSpan.FromSeconds(1),
        //        Duration = TimeSpan.FromSeconds(0.5 * quantity)
        //    };
        //    candies[row, column].img.BeginAnimation(Canvas.TopProperty, down);
        //    candies[row - quantity, column] = candies[row, column];

        //    //candies[row, column] = null;
        //}



    }

}
