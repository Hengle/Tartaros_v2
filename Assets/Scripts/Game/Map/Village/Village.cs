﻿namespace Tartaros.Map.Village
{
	using Tartaros.Entities;
	using Tartaros.Population;
	using Tartaros.Map;
	using Tartaros.ServicesLocator;
	using UnityEngine;
	using System;
	using Tartaros.Dialogue;

	public class Village : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private VillageData _data = null;

		[SerializeField] private bool _ENABLE_DIALOGUE_STATE_EDITOR = false;

		private IMap _map = null;
		private ISector _sector = null;
		private IPopulationManager _populationManager = null;
		private DialogueManager _dialogueManager = null;
		#endregion Fields

		#region Properties
		private int PopulationToIncrease => _data.PopulationAmount;
		public VillageData Data { get => _data; set => _data = value; }
		#endregion Properties



		public class VillageCapturedArgs : EventArgs
		{
		}
		public event EventHandler<VillageCapturedArgs> VillageCaptured;


		#region Methods		

		private void Awake()
		{
			_map = Services.Instance.Get<IMap>();
			_populationManager = Services.Instance.Get<IPopulationManager>();
			_data = GetComponent<Entity>().GetBehaviourData<VillageData>();
			_dialogueManager = GameObject.FindObjectOfType<DialogueManager>();
		}

		private void Start()
		{
			_sector = _map.GetSectorOnPosition(transform.position);

			_sector.Captured -= OnCaptureSector;
			_sector.Captured += OnCaptureSector;

			//Debug.Log(PopulationToIncrease);

			UpdateAbilityToSpawnUnits();
		}

		private void OnDisable()
		{
			_sector.Captured -= OnCaptureSector;
		}

		private void OnCaptureSector(object sender, CapturedArgs e)
		{
			VillageCaptured?.Invoke(this, new VillageCapturedArgs());
			
			_populationManager.IncrementMaxPopulation(PopulationToIncrease);
			UpdateAbilityToSpawnUnits();

			if(_dialogueManager != null && _ENABLE_DIALOGUE_STATE_EDITOR == true)
			{
				_dialogueManager.EnterDialogueState();
			}
		}

		private void UpdateAbilityToSpawnUnits()
		{
			if (TryGetComponent(out EntityUnitsSpawner entityUnitsSpawner))
			{
				entityUnitsSpawner.enabled = _sector.IsCaptured;
			}
			else
			{
				Debug.LogErrorFormat("Missing entity spawner on village {0}. The village will not be able to spawn units.");
			}
		}
		#endregion Methods
	}
}