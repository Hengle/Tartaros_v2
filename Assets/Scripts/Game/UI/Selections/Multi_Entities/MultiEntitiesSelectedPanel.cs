﻿namespace Tartaros.UI
{
	using Tartaros.Entities;
	using Tartaros.Selection;
	using Tartaros.ServicesLocator;
	using UnityEngine;

	public class MultiEntitiesSelectedPanel : APanel
	{
		#region Fields
		[SerializeField]
		private RectTransform _portraitsRoot = null;

		[SerializeField]
		private GameObject _prefabPortrait = null;

		private ISelection _currentSelection = null;
		#endregion Fields

		#region Methods
		private void Start()
		{
			_currentSelection = Services.Instance.Get<CurrentSelection>();

			_currentSelection.SelectionChanged -= SelectionChanged;
			_currentSelection.SelectionChanged += SelectionChanged;
		}

		private void OnEnable()
		{
			if (_currentSelection != null)
			{
				_currentSelection.SelectionChanged -= SelectionChanged;
				_currentSelection.SelectionChanged += SelectionChanged;
			}
		}

		private void OnDisable()
		{
			_currentSelection.SelectionChanged -= SelectionChanged;
		}

		private void SelectionChanged(object sender, SelectionChangedArgs e)
		{
			if (_currentSelection.SelectedSelectables.Length > 1)
			{
				UpdateInformations(_currentSelection.SelectedSelectables);
				Show();
			}
			else
			{
				Hide();
			}
		}

		void UpdateInformations(ISelectable[] selectables)
		{
			_portraitsRoot.DestroyChildren();

			foreach (ISelectable selected in selectables)
			{
				if (selected.GameObject.TryGetComponent(out Entity entity))
				{
					GameObject portrait = Instantiate(_prefabPortrait);
					portrait.transform.SetParent(_portraitsRoot);
					portrait.transform.localScale = Vector3.one;
					portrait.GetComponent<MultiSelectedEntityPortrait>().Entity = entity;
				}
				else
				{
					Debug.LogWarningFormat("Skipping to display entity {0}'s portrait because it is not an Entity.", selected.ToString());
				}
			}
		}
		#endregion Methods
	}
}