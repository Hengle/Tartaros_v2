﻿namespace Tartaros.FogOfWar
{
	using System.Collections.Generic;
	using System.Linq;
	using Tartaros.Math;
	using UnityEngine;

	public class FogOfWarManager : MonoBehaviour
	{
		#region Fields
		[ShowInRuntime] private List<IFogVision> _visions = new List<IFogVision>();
		[ShowInRuntime] private List<IFogCoverable> _coverables = new List<IFogCoverable>();

		private FOWCalculator _fowCalculator = new FOWCalculator();
		#endregion Fields

		#region Methods
		private void Update()
		{
			_fowCalculator.Update(_visions, _coverables);
		}

		private void LateUpdate()
		{
			_fowCalculator.LateUpdate(_coverables);
		}

		private void OnDisable()
		{
			UncoverAllCoverables();
		}

		private void UncoverAllCoverables()
		{
			foreach (var coverable in _coverables)
			{
				coverable.IsCovered = false;
			}
		}

		public void AddVision(IFogVision vision)
		{
			if (_visions.Contains(vision) == true)
			{
				Debug.LogErrorFormat("Cannot add fog vision {0}. It is already in visions list.", vision.ToString());
				return;
			}

			_visions.Add(vision);
		}

		public void RemoveVision(IFogVision vision)
		{
			if (_visions.Contains(vision) == false)
			{
				Debug.LogErrorFormat("Cannot remove fog vision {0}. It is not in visions list.", vision.ToString());
				return;
			}

			_visions.Remove(vision);
		}

		public void AddCoverable(IFogCoverable coverable)
		{
			if (_coverables.Contains(coverable) == true)
			{
				Debug.LogErrorFormat("Cannot add fog coverable {0}. It is already in coverables list.", coverable.ToString());
				return;
			}

			_coverables.Add(coverable);
		}

		public void RemoveCoverable(IFogCoverable coverable)
		{
			if (_coverables.Contains(coverable) == false)
			{
				Debug.LogErrorFormat("Cannot remove fog coverable {0}. It is not in coverables list.", coverable.ToString());
				return;
			}

			_coverables.Remove(coverable);
		}
		#endregion Methods
	}
}
