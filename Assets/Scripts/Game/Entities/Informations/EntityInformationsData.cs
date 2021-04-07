﻿namespace Tartaros.Entities.Informations
{
	using UnityEngine;

	public class EntityInformationsData : IEntityBehaviourData
	{
		#region Fields
		[SerializeField]
		private string _name = "NONE";

		[SerializeField]
		private string _description = "NONE";
		#endregion Fields

		#region Properties
		public string Name => _name;
		public string Description => _description;
		#endregion Properties

		#region Methods
		void IEntityBehaviourData.SpawnRequiredComponents(GameObject entityRoot)
		{ }
		#endregion Methods
	}
}