﻿namespace Tartaros.Sectors
{
    using System.Collections;
    using System.Collections.Generic;
    using Tartaros.Utilities;
    using Tartaros.ServicesLocator;
    using UnityEngine;

    public class Map_Test_Bounds : MonoBehaviour, IMap
    {

        Bounds2D IMap.MapBounds => new Bounds2D(-100, 100, -100, 100);

        bool IMap.CanBuild(Vector2 buildingPosition, Vector2 buildingSize)
        {
            throw new System.NotImplementedException();
        }

        private void Awake()
        {
            Services.Instance.RegisterService<IMap>(this);
        }
    }

}