﻿
namespace Tartaros.Construction
{
	using System.Collections;
	using UnityEngine;
	using Tartaros.Gamemode;
	using Tartaros.Economy;
	using Tartaros.Gamemode.State;
	using Tartaros.ServicesLocator;
	using Tartaros.Sectors;

	public class ConstructionState : AGameState
	{
		#region Fields
		private BuildingPreview _buildingPreview = null;
		private ConstructionInputs _constructionInput = null;
		private IConstructable _constructable = null;
		private IPlayerSectorResources _playerSectorRessources = null;
		private IMap _map = null;
		private UserErrorsLogger _errorsLogger = null;
		#endregion Fields

		#region Ctor
		public ConstructionState(GamemodeManager gamemodeManager, IConstructable constructable) : base(gamemodeManager)
		{
			_constructable = constructable;
			_constructionInput = new ConstructionInputs();
			_buildingPreview = new BuildingPreview(_constructable, _constructionInput.GetMousePosition());
			_playerSectorRessources = Services.Instance.Get<IPlayerSectorResources>();
			_errorsLogger = Services.Instance.Get<UserErrorsLogger>();
			_map = Services.Instance.Get<IMap>();
		}
		#endregion Ctor

		#region Methods
		public override void OnStateEnter()
		{
			base.OnStateEnter();

			_constructionInput.ValidatePerformed -= InputValidatePerformed;
			_constructionInput.ValidatePerformed += InputValidatePerformed;

            _constructionInput.LeavePerformed -= InputLeavePerformed;
            _constructionInput.LeavePerformed += InputLeavePerformed;
		}

        private void InputLeavePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
			LeaveState();

		}

        public override void OnStateExit()
		{
			base.OnStateExit();

			_buildingPreview.DestroyMethod();
			_constructionInput.ValidatePerformed -= InputValidatePerformed;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			_buildingPreview.SetBuildingPreviewPosition(_constructionInput.GetMousePosition());
		}

		private void InputValidatePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			if (CanConstructHere())
			{
				Validate();
			}
		}

		bool CanConstructHere()
		{
			Vector3 buildingPosition = _buildingPreview.GetBuildingPreviewPosition();

			return _map.CanBuild(buildingPosition, _constructable.Size) && DoCanConstructRulesAreValid();
		}

		private bool DoCanConstructRulesAreValid()
		{
			bool rulePass = _constructable.DoRulesPassAtPosition(_buildingPreview.GetBuildingPreviewPosition());

			if (rulePass == false)
			{
				LogFailedConstructRules();
			}

			return rulePass;
		}

		private void LogFailedConstructRules()
		{
			var failedRules = _constructable.GetFailedRules(_buildingPreview.GetBuildingPreviewPosition());

			foreach (var failedRule in failedRules)
			{
				_errorsLogger.Log(failedRule.ErrorMessage);
			}
		}

		private void Validate()
		{
			InstanciateBuilding();
			PayPriceRessources();
			LeaveState();
		}


		private void InstanciateBuilding()
		{
			Debug.Log(_constructable.GameplayPrefab);
			GameObject.Instantiate(_constructable.GameplayPrefab, _buildingPreview.GetBuildingPreviewPosition(), Quaternion.identity);
		}

		private void PayPriceRessources()
		{
			_playerSectorRessources.Buy(_constructable.Price);
		}

		private void LeaveState()
		{
			_stateOwner.SetState(new PlayState(_stateOwner));
		}
		#endregion Methods
	}
}