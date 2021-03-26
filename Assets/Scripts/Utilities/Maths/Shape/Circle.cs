﻿namespace Tartaros.Math
{
	using UnityEngine;

	public class Circle : IShape
	{
		#region Fields
		public Vector2 position = Vector2.zero;
		public float radius = 1;
		#endregion Fields

		#region Properties
		public float X => position.x;
		public float Y => position.y;
		#endregion Properties

		#region Ctor
		public Circle(Vector2 position, float radius)
		{
			this.position = position;
			this.radius = radius;
		}
		#endregion Ctor

		#region Methods
		public bool ContainsPosition(Vector2 worldPosition)
		{
			// TODO TF: (perf) use sqrt
			float dist = Vector2.Distance(position, worldPosition);
			return dist <= radius;
		}
		#endregion Methods
	}
}