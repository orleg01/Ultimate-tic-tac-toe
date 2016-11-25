using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.GameLogic
{
    class Manager
    {

        public enum BoardStatuse { WIN , LOSE , PROCCES , FAILED , ABSULOT_WIN , ABSULOT_LOSE};


        private LittleBoard[,] board = new LittleBoard[3,3];
        private BoardStatuse[,] winLose = new BoardStatuse[3, 3];

       /* public void SimulationMode(bool startSimulate = true)
        {
        
        }*/

        public Manager()
        {
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    board[i, j] = new LittleBoard();
                    winLose[i , j] = BoardStatuse.PROCCES;
                }
        }

        public BoardStatuse makeMove(PlaceInformation box , out int row , out int col , out BoardStatuse yourBoardStatuse)
        {
            check = 0;
            row = col = -1;

            LittleBoard tempBoard = board[box.BigRow, box.BigCol];

            yourBoardStatuse = winLose[box.BigRow, box.BigCol] = tempBoard.makeMove(LittleBoard.PositionOnBoard.PLAYER, box.PosRow, box.PosCol) ?
                                                                                BoardStatuse.WIN : BoardStatuse.PROCCES;
            

            if (isEnd(BoardStatuse.WIN))
                return BoardStatuse.ABSULOT_WIN; 

            tempBoard.Calculate = true;
            int number = 0;
            Position nextMove = getNextBestMove(box.PosRow , box.PosCol , LittleBoard.PositionOnBoard.COMPUTER , ref number , new int[3] , ref box);
            tempBoard.Calculate = false;

            winLose[box.PosRow , box.PosCol] = board[box.PosRow, box.PosCol].makeMove(LittleBoard.PositionOnBoard.COMPUTER, nextMove.row, nextMove.col) ?
                                                                BoardStatuse.LOSE : BoardStatuse.PROCCES;
            if (isEnd(BoardStatuse.LOSE))
                return BoardStatuse.ABSULOT_LOSE;

            row = nextMove.row;
            col = nextMove.col;

            if (winLose[row, col] != BoardStatuse.PROCCES)
            {
                row *= -1;
                col *= -1;
            }

            return winLose[box.PosRow, box.PosCol];
        }

        private float getMarkOfChoice(int[] choice)
        {
            return (choice[2]*2 + choice[1] + choice[0]*-2) /  (choice[2] == 0 ? 1 : choice[2] * 5);
        }

        private Random rand = new Random();
        private int check = 0;

        private Position getNextBestMove(int bigRow , int bigCol , LittleBoard.PositionOnBoard positionOnBoard , ref int num ,  int[] choices , ref PlaceInformation box )
        {

            PlaceInformation placeTemp = new PlaceInformation(-1 , -1 , -1 , -1);


            #region deep waiting
            if (num > 6)
            {
                check++;

                int myNumber = 0;
                int hisNumber = 0;

                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                    {
                        if (winLose[i, j] == BoardStatuse.WIN)
                        {
                            myNumber++;
                        }
                        else if (winLose[i, j] == BoardStatuse.LOSE)
                        {
                            hisNumber++;
                        }
                        else
                        {
                            choices[1]++;
                        }
                    }

                if (positionOnBoard == LittleBoard.PositionOnBoard.COMPUTER)
                {
                    choices[0] = myNumber;
                    choices[2] = hisNumber;
                }
                else
                {
                    choices[2] = myNumber;
                    choices[0] = hisNumber;
                }

                if (choices[2] > choices[0] && choices[2] > choices[1])
                {
                    choices[0] = choices[1] = 0;
                    choices[2] = 1;
                }
                else if (choices[0] > choices[2] && choices[0] > choices[1])
                {
                    choices[2] = choices[1] = 0;
                    choices[0] = 1;
                }
                else
                {
                    choices[0] = choices[2] = 0;
                    choices[1] = 1;
                }


                return null;
            }
            #endregion

            #region If steped on NoneProfit Area
            if (winLose[bigRow, bigCol] == BoardStatuse.WIN || BoardStatuse.LOSE == winLose[bigRow, bigCol])
            {
                Position pos = null;
                int[] overAll = new int[3];

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int[] tempCh = new int[3];

                        if (i == bigRow && j == bigCol)
                            continue;
                        else if (winLose[i, j] != BoardStatuse.PROCCES)
                            continue;

                        Position temp = getNextBestMove(i, j, positionOnBoard, ref num, tempCh , ref placeTemp);

                        if (pos == null)
                        {
                            pos = temp;
                            box.PosRow = i;
                            box.PosCol = j;
                        }

                        for (int w = 0; w < 3; w++)
                            overAll[w] += tempCh[w];

                        if (getMarkOfChoice(tempCh) > getMarkOfChoice(choices))
                        {
                            for (int w = 0; w < 3; w++)
                                choices[w] = tempCh[w];

                            pos = temp;
                            box.PosRow = i;
                            box.PosCol = j;
                        }
                        
                    }
                }

                for (int i = 0; i < 3; i++)
                    choices[i] = positionOnBoard == LittleBoard.PositionOnBoard.COMPUTER ? overAll[i] : overAll[2 - i];

                return pos;

            }
            #endregion

            #region end of recursia  

            ReportOnBoard mainReport = board[bigRow, bigCol].allOptions(positionOnBoard);

            if (mainReport.MoveFinish != null)
            {
                List<Position> compBoard = lastMove(positionOnBoard == LittleBoard.PositionOnBoard.COMPUTER ?BoardStatuse.LOSE :BoardStatuse.WIN);
                if (compBoard.Count != 0)
                {
                    foreach (Position pos in compBoard)
                    {
                        if (pos.row == bigRow && pos.col == bigCol)
                        {
                            choices[2] = 1;
                            return mainReport.MoveFinish[0];
                        }
                    }
                }
            }
            #endregion

            #region create simulation option
            List<Position> goingToTryForSimulation = new List<Position>();

            if (mainReport.AllPossibleMoves == null)
            {
                return null;
            }

            foreach (Position pos in mainReport.AllPossibleMoves)
            {
                bool whoPlay;
                ReportOnBoard report = board[pos.row, pos.col].allOptions((whoPlay = (positionOnBoard == LittleBoard.PositionOnBoard.COMPUTER)) ? LittleBoard.PositionOnBoard.PLAYER : LittleBoard.PositionOnBoard.COMPUTER);
                
                if (report.MoveFinish == null && report.MoveBlock == null )
                {
                    if (mainReport.AllPossibleMoves.Count > 3 && winLose[pos.row, pos.col] == BoardStatuse.PROCCES)
                    {
                        if (/*board[pos.row, pos.col].numberOfItemOnBoard(positionOnBoard) > board[pos.row, pos.col].numberOfItemOnBoard(whoPlay ? LittleBoard.PositionOnBoard.COMPUTER :
                                                                                                                                                  LittleBoard.PositionOnBoard.PLAYER)*/
                            winLose[pos.row, pos.col] == BoardStatuse.PROCCES)
                        {
                            goingToTryForSimulation.Add(pos);
                        }

                        if (goingToTryForSimulation.Count > 2)
                        {
                            goingToTryForSimulation.RemoveAt((int)(rand.NextDouble() * 500) % 3);
                        }
                    }
                    else
                        goingToTryForSimulation.Add(pos);
                }


            }
            if (goingToTryForSimulation.Count == 0)
                goingToTryForSimulation = mainReport.AllPossibleMoves;

            #endregion

            #region find best step

            int[] tempChoice;
            Position toReturn = goingToTryForSimulation[0];

            int[] infoToReturn = new int[3];

            num++;
            int randNum = (int)(rand.NextDouble() * 35000);

            for (int i = 0; i < goingToTryForSimulation.Count; i++)
            {

                tempChoice = new int[3];
                int number = num;
                Position pos = goingToTryForSimulation[i];

                int num1, num2;
                
                /*
                #region random add item

                randNum = (++randNum) % goingToTryForSimulation.Count;
                Position randPos = goingToTryForSimulation[randNum];
                if ((num1 = board[randPos.row, randPos.col].numberOfItemOnBoard(LittleBoard.PositionOnBoard.COMPUTER)) < 2 &&
                    (num2 = board[randPos.row, randPos.col].numberOfItemOnBoard(LittleBoard.PositionOnBoard.PLAYER)) < 2 &&
                    board[bigRow , bigCol].numberOfItemOnBoard(LittleBoard.PositionOnBoard.NONE) > 8)
                {
                    return randPos;
                }

                #endregion
                */
                if (!board[bigRow, bigCol].putable(pos.row, pos.col))
                    continue;

                winLose[bigRow , bigCol] =  board[bigRow, bigCol].makeMove(positionOnBoard, pos.row, pos.col) ? 
                                                            (positionOnBoard == LittleBoard.PositionOnBoard.PLAYER ? 
                                                                                            BoardStatuse.WIN : BoardStatuse.LOSE) :
                                                             BoardStatuse.PROCCES;

                Position item = getNextBestMove( pos.row, pos.col, 
                                                        positionOnBoard == LittleBoard.PositionOnBoard.PLAYER ? LittleBoard.PositionOnBoard.COMPUTER : positionOnBoard ,
                                                        ref num , tempChoice , ref placeTemp);

                board[bigRow, bigCol].goBack(1, false, positionOnBoard);

                winLose[bigRow, bigCol] = BoardStatuse.PROCCES;

                for (int j = 0; j < 3; j++)
                    infoToReturn[j] += tempChoice[j];

                if (getMarkOfChoice(tempChoice) > getMarkOfChoice(choices))
                {
                        for (int j = 0; j < 3; j++)
                            choices[j] = tempChoice[j];

                    toReturn = goingToTryForSimulation[i];
                }

            }

            num--;

            for (int i = 0; i < 3; i++)
                choices[i] =  positionOnBoard == LittleBoard.PositionOnBoard.COMPUTER ? infoToReturn[i] : infoToReturn[2 - i];

            #endregion

            return toReturn;

        }

        private bool isEnd(BoardStatuse state)
        {
            for (int i = 0; i < 3; i++)
            {
                if (winLose[i, 0] == winLose[i, 1] && winLose[i, 1] == winLose[i, 2] && winLose[i, 2] == state)
                    return true;
                else if (winLose[0, i] == winLose[1, i] && winLose[1, i] == winLose[2, i] && winLose[2, i] == state)
                    return true;
            }

            if (winLose[0, 0] == state && winLose[1, 1] == state && winLose[2, 2] == state)
                return true;
            else if (winLose[2, 0] == state && winLose[1, 1] == state && winLose[0, 2] == state)
                return true;

            return false;
        }

        private List<Position> lastMove(BoardStatuse pos)
        {
            List<Position> winOpertunity = new List<Position>();

            for (int i = 0; i < 3; i++)
            {
                if (winLose[i, 0] == pos && winLose[i, 1] == pos)
                    winOpertunity.Add(new Position(i, 2));
                else if (winLose[i, 0] == pos && winLose[i, 2] == pos)
                    winOpertunity.Add(new Position(i, 1));
                else if (winLose[i, 1] == pos && winLose[i, 2] == pos)
                    winOpertunity.Add(new Position(i, 0));

                if (winLose[0 , i] == pos && winLose[1 , i] == pos)
                    winOpertunity.Add(new Position(2 , i));
                else if (winLose[0 , i] == pos && winLose[2 , i] == pos)
                    winOpertunity.Add(new Position( 1 , i));
                else if (winLose[1 , i] == pos && winLose[2 , i] == pos)
                    winOpertunity.Add(new Position( 0 , i));
            }

            if (winLose[0, 0] == pos && winLose[1, 1] == pos)
                winOpertunity.Add(new Position(2, 2));
            else if (winLose[0, 0] == pos && winLose[2, 2] == pos)
                winOpertunity.Add(new Position(1, 1));
            else if (winLose[1, 1] == pos && winLose[2, 2] == pos)
                winOpertunity.Add(new Position(0, 0));

            if (winLose[0, 2] == pos && winLose[1, 1] == pos)
                winOpertunity.Add(new Position(2, 0));
            else if (winLose[0, 2] == pos && winLose[2, 0] == pos)
                winOpertunity.Add(new Position(1, 1));
            else if (winLose[1, 1] == pos && winLose[2, 0] == pos)
                winOpertunity.Add(new Position(0, 2));


            return winOpertunity;
        }
    }
}
