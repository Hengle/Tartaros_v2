﻿namespace Tartaros.UI
{
	using Sirenix.OdinInspector;
	using Tartaros.Economy;
	using Tartaros.Map;
	using Tartaros.ServicesLocator;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public class CaptureSectorButton : MonoBehaviour
	{
		#region Fields
		[SerializeField]
		[Required]
		private Button _button = null;

		[SerializeField]
		[Required]
		private TextMeshProUGUI _text = null;

		private ISector _sector = null;
		private ISectorsCaptureManager _sectorsCaptureManager = null;
		#endregion Fields

		#region Properties
		public ISector Sector
		{
			get => _sector;

			set
			{
				if (_sector != null)
				{
					_sector.Captured -= SectorCaptured;
				}

				_sector = value;

				if (_sector != null)
				{
					_sector.Captured -= SectorCaptured;
					_sector.Captured += SectorCaptured;
				}

				UpdateButtonInteractability();
				UpdateButtonText();
			}
		}
		#endregion Properties

		#region Methods
		private void Start()
		{
			_sectorsCaptureManager = Services.Instance.Get<ISectorsCaptureManager>();
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

		private void OnButtonClick()
		{
			if (_sectorsCaptureManager.CanCapture(_sector) == true)
			{
				_sectorsCaptureManager.Capture(_sector);
			}
		}

		private void SectorCaptured(object sender, CapturedArgs e)
		{
			UpdateButtonInteractability();
			UpdateButtonText();
		}

		private void UpdateButtonInteractability()
		{
			_button.interactable = _sector != null && _sector.IsCaptured == false;
		}

		private void UpdateButtonText()
		{
			if (_sector.IsCaptured)
			{
				_text.text = TartarosTexts.SECTOR_CAPTURED;
			}
			else
			{
				string price = _sector.CapturePrice.ToRichTextString();
				_text.text = string.Format("{1} ({0})", price, TartarosTexts.CAPTURE_SECTOR);
			}
		}
		#endregion Methods
	}
}