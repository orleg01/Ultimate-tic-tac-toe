using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.GameLogic
{
    class AlphaBetaPosition : Position
    {
        
        public int BigRow
        {
            get;
            set;
        }

        public int BigCol
        {
            get;
            set;
        }

        public float Score
        {
            get;
            set;
        }

        public AlphaBetaPosition(int row, int col, int score = 0) : base(row, col)
        {
            Score = score;
        }
    }
}
