using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Interface;

namespace Suijin
{
  internal class Visualizer
  {
    private const string _trueSquare = "[X]";
    private const string _falseSquare = "[O]";
    private const string _nullSquare = "[ ]";

    private int _xMax;
    private int _yMax;
    private bool?[,] _field;

    public Visualizer(int xMax, int yMax)
    {
      _xMax = xMax;
      _yMax = yMax;
      _field = (bool?[,])Array.CreateInstance(typeof(bool?), _xMax, _yMax);
    }

    public void SetSquare(int x, int y, bool? value)
    {
      if (x > _xMax || x < 1)
        throw new ArgumentOutOfRangeException("x", "X was out of range.");
      if (y > _yMax || y < 1)
        throw new ArgumentOutOfRangeException("y", "Y was out of range.");

      _field[x - 1, y - 1] = value;
    }

    public string FieldView()
    {
      StringBuilder sb = new StringBuilder();

      bool? currentSquare;
      for (int y = _yMax - 1; y >= 0; y--)
      {
        for (int x = 0; x < _xMax; x++)
        {
          currentSquare = _field[x, y];
          if (!currentSquare.HasValue)
            sb.Append(_nullSquare);
          else if (currentSquare.Value)
            sb.Append(_trueSquare);
          else
            sb.Append(_falseSquare);
        }
        sb.Append(Environment.NewLine);
      }

      return sb.ToString();
    }

    public static void ConsolePrintField(int xMax, int yMax, List<Coordinate> coordinates)
    {
      Visualizer viz = new Visualizer(xMax, yMax);

      foreach (Coordinate coord in coordinates)
      {
        viz.SetSquare(coord.X, coord.Y, true);
      }

      Console.WriteLine(viz.FieldView());
      Console.WriteLine();
    }
  }
}
