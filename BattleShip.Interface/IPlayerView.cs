using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleShip.Interface
{
    public interface IPlayerView
    {
        int GetXMax();
        int GetYMax();
        // Only works during the PlaceShip() phase of the game.
        bool PutShip(Placement placement);
    }
}
