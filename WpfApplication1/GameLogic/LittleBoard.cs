using System;
using System.Collections.Generic;

namespace WpfApplication1.GameLogic
{
    class LittleBoard
    {
        public enum PositionOnBoard { NONE , PLAYER , COMPUTER , TEMP_PLAYER , TEMP_COMPUTER};

        private PositionOnBoard[,] board = new PositionOnBoard[3,3];
        private List<Position> computerPosition = new List<Position>();
        private List<Position> playerPosition   = new List<Position>();

        private bool calculate = false;
        public bool Calculate
        {
            get
            {
                return calculate;
            }
            set
            {
                calculate = value;
                if (!calculate)
                {
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                        {
                            if (board[i, j] == PositionOnBoard.TEMP_PLAYER || board[i, j] == PositionOnBoard.TEMP_COMPUTER)
                                board[i, j] = PositionOnBoard.NONE;
                        }
                }
            }
        }
        public bool Finish
        {
            get;
            set;
        }

        public LittleBoard()
        {
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    board[i, j] = PositionOnBoard.NONE;
        }

        public ReportOnBoard goingToLose()
        {
           

            List<Position>[] posComp = new List<Position>[]{
                new List<Position>() ,
                new List<Position>() ,
                new List<Position>() 
            };

            List<Position>[] posPlayer = new List<Position>[]{
                new List<Position>() ,
                new List<Position>() ,
                new List<Position>() 
            };

            

            return null;
        }

        private void fillPosArrayOfLists(List<Position>[] list, List<Position> placeOnBoard ,PositionOnBoard whichPlayer)
        {
            int i;
            while()
        }

        public bool makeMove(PositionOnBoard posOnBoard, int row, int col)
        {
            if (board[row, col] == PositionOnBoard.NONE || !Finish)
            {
                board[row, col] = Calculate ? 
                                    (posOnBoard == PositionOnBoard.PLAYER ? PositionOnBoard.TEMP_PLAYER : PositionOnBoard.TEMP_COMPUTER) 
                                    : posOnBoard;
            }

            throw new Exception("cannot place a move on this board");
        }
    }
}