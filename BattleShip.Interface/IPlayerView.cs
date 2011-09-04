namespace BattleShip.Interface
{
    public interface IPlayerView
    {
        int GetXMax();
        int GetYMax();
        // Only works during the PlaceShip() phase of the game.
        bool PutShip(Placement placement);
    }
}
