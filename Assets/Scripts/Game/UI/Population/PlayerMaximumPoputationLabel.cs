﻿namespace Tartaros.UI
{
	using Sirenix.OdinInspector;
	using Tartaros.Population;
	using Tartaros.ServicesLocator;
	using TMPro;
	using UnityEngine;

	public class PlayerMaximumPoputationLabel : MonoBehaviour
	{
		#region Fields
		[SerializeField]
		[SuffixLabel("self if null")]
		private TextMeshProUGUI _maximumPopulationLabel = null;

		private IPopulationManager _populationManager = null;
		#endregion Fields

		#region Properties
		public TextMeshProUGUI MaximumPopulationLabel { get => _maximumPopulationLabel; set => _maximumPopulationLabel = value; }
		#endregion Properties

		#region Methods
		private void Awake()
		{
			if (_maximumPopulationLabel == null)
			{
				_maximumPopulationLabel = GetComponent<TextMeshProUGUI>();
			}

			_populationManager = Services.Instance.Get<IPopulationManager>();
		}

		private void Start()
		{
			UpdateLabel();
		}

		private void OnEnable()
		{
			_populationManager.MaxPopulationChanged -= MaxPopulationChanged;
			_populationManager.MaxPopulationChanged += MaxPopulationChanged;
		}

		private void OnDisable()
		{
			_populationManager.MaxPopulationChanged -= MaxPopulationChanged;
		}

		private void MaxPopulationChanged(object sender, MaxPopulationChangedArgs e)
		{
			UpdateLabel();
		}

		private void UpdateLabel()
		{
			_maximumPopulationLabel.text = _populationManager.MaximumPopulation.ToString();
		}
		#endregion Methods
	}
}
