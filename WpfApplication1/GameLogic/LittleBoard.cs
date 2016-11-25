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

        public ReportOnBoard allOptions(PositionOnBoard pos)
        {
            ReportOnBoard toReturn = new ReportOnBoard();

            if (Finish)
            {
                toReturn.gameFinish = true;
                toReturn.youWon = WhoWon == pos || (int)WhoWon == (int)pos * 4;
            }
            else
            {
                ReportOnBoard computer = fillPosArrayOfLists(computerPosition, PositionOnBoard.COMPUTER);
                ReportOnBoard player = fillPosArrayOfLists(playerPosition, PositionOnBoard.PLAYER);

                if (pos == PositionOnBoard.PLAYER)
                {
                    player.addBlockList(computer.getList(ReportOnBoard.TypeOfArr.FINISH));
                    toReturn = player;
                }
                else
                {
                    computer.addBlockList(player.getList(ReportOnBoard.TypeOfArr.FINISH));
                    toReturn = computer;
                }


            }
            
            return toReturn;
        }

        private ReportOnBoard fillPosArrayOfLists(List<Position> placeOnBoard ,PositionOnBoard whichPlayer)
        {

            ReportOnBoard report = new ReportOnBoard();

            for (int i = 0; i < placeOnBoard.Count - 1; i++)
                for (int j = i + 1; j < placeOnBoard.Count; j++)
                {
                    int sizeBetweenBoxes = findMaxSize(placeOnBoard[i], placeOnBoard[j] , whichPlayer);
                    switch (sizeBetweenBoxes)
                    {
                        case 0:

                            int row = placeOnBoard[i].row - placeOnBoard[j].row;
                            int col = placeOnBoard[i].col - placeOnBoard[j].col;
                            int tempRow, tempCol;

                            if ((tempRow = placeOnBoard[i].row + row) < 3 && tempRow >= 0 &&
                                (tempCol = placeOnBoard[i].col + col) < 3 && tempCol >= 0)
                            {
                                if (board[tempRow, tempCol] == PositionOnBoard.NONE)
                                {
                                    report.addLastMoveToWin(new Position(tempRow, tempCol));
                                }
                            }
                            else if ((tempRow = placeOnBoard[j].row - row) < 3 && tempRow >= 0 &&
                                   (tempCol = placeOnBoard[j].col - col) < 3 && tempCol >= 0)
                            {
                                if (board[tempRow, tempCol] == PositionOnBoard.NONE)
                                {
                                    report.addLastMoveToWin(new Position(tempRow, tempCol));
                                }
                            }
                            break;
                    }
                }

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (board[i, j] == PositionOnBoard.NONE)
                        report.addAnotherPossibleMove(new Position(i, j));

            return report;
        }

        private int findMaxSize(Position position1, Position position2 , PositionOnBoard posOnBoard)
        {
            Position temp = new Position(position1);
            bool[,] tempBoard = new bool[3, 3];

            tempBoard[temp.row, temp.col] = true;
            tempBoard[position2.row, position2.col] = true;
            int numberOfIteration = -1;

            if(!temp.Equals(position2) )
            {
                numberOfIteration++;
                int row, col;
                temp.diffrent(position2,out row,out col);

                if (((int)board[temp.row + row, temp.col + col] | (int)posOnBoard | ((int)posOnBoard) * 4) == ((int)posOnBoard | ((int)posOnBoard * 4)))
                {
                    temp.col += col;
                    temp.row += row;
                    if (temp.Equals(position2))
                        return numberOfIteration;
                    else
                        return -1;
                }
                else if (board[temp.row + row, temp.col + col] == PositionOnBoard.NONE)
                {
                    temp.col += col;
                    temp.row += row;
                    return 1 + numberOfIteration + findMaxSize(temp, position2, posOnBoard);
                }
                else
                {
                    Position posRow = new Position(temp);
                    Position posCol = new Position(temp);
                    int one = -1, sec = -1;

                    if (row != 0)
                    {
                        posRow.row += row;
                        one = 1 + numberOfIteration + findMaxSize(posRow, position2, posOnBoard);
                    }
                    if (col != 0)
                    {
                        posCol.col += col;
                        sec = 1 + numberOfIteration + findMaxSize(posCol, position2, posOnBoard);
                    }

                    if (one == -1)
                        one = 100;
                    if (sec == -1)
                        sec = 100;
                    if (one == sec && sec == 100)
                        return -1;
                    return one < sec ? one : sec;
                }

            }

            return numberOfIteration;
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

        public void goBack(int number, bool fullRound , PositionOnBoard pos)
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


    }
}