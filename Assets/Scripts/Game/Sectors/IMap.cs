﻿namespace Tartaros.Sectors
{
    using UnityEngine;
    using Tartaros.Utilities;
    public interface IMap
    {
        Utilities.Bounds MapBounds { get; }
        bool CanBuild(Vector2 buildingPosition, Vector2 buildingSize);
    }
}