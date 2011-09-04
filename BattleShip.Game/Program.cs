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
         PlayerInfo player1 = new PlayerInfo();
         PlayerInfo player2 = new PlayerInfo();

         bool printTurns = Boolean.Parse(ConfigurationManager.AppSettings["printTurns"] ?? "False");
         player1.Assembly = ConfigurationManager.AppSettings["player1"];
         player2.Assembly = ConfigurationManager.AppSettings["player2"];

         bool isInteractive = args.Any(s => s == "-i");
         bool isMultiRound = args.Any(s => s == "-r");
         printTurns = (printTurns || args.Any(s => s == "-t")) && !args.Any(s => s == "--noturns");
         int totalRounds = 1, round = 1;

         if (isMultiRound)
         {
            totalRounds = round = Int32.Parse(args[Array.IndexOf(args, "-r") + 1]);
            printTurns = false;
         }

         while (round-- > 0)
         {

            GameSimulator game = new GameSimulator(player1.Assembly, player2.Assembly);
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

            Console.WriteLine("Winner is {0} after {1} turns!", game.GetWinningPlayer().Name, game.TurnCount);

            if (game.GetWinner() == Player.One)
               player1.Won();
            else
               player2.Won();

            if (round == 0)
            {
               player1.Name = game.Player1.Name;
               player2.Name = game.Player2.Name;
            }

            InteractivePause(isInteractive);
         }

         if (isMultiRound)
            Console.WriteLine("{0}Results:{0}\tMatches: {1}{0}\tPlayer One wins: {2,-7} ({3,5:0.#%}){0}\tPlayer Two wins: {4,-7} ({5,5:0.#%})",
               Environment.NewLine,
               totalRounds,
               player1.Wins, ((double)player1.Wins / totalRounds),
               player2.Wins, ((double)player2.Wins / totalRounds));
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
