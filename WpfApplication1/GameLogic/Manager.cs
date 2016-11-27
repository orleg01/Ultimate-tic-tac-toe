using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.GameLogic
{
    class Manager
    {
        
        private const bool DEBUG = false;

        public enum BoardStatuse { WIN , LOSE , PROCCES , FAILED , ABSULOT_WIN , ABSULOT_LOSE};


        private LittleBoard[,] board = new LittleBoard[3,3];
        private BoardStatuse[,] winLose = new BoardStatuse[3, 3];


        public Manager()
        {
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    board[i, j] = new LittleBoard();
                    winLose[i , j] = BoardStatuse.PROCCES;
                }
        }

        public BoardStatuse makeMove(PlaceInformation box , LittleBoard.PositionOnBoard positionOnBoard , bool print = false)
        {
            int col = Math.Abs(box.PosCol);
            int row = Math.Abs(box.PosRow);
            BoardStatuse state = positionOnBoard == LittleBoard.PositionOnBoard.PLAYER ? BoardStatuse.WIN : BoardStatuse.LOSE;

            winLose[box.BigRow , box.BigCol] = board[box.BigRow, box.BigCol].makeMove(positionOnBoard, row, col) ?
                                                                            state : BoardStatuse.PROCCES;

            if (print)
            {
                Console.WriteLine(box + "\n\n");
                printTable();
                int numberOfDotMat;
                int numberOfDot;

                numberOfDot = numberOfItemOnBoardDebug(computer , out numberOfDotMat);
                Console.WriteLine("computer has len of " + numberOfDot + " dots and the matrix have " + numberOfDotMat);
                numberOfDot = numberOfItemOnBoardDebug(player, out numberOfDotMat);
                Console.WriteLine("player has len of " + numberOfDot + " dots and the matrix have " + numberOfDotMat);
            }

            if (isEnd(state))
                return positionOnBoard == LittleBoard.PositionOnBoard.PLAYER ? BoardStatuse.ABSULOT_WIN : BoardStatuse.ABSULOT_LOSE;

            return winLose[box.BigRow, box.BigCol];
        }

        /*
        private float getMarkOfChoice(LittleBoard.PositionOnBoard positionOnBoard)
        {
            float toReturn = 0;
            BoardStatuse state = positionOnBoard == LittleBoard.PositionOnBoard.PLAYER ? BoardStatuse.WIN : BoardStatuse.LOSE;
            BoardStatuse oppState = getOpposite(state);
            if (isEnd(state))
                return 1000;
            else if (isEnd(oppState))
                return -1000;

            toReturn += (lastMove(state).Count*200);
            toReturn -= (lastMove(oppState).Count * 200);

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (winLose[i, j] != BoardStatuse.PROCCES)
                        toReturn += winLose[i, j] == state ? 50 : -50;
                    else
                    {
                        toReturn += board[i, j].lastMove(positionOnBoard);
                        toReturn -= board[i, j].lastMove(LittleBoard.getOpposit(positionOnBoard));
                    }


            float num = 1;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (!board[i, j].Finish)
                    {
                        int sizeMe = board[i, j].numberOfItemOnBoard(positionOnBoard);
                        int sizeOther = board[i, j].numberOfItemOnBoard(LittleBoard.getOpposit(positionOnBoard));
                        if (sizeMe == 0 && sizeOther == 0)
                            continue;
                        num += ((sizeMe) / (sizeMe + sizeOther));
                    }
                }
            toReturn += num;
            
            return toReturn;
        }*/

        private bool boardIsFull()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (!board[i, j].isFull())
                        return false;
            return true;
        }

        public static readonly int APPROXIMATE_WIN_SCORE = 7;

        private int getMarkOfChoice(LittleBoard.PositionOnBoard positionOnBoard)
        {
            const int BIG_BOARD_WEIGHT      = 23;
            const int WIN_SCORE             = 1000000;

            int toReturn = 0;
            BoardStatuse state = positionOnBoard == LittleBoard.PositionOnBoard.PLAYER ? BoardStatuse.WIN : BoardStatuse.LOSE;
            BoardStatuse oppState = getOpposite(state);
            bool winner;

            if (winner = isEnd(state) || isEnd(oppState))
            {
                int freeCell = 0;
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        freeCell += board[i, j].numberOfItemOnBoard(LittleBoard.PositionOnBoard.NONE);

                return (winner ? WIN_SCORE + freeCell : -WIN_SCORE - freeCell);
            }
            
            if (boardIsFull())
                return 0;

            toReturn = getBigBoardScore(state) * BIG_BOARD_WEIGHT;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (!board[i, j].isFull())
                        toReturn += board[i, j].getboardScore(positionOnBoard);

            return toReturn;
        }

        public static readonly int[,] winingPosability=  {{0 , 1 , 2},
                                                          {3 , 4 , 5},
                                                          {6 , 7 , 8},
                                                          {0 , 3 , 6},
                                                          {1 , 4 , 7},
                                                          {2 , 5 , 8},
                                                          {0 , 4 , 8},
                                                          {2 , 4 , 6}};

        public int getBigBoardScore(BoardStatuse state)
        {
            if (boardIsFull())
                return 0;
            BoardStatuse opposit = getOpposite(state);
            int check = 0;
            int rivial = 0;

            for (int i = 0; i < winingPosability.GetLength(0); i++)
            {
                int[] checkArr = { winingPosability[i, 0], winingPosability[i, 1], winingPosability[i, 2] };
                BoardStatuse[] statesB = new BoardStatuse[3];
                for (int j = 0; j < 3; j++)
                {
                    statesB[j] = winLose[checkArr[j] / 3, checkArr[j] % 3];
                }

                bool playerInArr  = false;
                int playerCounter = 0;
                bool rivialInArr = false;
                int rivialCounter = 0;

                for (int j = 0; j < 3; j++)
                {
                    if (state == statesB[j])
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
                        check += APPROXIMATE_WIN_SCORE;
                    check += 1;
                }
                else if (rivialInArr)
                {
                    if (rivialCounter > 1)
                        rivial += APPROXIMATE_WIN_SCORE;
                    rivial += 1;
                }
            }
            return check - rivial;
        }

        public void setCalculateMode(bool calc)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    board[i, j].Calculate = calc;
                }
        }

        public PlaceInformation getNextBestMove(int row , int col)
        {

            PlaceInformation toReturn = null;
            setCalculateMode(true);
            float noneImportents;
            int numberOfInterval = 5;
            float min = float.MinValue, max = float.MaxValue;

            Console.WriteLine("\n\n\n\n\n\n\n\n");
            printTable();

            toReturn = getNextBestMove(row , col ,min ,max , numberOfInterval, out noneImportents);

            Console.WriteLine("next move score is" + noneImportents);
            Console.WriteLine(toReturn + "\n");
            if (toReturn.PosCol == -1 || toReturn.PosRow == -1)
                throw new Exception("here");

            setCalculateMode(false);

            if (board[toReturn.BigRow, toReturn.BigCol].Finish)
            {
                toReturn.PosRow *= -1;
                toReturn.PosCol *= -1;
            }

            return toReturn;
        }

        private static readonly LittleBoard.PositionOnBoard computer = LittleBoard.PositionOnBoard.COMPUTER;
        private static readonly LittleBoard.PositionOnBoard player   = LittleBoard.PositionOnBoard.PLAYER;

        private PlaceInformation getNextBestMove(int row, int col, float min, float max, int interval , out float bestWeigth)
        {
            float value = float.MinValue;
            PlaceInformation tempPlace;
            PlaceInformation toReturn = new PlaceInformation(row, -1, col, -1);
            Random rand = new Random();
            bestWeigth = 0;

            if (board[row, col].Finish)
            {

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (!board[i, j].Finish)
                        {
                            tempPlace = getNextBestMove(i, j, min, max, interval , out bestWeigth);
                            if (bestWeigth > value)
                            {
                                value = bestWeigth;
                                toReturn = tempPlace;
                            }
                        }
                    }
                }

                bestWeigth = value;
                return toReturn;

            }

            List<Position> allPossiblePos  = board[row, col].allOptions();

            int whereToStart = ((int)(rand.NextDouble() * 50000)) % allPossiblePos.Count;

            float temp;

            for(int i = 0; i < allPossiblePos.Count; i++)
            {
                Position pos = allPossiblePos[(whereToStart + i) % allPossiblePos.Count];
                makeMove(new PlaceInformation(row, pos.row, col, pos.col), computer);
                temp = getNextBestMove(pos.row, pos.col, player, min, max, interval - 1 );
                if (temp > value)
                {
                    value = temp;
                    toReturn.PosRow = pos.row;
                    toReturn.PosCol = pos.col;
                }
                winLose[row, col] = board[row, col].goBack(1, false, computer) ? playerToState(computer) : BoardStatuse.PROCCES;
            }
            temp = value;
            return toReturn;
        }


        float helpForDebug = float.MinValue;
        private float getNextBestMove(int row , int col , LittleBoard.PositionOnBoard posOnBoard ,float alpha , float beta , int iteration)
        {

            float valueComputer = float.MinValue;
            float valuePlayer   = float.MaxValue;
            float goal;

            if (isEnd(getOpposite(playerToState(posOnBoard))) || iteration == 0)
            {
                goal = getMarkOfChoice(computer);
                if (helpForDebug != goal)
                {
                    helpForDebug = goal;
                    //Console.WriteLine(goal);
                }
                if (DEBUG)
                {
                    printTable();
                    Console.WriteLine("the score is : " + goal + "\n\n\n\n\n\n\n\n");
                    Console.ReadLine();
                }
                return goal;
            }
          
            if (winLose[row, col] != BoardStatuse.PROCCES)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (winLose[i, j] == BoardStatuse.PROCCES)
                        {
                            float v = getNextBestMove(i, j, posOnBoard, alpha, beta, iteration );
                            if (posOnBoard == computer)
                            {
                                valueComputer = Math.Max(valueComputer, v);
                                alpha = Math.Max(valueComputer, alpha);

                                if (beta <= alpha)
                                {
                                    return valueComputer;
                                }
                            }
                            else
                            {
                                valuePlayer = Math.Min(valuePlayer, v);
                                beta = Math.Min(beta, valuePlayer);

                                if (beta <= alpha)
                                {
                                    return valuePlayer;
                                }
                            }
                        }
                    }
                }

                if (posOnBoard == computer)
                    return valueComputer;
                else
                    return valuePlayer;
            }

            List<Position> allPossibilities = board[row, col].allOptions();

            foreach (Position pos in allPossibilities)
            {

                makeMove(new PlaceInformation(row, pos.row, col, pos.col) , posOnBoard);

                if (posOnBoard == computer)
                {
                    valueComputer = Math.Max(valueComputer, getNextBestMove(pos.row, pos.col, player, alpha, beta, iteration - 1));
                    alpha = Math.Max(valueComputer, alpha);
                    
                    winLose[row, col] = board[row, col].goBack(1, false, computer) ? playerToState(posOnBoard) : BoardStatuse.PROCCES;

                    if (beta <= alpha)
                    {
                        return valueComputer;
                    }
                }
                else
                {
                    valuePlayer = Math.Min(valuePlayer, getNextBestMove(pos.row, pos.col, computer, alpha, beta, iteration - 1));
                    beta = Math.Min(beta, valuePlayer);

                    winLose[row, col] = board[row, col].goBack(1, false, player) ? playerToState(posOnBoard) : BoardStatuse.PROCCES;

                    if (beta <= alpha)
                    {
                        return valuePlayer;
                    }
                }
            }

            if (posOnBoard == computer)
                return valueComputer;
            else
                return valuePlayer;

        }

        private void printTable()
        {
            string toPrint = "";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    toPrint += board[i / 3, j / 3].whoThere(i%3 , j%3);
                    if (j % 3 == 2 && j != 8)
                        toPrint += "|";
                }
                toPrint += "\n";
                if (i % 3 == 2 && i != 8)
                    toPrint += "________________\n";
            }
            Console.WriteLine(toPrint);
        }

        private int numberOfItemOnBoardDebug(LittleBoard.PositionOnBoard posOnBoard , out int matNumber)
        {
            int temp = matNumber = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    temp += board[i, j].numberOfItemOnBoard(posOnBoard);
                    if (posOnBoard == computer)
                        matNumber += board[i, j].LenOfComputerPositionForDebug;
                    else
                        matNumber += board[i, j].LenOfPlayerPositionForDebug;
                }

            return temp;
        }

        private BoardStatuse playerToState(LittleBoard.PositionOnBoard pos)
        {
            if (pos == computer)
                return BoardStatuse.LOSE;
            else if (pos == player)
                return BoardStatuse.WIN;

            throw new Exception("cannot convert this kind of enum!");
        }

        private BoardStatuse getOpposite(BoardStatuse opp)
        {
            if (opp == BoardStatuse.WIN)
                return BoardStatuse.LOSE;
            else if (opp == BoardStatuse.LOSE)
                return BoardStatuse.WIN;

            throw new Exception("cannot convert this kind of BoardStatuse enum!");
        }

        private bool isEnd(BoardStatuse state)
        {
            for (int i = 0; i < winingPosability.GetLength(0); i++)
            {
                if (winLose[winingPosability[i, 0] / 3, winingPosability[i, 0] % 3] == state &&
                   winLose[winingPosability[i, 1] / 3, winingPosability[i, 1] % 3] == state &&
                   winLose[winingPosability[i, 2] / 3, winingPosability[i, 2] % 3] == state)
                    return true;
            }

            return boardIsFull();
        }

        private List<Position> lastMove(BoardStatuse pos)
        {
            List<Position> winOpertunity = new List<Position>();

            for (int i = 0; i < 3; i++)
            {
                if (winLose[i, 0] == pos && winLose[i, 1] == pos && winLose[i,2] == BoardStatuse.PROCCES)
                    winOpertunity.Add(new Position(i, 2));
                else if (winLose[i, 0] == pos && winLose[i, 2] == pos && winLose[i, 1] == BoardStatuse.PROCCES)
                    winOpertunity.Add(new Position(i, 1));
                else if (winLose[i, 1] == pos && winLose[i, 2] == pos && winLose[i, 0] == BoardStatuse.PROCCES)
                    winOpertunity.Add(new Position(i, 0));

                if (winLose[0 , i] == pos && winLose[1 , i] == pos && winLose[2 , i] == BoardStatuse.PROCCES)
                    winOpertunity.Add(new Position(2 , i));
                else if (winLose[0 , i] == pos && winLose[2 , i] == pos && winLose[1 , i] == BoardStatuse.PROCCES)
                    winOpertunity.Add(new Position( 1 , i));
                else if (winLose[1 , i] == pos && winLose[2 , i] == pos && winLose[0 , i] == BoardStatuse.PROCCES)
                    winOpertunity.Add(new Position( 0 , i));
            }

            if (winLose[0, 0] == pos && winLose[1, 1] == pos && winLose[2, 2] == BoardStatuse.PROCCES)
                winOpertunity.Add(new Position(2, 2));
            else if (winLose[0, 0] == pos && winLose[2, 2] == pos && winLose[1, 1] == BoardStatuse.PROCCES)
                winOpertunity.Add(new Position(1, 1));
            else if (winLose[1, 1] == pos && winLose[2, 2] == pos && winLose[0, 0] == BoardStatuse.PROCCES)
                winOpertunity.Add(new Position(0, 0));

            if (winLose[0, 2] == pos && winLose[1, 1] == pos && winLose[2, 0] == BoardStatuse.PROCCES)
                winOpertunity.Add(new Position(2, 0));
            else if (winLose[0, 2] == pos && winLose[2, 0] == pos && winLose[1, 1] == BoardStatuse.PROCCES)
                winOpertunity.Add(new Position(1, 1));
            else if (winLose[1, 1] == pos && winLose[2, 0] == pos && winLose[0, 2] == BoardStatuse.PROCCES)
                winOpertunity.Add(new Position(0, 2));


            return winOpertunity;
        }

        public bool PositionIsGood(int row, int col)
        {
            return !board[row, col].Finish;
        }
    }
}
