using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Interface;

namespace Suijin
{
  public class Suigetsu : IPlayer
  {
    internal int _xMax;
    internal int _yMax;
    internal Random _rand;
    private List<Coordinate> _firingPattern;
    private List<Coordinate> _eliminatedCoords;
    private List<Coordinate> _hitRecord;
    private Shot _lastShot;
    private int _turn;
    private RyujinBattleDrone _attackDrone;

    internal List<Coordinate> EliminatedCoords { get { return _eliminatedCoords; } }
    private bool IsDroneDeployed { get { return _attackDrone != null; } }

    public Suigetsu()
    {

    }

    #region IPlayer implementation

    public string Name
    {
      get { return "Suigetsu"; }
    }

    public void PlaceShips(IPlayerView playerView, ICollection<IVessel> ships)
    {
      InitializeAI(playerView);

      int x, y;
      Orientation orient;
#if DEBUG
      Visualizer viz = new Visualizer(_xMax, _yMax);
#endif

      foreach (IVessel ship in ships)
      {
        do
        {
          x = _rand.Next(0, _xMax) + 1;
          y = _rand.Next(0, _yMax) + 1;
          orient = (_rand.NextDouble() < 0.5) ? Orientation.Horizontal : Orientation.Vertical;
        } while (!playerView.PutShip(ship.SailTo(x, y, orient)));
#if DEBUG
        for (int i = 0; i < ship.Length; i++)
        {
          if (orient == Orientation.Horizontal)
            viz.SetSquare(x + i, y, false);
          else
            viz.SetSquare(x, y - i, false);
        }
#endif
      }
#if DEBUG
      Console.WriteLine("Ships:");
      Console.WriteLine(viz.FieldView());
#endif
    }

    public Shot YourTurn(IPlayerView playerView)
    {
      _turn++;

#if DEBUG
      if ((_turn % 20) == 0 && _turn < 150)
      {
        Console.WriteLine("Eliminated squares:");
        Visualizer.ConsolePrintField(_xMax, _yMax, _eliminatedCoords);
        Console.WriteLine("Ships hit:");
        Visualizer.ConsolePrintField(_xMax, _yMax, _hitRecord);
        Console.WriteLine("Firing Solution:");
        Visualizer.ConsolePrintField(_xMax, _yMax, _firingPattern);
        Console.ReadKey(true);
      }
#endif
      Coordinate firingSolution;

      if (IsDroneDeployed)
      {
        firingSolution = _attackDrone.ContinueMission();
      }
      else
      {
        firingSolution = PickRandomFiringSolution();

        // FIXME
        if (firingSolution == null)
          firingSolution = new Coordinate(1, 1);

        _firingPattern.Remove(firingSolution);
        _eliminatedCoords.Add(firingSolution);
      }

      Shot bang = new Shot(firingSolution.X, firingSolution.Y);

      _lastShot = bang;

      return bang;
    }

    public void ShotFeedback(int hits, int sunkShips)
    {
      if (sunkShips > 0)
        RecallAndDebriefDrone();
      else if (IsDroneDeployed)
        _attackDrone.ContinueMission(hits);
      else if (hits > 0)
        LaunchAttackDrone();
    }

    #endregion

    #region Event-like methods

    private void InitializeAI(IPlayerView playerView)
    {
#if DEBUG
      Console.WindowHeight = 60;
#endif
      _xMax = playerView.GetXMax();
      _yMax = playerView.GetYMax();
      _rand = new Random(Environment.TickCount + 1337 + 42);

      SetUpStrategy();
    }

    private void SetUpStrategy()
    {
      _firingPattern = new List<Coordinate>(_xMax * _yMax);
      _eliminatedCoords = new List<Coordinate>();
      _hitRecord = new List<Coordinate>();

      for (int y = 1; y <= _yMax; y++)
        for (int x = 1 + (y % 2); x <= _xMax; x += 2)
          _firingPattern.Add(new Coordinate(x, y));

#if DEBUG
      Console.WriteLine("Firing pattern:");
      Visualizer.ConsolePrintField(_xMax, _yMax, _firingPattern);
#endif
    }

    private void LaunchAttackDrone()
    {
      _attackDrone = new RyujinBattleDrone(this);
      _attackDrone.StartMission(_lastShot);
    }

    private void RecallAndDebriefDrone()
    {
      _attackDrone.ReturnToBase();

      List<Coordinate> hits = _attackDrone.TargetDamageReport();
      List<Coordinate> nearbyIntel = _attackDrone.DownloadIntel();

#if DEBUG
      Console.WriteLine("Attack Drone hits:");
      Visualizer.ConsolePrintField(_xMax, _yMax, hits);
      Console.WriteLine("nearbyIntel:");
      Visualizer.ConsolePrintField(_xMax, _yMax, nearbyIntel);
#endif

      _eliminatedCoords.AddRange(hits);
      _eliminatedCoords.AddRange(nearbyIntel);

      RefreshFiringSolution();

      _hitRecord.AddRange(hits);

      _attackDrone = null;
    }

    private void RefreshFiringSolution()
    {
      Coordinate check;
      foreach (Coordinate elimCoord in _eliminatedCoords)
      {
        check = _firingPattern.SingleOrDefault(c => (c.X == elimCoord.X && c.Y == elimCoord.Y));
        if (check != null)
          _firingPattern.Remove(check);
      }
    }

    #endregion

    #region Helpers

    private Coordinate PickRandomFiringSolution()
    {
      if (_firingPattern.Count == 0)
        return null;

      return _firingPattern[_rand.Next(_firingPattern.Count)];
    }

    #endregion
  }
}
