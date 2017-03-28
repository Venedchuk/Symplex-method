using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Symplex_method
{
    public class SymplexMethodSolver
    {
        public int[,] coeffTable;
        public List<int> yBasisIndexes = new List<int>();
        private int x;
        private int y;

        public SymplexMethodSolver(List<List<TextBox>> symplexTable)
        {
            x = symplexTable.Count;
            y = symplexTable[0].Count;

            coeffTable = new int[x, y];

            foreach (var row in symplexTable)
            {
                foreach (var item in row)
                {
                    var val = int.Parse(item.Text);

                    coeffTable[symplexTable.IndexOf(row), row.IndexOf(item)] = val;
                }
            }
        }

        public bool FindBasisVariables()
        {
            var totalbasisfound = true;

            for (int i = 0; i < x - 1; i++)
            {
                var isrowcontainsbasis = false;

                for (int j = 0; j < y; j++)
                {
                    if (coeffTable[i, j] != 1)
                        continue;

                    var zerosCount = 0;

                    for (int k = 0; k < x - 1; k++)
                    {
                        if (k == i)
                            continue;

                        if (coeffTable[k, j] == 0)
                            zerosCount++;
                    }

                    isrowcontainsbasis = zerosCount == (x - 2);

                    if (isrowcontainsbasis)
                    {
                        yBasisIndexes.Add(j);
                        break;
                    }
                }

                totalbasisfound = totalbasisfound && isrowcontainsbasis;
            }
            
            return totalbasisfound;
        }


        public bool CheckNegativeLastColumn()
        {

            for (int i = 0; i < x; i++)
            {
                if (coeffTable[i, y - 1] < 0)
                    return false;
            }

            return true;
        }
    }
}
