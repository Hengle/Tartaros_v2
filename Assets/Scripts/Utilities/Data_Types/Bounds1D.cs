﻿namespace Tartaros
{
	using UnityEngine;

	[System.Serializable]
	public struct Bounds1D
	{
		#region Fields
		[SerializeField]
		public float min;

		[SerializeField]
		public float max;
		#endregion Fields

		#region Properties
		public float Center => (max + min) / 2;
		public float Size => (max - min);
		#endregion Properties

		#region Ctor
		public Bounds1D(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
		#endregion Ctor
	}
}