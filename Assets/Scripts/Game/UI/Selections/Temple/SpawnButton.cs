﻿namespace Tartaros.UI
{
	using Tartaros.Entities;
	using Tartaros.UI.HoverPopup;
	using TMPro;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	[RequireComponent(typeof(Button), typeof(OpenHoverPopupOnHover))]
	public class SpawnButton : MonoBehaviour, IPointerClickHandler
	{
		#region Fields
		[SerializeField] private RectTransform _rootInQueue = null;
		[SerializeField] private TextMeshProUGUI _inQueue = null;
		[SerializeField] private Slider _slider = null;
		[SerializeField] private Image _portrait = null;

		private EntityUnitsSpawner _unitsSpawner = null;
		private ISpawnable _toSpawn = null;

		private Button _button = null;
		private OpenHoverPopupOnHover _openHoverPopupOnHover = null;
		#endregion Fields

		#region Methods
		private void Awake()
		{
			_button = GetComponent<Button>();
			_openHoverPopupOnHover = GetComponent<OpenHoverPopupOnHover>();
		}

		private void OnEnable()
		{
			_button.onClick.RemoveListener(OnButtonClick);
			_button.onClick.AddListener(OnButtonClick);
		}

		private void OnDisable()
		{
			_button.onClick.RemoveListener(OnButtonClick);
		}

		private void Update()
		{
			if (_unitsSpawner != null && _toSpawn != null)
			{
				UpdateCountSpawnableRoot();
				UpdateSlider();
			}
		}

		public void Construct(EntityUnitsSpawner unitsSpawner, ISpawnable toSpawn)
		{
			_unitsSpawner = unitsSpawner;
			_toSpawn = toSpawn;
			_portrait.sprite = toSpawn.Portrait;
			_openHoverPopupOnHover.ToShowData = toSpawn.HoverPopupData;
		}

		private void UpdateCountSpawnableRoot()
		{
			int count = _unitsSpawner.GetCountSpawnablesInQueue(_toSpawn);

			_inQueue.text = count.ToString();
			_rootInQueue.gameObject.SetActive(count > 0);
		}

		private void UpdateSlider()
		{
			if (_unitsSpawner.CurrentPrefabSpawning == _toSpawn)
			{
				_slider.value = _unitsSpawner.CurrentProgression;
			}
			else
			{
				_slider.value = 1;
			}
		}

		private void OnButtonClick()
		{
			_unitsSpawner.EnqueueSpawn(_toSpawn);
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right)
			{
				_unitsSpawner.CancelSpawn(_toSpawn);
			}
		}
		#endregion Methods
	}
}
