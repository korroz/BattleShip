using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Interface;

namespace BattleShip.Simulator
{
    public class Vessel : IVessel
    {
        private int _length;
        private bool[] _sectionDestroyed;

        public int Length { get { return _length; } }
        public bool IsSunk { get { return _sectionDestroyed.All(b => b); } }

        public Vessel(int shipSize)
        {
            _length = shipSize;
            _sectionDestroyed = (bool[])Array.CreateInstance(typeof(Boolean), shipSize);
        }
        public void Hit(int section)
        {
            _sectionDestroyed[section] = true;
        }
        public Placement SailTo(int x, int y, Orientation orientation)
        {
            return new Placement(this, x, y, orientation);
        }
        public Placement SailTo(Coordinate coord, Orientation orientation)
        {
            return new Placement(this, coord, orientation);
        }
    }
}
