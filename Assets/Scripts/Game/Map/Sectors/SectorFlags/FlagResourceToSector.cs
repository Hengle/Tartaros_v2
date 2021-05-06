﻿namespace Tartaros.Map
{
	using Sirenix.OdinInspector;
	using System.Linq;
	using Tartaros.Construction;
	using Tartaros.Economy;
	using Tartaros.Entities;
	using Tartaros.ServicesLocator;
	using Tartaros.UI.MiniMap;
	using UnityEngine;

	[RequireComponent(typeof(SectorObject)), InfoBox("Fill constructable of building slot AUTOMATICALLY")]
	public partial class FlagResourceToSector : MonoBehaviour
	{
		#region Fields
		[SerializeField]
		private SectorRessourceType _type = SectorRessourceType.Food;

		private ResourceMiniMapIcon _miniMapIcon = null;
		#endregion Fields

		#region Properties
		public SectorRessourceType Type
		{
			get => _type;

			set
			{
				_type = value;
				_miniMapIcon.ResourceType = _type;
			}
		}
		#endregion Properties

		#region Methods
		private void Awake()
		{
			_miniMapIcon = gameObject.GetOrAddComponent<ResourceMiniMapIcon>();
			_miniMapIcon.ResourceType = _type;

			CheckIfBuildingSlotIsMissing();
			SetBuildingSlotConstructable();
		}

		private void SetBuildingSlotConstructable()
		{
			IMap map = Services.Instance.Get<IMap>();
			BuildingsDatabase buildingsDatabase = Services.Instance.Get<BuildingsDatabase>();

			ISector sector = map.GetSectorOnPosition(transform.position);
			BuildingSlot buildingsSlot = sector.GetBuildingSlotAvailable();

			buildingsSlot.Constructable = buildingsDatabase.GetResourceBuildingAsConstructable(_type);
		}

		private void CheckIfBuildingSlotIsMissing()
		{
			IMap map = Services.Instance.Get<IMap>();
			ISector sector = map.GetSectorOnPosition(transform.position);

			Debug.Assert(sector != null, "Resource flag must be placed on a sector.", this);

			if (sector != null)
			{
				int slotCount = sector.GetBuildingSlots().Count();
				Debug.Assert(slotCount > 0, "There is no building slot whereas we have a flag resource to sector! Please add one.", this);
			}
		}
		#endregion Methods
	}

#if UNITY_EDITOR
	public partial class FlagResourceToSector
	{
		private void OnDrawGizmos()
		{
			_type.DrawIcon(transform.position);
		}
	}
#endif
}
