using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.GameLogic
{
    class Manager
    {

        public enum BoardStatuse { WIN, LOSE, PROCCES, FAILED, ABSULOT_WIN, ABSULOT_LOSE };


        private LittleBoard[,] board = new LittleBoard[3, 3];

        public void SimulationMode(bool startSimulate = true)
        {

        }

        public Manager()
        {
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                    board[i, j] = new LittleBoard();
        }
        enum ToWinLose { WIN , LOSE}
        private List<Position> getBestBigMove(ToWinLose motive)
        {
            List<Position> positions = new List<Position>();
            LittleBoard.PositionOnBoard me = motive == ToWinLose.WIN ? LittleBoard.PositionOnBoard.COMPUTER : LittleBoard.PositionOnBoard.PLAYER;
            LittleBoard.PositionOnBoard enemy = motive == ToWinLose.WIN ? LittleBoard.PositionOnBoard.PLAYER: LittleBoard.PositionOnBoard.COMPUTER;
            for (int k = 0; k < 2; k++)
            {
                for (int i = 0; i < 3; i++)
                {
                    int numOfComp = 0;
                    int place = -1;
                    for (int j = 0; j < 3; j++)
                    {
                        int thisRow = k == 0 ? i : j;
                        int thisCol = k == 0 ? j : i;
                        if (board[thisRow, thisCol].State == me)
                            numOfComp++;
                        else if (board[thisRow, thisCol].State == enemy)
                            place = j;
                    }
                    if (numOfComp == 2 || place != -1)
                    {
                        int row = (k == 0) ? place : i;
                        int col = (k == 0) ? i : place;
                        positions.Add(new Position(row, col));
                    }
                }
            }
            for(int k = -1; k <= 1; k += 2){
                int numOfComp = 0;
                int place = -1;
                for (int i = 0; i < 3; i++)
                {
                    int ir = k == 0 ? i : 2 - i;
                    if (board[i, ir].State == me)
                        numOfComp++;
                    else if (board[i, ir].State == enemy)
                        place = i;
                }
                positions.Add(new Position(place, 2 - place));
            }
            return positions;
        }

        private List<Position> completing(List<Position> positions)
        {
            List<Position> completedList = new List<Position>();
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    Position pos = new Position(i, j);
                    if (!positions.Contains(pos))
                    {
                        completedList.Add(pos);
                    }
                }
            }
            return completedList;
        }

        public BoardStatuse makeMove(PlaceInformation boxInformation, out int row, out int col)
        {
            int currentCol = boxInformation.BigCol;
            int currentRow = boxInformation.BigRow;
            LittleBoard thisBoard = board[currentCol, currentRow];
            ReportOnBoard report = thisBoard.allOptions(); 
            
            #region One move to win to prevent lost
            List<Position> wayToWin = getBestBigMove(ToWinLose.WIN);
            List<Position> wayToLose = getBestBigMove(ToWinLose.LOSE);
            Position pos = wayToWin.Find(x => x.col == currentCol && x.row == currentRow);
            if(pos != null)
            {
                List<Position> moves = report.OneMoveToFinish;
                if(moves.Count != 0)
                {
                    row = moves[0].row;
                    col = moves[0].col;
                    return BoardStatuse.WIN;
                }
            }
            pos = wayToLose.Find(x => x.col == currentCol && x.row == currentRow);
            if (pos != null)
            {
                List<Position> moves = report.NecessaryMove;
                moves.AddRange(report.OneMoveToBlock);
                moves.AddRange(report.OneMoveToFinish);
                if (moves.Count != 0)
                {
                    row = moves[0].row;
                    col = moves[0].col;
                    return BoardStatuse.PROCCES;
                }
            }
            #endregion
            #region if little board is empty choose other player worst chance
            if (thisBoard.IsEmpty)
            {
                #region if in big scale the player have one step to win or lose
                if (wayToLose.Count != 0 || wayToLose.Count != 0)
                {
                    Dictionary<Position, int> bestChance = new Dictionary<Position, int>();
                    foreach (Position position in wayToWin)
                    {
                        ReportOnBoard reportTemp = board[position.row, position.col].allOptions();
                        if (report.TwoWinPosability.Count != 0)
                        {
                            if (bestChance.ContainsKey(position))
                                bestChance[position]++;
                            else
                                bestChance.Add(position, 1);
                        }
                    }
                    foreach (Position position in completing(wayToWin))
                    {
                        if (bestChance.ContainsKey(position))
                            bestChance[position]++;
                        else
                            bestChance.Add(position, 1);
                    }
                    foreach (Position position in completing(wayToLose))
                    {
                        if (bestChance.ContainsKey(position))
                            bestChance[position]++;
                        else
                            bestChance.Add(position, 1);
                    }
                    int max = 0;
                    Position bestPos = null;
                    foreach (KeyValuePair<Position, int> pair in bestChance)
                    {
                        if (max < pair.Value)
                        {
                            max = pair.Value;
                            bestPos = pair.Key;
                        }
                    }
                    col = bestPos.col;
                    row = bestPos.row;
                    return BoardStatuse.PROCCES;
                }
                #endregion
                //TODO else
            }
            //foreach (Position position in wayToWin)
            #endregion
            row = 0;
            col = 0;
            return BoardStatuse.WIN;
        }
    }
}
