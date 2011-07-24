using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Interface;

namespace BattleShip.Simulator
{
    public class TurnEvent
    {
        private IPlayer _player;
        private Player _playerNr;
        private Shot _shot;
        private bool _hit;
        private int _turn;

        public IPlayer Player { get { return _player; } set { _player = value; } }
        public Player PlayerNr { get { return _playerNr; } set { _playerNr = value; } }
        public Shot Shot { get { return _shot; } set { _shot = value; } }
        public bool Hit { get { return _hit; } set { _hit = value; } }
        public int Turn { get { return _turn; } set { _turn = value; } }
    }
}
