﻿namespace Tartaros.Gamemode.State
{
	using System.Collections;
	using System.Collections.Generic;
	using Tartaros.Construction;
	using Tartaros.Economy;
	using Tartaros.Map;
	using Tartaros.Selection;
	using Tartaros.ServicesLocator;
	using UnityEngine;

	public class WallConstructionState : AGameState
	{
		private BuildingPreview _buildingPreview = null;
		private ISectorResourcesWallet _pricePreview = SectorResourcesWallet.Zero;
		private List<GameObject> _wallSections = new List<GameObject>();
		private WallBuildingPreview _wallSectionPreview = null;
		private GameObject _wallToHideAndShow = null;
		private int _indexStartModel = 1;

		private readonly IConstructable _constructable = null;
		private readonly ConstructionInputs _inputs = null;
		private readonly IPlayerSectorResources _playerSectorRessources = null;
		private readonly IMap _map = null;
		private readonly List<GameObject> _wallCorners = new List<GameObject>();

		public WallConstructionState(GamemodeManager gamemodeManager, IConstructable constructable) : base(gamemodeManager)
		{
			_constructable = constructable;
			_inputs = new ConstructionInputs();
			_playerSectorRessources = Services.Instance.Get<IPlayerSectorResources>();
			_map = Services.Instance.Get<IMap>();
			_buildingPreview = new BuildingPreview(_constructable, _inputs.GetMousePosition());
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();

			_inputs.ValidatePerformed -= InputsValidatePerformed;
			_inputs.ValidatePerformed += InputsValidatePerformed;

			_inputs.LeavePerformed -= InputsLeavePerformed;
			_inputs.LeavePerformed += InputsLeavePerformed;

			SetActiveSelectionInputRect(false);
		}

		private void InputsLeavePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			LeaveState();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			SetWallSectionPreview();
			SetFirstBuildingPreview();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();

			_inputs.ValidatePerformed -= InputsValidatePerformed;
			_inputs.LeavePerformed -= InputsLeavePerformed;

			DestroyPreviews();

			if (_buildingPreview != null)
			{
				_buildingPreview.DestroyMethod();
			}

			if (_wallSectionPreview != null)
			{
				_wallSectionPreview.DestroyMethod();
			}

			if (_wallToHideAndShow != null)
			{
				_wallToHideAndShow.SetActive(true);
			}

			SetActiveSelectionInputRect(true);
		}

		private void InputsValidatePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			if (CanConstructHere())
			{
				bool isInPreviewMode = _wallSectionPreview != null;

				if (isInPreviewMode)
				{
					if (_inputs.IsAddNewWallSectionsPerformed() == true)
					{
						ContinueWallPreview();
					}
					else
					{
						ValidateFinish();
					}
				}
				else
				{
					ValidateFirstPreview();
				}
			}
		}

		private void ContinueWallPreview()
		{
			AddWallPreviewOnList();
			_pricePreview = GetTotalPriceOfConstruction();
			Vector3 lastPosition = _wallSectionPreview.GetAllCornerPreview()[1].transform.position;
			_wallSectionPreview = null;
			_wallSectionPreview = new WallBuildingPreview(_constructable, lastPosition, _constructable.PreviewPrefab);
		}

		private void SetFirstBuildingPreview()
		{
			if (_buildingPreview != null)
			{
				_buildingPreview.SetBuildingPreviewPosition(_inputs.GetMousePosition());
			}
		}

		private void SetWallSectionPreview()
		{
			if (_wallSectionPreview != null)
			{
				_wallSectionPreview.Update(_inputs.GetMousePosition());
				ShowPriceTotal();
			}
		}

		private void ValidateFirstPreview()
		{
			GameObject previewStart = _constructable.PreviewPrefab;

			if (_buildingPreview.GetNeigboorManager() != null)
			{
				_wallToHideAndShow = _buildingPreview.GetObjectUnderCursor();
				_wallToHideAndShow.SetActive(false);
				_wallToHideAndShow.name = Random.Range(0, 10000).ToString();

				previewStart = SelectPreviewStartPrefab();
			}
			_wallSectionPreview = new WallBuildingPreview(_constructable, _buildingPreview.GetBuildingPreviewPosition(), previewStart);
			_buildingPreview.DestroyMethod();
			_buildingPreview = null;
		}



		private GameObject SelectPreviewStartPrefab()
		{
			var neigboorManager = _buildingPreview.GetNeigboorManager();
			GameObject previewStart = null;

			if (neigboorManager.GetNumberOfNeigboor() == 1)
			{
				_indexStartModel = 1;
				previewStart = _constructable.WallLModel;
			}
			else if (neigboorManager.GetNumberOfNeigboor() == 2)
			{
				_indexStartModel = 2;
				previewStart = _constructable.WallTModel;
			}
			else if (neigboorManager.GetNumberOfNeigboor() == 3)
			{
				_indexStartModel = 3;
				previewStart = _constructable.WallXModel;
			}

			return previewStart;
		}

		private GameObject SelectGameplayStartPrefab()
		{
			GameObject gameplayPrefab = null;

			if (_indexStartModel == 1)
			{
				gameplayPrefab = _constructable.WallLGameplay;
			}
			else if (_indexStartModel == 2)
			{
				gameplayPrefab = _constructable.WallTGameplay;
			}
			else if (_indexStartModel == 3)
			{
				gameplayPrefab = _constructable.WallXGameplay;
			}

			return gameplayPrefab;
		}
		private void ValidateFinish()
		{
			AddWallPreviewOnList();
			PayPriceRessources();
			_stateOwner.StartCoroutine(InstanciateWallSection());
			
		}

		private void AddWallPreviewOnList()
		{
			if (_wallSectionPreview.GetAllSectionPreview() != null)
			{
				foreach (GameObject wallSection in _wallSectionPreview.GetAllSectionPreview())
				{
					_wallSections.Add(wallSection);

				}

				foreach (GameObject wallCorner in _wallSectionPreview.GetAllCornerPreview())
				{
					_wallCorners.Add(wallCorner);
				}
			}
		}

		private IEnumerator InstanciateWallSection()
		{
			GameObject gameplayStartPrefab = _constructable.GameplayPrefab;


			if (_wallToHideAndShow != null)
			{
				gameplayStartPrefab = SelectGameplayStartPrefab();
				_wallToHideAndShow.SetActive(false);
				GameObject.Destroy(_wallToHideAndShow);
			}

			yield return new WaitForEndOfFrame();


			Transform transformStart = _wallCorners[0].transform;
			GameObject.Destroy(_wallCorners[0]);
			GameObject wallStartInstanciate = GameObject.Instantiate(gameplayStartPrefab, transformStart.position, transformStart.rotation);

			Transform transformEnd = _wallCorners[1].transform;
			GameObject.Destroy(_wallCorners[1]);
			GameObject wallEndInstanciate = GameObject.Instantiate(_constructable.GameplayPrefab, transformEnd.position, transformEnd.rotation);

			foreach (GameObject wall in _wallSections)
			{
				Transform transform = wall.transform;
				GameObject.Destroy(wall);
				GameObject wallInstanciate = GameObject.Instantiate(_constructable.GameplayPrefab, transform.position, transform.rotation);
			}

			LeaveState();
		}

		private bool CanConstructHere()
		{
			//return true;

			return DoCanConstructOnMap() && DoCanConstructRulesAreValid();
		}

		private bool DoCanConstructOnMap()
		{
			bool isNotInPreviewMode = _wallSectionPreview == null;

			if (isNotInPreviewMode)
			{
				return _map.CanBuild(_buildingPreview.GetBuildingPreviewPosition(), _constructable.Size);
			}
			else
			{
				foreach (GameObject wallPreview in _wallSectionPreview.GetWallBuildingPreview())
				{
					if (_map.CanBuild(wallPreview.transform.position, _constructable.Size) == false)
					{
						return false;
					}
				}
				return true;
			}
		}

		private bool DoCanConstructRulesAreValid()
		{
			bool isNotInPreviewMode = _wallSectionPreview == null;

			if (isNotInPreviewMode)
			{
				return _constructable.DoRulesPassAtPosition(_buildingPreview.GetBuildingPreviewPosition());
			}
			else
			{
				foreach (GameObject wallPreview in _wallSectionPreview.GetWallBuildingPreview())
				{
					if (_constructable.DoRulesPassAtPosition(wallPreview.transform.position) == false)
					{
						return false;
					}
				}
				return true;
			}
		}

		private ISectorResourcesWallet GetTotalPriceOfConstruction()
		{
			ISectorResourcesWallet totalPrice = SectorResourcesWallet.Zero;

			// TODO: totalPrice.AddWallet(_constructable.Price * _wallSectionPreview.GetWallBuildingPreview().Count);
			foreach (GameObject wallPreview in _wallSectionPreview.GetWallBuildingPreview())
			{
				totalPrice.AddWallet(_constructable.Price);
			}

			totalPrice.AddWallet(_pricePreview);
			return totalPrice;
		}

		private void ShowPriceTotal()
		{
			//TODO DJ: Ref l'UI Z
			//Debug.LogFormat("{0}", GetTotalPriceOfConstruction().ToString());
		}

		private void PayPriceRessources()
		{
			_playerSectorRessources.Buy(GetTotalPriceOfConstruction());
		}

		private void DestroyPreviews()
		{
			foreach (var wallSection in _wallSections)
			{
				GameObject.Destroy(wallSection);
			}

			foreach (var wallCorner in _wallCorners)
			{
				GameObject.Destroy(wallCorner);
			}
		}


		private static void SetActiveSelectionInputRect(bool enable)
		{
			var rectangleSelectionInput = GameObject.FindObjectOfType<RectangleSelectionInput>();

			if (rectangleSelectionInput != null)
			{
				rectangleSelectionInput.enabled = enable;
			}
		}
	}
}