using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.GameLogic
{
    class Position : ICloneable
    {
        public int row;
        public int col;

        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public Position(Position position)
        {
            this.row = position.row;
            this.col = position.col;
        }

        public void diffrent(Position pos, out int newRow, out int newCol)
        {
            newRow = pos.row - row;
            newCol = pos.col - col;

            if (newRow != 0)
                newRow = newRow < 0 ? -1 : 1;

            if (newCol != 0)
                newCol = newCol < 0 ? -1 : 1;

        }

        
        public override bool Equals(Object other)
        {
            Position pos = (Position)other;
            if (pos.row == row && pos.col == col)
                return true;
            return false;
        }

        public object Clone()
        {
            return new Position(this);
        }
    }
}
