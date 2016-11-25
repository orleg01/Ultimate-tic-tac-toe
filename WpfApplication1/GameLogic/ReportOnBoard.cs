using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.GameLogic
{
    class ReportOnBoard
    {
        public bool gameFinish;
        public bool youWon;

        private bool canWin;

        public enum TypeOfArr { FINISH , BLOCK , OTHER };

        private List<Position> oneMoveToFinish;
        private List<Position> needToBlock;
        private List<Position> allPossibleMove;

        public List<Position> MoveFinish
        {
            get
            {
                return oneMoveToFinish.Count > 0 ? oneMoveToFinish : null;
            }
        }

        public List<Position> MoveBlock
        {
            get
            {
                return needToBlock.Count > 0 ? needToBlock : null;
            }
        }

        public List<Position> AllPossibleMoves
        {
            get
            {
                return allPossibleMove.Count > 0 ? allPossibleMove : null;
            }
        }

        public ReportOnBoard()
        {
            oneMoveToFinish = new List<Position>();
            allPossibleMove = new List<Position>();
            needToBlock     = new List<Position>();
        }

        public int getNumberOfDots()
        {
            return oneMoveToFinish.Count  +
                   allPossibleMove.Count ;
        }

        public bool CanWin
        {
            get
            {
                return canWin;
            }
            set
            {
                canWin = value;
            }
        }

        public void addLastMoveToWin(Position pos)
        {
            CanWin = true;
            oneMoveToFinish.Add(pos);
        }

        public void addAnotherPossibleMove(Position pos)
        {
            allPossibleMove.Add(pos);
        }

        public void addBlockList(List<Position> canBlock)
        {
            foreach (Position pos in canBlock)
            {
                needToBlock.Add(pos);
            }
        }

        public List<Position> getList(TypeOfArr type)
        {
            switch (type)
            {
                case TypeOfArr.BLOCK:
                    return needToBlock;
                case TypeOfArr.FINISH:
                    return oneMoveToFinish;
                case TypeOfArr.OTHER:
                    return allPossibleMove;
            }

            return null;
        }
    }
}
