﻿namespace BattleShip.Interface
{
    public interface IVessel
    {
        int Length { get; }
        Placement SailTo(Coordinate coord, Orientation orientation);
        Placement SailTo(int x, int y, Orientation orientation);
    }
}
