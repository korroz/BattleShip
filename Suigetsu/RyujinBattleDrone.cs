using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Interface;

namespace Suijin
{
  internal class RyujinBattleDrone
  {
    private enum MissionPhase { DetermineTargetOrientation = 1, HitTargetEndPoint, DestroyTarget, MissionSuccess }
    private enum Direction { YPlus, XPlus, YMinus, XMinus }

    private Suigetsu _mothership;
    private Coordinate _missionTarget;
    private MissionPhase _phase;
    private Orientation _orientation;
    private Coordinate _endPoint;
    private Direction _strafingDirection;

    private Coordinate _lastTarget;
    private List<Coordinate> _targetHits;
    private List<Coordinate> _nextTargetCandidate;
    private List<Coordinate> _eliminatedCoords;

    public RyujinBattleDrone(Suigetsu suigetsuClassMothership)
    {
      _mothership = suigetsuClassMothership;
      _targetHits = new List<Coordinate>();
      _nextTargetCandidate = new List<Coordinate>();
      _eliminatedCoords = new List<Coordinate>();
    }

    #region Mothership dependant methods

    internal void StartMission(Shot _lastShot)
    {
      _missionTarget = new Coordinate(_lastShot);
      _targetHits.Add(_missionTarget);
      _phase = MissionPhase.DetermineTargetOrientation;

      _eliminatedCoords.AddRange(_mothership.EliminatedCoords.Where(c => c.X == _missionTarget.X || c.Y == _missionTarget.Y));

      ProcessIntel();
    }

    internal Coordinate ContinueMission()
    {
      Coordinate laserTarget = SensorScan();
      _lastTarget = laserTarget;
      return laserTarget;
    }

    internal void ContinueMission(int hits)
    {
      if (hits == 0)
      {
        _eliminatedCoords.Add(_lastTarget);

        if (_phase == MissionPhase.HitTargetEndPoint)
        {
          _endPoint = _targetHits.Last();
          _lastTarget = _targetHits.First();
          _strafingDirection = CalculateDirection(_endPoint, _lastTarget, _orientation);

          _phase = MissionPhase.DestroyTarget;
        }

        ProcessIntel();
      }
      else
      {
        _targetHits.Add(_lastTarget);

        if (_targetHits.Count == 2)
        {
          _orientation = CalculateOrientation(_targetHits[0], _targetHits[1]);
          _strafingDirection = CalculateDirection(_targetHits[0], _targetHits[1], _orientation);

          _phase = MissionPhase.HitTargetEndPoint;
        }

        ProcessIntel();
      }
    }

    internal void ReturnToBase()
    {
      _targetHits.Add(_lastTarget);
    }

    internal List<Coordinate> TargetDamageReport()
    {
      return _targetHits;
    }

    internal List<Coordinate> DownloadIntel()
    {
      return CompileIntelData();
    }

    #endregion

    private Coordinate SensorScan()
    {
      if (_nextTargetCandidate.Count > 1)
        return _nextTargetCandidate[_mothership._rand.Next(_nextTargetCandidate.Count)];
      else
        return _nextTargetCandidate[0];
    }

    private void ProcessIntel()
    {
      if (_phase == MissionPhase.DetermineTargetOrientation)
        DeduceOrientation();

      if (_phase == MissionPhase.HitTargetEndPoint)
        StrafeRun();

      if (_phase == MissionPhase.DestroyTarget)
        StrafeRun();
    }

    private void StrafeRun()
    {
      _nextTargetCandidate.Clear();
      _nextTargetCandidate.Add(CoordStep(_lastTarget, _strafingDirection, 1));
    }

    private void DeduceOrientation()
    {
      _nextTargetCandidate.Clear();
      Direction dir;
      Coordinate check;
      for (int i_dir = 0; i_dir <= (int)Direction.XMinus; i_dir++)
      {
        dir = (Direction)i_dir;
        check = CoordStep(_missionTarget, dir, 1);
        if (!IsEliminated(check))
          _nextTargetCandidate.Add(check);
      }

    }

    private List<Coordinate> CompileIntelData()
    {
      List<Coordinate> intel = new List<Coordinate>();
      Orientation plane = CalculateOrientation(_targetHits[0], _targetHits[1]);
      Coordinate bow, stern, topLeft, bottomRight;
      IOrderedEnumerable<Coordinate> sortedHits;

      if (plane == Orientation.Horizontal)
      {
        sortedHits = _targetHits.OrderBy(c => c.X);
        bow = sortedHits.First();
        stern = sortedHits.Last();
      }
      else
      {
        sortedHits = _targetHits.OrderBy(c => c.Y);
        bow = sortedHits.Last();
        stern = sortedHits.First();
      }

      topLeft = new Coordinate(bow.X - 1, bow.Y + 1);
      bottomRight = new Coordinate(stern.X + 1, stern.Y - 1);

      int deltaX = bottomRight.X - topLeft.X;
      int deltaY = topLeft.Y - bottomRight.Y;

      intel.Add(topLeft);
      intel.Add(bottomRight);

      for (int xModifier = 0; xModifier < deltaX; xModifier++)
      {
        intel.Add(CoordStep(topLeft, Direction.XPlus, xModifier + 1));
        intel.Add(CoordStep(bottomRight, Direction.XMinus, xModifier + 1));
      }

      for (int yModifier = 0; yModifier < deltaY; yModifier++)
      {
        intel.Add(CoordStep(topLeft, Direction.YMinus, yModifier + 1));
        intel.Add(CoordStep(bottomRight, Direction.YPlus, yModifier + 1));
      }

      for (int i = 0; i < intel.Count; i++)
      {
        if (OutOfAO(intel[i]))
        {
          intel.RemoveAt(i);
          i--;
        }
      }

      return intel;
    }

    #region Helpers

    private bool IsEliminated(Coordinate coord)
    {
      if (OutOfAO(coord))
        return true;

      return _eliminatedCoords.Any(c => c.X == coord.X && c.Y == coord.Y);
    }

    private bool OutOfAO(Coordinate coord)
    {
      return (coord.X > _mothership._xMax || coord.X < 1) || (coord.Y > _mothership._yMax || coord.Y < 1);
    }

    private Coordinate CoordStep(Coordinate startPoint, Direction dir, int steps)
    {
      switch (dir)
      {
        case Direction.YPlus:
          return new Coordinate(startPoint.X, startPoint.Y + steps);
        case Direction.XPlus:
          return new Coordinate(startPoint.X + steps, startPoint.Y);
        case Direction.YMinus:
          return new Coordinate(startPoint.X, startPoint.Y - steps);
        case Direction.XMinus:
          return new Coordinate(startPoint.X - steps, startPoint.Y);
        default:
          throw new ArgumentException("dir");
      }
    }

    private Orientation CalculateOrientation(Coordinate coord1, Coordinate coord2)
    {
      if (coord1.X == coord2.X)
        return Orientation.Vertical;
      else if (coord1.Y == coord2.Y)
        return Orientation.Horizontal;
      else
        throw new Exception("The two coordinates (coord1 and coord2) was not coplanar");
    }

    private Direction CalculateDirection(Coordinate startPoint, Coordinate toPoint, Orientation plane)
    {
      switch (plane)
      {
        case Orientation.Horizontal:
          if (startPoint.X < toPoint.X)
            return Direction.XPlus;
          else
            return Direction.XMinus;
        case Orientation.Vertical:
          if (startPoint.Y < toPoint.Y)
            return Direction.YPlus;
          else
            return Direction.YMinus;
        default:
          throw new Exception();
      }
    }

    #endregion
  }
}
