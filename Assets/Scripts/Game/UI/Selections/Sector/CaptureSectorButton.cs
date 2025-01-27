﻿namespace Tartaros.UI
{
	using Tartaros.Map;
	using Tartaros.ServicesLocator;
	using UnityEngine.UI;

	public class CaptureSectorButton : AButtonActionAttacher
	{
		#region Fields
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
		private void Awake()
		{
			_sectorsCaptureManager = Services.Instance.Get<ISectorsCaptureManager>();
		}

		protected override void OnButtonClick()
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
			Button.interactable = _sector != null && _sector.IsCaptured == false;
		}

		private void UpdateButtonText()
		{
			if (_sector.IsCaptured)
			{
				Label.text = TartarosTexts.SECTOR_CAPTURED;
			}
			else
			{
				string price = _sector.CapturePrice.ToRichTextString();
				Label.text = string.Format("{1} ({0})", price, TartarosTexts.CAPTURE_SECTOR);
			}
		}
		#endregion Methods
	}
}
