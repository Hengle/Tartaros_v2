﻿namespace Tartaros.Map
{
	using Sirenix.OdinInspector;
	using System.Collections;
	using Tartaros.Economy;
	using Tartaros.ServicesLocator;
	using UnityEngine;

	[RequireComponent(typeof(SectorObject))]
	public class SpecialSectorIncome : SerializedMonoBehaviour, IIncomeGenerator
	{
		[SerializeField] private SectorRessourceType _ressourceType = SectorRessourceType.Iron;
		[SerializeField] private int _ressourcePerTick = 5;
		[SerializeField] private int _gloryIncomeOnCapture = 0;
		[SerializeField] private bool _autoCapture = false;

		private int _maxRessourceBeforeEmpty = 0;
		private IPlayerIncomeManager _playerIncomeManager = null;
		private PlayerGloryIncomeManager _playerGloryIncomeManager = null;
		private SectorObject _sectorObject = null;

		public int GloryIncomeOnCapture => _gloryIncomeOnCapture;

		SectorRessourceType IIncomeGenerator.SectorRessourceType => _ressourceType;

		int IIncomeGenerator.ResourcesPerTick => _ressourcePerTick;

		int IIncomeGenerator.MaxRessourcesBeforeEmpty => _maxRessourceBeforeEmpty;

		private void Awake()
		{
			_playerIncomeManager = Services.Instance.Get<IPlayerIncomeManager>();
			_playerGloryIncomeManager = FindObjectOfType<PlayerGloryIncomeManager>();
			_sectorObject = GetComponent<SectorObject>();
		}

		private void OnEnable()
		{
			var sector = _sectorObject.CurrentSector;

			sector.Captured -= OnSectorCapture;
			sector.Captured += OnSectorCapture;
		}

		private void Start()
		{
			if(_autoCapture == true)
			{
				var sector = _sectorObject.CurrentSector;

				sector.IsCaptured = true;
			}
		}

		private void OnSectorCapture(object sender, CapturedArgs e)
		{
			AddIncome();
			AddGlory();
		}

		public void AddIncome()
		{
			if (_playerIncomeManager != null)
			{
				_playerIncomeManager.AddGeneratorIncome(this);
			}
		}

		public void RemoveIncome()
		{
			if (_playerIncomeManager != null)
			{
				_playerIncomeManager.RemoveGeneratorIncome(this);
			}
		}

		public void AddGlory()
		{
			if (_playerGloryIncomeManager != null)
			{
				_playerGloryIncomeManager.AddGlory(transform, _gloryIncomeOnCapture);
			}
		}

		void IIncomeGenerator.RessourcesIsEmpty()
		{

		}
	}
}