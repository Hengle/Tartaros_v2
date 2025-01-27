﻿namespace Tartaros.Entities
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Tartaros.Entities;

	public class EntityMovementData : IEntityBehaviourData
	{
		[SerializeField]
		private float _speed = 1;

		[SerializeField]
		private float _separationRange = 1;

		public EntityMovementData(float speed, float separationRange)
		{
			_speed = speed;
			_separationRange = separationRange;
		}

		public float Speed => _speed;
		public float SeparationRange => _separationRange;

#if UNITY_EDITOR
		void IEntityBehaviourData.SpawnRequiredComponents(GameObject entityRoot)
		{
			entityRoot.GetOrAddComponent<EntityMovement>();
		} 
#endif
	}
}