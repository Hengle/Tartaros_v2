﻿namespace Tartaros.Entities
{
	using System.Collections;
	using Tartaros.Map;
	using UnityEngine;
	using UnityEngine.AI;

	public class MoveToDestination : AGoalEntity
	{
		private Vector3 _targetDestination = Vector3.zero;
		private EntityMovement _entityMovement = null;
		private bool _completed = false;
		private NavMeshPath _path = null;
		//private bool _isActive = false;
		

		public MoveToDestination(Entity goalOwner, Vector3 targetDestination, NavMeshPath path) : base(goalOwner)
		{
			_targetDestination = targetDestination;
			_entityMovement = _goalOwner.GetComponent<EntityMovement>();
			_path = path;
		}

		public override void OnEnter()
		{
			base.OnEnter();

			_entityMovement.StopMovement();
			_entityMovement.MoveToPoint(_targetDestination);

			_entityMovement.DestinationReached -= HasReachDestination;
			_entityMovement.DestinationReached += HasReachDestination;
		}

		public override void OnExit()
		{
			base.OnExit();

			_entityMovement.StopMovement();
			_entityMovement.DestinationReached -= HasReachDestination;
		}

		public override bool IsCompleted()
		{
			return _completed;
		}

		public override void OnUpdate()
		{
			
		}

		private void HasReachDestination(object sender, EntityMovement.DestinationReachedArgs e)
		{
			_completed = true;
		}

	}
}