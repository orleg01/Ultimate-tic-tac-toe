using System;
using System.Collections.Generic;

namespace WpfApplication1.GameLogic
{
    class LittleBoard
    {
        public enum PositionOnBoard { NONE = 1 , PLAYER = 2 , COMPUTER = 4 , TEMP_PLAYER = 8 , TEMP_COMPUTER = 16};

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
                            {
                                if (board[i, j] == PositionOnBoard.TEMP_PLAYER)
                                    playerPosition.RemoveAt(playerPosition.Count - 1);
                                else
                                    computerPosition.RemoveAt(computerPosition.Count - 1);
                                board[i, j] = PositionOnBoard.NONE;
                            }
                        }
                    if(WhoWon == PositionOnBoard.TEMP_PLAYER || WhoWon == PositionOnBoard.TEMP_COMPUTER)
                     {
                        WhoWon = PositionOnBoard.NONE;
                        Finish = false;
                    }
                }
            }
        }
        public bool Finish
        {
            get;
            set;
        }
        public PositionOnBoard WhoWon
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

        public List<Position> allOptions()
        {
            if (Finish)
                throw new Exception("this board finished .. no more posabilities");

            List<Position> toReturn = new List<Position>();

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (board[i, j] == PositionOnBoard.NONE)
                        toReturn.Add(new Position(i, j));

            return toReturn;
        }

        public bool makeMove(PositionOnBoard posOnBoard, int row, int col)
        {
            if (board[row, col] == PositionOnBoard.NONE && !Finish)
            {
                board[row, col] = Calculate ? 
                                    (posOnBoard == PositionOnBoard.PLAYER ? PositionOnBoard.TEMP_PLAYER : PositionOnBoard.TEMP_COMPUTER) 
                                    : posOnBoard;
                if (posOnBoard == PositionOnBoard.PLAYER)
                    playerPosition.Add(new Position(row, col));
                else
                    computerPosition.Add(new Position(row, col));

                Finish = isEnd(posOnBoard);
                if (Finish)
                    WhoWon = Calculate ?
                                (posOnBoard == PositionOnBoard.PLAYER ? PositionOnBoard.TEMP_PLAYER : PositionOnBoard.TEMP_COMPUTER)
                                : posOnBoard;
                return Finish;

            }

            if (board[row, col] != PositionOnBoard.NONE)
                throw new Exception("there are allready item on the board");
            else if (Finish)
                throw new Exception("the board allready finished its rule in the game :)");
            else
                throw new Exception("Fuck you ! you can never come here 0.0");
        }

        public bool putable(int row, int col)
        {
            if (row == -1 || col == -1)
                return true;
            return board[row, col] == PositionOnBoard.NONE;
        }

        private bool isEnd(PositionOnBoard pos)
        {
            for (int i = 0; i < Manager.winingPosability.GetLength(0); i++)
            {
                if (board[Manager.winingPosability[i, 0] / 3, Manager.winingPosability[i, 0] % 3] == pos &&
                    board[Manager.winingPosability[i, 1] / 3, Manager.winingPosability[i, 1] % 3] == pos &&
                    board[Manager.winingPosability[i, 2] / 3, Manager.winingPosability[i, 2] % 3] == pos)
                    return true;
            }

            return isFull();
        }

        public bool goBack(int number, bool fullRound , PositionOnBoard pos)
        {
            bool whosTurn = pos == PositionOnBoard.PLAYER; 
            for (int i = 0; i < number; i++)
            {
                if (fullRound)
                {
                    Position posTemp = computerPosition[computerPosition.Count - 1];
                    board[posTemp.row, posTemp.col] = PositionOnBoard.NONE;
                    computerPosition.RemoveAt(computerPosition.Count - 1);
                    posTemp = playerPosition[playerPosition.Count - 1];
                    board[posTemp.row, posTemp.col] = PositionOnBoard.NONE;
                    playerPosition.RemoveAt(playerPosition.Count - 1);
                }
                else
                {
                    Position posTemp;
                    if (whosTurn)
                    {
                        posTemp = playerPosition[playerPosition.Count - 1];
                        playerPosition.RemoveAt(playerPosition.Count - 1);
                    }
                    else
                    {
                        posTemp = computerPosition[computerPosition.Count - 1];
                        computerPosition.RemoveAt(computerPosition.Count - 1);
                    }
                    board[posTemp.row, posTemp.col] = PositionOnBoard.NONE;

                    whosTurn = !whosTurn;
                }
            }

            Finish = isEnd(pos);
            if (!Finish)
                WhoWon = PositionOnBoard.NONE;

            return Finish;
        }

        public int numberOfItemOnBoard(PositionOnBoard pos)
        {
            if (pos == PositionOnBoard.PLAYER)
                return playerPosition.Count;
            else if (pos == PositionOnBoard.COMPUTER)
                return computerPosition.Count;
            else
                return 9 - playerPosition.Count - computerPosition.Count;
        }



        public int lastMove(PositionOnBoard pos)
        {

            int toReturn = 0;

            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] == pos && board[i, 1] == pos && board[i , 2] == PositionOnBoard.NONE)
                    toReturn++;
                else if (board[i, 0] == pos && board[i, 2] == pos && board[i, 1] == PositionOnBoard.NONE)
                    toReturn++;
                else if (board[i, 1] == pos && board[i, 2] == pos && board[i, 0] == PositionOnBoard.NONE)
                    toReturn++;

                if (board[0, i] == pos && board[1, i] == pos && board[2, i] == PositionOnBoard.NONE)
                    toReturn++;
                else if (board[0, i] == pos && board[2, i] == pos && board[1, i] == PositionOnBoard.NONE)
                    toReturn++;
                else if (board[1, i] == pos && board[2, i] == pos && board[0, i] == PositionOnBoard.NONE)
                    toReturn++;
            }

            if (board[0, 0] == pos && board[1, 1] == pos && board[2, 2] == PositionOnBoard.NONE)
                toReturn++;
            else if (board[0, 0] == pos && board[2, 2] == pos && board[1, 1] == PositionOnBoard.NONE)
                toReturn++;
            else if (board[1, 1] == pos && board[2, 2] == pos && board[0, 0] == PositionOnBoard.NONE)
                toReturn++;

            if (board[0, 2] == pos && board[1, 1] == pos && board[2, 0] == PositionOnBoard.NONE)
                toReturn++;
            else if (board[0, 2] == pos && board[2, 0] == pos && board[1, 1] == PositionOnBoard.NONE)
                toReturn++;
            else if (board[1, 1] == pos && board[2, 0] == pos && board[0, 2] == PositionOnBoard.NONE)
                toReturn++;


            return toReturn;
        }


        public static PositionOnBoard getOpposit(PositionOnBoard pos)
        {
            if (pos == PositionOnBoard.COMPUTER)
                return PositionOnBoard.PLAYER;
            else if (pos == PositionOnBoard.PLAYER)
                return PositionOnBoard.COMPUTER;

            throw new Exception("Didnt impliment an opposit choice for this enum");
        }

        internal string whoThere(int v1, int v2)
        {
            PositionOnBoard pos = board[v1, v2];
            switch (pos)
            {
                case PositionOnBoard.PLAYER :
                case PositionOnBoard.TEMP_PLAYER:
                    return " X ";
                case PositionOnBoard.NONE:
                    return " - ";
                case PositionOnBoard.COMPUTER:
                case PositionOnBoard.TEMP_COMPUTER:
                    return " O ";
                
            }
            return "GAY";
        }

        public int getboardScore(PositionOnBoard posOnBoard)
        {
            if (isFull())
                return 0;
            PositionOnBoard opposit = getOpposit(posOnBoard);
            int check = 0;
            int rivial = 0;

            for (int i = 0; i < Manager.winingPosability.GetLength(0); i++)
            {
                int[] checkArr = { Manager.winingPosability[i, 0], Manager.winingPosability[i, 1], Manager.winingPosability[i, 2] };
                PositionOnBoard[] statesB = new PositionOnBoard[3];
                for (int j = 0; j < 3; j++)
                {
                    statesB[j] = board[checkArr[j] / 3, checkArr[j] % 3];
                }

                bool playerInArr = false;
                int playerCounter = 0;
                bool rivialInArr = false;
                int rivialCounter = 0;

                for (int j = 0; j < 3; j++)
                {
                    if (posOnBoard == statesB[j])
                    {
                        playerInArr = true;
                        playerCounter++;
                    }
                    else if (opposit == statesB[j])
                    {
                        rivialCounter++;
                        rivialInArr = true;
                    }
                }

                if (playerInArr)
                {
                    if (rivialInArr)
                        continue;

                    if (playerCounter > 1)
                        check += Manager.APPROXIMATE_WIN_SCORE;
                    check += 1;
                }
                else if (rivialInArr)
                {
                    if (rivialCounter > 1)
                        rivial += Manager.APPROXIMATE_WIN_SCORE;
                    rivial += 1;
                }
            }
            return check - rivial;
        }

        public bool isFull()
        {
            return numberOfItemOnBoard(PositionOnBoard.NONE) == 0;
        }
    }
}