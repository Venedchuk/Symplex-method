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

#if DEBUG
                    box.Text = rnd.Next(-2, 10).ToString();
#endif

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
                box.Text = "x" + ++i;
                i--;

                gridForView.Children.Add(box);

            }

            var box1 = new TextBox();
            box1.Margin = new Thickness(35 * i, 0, -35 * i, 0);
            box1.Width = 30;
            box1.Text = "free";
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
                box.Text = "x" + ++i;
                i--;
                listBoxBasis.Items.Add(box);
            }
            var box2 = new TextBox();
            box2.Width = 30;
            box2.Text = "Func";
            listBoxBasis.Items.Add(box2);//vertical
            #endregion

        }

        private void Solve(object sender, RoutedEventArgs e)
        {
            var solver = new SymplexMethodSolver(SymplexTable);

            var isBasisVariablesExist = solver.FindBasisVariables();

            if (!isBasisVariablesExist)
            {
                MessageBox.Show("NO BASIS LUL PogChamp");
                return;
            }


            if (!solver.CheckNegativeLastColumn())
            {
                MessageBox.Show("Negative in last column LUL PogChamp");
                return;
            }

        }
    }
}
