using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Interface;

namespace BattleShip.Simulator
{
    public class GameView : IPlayerView
    {
        private Gameboard _board;
        private Player _player;

        public GameView(Gameboard board, Player player)
        {
            _board = board;
            _player = player;
        }

        public int GetXMax()
        {
            return _board.XMax;
        }

        public int GetYMax()
        {
            return _board.YMax;
        }

        public bool PutShip(Placement placement)
        {
            if (!(placement.Vessel is Vessel))
                throw new ArgumentException("Expecting ship to be of type Vessel, but it was something else (possible cheat).", "placement");

            return _board.PlacePlayerShip(_player, placement);
        }
    }
}
