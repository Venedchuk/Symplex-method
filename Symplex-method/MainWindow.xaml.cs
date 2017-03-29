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
        public int[,] CoeffTable;

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
        internal void ShowIteration(int[,] coeffTableNow)
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

            for (int i = 0; i < SymplexTable[0].Count; i++)
            {
                float sum = 0;
                var j = 0;
                foreach (var item in listBoxKoef.Items)
                {
                    sum += CoeffTable[j, i] * (int)item;
                    j++;
                }
                CoeffTable[j, i] = (int)sum - CoeffTable[j, i];
            }
            ShowIteration(CoeffTable);
            x = SymplexTable.Count;
            y = SymplexTable[0].Count;

        }

     

        private void NextStep(object sender, RoutedEventArgs e)
        {

        }
    }
}
