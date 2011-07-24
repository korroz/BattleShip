using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using BattleShip.Simulator;
using BattleShip.Interface;

namespace BattleShip.Game
{
   class Program
   {
      static void Main(string[] args)
      {
         bool isInteractive = args.Any(s => s == "-i");
         bool isMultiRound = args.Any(s => s == "-r");
         int nrRounds = 1;

         bool printTurns = bool.Parse(ConfigurationManager.AppSettings["printTurns"]);
         string player1 = ConfigurationManager.AppSettings["player1"];
         string player2 = ConfigurationManager.AppSettings["player2"];

         if (isMultiRound)
         {
            nrRounds = Int32.Parse(args[Array.IndexOf(args, "-r") + 1]);
            printTurns = false;
         }

         while (nrRounds-- > 0)
         {

            GameSimulator game = new GameSimulator(player1, player2);
            TurnEvent te;

            game.SetUpShips();

            if (printTurns)
            {
               PrintShipPlacements(game.Player1, Player.One, game.GetPlayerShips(Player.One));
               PrintShipPlacements(game.Player2, Player.Two, game.GetPlayerShips(Player.Two));
            }

            InteractivePause(isInteractive);

            while (!game.GameOver)
            {
               te = game.NextTurn();

               if (printTurns)
               {
                  Console.WriteLine("Turn {0}: Player {1} ({2}) fires a shot at coordinates [{3},{4}] - {5}",
                      te.Turn,
                      te.PlayerNr,
                      te.Player.Name,
                      te.Shot.X,
                      te.Shot.Y,
                      te.Hit ? "BOOM!" : "splash..."
                      );
               }
            }

            Console.WriteLine("Winner is {0} after {1} turns!", game.GetWinner().Name, game.TurnCount);

            InteractivePause(isInteractive);
         }
      }

      static void PrintShipPlacements(IPlayer playerAI, Player player, ICollection<Placement> shipPlacements)
      {
         Console.WriteLine("Player {0} ({1}) placed his ships as follows:", player, playerAI.Name);
         int i = 0;
         foreach (Placement ship in shipPlacements)
         {
            i++;
            Console.WriteLine("  Ship #{0} [ Size = {1}, Coordinate = ({2},{3}), Orientation = {4} ] ",
                i,
                ship.Vessel.Length,
                ship.X,
                ship.Y,
                ship.Orientation
                );
         }
         Console.WriteLine();
      }

      static void InteractivePause(bool isInteractive)
      {
         if (isInteractive)
            Console.ReadKey(true);
      }
   }
}
