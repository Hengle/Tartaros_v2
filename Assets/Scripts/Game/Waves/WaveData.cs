﻿namespace Tartaros.Wave
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    public class WaveData
    {
        private Dictionary<SpawnPointIdentifier, UnitSequence[]> _sequencesBySpawnPoint;

        public UnitSequence[] GetUnitSequences(SpawnPointIdentifier identifier)
        {
            return _sequencesBySpawnPoint[identifier];
        }

        public int GetSpawnedEntitiesCount()
        {
            int countOfSpawnedEntities = 0;
            foreach (KeyValuePair<SpawnPointIdentifier, UnitSequence[]> kvp in _sequencesBySpawnPoint)
            {
                for (int i = 0; i < kvp.Value.Length; i++)
                {
                    countOfSpawnedEntities += kvp.Value[i].EntitiesCount;
                }
            }
            return countOfSpawnedEntities;

            //return _sequencesBySpawnPoint
            //    .Select(x => x.Value)
            //    .SelectMany(x => x)
            //    .Sum(x => x.EntitiesCount);
        }
    }
}