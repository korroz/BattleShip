using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleShip.Interface
{
    public class Placement : Coordinate
    {
        private Orientation _orientation;
        private IVessel _vessel;

        public Orientation Orientation { get { return _orientation; } set { _orientation = value; } }
        public IVessel Vessel { get { return _vessel; } set { _vessel = value; } }

        public Placement(IVessel vessel, int x, int y, Orientation orientation) : base(x, y)
        {
            _vessel = vessel;
            _orientation = orientation;
        }
        public Placement(IVessel vessel, Coordinate coord, Orientation orientation) : this(vessel, coord.X, coord.Y, orientation)
        {
        }
    }
}
