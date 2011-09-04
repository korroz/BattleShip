using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleShip.Game
{
   internal class PlayerInfo
   {
      public int Wins { get; set; }
      public string Name { get; set; }
      public string Assembly { get; set; }

      internal void Won()
      {
         Wins++;
      }
   }
}
