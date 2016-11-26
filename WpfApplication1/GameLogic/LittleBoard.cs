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
            return board[row, col] == PositionOnBoard.NONE;
        }

        private bool isEnd(PositionOnBoard pos)
        {
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2] && board[i, 2] == pos)
                    return true;
                else if (board[0 , i] == board[1 , i] && board[1 , i] == board[2 , i] && board[2 , i] == pos)
                    return true;
            }

            if (board[0, 0] == pos && board[1, 1] == pos && board[2, 2] == pos)
                return true;
            else if (board[2, 0] == pos && board[1, 1] == pos && board[0, 2] == pos)
                return true;

            return false;
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
            //List<Position> winOpertunity = new List<Position>();

            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] == pos && board[i, 1] == pos)
                    //winOpertunity.Add(new Position(i, 2));
                    toReturn++;
                else if (board[i, 0] == pos && board[i, 2] == pos)
                    //winOpertunity.Add(new Position(i, 1));
                    toReturn++;
                else if (board[i, 1] == pos && board[i, 2] == pos)
                    //winOpertunity.Add(new Position(i, 0));
                    toReturn++;

                if (board[0, i] == pos && board[1, i] == pos)
                    //winOpertunity.Add(new Position(2, i));
                    toReturn++;
                else if (board[0, i] == pos && board[2, i] == pos)
                    //winOpertunity.Add(new Position(1, i));
                    toReturn++;
                else if (board[1, i] == pos && board[2, i] == pos)
                    //winOpertunity.Add(new Position(0, i));
                    toReturn++;
            }

            if (board[0, 0] == pos && board[1, 1] == pos)
                //winOpertunity.Add(new Position(2, 2));
                toReturn++;
            else if (board[0, 0] == pos && board[2, 2] == pos)
                //winOpertunity.Add(new Position(1, 1));
                toReturn++;
            else if (board[1, 1] == pos && board[2, 2] == pos)
                //winOpertunity.Add(new Position(0, 0));
                toReturn++;

            if (board[0, 2] == pos && board[1, 1] == pos)
                //winOpertunity.Add(new Position(2, 0));
                toReturn++;
            else if (board[0, 2] == pos && board[2, 0] == pos)
                //winOpertunity.Add(new Position(1, 1));
                toReturn++;
            else if (board[1, 1] == pos && board[2, 0] == pos)
                //winOpertunity.Add(new Position(0, 2));
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
    }
}