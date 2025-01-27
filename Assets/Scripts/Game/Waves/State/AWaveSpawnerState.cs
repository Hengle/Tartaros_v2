﻿namespace Tartaros.Wave
{
    using System.Collections;
    
    using UnityEngine;
    public class AWaveSpawnerState : AState<EnemiesWavesManager>
    {

        public AWaveSpawnerState(EnemiesWavesManager stateOwner) : base(stateOwner)
        {

        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
        }

        public override void OnUpdate()
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}