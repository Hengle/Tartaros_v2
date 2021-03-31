﻿namespace Tartaros.Wave
{
	using System.Collections;
	using UnityEngine;

	public class WaveSpawningState : AWaveSpawnerState
	{
		#region Fields
		[ShowInRuntime]
		private int _pendingSpawnPointsCount = 0;

		private readonly int _waveIndex = 0;
		private readonly WaveData _waveData = null;
		private readonly ISpawnPoint[] _spawnPoints = null;
		private readonly WavesEnemiesStillAliveManager _stillAliveManger = null;
		private readonly WaveSpawnerFSM _waveFSM = null;
		#endregion Fields

		#region Ctor
		public WaveSpawningState(EnemiesWavesManager stateOwner) : base(stateOwner)
		{
			_waveIndex = stateOwner.CurrentWaveIndex;

			try
			{
				_waveData = stateOwner.WaveSpawnerData.Waves[_waveIndex];
			}
			catch (System.IndexOutOfRangeException)
			{
				Debug.LogErrorFormat("Cannot spawn wave {0}. It doesn't exist.", _waveIndex);
			}

			_spawnPoints = stateOwner.SpawnPoints;
			_pendingSpawnPointsCount = _spawnPoints.Length;
			_stillAliveManger = new WavesEnemiesStillAliveManager();
			_waveFSM = stateOwner.WaveFSM;
		}
		#endregion Ctor

		#region Methods
		public override void OnStateEnter()
		{
			base.OnStateEnter();

			_stateOwner.InvokeWaveSpawn();
			SpawnWave(_stateOwner);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			if (CheckIfSpawnIsFinish())
			{
				_stateOwner.InvokeWaveFinished();
				_waveFSM.CurrentState = new WaitDeathOfSpawnedEnemiesWaveState(_stateOwner, _stillAliveManger);
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();

			if (CheckIfSpawnIsFinish() == false)
			{
				Debug.LogErrorFormat("Wave spawning state leaved while spawning is not finished.");
			}
		}

		private void SpawnWave(MonoBehaviour coroutineOwner)
		{
			SpawnPointIdentifier[] pointsUses = _waveData.GetSpawnPointActiveInTheWave();

			foreach (ISpawnPoint spawnPoint in _spawnPoints)
			{
				foreach (SpawnPointIdentifier identifier in pointsUses)
				{
					if (identifier == spawnPoint.Identifier)
					{
						UnitSequence[] unitSequences = _waveData.GetUnitSequences(spawnPoint.Identifier);
						coroutineOwner.StartCoroutine(SpawnPointsSequences(unitSequences, spawnPoint));
					}
				}
			}
		}

		private IEnumerator SpawnPointsSequences(UnitSequence[] unitSequences, ISpawnPoint spawnPoint)
		{
			foreach (UnitSequence sequence in unitSequences)
			{
				yield return new WaitForSeconds(sequence.SecondsBeforeSpawn);
				yield return SpawnUnits(spawnPoint, sequence);
			}

			_pendingSpawnPointsCount--;
		}


		private IEnumerator SpawnUnits(ISpawnPoint spawnPoint, UnitSequence unitSequence)
		{
			if (unitSequence.PrefabToSpawn == null)
			{
				Debug.LogErrorFormat("Missing prefab on a unit sequence of wave {0}.", _waveIndex);
				yield return null;
			}

			for (int i = 0; i < unitSequence.EntitiesCount; i++)
			{
				GameObject spawnedEntity = GameObject.Instantiate(unitSequence.PrefabToSpawn, spawnPoint.SpawnPoint, Quaternion.identity);
				yield return null; // wait for the entity to configure itself (see Entity.SpawnRequiredComponents)

				IWaveSpawnable waveSpawnable = spawnedEntity.GetComponent<IWaveSpawnable>();
				waveSpawnable.Attack(_stateOwner.EnemiesTarget);
				_stillAliveManger.AddEnemyWave(waveSpawnable);


				yield return new WaitForSeconds(unitSequence.SecondsBetweenUnits);
			}
		}

		private bool CheckIfSpawnIsFinish()
		{
			return (_pendingSpawnPointsCount == 0);
		}
		#endregion Methods
	}
}