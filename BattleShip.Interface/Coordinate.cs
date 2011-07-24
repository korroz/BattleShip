using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleShip.Interface
{
    public class Coordinate
    {
        private int _x;
        private int _y;

        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }

        public Coordinate(int x, int y)
        {
            _x = x;
            _y = y;
        }
        public Coordinate(Coordinate coord)
        {
            _x = coord._x;
            _y = coord._y;
        }
    }
}
