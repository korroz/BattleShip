using System.Collections.Generic;

namespace BattleShip.Interface
{
    public interface IPlayer
    {
        string Name { get; }
        void PlaceShips(IPlayerView playerView, ICollection<IVessel> ships);
        Shot YourTurn(IPlayerView playerView);
        void ShotFeedback(int hits, int sunkShips);
    }
}
