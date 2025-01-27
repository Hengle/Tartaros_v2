﻿namespace Tartaros.Entities
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.AI;

	public class VillagerSpawnerManager : MonoBehaviour
	{
		private const float THRESHOLD_DISTANCE = 1;

		[SerializeField]
		private Transform[] _spawnPoints = null;

		[SerializeField]
		private GameObject _villagerMalePrefab = null;

		[SerializeField]
		private GameObject _villagetFemalePrefab = null;
		private Transform _templePosition = null;

		private EntityUnitsSpawner _spawner = null;
		private Vector3 _targetPosition = Vector3.zero;
		private List<GameObject> _villagers = new List<GameObject>();
		//private GameObject _particleSystem = null;

		private void Start()
		{
			_spawner = GetComponent<EntityUnitsSpawner>();
			_templePosition = GetComponent<Transform>();
			_targetPosition = GetTargetPosition();

			if(_villagetFemalePrefab == null || _villagerMalePrefab == null)
			{
				Debug.LogError("there is no villager Prefab");
			}
		}

		private void Update()
		{
			if(_villagers.Count != 0)
			{
				AsReachDestination();
			}
		}

		public void SpawnFuturHoplite()
		{
			if(_spawnPoints.Length <= 0)
			{
				Debug.LogError("there is no spawnPoint to spawn villager");
				return;
			}

			SpawnVillager(_villagerMalePrefab);
		}

		public void SpawnFuturArcher()
		{
			if (_spawnPoints.Length <= 0)
			{
				Debug.LogError("there is no spawnPoint to spawn villager");
				return;
			}

			SpawnVillager(_villagetFemalePrefab);
		}

		private void SpawnVillager(GameObject prefab)
		{
			Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length - 1)];
			GameObject villager = GameObject.Instantiate(prefab, spawnPoint.position, Quaternion.identity);
			_villagers.Add(villager);
			SetDestinationToVillager(villager);
		}

		private void SetDestinationToVillager(GameObject villager)
		{
			villager.GetComponent<NavMeshAgent>().SetDestination(_targetPosition);
		}

		private Vector3 GetTargetPosition()
		{
			var templeLenght = _templePosition.GetComponent<NavMeshObstacle>().size.z;
			var position = _templePosition.position + Vector3.back * (templeLenght / 2 + 1f);

			if (NavMeshHelper.IsPositionOnNavMesh(position))
			{
				return position;
			}
			else
			{
				return NavMeshHelper.AdjustPositionToFitNavMesh(position);
			}
		}

		public void SetSpawnPoint(Transform[] spawnPoints) 
		{
			if(spawnPoints != null)
			{
				_spawnPoints = spawnPoints;
			}
		}

		private void AsReachDestination()
		{
			foreach (var villager in _villagers)
			{
				Debug.Log(villager);

				if (villager != null)
				{
					var distanceFromTargetPoint = Vector3.Distance(villager.transform.position, _targetPosition);

					Debug.Log(distanceFromTargetPoint);

					if (distanceFromTargetPoint <= 3)
					{
						Destroy(villager);
					}
				}
			}
		}
	}
}