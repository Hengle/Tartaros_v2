﻿namespace Tartaros.Selection
{
	using Sirenix.OdinInspector;
	using UnityEngine;

	public class InstantiateGameObjectOnSelection : MonoBehaviour, ISelectionEffect
	{
		#region Fields
		[SerializeField]
		[AssetsOnly]
		[Required]
		private GameObject _prefabToInstantiateOnSelection = null;

		[ShowInRuntime]
		private GameObject _currentPrefab = null;
		#endregion Fields

		#region Methods
		void Start()
		{
			if (_prefabToInstantiateOnSelection == null)
			{
				Debug.LogWarningFormat("Missing projector prefab on {0}.", name);
			}
		}

		void ISelectionEffect.OnSelected()
		{
			InstantiatePrefab();
		}

		void ISelectionEffect.OnUnselected()
		{
			DestroyCurrentPrefab();
		}

		private void InstantiatePrefab()
		{
			if (_currentPrefab != null)
			{
				Debug.LogErrorFormat("Trying to instanciate the prefab while it is already instanciated. We destroy the current prefab and instantiate a new one to avoid error.");
				DestroyCurrentPrefab();
			}

			_currentPrefab = Instantiate(_prefabToInstantiateOnSelection, transform);
		}

		private void DestroyCurrentPrefab()
		{
			if (_currentPrefab != null)
			{
				GameObject.Destroy(_currentPrefab);
			}
		}
		#endregion Methods
	}
}