﻿namespace Tartaros.Math
{
	using UnityEngine;

	public struct Circle : IShape
	{
		#region Fields
		public Vector2 position;
		public float radius;
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

		public Circle(Vector3 position, float radius) : this(new Vector2(position.x, position.z), radius)
		{
			
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
