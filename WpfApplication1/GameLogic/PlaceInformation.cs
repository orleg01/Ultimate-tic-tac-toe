using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.GameLogic
{
    class PlaceInformation
    {
        public int BigCol
        {
            get;
            set;
        }
        public int BigRow
        {
            get;
            set;
        }
        public int PosRow
        {
            get;
            set;
        }
        public int PosCol
        {
            get;
            set;
        }

        public PlaceInformation(int bigRow, int posRow, int bigCol, int posCol)
        {
            BigRow = bigRow;
            PosRow = posRow;
            BigCol = bigCol;
            PosCol = posCol;
        }
    }
}
