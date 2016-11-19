using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.GameLogic
{
    class ReportOnBoard
    {
        bool gameFinish;
        bool youWon;

        bool canWin;
        bool canDraw;

        List<Position> oneMoveToFinish;
        List<Position> oneMoveToBlock;

        List<Position> twoWinPosability;
        List<Position> necessaryMove;


    }
}
