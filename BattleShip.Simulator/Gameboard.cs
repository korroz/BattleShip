using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Interface;

namespace BattleShip.Simulator
{
    public class Gameboard
    {
        private static readonly int[] _shipSizes = { 5, 4, 3, 3, 2 };
        private int _xMax;
        private int _yMax;
        private bool _gameStarted;
        internal Dictionary<Player, List<Placement>> _playerShips = new Dictionary<Player, List<Placement>>(2);

        public int XMax { get { return _xMax; } }
        public int YMax { get { return _yMax; } }

        public bool GameStarted
        {
            get { return _gameStarted; }
            internal set { _gameStarted = value; }
        }


        public Gameboard() : this(10, 10)
        {
        }
        public Gameboard(int gridXSize, int gridYSize)
        {
            _xMax = gridXSize;
            _yMax = gridYSize;
            _playerShips.Add(Player.One, new List<Placement>(_shipSizes.Length));
            _playerShips.Add(Player.Two, new List<Placement>(_shipSizes.Length));
        }

        internal IPlayerView GetPlayerView(Player whosTurn)
        {
            return new GameView(this, whosTurn);
        }

        internal ShotFeedback FireShot(Player whosTurn, Shot playerShot)
        {
            List<Placement> targetFleet = _playerShips[whosTurn == Player.One ? Player.Two : Player.One];
            Vessel ship;
            int start, end, shot, segment;
            int hits = 0;
            int sunkShips = 0;

            foreach (Placement shipPlacement in targetFleet)
            {
                ship = (Vessel)shipPlacement.Vessel;

                if (shipPlacement.Orientation == Orientation.Horizontal && shipPlacement.Y == playerShot.Y)
                {
                    start = shipPlacement.X;
                    end = shipPlacement.X + (ship.Length - 1);
                    shot = playerShot.X;
                }
                else if (shipPlacement.Orientation == Orientation.Vertical && shipPlacement.X == playerShot.X)
                {
                    start = shipPlacement.Y - (ship.Length - 1);  // TODO: This should be taken care of.
                    end = shipPlacement.Y;
                    shot = playerShot.Y;
                }
                else
                    continue;

                segment = shot - start;

                if (segment < ship.Length && segment >= 0)
                {
                    ship.Hit(segment);
                    hits++;

                    if (ship.IsSunk)
                        sunkShips++;
                }

            }

            return new ShotFeedback(hits, sunkShips);
        }

        internal ICollection<IVessel> CreateFlotilla()
        {
            List<IVessel> flotilla = new List<IVessel>(5);
            foreach (int ship in _shipSizes)
            {
                flotilla.Add(new Vessel(ship));
            }
            return flotilla;
        }
        internal bool SetUpComplete()
        {
            List<int> shipCheckList = new List<int>(_shipSizes.Length);

            foreach (KeyValuePair<Player, List<Placement>> pair in _playerShips)
            {
                if (pair.Value.Count != _shipSizes.Length)
                    return false;

                shipCheckList.AddRange(_shipSizes);

                foreach (Placement placedShip in pair.Value)
                {
                    shipCheckList.Remove(placedShip.Vessel.Length);
                }

                if (shipCheckList.Count != 0)
                    return false;
            }

            return true;
        }
        internal bool IsAnyPlayerDead()
        {
            int shipCount;
            foreach (KeyValuePair<Player, List<Placement>> pair in _playerShips)
            {
                shipCount = 0;
                foreach (Placement placement in pair.Value)
                {
                    if (!((Vessel)placement.Vessel).IsSunk)
                        shipCount++;
                }

                if (shipCount == 0)
                    return true;
            }
            return false;
        }
        internal bool PlacePlayerShip(Player _player, Placement placement)
        {
            if (_gameStarted)
                throw new Exception(String.Format("Player {0} tried to place a ship after the game started. Bad player.", _player));

            List<Placement> playerFleet = _playerShips[_player];

            if (IsLegalBoardPlacement(placement))
            {
                foreach (Placement placedShip in playerFleet)
                {
                    if (IsInTheWayOf(placedShip, placement))
                        return false;
                }
            }
            else
                return false;

            playerFleet.Add(placement);
            return true;
        }

        private bool IsLegalBoardPlacement(Placement placement)
        {
            Coordinate start, end, boardTopLeft, boardBottomRight;
            
            start = new Coordinate(placement);
            end = ShipEndPoint(placement);

            boardTopLeft = new Coordinate(1, YMax);
            boardBottomRight = new Coordinate(XMax, 1);

            if (CoordinateInSquare(start, boardTopLeft, boardBottomRight) &&
                CoordinateInSquare(end, boardTopLeft, boardBottomRight))
                return true;

            return false;
        }
        private bool IsInTheWayOf(Placement placedShip, Placement placement)
        {
            Coordinate shipTopLeft, shipBottomRight, endPoint, tmpSegment;

            // TODO: This should be BottomLeft and TopRight if going away from origo.
            shipTopLeft = new Coordinate(placedShip.X - 1, placedShip.Y + 1);
            endPoint = ShipEndPoint(placedShip);
            shipBottomRight = new Coordinate(endPoint.X + 1, endPoint.Y - 1);

            for (int i = 0; i <= placement.Vessel.Length; i++)
            {
                if (placement.Orientation == Orientation.Horizontal)
                    tmpSegment = new Coordinate(placement.X + i, placement.Y);
                else //if (placement.Orientation == Orientation.Vertical)
                    tmpSegment = new Coordinate(placement.X, placement.Y - i);

                if (CoordinateInSquare(tmpSegment, shipTopLeft, shipBottomRight))
                    return true;
            }

            return false;
        }
        private Coordinate ShipEndPoint(Placement placement)
        {
            Coordinate end;
            if (placement.Orientation == Orientation.Horizontal)
                end = new Coordinate(placement.X + (placement.Vessel.Length - 1), placement.Y);
            else //if (placement.Orientation == Orientation.Vertical)
                end = new Coordinate(placement.X, placement.Y - (placement.Vessel.Length - 1));  // TODO: This should be changed.

            return end;
        }
        // TODO: This should be rewritten to work with BottomLeft and TopRight.
        private bool CoordinateInSquare(Coordinate testCoord, Coordinate topLeft, Coordinate bottomRight)
        {
            if ((testCoord.X >= topLeft.X && testCoord.X <= bottomRight.X) &&
                (testCoord.Y <= topLeft.Y && testCoord.Y >= bottomRight.Y))
                return true;

            return false;
        }
    }
}
