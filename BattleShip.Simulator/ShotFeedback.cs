using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleShip.Simulator
{
    class ShotFeedback
    {
        private int _hits;
        private int _sunkShips;

        public int Hits { get { return _hits; } }
        public int SunkShips { get { return _sunkShips; } }

        public ShotFeedback(int hits, int sunkShips)
        {
            _hits = hits;
            _sunkShips = sunkShips;
        }
    }
}
