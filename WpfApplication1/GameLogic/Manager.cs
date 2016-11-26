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


        public Manager()
        {
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    board[i, j] = new LittleBoard();
                    winLose[i , j] = BoardStatuse.PROCCES;
                }
        }

        public BoardStatuse makeMove(PlaceInformation box , LittleBoard.PositionOnBoard positionOnBoard)
        {

            BoardStatuse state = positionOnBoard == LittleBoard.PositionOnBoard.PLAYER ? BoardStatuse.WIN : BoardStatuse.LOSE;

            winLose[box.BigRow , box.BigCol] = board[box.BigRow, box.BigCol].makeMove(positionOnBoard, box.PosRow, box.PosCol) ?
                                                                            state : BoardStatuse.PROCCES;

            if (isEnd(state))
                return positionOnBoard == LittleBoard.PositionOnBoard.PLAYER ? BoardStatuse.ABSULOT_WIN : BoardStatuse.ABSULOT_LOSE;

            return winLose[box.BigRow, box.BigCol];
        }

        private int getMarkOfChoice(LittleBoard.PositionOnBoard positionOnBoard)
        {
            int toReturn = 0;
            BoardStatuse state = positionOnBoard == LittleBoard.PositionOnBoard.PLAYER ? BoardStatuse.WIN : BoardStatuse.LOSE;
            BoardStatuse oppState = getOpposite(state);
            if (isEnd(state))
                return 100;

            toReturn += (lastMove(state).Count*15);
            toReturn -= (lastMove(oppState).Count * 15);

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (winLose[i, j] != BoardStatuse.PROCCES)
                        toReturn += winLose[i, j] == state ? 5 : -5;
                    else
                        toReturn +=  board[i, j].lastMove(positionOnBoard) - board[i, j].lastMove(LittleBoard.getOpposit(positionOnBoard));

            return toReturn;

        }

        public PlaceInformation getNextBestMove(int row , int col)
        {
            AlphaBetaPosition toReturn = null;
            board[row, col].Calculate = true;
            toReturn = getNextBestMove(row , col , computer , int.MinValue , int.MaxValue , 2 );
            board[row, col].Calculate = false;

            if (board[toReturn.BigRow, toReturn.BigCol].Finish)
            {
                toReturn.row *= -1;
                toReturn.col *= -1;
            }

            return new PlaceInformation(toReturn);
        }

        private static readonly LittleBoard.PositionOnBoard computer = LittleBoard.PositionOnBoard.COMPUTER;
        private static readonly LittleBoard.PositionOnBoard player   = LittleBoard.PositionOnBoard.PLAYER;
        private static readonly LittleBoard.PositionOnBoard none     = LittleBoard.PositionOnBoard.NONE;

        private AlphaBetaPosition getNextBestMove(int row , int col , LittleBoard.PositionOnBoard posOnBoard , int alpha ,  int beta , int iteration)
        {
            int valueComputer = int.MinValue;
            int valuePlayer = int.MaxValue;
            AlphaBetaPosition toReturn = new AlphaBetaPosition(row , col);
            //toReturn.BigRow = row;
            //toReturn.BigCol = col;
            if (isEnd(getOpposite(playerToState(posOnBoard))) || iteration == 0)
            {
                toReturn.Score = getMarkOfChoice(computer);
                return toReturn;
            }
          
            if (winLose[row, col] != BoardStatuse.PROCCES)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        toReturn = getNextBestMove(i, j, posOnBoard, alpha, beta, iteration);
                        if (winLose[i, j] == BoardStatuse.PROCCES)
                        {
                            if (posOnBoard == computer)
                            {
                                valueComputer = Math.Max(valueComputer, toReturn.Score);
                                alpha = Math.Max(valueComputer, alpha);
                                if (beta <= alpha)
                                {
                                    toReturn.BigRow = i;
                                    toReturn.BigCol = j;
                                    toReturn.Score = valueComputer;
                                    return toReturn;
                                }
                            }
                            else
                            {
                                valuePlayer = Math.Min(valuePlayer, toReturn.Score);
                                beta = Math.Min(beta, valuePlayer);
                                if (beta <= alpha)
                                {
                                    toReturn.BigRow = i;
                                    toReturn.BigCol = j;
                                    toReturn.Score = valueComputer;
                                    return toReturn;
                                }
                            }
                        }
                    }
                }

                return toReturn;
            }

            List<Position> allPossibilities = board[row, col].allOptions();
            
            foreach (Position pos in allPossibilities)
            {
                makeMove(new PlaceInformation(row, col, pos.row, pos.col) , posOnBoard);

                if (posOnBoard == computer)
                {
                    toReturn = getNextBestMove(pos.row, pos.col, player, alpha, beta, iteration - 1);
                    valueComputer = Math.Max(valueComputer, toReturn.Score);
                    alpha = Math.Max(valueComputer, alpha);

                    winLose[row, col] = board[row, col].goBack(1, false, computer) ? playerToState(posOnBoard) : BoardStatuse.PROCCES;

                    if (beta <= alpha)
                    {
                        toReturn.BigRow = row;
                        toReturn.BigCol = col;
                        toReturn.Score = valueComputer;
                        return toReturn;
                    }
                }
                else
                {
                    toReturn = getNextBestMove(pos.row, pos.col, computer, alpha, beta, iteration - 1);
                    valuePlayer = Math.Min(valuePlayer, toReturn.Score);
                    beta = Math.Min(beta, valuePlayer);

                    winLose[row, col] = board[row, col].goBack(1, false, player) ? playerToState(posOnBoard) : BoardStatuse.PROCCES;

                    if (beta <= alpha)
                    {
                        toReturn.BigRow = row;
                        toReturn.BigCol = col;
                        toReturn.Score = valueComputer;
                        return toReturn;
                    }
                }
            }

            toReturn.BigRow = row;
            toReturn.BigCol = col;

            return toReturn;

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
            

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (winLose[i, j] == BoardStatuse.PROCCES)
                        return false;

            return true;
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
