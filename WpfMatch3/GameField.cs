using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfMatch3
{
    class GameField
    {
        Label ScoreLabel;
        int score;
        int rows;
        int columns;
        Candy[,] candies;
        public int size;
        Canvas gameField;
        Random rand;

        public GameField(Canvas GF, Label scoreLab, int rows = 8, int columns = 8, int colors = 5)
        {
            score = 0;
            ScoreLabel = scoreLab;
            ScoreLabel.Content = score;
            this.gameField = GF;
            GF.Height = 500;
            this.rows = rows;
            this.columns = columns;
            this.size = (int)500 / rows; //они же квадратные
            candies = new Candy[rows, columns];
            combinations = new List<string>();
            rand = new Random();
            //upper left corner
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    candies[i, j] = new Candy(rand, GF, i, j, size, colors);
                    candies[i, j].img.MouseLeftButtonUp += ElementSelected;
                }
            FindAndDeleteCombos();
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
                if (difference == 1)
                {
                    SwapElements(row1, col1, row2, col2);
                    if (CheckForOneElementCombination(row1, col1) ||
                        CheckForOneElementCombination(row2, col2))
                    {
                        FindAndDeleteCombos();
                    }
                    else
                    {
                        SwapElements(row1, col1, row2, col2);
                    }
                }
            }
        }
        
        private void ChangeIndexesOnly(int row1, int col1, int row2, int col2)
        {
            Candy temp = new Candy(candies[row2, col2]);
            candies[row2, col2] = candies[row1, col1];
            candies[row1, col1] = temp;
        }

        private void SwapElements(int row1, int col1, int row2, int col2, int quantity = 1)
        {
            ChangeIndexesOnly(row1, col1, row2, col2);

            if (row1 == row2)
                MyAnimation.SwapTwoImages(candies[row1, col1].img, candies[row2, col2].img, true, quantity);
            else if (col1 == col2)
                MyAnimation.SwapTwoImages(candies[row1, col1].img, candies[row2, col2].img, false, quantity);
        }

        private bool NeedToMoveImage(int row, int column)
        {
            if  (row == GetRow(candies[row, column].img) && 
                (column == GetColumn(candies[row, column].img)))
                return false;
            else
                return true;
        }
        public void MoveImages()
        {
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    if (NeedToMoveImage(i, j))
                        MyAnimation.MoveOneImage(i, j, size, candies[i, j].img);
        }

        List<string> combinations; //список элементов, которые нужно удалить
        //combinations = {column, row}

        public void FindAndDeleteCombos()
        {
            FindCombinations();
            RemovingElementsAnimation();
            PutDownAfterRemoving();

            while (combinations.Count() != 0)
            {
                FindCombinations();
                RemovingElementsAnimation();

                PutDownAfterRemoving();
            }
            MoveImages();
        }

        public void FindCombinations()
        {
            combinations.Clear();
            
            for (int i = 0; i < rows; i++) //ГОРИЗОНТАЛЬНЫЕ ЛИНИИ
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
                        {   
                            line++;
                            if (j == columns - 1)
                                if (line >= 3)
                                    for (int k = startj; k <= j; k++)
                                        combinations.Add(""+ k + i);
                        }
                        else
                        {
                            if (line >= 3)
                                for (int k = startj; k < j; k++)
                                    combinations.Add("" + k + i);
                            line = 1;
                            startj = j;
                        }
                    }
                }
            }
            for (int j = 0; j < columns; j++) //ВЕРТИКАЛЬНЫЕ ЛИНИИ
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
                                if (line >= 3)
                                    for (int k = starti; k <= i; k++)
                                        combinations.Add("" +  j + k );
                        }
                        else
                        {
                            if (line >= 3)
                                for (int k = starti; k < i; k++)
                                    combinations.Add("" + j + k);
                            line = 1;
                            starti = i;
                        }
                    }
                }
            }
        }

        public bool CheckForOneElementCombination(int row, int column)
        {
            int[] result = new int[4] { 0, 0, 0, 0 }; //top, right, bottom, left
            for (int i = row - 1; (i >= 0) && (candies[row, column].Compare(candies[i, column])); i--)
                result[0]++;
            for (int j = column + 1; (j < columns) && (candies[row, column].Compare(candies[row, j])); j++)
                result[1]++;
            for (int i = row + 1; (i < rows) && (candies[row, column].Compare(candies[i, column])); i++)
                result[2]++;
            for (int j = column - 1; (j >= 0) && (candies[row, column].Compare(candies[row, j])); j--)
                result[3]++;

            bool res = ((result[0] + result[2]) >= 2 || (result[1] + result[3]) >= 2) ? true : false;
            
            return res;
        }

        void RemovingElementsAnimation()
        {
            for (int i = 0; i < combinations.Count(); i++)
            {
                MyAnimation.OpacityAnimation(
                        candies[Int32.Parse("" + combinations[i][1]), Int32.Parse("" + combinations[i][0])].img, true);
            }
        }

        void PutDownAfterRemoving()
        {
            for (int j = 0; j < columns; j++)
            {
                int qDown = 0;
                for (int i = rows - 1; i >= 0; i--)
                {
                    if (combinations.IndexOf("" + j + i) != -1) 
                        qDown++;
                    else if (qDown != 0)
                        ChangeIndexesOnly(i, j, i + qDown, j);
                }
                MoveImages();

                for (int q = 0; q < qDown; q++)
                {
                    CreateNewCandy(q, j);
                    score += 10;
                    ScoreLabel.Content = score;
                }
            }
        }

        void CreateNewCandy(int row, int column)
        {
            candies[row, column].Change(rand);
            MyAnimation.OpacityAnimation(candies[row, column].img, false);
        }
    }

}
