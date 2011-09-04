using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleShip.Interface;

namespace BattleShip.Simulator
{
   public class GameSimulator
   {
      private IPlayer _player1;
      private IPlayer _player2;
      private Player _whosTurn = Player.One;
      private bool _gameOver;
      private int _turns = 1;
      private Gameboard _board = new Gameboard();

      public IPlayer Player1 { get { return _player1; } set { _player1 = value; } }
      public IPlayer Player2 { get { return _player2; } set { _player2 = value; } }
      public int TurnCount { get { return _turns; } }
      public bool GameOver { get { return _gameOver; } }

      public GameSimulator(string player1AI, string player2AI)
      {
         IPlayer p1 = null;
         IPlayer p2 = null;

         try
         {
            p1 = Activator.CreateInstance(Type.GetType(player1AI)) as IPlayer;
            p2 = Activator.CreateInstance(Type.GetType(player2AI)) as IPlayer;

         }
         catch (ArgumentNullException)
         {
         }

         if (p1 == null || p2 == null)
            throw new Exception("Error while trying to load AI assemblies.");

         _player1 = p1;
         _player2 = p2;
      }
      public GameSimulator(IPlayer player1, IPlayer player2)
      {
         _player1 = player1;
         _player2 = player2;
      }
      public void RunGame()
      {

      }
      public void SetUpShips()
      {
         _player1.PlaceShips(_board.GetPlayerView(Player.One), _board.CreateFlotilla());
         _player2.PlaceShips(_board.GetPlayerView(Player.Two), _board.CreateFlotilla());

         if (!_board.SetUpComplete())
            throw new Exception("A player has failed to place his fleet on the game board.");

         _board.GameStarted = true;
      }
      public TurnEvent NextTurn()
      {
         IPlayer currentPlayer = GetCurrentPlayer();
         IPlayerView currentPlayersView = _board.GetPlayerView(_whosTurn);

         Shot playerShot = currentPlayer.YourTurn(currentPlayersView);
         ShotFeedback shotFeedback = _board.FireShot(_whosTurn, playerShot);

         TurnEvent turnEvent = new TurnEvent();
         turnEvent.Player = currentPlayer;
         turnEvent.PlayerNr = _whosTurn;
         turnEvent.Shot = playerShot;
         turnEvent.Hit = shotFeedback.Hits > 0;
         turnEvent.Turn = _turns;

         if (shotFeedback.Hits > 0 && _board.IsAnyPlayerDead())
         {
            _gameOver = true;
            return turnEvent;
         }

         currentPlayer.ShotFeedback(shotFeedback.Hits, shotFeedback.SunkShips);


         EndTurn();

         return turnEvent;
      }

      public IPlayer GetWinningPlayer()
      {
         return GetCurrentPlayer();
      }
      public Player GetWinner()
      {
         return _whosTurn;
      }
      public ICollection<Placement> GetPlayerShips(Player player)
      {
         return _board._playerShips[player];
      }

      private IPlayer GetCurrentPlayer()
      {
         if (_whosTurn == Player.One)
            return _player1;
         else
            return _player2;
      }
      private void EndTurn()
      {
         if (_whosTurn == Player.Two)
            _turns++;
         SwitchCurrentPlayer();

      }
      private void SwitchCurrentPlayer()
      {
         if (_whosTurn == Player.One)
            _whosTurn = Player.Two;
         else
            _whosTurn = Player.One;
      }
   }
}
