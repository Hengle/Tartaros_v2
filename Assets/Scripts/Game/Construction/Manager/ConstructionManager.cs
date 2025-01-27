﻿namespace Tartaros.Construction
{
	using Tartaros.Economy;
	using Tartaros.Gamemode;
	using Tartaros.Gamemode.State;
	using Tartaros.ServicesLocator;
	using UnityEngine;

	public class ConstructionManager : MonoBehaviour
	{
		#region Fields
		[SerializeField]
		private ConstructionManagerData _constructionManagerData = null;
		private GamemodeManager _gamemodeManager = null;
		private IPlayerSectorResources _playerSectorRessources = null;
		#endregion

		#region Properties
		public ConstructionManagerData ConstructionManagerData => _constructionManagerData;
		#endregion Properties

		#region Methods
		private void Awake()
		{
			_gamemodeManager = Services.Instance.Get<GamemodeManager>();
			_playerSectorRessources = Services.Instance.Get<IPlayerSectorResources>();
		}

		public bool CanEnterConstruction(IConstructable constructable)
		{
			return _playerSectorRessources.CanBuy(constructable.Price);
		}

		public void EnterConstructionMode(IConstructable toBuild)
		{
            if (toBuild.IsWall)
            {
				_gamemodeManager.SetState(new WallConstructionState(_gamemodeManager, toBuild));
            }
            else
            {
				_gamemodeManager.SetState(new ConstructionState(_gamemodeManager, toBuild));
            }
		}
		#endregion
	}

}