﻿namespace Tartaros.UI
{
	using Sirenix.OdinInspector;
	using Tartaros.Entities;
	using Tartaros.Orders;
	using Tartaros.Selection;
	using Tartaros.ServicesLocator;
	using UnityEngine;

	public class OneEntitySelectedPanel : APanel
	{
		#region Fields
		[SerializeField]
		[SuffixLabel("self if null")]
		private UIOrderButtonsGenerator _topButtons = null;

		[SerializeField]
		private RadialHealthSlider _radialHealthSlider = null;

		[SerializeField]
		private EntityInformationsUI _entityInformations = null;

		[SerializeField]
		private EntityAttackStatsUI _attacksStatsUI = null;

		private ISelection _currentSelection = null;
		#endregion Fields

		#region Methods
		protected override void Awake()
		{
			base.Awake();

			_topButtons = GetComponent<UIOrderButtonsGenerator>();
			_currentSelection = Services.Instance.Get<CurrentSelection>();
		}

		private void OnEnable()
		{
			_currentSelection.SelectionChanged -= SelectionChanged;
			_currentSelection.SelectionChanged += SelectionChanged;
		}

		private void OnDisable()
		{
			_currentSelection.SelectionChanged -= SelectionChanged;
		}

		private void SelectionChanged(object sender, SelectionChangedArgs e)
		{
			if (_currentSelection.SelectedSelectables.Length == 1)
			{
				ISelectable firtSelectable = _currentSelection.SelectedSelectables[0];

				if (firtSelectable.GameObject.TryGetComponent(out Entity entity))
				{
					UpdatePanelInformations(entity);
					Show();
				}
			}
			else
			{
				Hide();
			}
		}

		private void UpdatePanelInformations(Entity entity)
		{
			_topButtons.GenerateButtons(entity);
			_radialHealthSlider.Healthable = entity.GetComponent<IHealthable>();
			_entityInformations.Entity = entity;
			_attacksStatsUI.Entity = entity;
		}
		#endregion Methods
	}
}
