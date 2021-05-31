﻿namespace Tartaros.Wave
{
	using System.Collections;
	using UnityEngine;
	using Tartaros;
	public partial class SpawnPoint : MonoBehaviour, ISpawnPoint
	{
		#region Fields
		[SerializeField]
		private float _randomRadius = 0;

		[SerializeField]
		private SpawnPointIdentifier _identifier;

		[SerializeField]
		private Vector3[] _waypoints = null;
		#endregion Fields

		#region Properties
		SpawnPointIdentifier ISpawnPoint.Identifier => _identifier;

		Vector3 ISpawnPoint.SpawnPoint => Random.insideUnitCircle.ToXZ() * _randomRadius + transform.position;

		Vector3[] ISpawnPoint.Waypoints => _waypoints;

		
		#endregion Properties
	}

#if UNITY_EDITOR
	public partial class SpawnPoint
	{
		#region Methods
		private void OnDrawGizmos()
		{
			Gizmos.DrawIcon(transform.position, "skull-crossed-bones.png");
			Editor.HandlesHelper.DrawWireCircle(transform.position, Vector3.up, _randomRadius, Color.white);			
			UnityEditor.Handles.Label(transform.position, _identifier.ToString());
		}
		#endregion Methods
	}
#endif
}