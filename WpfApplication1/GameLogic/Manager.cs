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

        

        public BoardStatuse makeMove(PlaceInformation boxInformation, out int row, out int col)
        {
            // (board[boxInformation.BigRow, boxInformation.BigCol]).makeMove(LittleBoard.PositionOnBoard.PLAYER , boxInformation.PosRow, boxInformation.PosCol);
            int currentCol = boxInformation.BigCol;
            int currentRow = boxInformation.BigRow;
            
            ReportOnBoard report = board[currentCol, currentRow].goingToLose();
           
            row = 1;
            col = 2;
            return BoardStatuse.WIN;
        }
    }
}
