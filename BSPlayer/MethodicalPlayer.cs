using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Interface;

namespace BSPlayer
{
    public class MethodicalPlayer : IPlayer
    {
        private Coordinate _nextShot = new Coordinate(1, 1);

        #region IPlayer implementation

        public string Name
        {
            get { return "MethodicalPlayer"; }
        }

        public void PlaceShips(IPlayerView playerView, ICollection<IVessel> ships)
        {
            /* This AI places ships in the upper right corner and to the left.
             * 
             * E.g.
             * 
             * 10  #   #   #   #   #
             * 9   #   #   #   #   #
             * 8       #   #   #   #
             * 7               #   #
             * 6                   #
             * 5
             * 4
             * 3
             * 2
             * 1
             *   1 2 3 4 5 6 7 8 9 10
             */

            Placement place;
            Coordinate coord;
            int xMax = playerView.GetXMax();
            int yMax = playerView.GetYMax();

            int i = 0;
            foreach (IVessel ship in ships)
            {
                coord = new Coordinate(xMax - (2 * i), yMax);
                place = new Placement(ship, coord, Orientation.Vertical);
                playerView.PutShip(place);

                i++;
            }
        }

        public void ShotFeedback(int hits, int sunkShips)
        {
            // Haha!! I don't care if I hit or miss. :D
        }

        public Shot YourTurn(IPlayerView playerView)
        {
            int xMax = playerView.GetXMax();
            int yMax = playerView.GetYMax();

            Shot fireZeMissles = new Shot(_nextShot.X, _nextShot.Y);

            _nextShot.X++;

            if (_nextShot.X > xMax)
            {
                _nextShot.Y++;
                _nextShot.X = 1;
            }

            if (_nextShot.Y > yMax)
                _nextShot.Y = 1;

            return fireZeMissles;
        }

        #endregion
    }
}
