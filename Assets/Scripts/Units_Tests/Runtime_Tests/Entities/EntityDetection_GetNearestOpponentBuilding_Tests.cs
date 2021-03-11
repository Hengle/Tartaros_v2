﻿namespace Tartaros.Tests
{
	using NUnit.Framework;
	using System.Collections;
	using Tartaros.Entities;
	using Tartaros.Entities.Detection;
	using UnityEngine;
	using UnityEngine.TestTools;

	public class EntityDetection_GetNearestOpponentBuilding_Tests
	{
		#region Fields
		private EntityDetection _playerEntDetection = null;
		private Entity _enemyEntity = null;
		#endregion Fields

		#region Methods
		[UnitySetUp]
		public IEnumerator SetUp()
		{
			LogAssert.ignoreFailingMessages = true;

			SetupHelper.CreateService();
			SetupHelper.CreateEntitiesKDTree();

			yield return null;
		}

		[TearDown]
		public void TearDown()
		{
			LogAssert.ignoreFailingMessages = false;
		}

		[UnityTest]
		public IEnumerator When_OpponentIsBuilding_Should_ReturnTrue()
		{
			yield return SetupPlayerAndEnemy(EntityType.Unit, EntityType.Building);

			Assert.AreEqual(_enemyEntity, _playerEntDetection.GetNearestOpponentBuilding());
		}


		[UnityTest]
		public IEnumerator When_OpponentIsUnit_Should_ReturnNull()
		{
			yield return SetupPlayerAndEnemy(EntityType.Unit, EntityType.Unit);

			Assert.AreEqual(null, _playerEntDetection.GetNearestOpponentBuilding());
		}


		private IEnumerator SetupPlayerAndEnemy(EntityType playerType, EntityType enemyType)
		{
			LogAssert.ignoreFailingMessages = true;

			Entity playerEnt = SetupHelper.CreateEntity(Vector3.zero, Team.Player, playerType, "Player");
			_playerEntDetection = SetupHelper.AddDetectionBehaviour(playerEnt, 5);

			_enemyEntity = SetupHelper.CreateEntity(new Vector3(0, 1, 1), Team.Enemy, enemyType, "Enemy");

			yield return null;
		}
		#endregion Methods
	}
}