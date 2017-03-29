using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Symplex_method
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        public List<List<TextBox>> SymplexTable = new List<List<TextBox>>();
        public List<TextBox> BasicValue = new List<TextBox>();
        public float[,] CoeffTable;
        public int basisXColumn, basisYRow;
        float basis;

        private int x;
        private int y;

        #region Validation

        private void ValidateInputOnlyPositive(object sender, EventArgs e)
        {
            Validate(sender, true);
        }

        private void ValidateInputBoth(object sender, EventArgs e)
        {
            Validate(sender, false);
        }

        private static void Validate(object sender, bool excludeNonPositives)
        {
            if (!(sender is TextBox))
                return;

            var text = ((TextBox)sender).Text;

            var result = "";

            foreach (var c in text)
            {
                if (!excludeNonPositives && text.IndexOf(c) == 0 && c == '-')
                {
                    result += c;
                    continue;
                }

                var o = 0;
                if (int.TryParse(c.ToString(), out o))
                {
                    result += c;
                }
            }

                    ((TextBox)sender).Text = result;
        }

        #endregion

        private void GenerateTable(object sender, RoutedEventArgs e)
        {
#if DEBUG
            var rnd = new Random();
#endif


            List<TextBox> ListBoxValue = new List<TextBox>();
            List<TextBox> ListBoxBasis = new List<TextBox>();


            var varCount = 0; // columns
            int.TryParse(variablesCount.Text, out varCount);

            var limCount = 0;  //rows
            int.TryParse(limitsCount.Text, out limCount);

            designTable(varCount, limCount);


            SymplexTable = new List<List<TextBox>>();
            listBox.Items.Clear();

#if DEBUG
            var templatesimpl = new int[,] { { -1, 1, 1, 0, 0, 2 }, { 3, 0, -2, 1, 0, 3 }, { 1, 0, 3, 0, 1, 12 }, { -2, -1, 0, 0, 0, 0 } };
            for (int x = 0; x <= limCount; x++)
            {
                var rowModel = new List<TextBox>();
                var grid = new Grid();
                for (int y = 0; y <= varCount; y++)
                {
                    var box = new TextBox();
                    box.Margin = new Thickness(35 * y, 0, -35 * y, 0);
                    box.Width = 30;
                    box.Text = templatesimpl[x, y].ToString();
                    box.TextChanged += ValidateInputBoth;
                    rowModel.Add(box);
                    grid.Children.Add(box);
                }
                SymplexTable.Add(rowModel);
                listBox.Items.Add(grid);
            }
            return;
#endif
            for (int x = 0; x <= limCount; x++)
            {


                var rowModel = new List<TextBox>();
                var grid = new Grid();
                for (int y = 0; y <= varCount; y++)
                {
                    var box = new TextBox();
                    box.Margin = new Thickness(35 * y, 0, -35 * y, 0);
                    box.Width = 30;
                    box.TextChanged += ValidateInputBoth;
                    rowModel.Add(box);
                    grid.Children.Add(box);
                }
                SymplexTable.Add(rowModel);
                listBox.Items.Add(grid);
            }


        }

        private void designTable(int varCount = 0, int limCount = 0)
        {
            #region horiz_table
            listBoxValue.Items.Clear();

            var gridForView = new Grid();
            int i;
            for (i = 0; i < varCount; i++)
            {
                var box = new TextBox();
                box.Margin = new Thickness(35 * i, 0, -35 * i, 0);
                box.Width = 30;
                box.IsReadOnly = true;
                box.Text = "x" + ++i;
                i--;

                gridForView.Children.Add(box);
            }

            var box1 = new TextBox();
            box1.Margin = new Thickness(35 * i, 0, -35 * i, 0);
            box1.Width = 30;
            box1.Text = "free";
            box1.IsReadOnly = true;
            gridForView.Children.Add(box1);

            listBoxValue.Items.Add(gridForView);//ramka horizontal

            #endregion
            #region vert_table

            var rowModel = new List<TextBox>();
            var grid = new Grid();
            listBoxBasis.Items.Clear();
            gridForView = new Grid();
            for (i = 0; i < limCount; i++)
            {
                var box = new TextBox();
                box.Width = 30;
                box.IsReadOnly = true;
                box.Text = "x" + ++i;
                i--;
                //listBoxBasis.Items.Add(box);
                BasicValue.Add(box);
            }
            var box2 = new TextBox();
            box2.Width = 30;
            box2.IsReadOnly = true;
            box2.Text = "Func";
            //listBoxBasis.Items.Add(box2);//vertical
            BasicValue.Add(box2);

            //listBoxBasis.ItemsSource = BasicValue;

            #endregion

        }

        internal void ShowIteration(float[,] coeffTableNow)
        {
            Console.WriteLine();
            for (int i = 0; i < SymplexTable.Count; i++)
            {
                for (int j = 0; j < SymplexTable[0].Count; j++)
                {
                    if (coeffTableNow[i, j] < 0)
                        Console.Write(coeffTableNow[i, j] + " ");
                    else
                        Console.Write(" " + coeffTableNow[i, j] + " ");
                }
                Console.WriteLine(Environment.NewLine);
            }
        }
        private void Solve(object sender, RoutedEventArgs e)
        {

            var solver = new SymplexMethodChecker(SymplexTable);
            CoeffTable = solver.GetTable();
            var isBasisVariablesExist = solver.FindBasisVariables();

            if (!isBasisVariablesExist)
            {
                MessageBox.Show("NO BASIS");
                return;
            }


            if (!solver.CheckNegativeLastColumn())
            {
                MessageBox.Show("Negative in last column");
                return;
            }
            listBoxBasis.Items.Clear();
            listBoxKoef.Items.Clear();
            foreach (var item in solver.yBasisIndexes)
            {
                listBoxBasis.Items.Add(item + 1);
                listBoxKoef.Items.Add(CoeffTable[SymplexTable.Count - 1, item]);
            }
            listBoxBasis.Items.Add("Q");

            for (int i = 0; i < SymplexTable[0].Count; i++)
            {
                float sum = 0;
                var j = 0;
                foreach (var item in listBoxKoef.Items)
                {
                    sum += CoeffTable[j, i] * (float)item;
                    j++;
                }
                CoeffTable[j, i] = (int)sum - CoeffTable[j, i];
            }
            ShowIteration(CoeffTable);
            x = SymplexTable.Count;
            y = SymplexTable[0].Count;

        }



        private void NextStepPrepare(object sender, RoutedEventArgs e)
        {
            Iteration();
        }
        public void Iteration()
        {
            float MaxColumnKoef = -999;

            for (int i = 0; i < y - 1; i++)
            {
                if (MaxColumnKoef < CoeffTable[x - 1, i]&& CoeffTable[x - 1, i]>0)
                {

                    MaxColumnKoef = CoeffTable[x - 1, i];
                    basisXColumn = i;

                }

            }

            basisYRow = 999;
            for (int i = 0; i < x; i++)
            {
                if (CoeffTable[i, basisXColumn] > 0 && CoeffTable[i, basisXColumn] < basisYRow)
                {
                    basisYRow = i;
                }

                //Console.Write(CoeffTable[i, basisXColumn]);//basis column
            }
            listBoxBasis.Items[basisYRow] = basisXColumn + 1;
            Console.WriteLine(Environment.NewLine+basisYRow + "= basis coef=" + CoeffTable[basisYRow, basisXColumn]);
            if (MaxColumnKoef <= 0)
            {
                Console.WriteLine("Answer:");
                for (int i = 0; i <= x - 1; i++)
                {
                    Console.Write("x" + listBoxBasis.Items[i] + ":" + CoeffTable[i, y - 1] + " ");
                }
                return;
            }
            basis = CoeffTable[basisYRow, basisXColumn];//basis point

            for (int i = 0; i < y; i++)
            {
                CoeffTable[basisYRow, i] = CoeffTable[basisYRow, i] / basis;
            }
            for (int i = 0; i < x; i++)//walk to tables exlude basis row and cell
            {
                for (int j = 0; j < y; j++)
                {
                    if (i == basisXColumn + 1 || j == basisYRow - 1)//
                        continue;
                    CoeffTable[i, j] = CoeffTable[i, j] - (CoeffTable[basisYRow, j] * CoeffTable[i, basisXColumn]);
                }
                Console.WriteLine();
            }
            for (int i = 0; i < x ; i++)
            {
                if (i!=basisYRow)
                    CoeffTable[i, basisXColumn] = 0;
                else
                    CoeffTable[i, basisXColumn] = 1;

            }

            for (int i = 0; i < x; i++)//walk to tables exlude basis row and cell
            {
                for (int j = 0; j < y; j++)
                {
                    //if (i == basisXColumn + 1 || j == basisYRow - 1)//
                    //    continue;
                    Console.Write(" {0:0.0}", CoeffTable[i, j]);
                    //CoeffTable[i, j] = CoeffTable[i, j] - (CoeffTable[basisXColumn - 1, j] * CoeffTable[i, basisYRow - 1]);
                }
                Console.WriteLine();
            }
        }
    }
}
