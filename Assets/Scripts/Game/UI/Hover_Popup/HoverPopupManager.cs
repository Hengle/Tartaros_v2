﻿namespace Tartaros.UI.HoverPopup
{
	using Sirenix.OdinInspector;
	using TMPro;
	using UnityEngine;

	public class HoverPopupManager : MonoBehaviour
	{
		public enum HoverPopupPosition
		{
			Top,
			Bottom
		}


		#region Fields
		[Title("Settings")]
		[SerializeField] private float _heightOffset = 30;
		[SerializeField] private HoverPopupPosition _popupPosition = HoverPopupPosition.Top;

		[Title("References")]
		[SerializeField] private RectTransform _root = null;
		[SerializeField] private TextMeshProUGUI _name = null;
		[SerializeField] private TextMeshProUGUI _description = null;
		[SerializeField] private TextMeshProUGUI _costLabel = null;
		[SerializeField] private TextMeshProUGUI _cooldownLabel = null;
		[SerializeField] private TextMeshProUGUI _loreDescription = null;
		[SerializeField] private TextMeshProUGUI _hotkey = null;

		private HoverPopupData _displayedData = null;
		#endregion Fields

		#region Properties
		public HoverPopupData HoverPopupData => _displayedData;

		public HoverPopupPosition PopupPosition { get => _popupPosition; set => _popupPosition = value; }
		#endregion Properties

		#region Methods
		private void Awake()
		{
			Hide();
		}

		public void Show(HoverPopupData toDisplayData, RectTransform hovered)
		{
			if (hovered is null) throw new System.ArgumentNullException(nameof(hovered));
			if (toDisplayData is null) throw new System.ArgumentNullException(nameof(toDisplayData));

			_displayedData = toDisplayData;

			_root.gameObject.SetActive(true);
			SetRootPosition(hovered);

			UpdateName();
			UpdateDescription();
			UpdateCost();
			UpdateCooldown();
			UpdateLoreDescription();
			UpdateHotkey();
		}

		private void SetRootPosition(RectTransform hovered)
		{
			_root.CenterAnchor();

			_root.position = hovered.position;

			switch (_popupPosition)
			{
				case HoverPopupPosition.Top:
					_root.anchoredPosition += new Vector2(0, _heightOffset + hovered.rect.height / 2);
					break;

				case HoverPopupPosition.Bottom:
					_root.anchoredPosition -= new Vector2(0, _heightOffset + hovered.rect.height / 2);
					break;

				default:
					throw new System.NotSupportedException();
			}
		}

		private void UpdateHotkey()
		{
			bool hasHotkey = _displayedData.HasHotkey;

			_hotkey.gameObject.SetActive(hasHotkey);

			if (hasHotkey == true)
			{
				_hotkey.text = _displayedData.Hotkey;
			}
		}

		private void UpdateCooldown()
		{
			_cooldownLabel.gameObject.SetActive(_displayedData.HasCooldown);
			_cooldownLabel.text = _displayedData.CooldownFormated;
		}

		private void UpdateCost()
		{
			bool hasCost = _displayedData.HasCost;
			_costLabel.gameObject.SetActive(hasCost);

			if (hasCost == true)
			{
				_costLabel.text = _displayedData.Cost;
			}
		}

		private void UpdateDescription()
		{
			_description.gameObject.SetActive(_displayedData.HasDescription);
			_description.text = _displayedData.Description;
		}

		private void UpdateName()
		{
			_name.gameObject.SetActive(_displayedData.HasName);
			_name.text = _displayedData.Name;
		}

		private void UpdateLoreDescription()
		{
			_loreDescription.gameObject.SetActive(_displayedData.HasLoreDescription);
			_loreDescription.text = _displayedData.LoreDescription;
		}

		public void Hide()
		{
			_root.gameObject.SetActive(false);
			_displayedData = null;
		}
		#endregion Methods
	}
}