using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.GameLogic
{
    class ReportOnBoard
    {
        public bool GameFinish { get; set; }
        public bool YouWon { get; set; }

        public bool CanWin { get; set; }
        public bool CanDraw { get; set; }

        public List<Position> OneMoveToFinish { get; set; }
        public List<Position> OneMoveToBlock { get; set; }

        public List<Position> TwoWinPosability { get; set; }
        public List<Position> NecessaryMove { get; set; }


    }
}
