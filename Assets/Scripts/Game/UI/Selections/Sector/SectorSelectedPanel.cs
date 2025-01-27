﻿namespace Tartaros.UI
{
	using Sirenix.OdinInspector;
	using System.Threading;
	using Tartaros.Economy;
	using Tartaros.Map;
	using Tartaros.Selection;
	using Tartaros.ServicesLocator;
	using Tartaros.UI.Sectors.Orders;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.Video;

	public class SectorSelectedPanel : APanel
	{
		#region Fields
		public static readonly SectorUIContent FALLBACK_CONTENT = new SectorUIContent(TartarosTexts.DEFAULT_SECTOR_NAME, TartarosTexts.DEFAULT_SECTOR_DESCRIPTION);

		[Title("UI Styles References")]
		[SerializeField] private Image _icon = null;
		[SerializeField] private Image _background = null;

		[Title("UI Texts References")]
		[SerializeField] private TextMeshProUGUI _name = null;
		[SerializeField] private TextMeshProUGUI _description = null;

		[Title("Buttons References")]
		[SerializeField] private RectTransform _buttonsRoot = null;
		[SerializeField] private CaptureSectorButton _captureButton = null;
		[SerializeField] private SectorOrderButton _orderButton = null;

		private ISector _displaySector = null;

		// SERVICES
		private ISelection _currentSelection = null;
		private UIStyles _uiStyles = null;
		#endregion Fields

		#region Methods
		protected override void Awake()
		{
			base.Awake();

			_currentSelection = Services.Instance.Get<CurrentSelection>();
			_uiStyles = Services.Instance.Get<UIStyles>();
		}

		private void Update()
		{
			if (IsShow == true)
			{
				UpdateContent(); // update the resources amount (bad way, we should use event for eg)
			}
		}

		private void OnEnable()
		{
			_currentSelection.SelectionChanged -= SelectionChanged;
			_currentSelection.SelectionChanged += SelectionChanged;

			_captureButton.LateButtonClicked -= OnAnyButtonClick;
			_captureButton.LateButtonClicked += OnAnyButtonClick;
		}

		private void OnDisable()
		{
			_currentSelection.SelectionChanged -= SelectionChanged;

			_captureButton.LateButtonClicked -= OnAnyButtonClick;
		}

		private void OnAnyButtonClick(object sender, AButtonActionAttacher.LateButtonClickedArgs e)
		{
			UpdateUI();
		}

		private void SelectionChanged(object sender, SelectionChangedArgs e)
		{
			if (_currentSelection.ObjectsCount == 1)
			{
				ISelectable firtSelectable = _currentSelection.Objects[0];

				if (firtSelectable.GameObject.TryGetComponent(out ISector sector))
				{
					_displaySector = sector;
					UpdateUI();
				}
			}
		}

		private void UpdateUI()
		{
			UpdateContent();
			UpdateStyle();

			UpdateOrderButton();
			UpdateCaptureButton();

			_buttonsRoot.gameObject.SetActive(_captureButton.gameObject.activeSelf || _orderButton.gameObject.activeSelf);
		}

		private void UpdateContent()
		{
			var uiContent = _displaySector.GetUIContent();

			if (uiContent != null)
			{
				ApplyContent(uiContent);
			}
			else
			{
				ApplyContent(FALLBACK_CONTENT);
			}
		}

		private void ApplyContent(SectorUIContent content)
		{
			_name.text = content.name;
			_description.text = content.description;

			_name.enabled = content != null;
			_description.enabled = content != null;
		}

		private void UpdateStyle()
		{
			ISectorUIStylizer stylizer = _displaySector.GetUIStylizer();

			if (stylizer != null)
			{
				ApplyStyle(stylizer.SectorStyle);
			}
			else
			{
				ApplyStyle(_uiStyles.SectorStyles.DefaultSector);
			}
		}

		private void ApplyStyle(SectorStyle sectorStyle)
		{
			_icon.enabled = sectorStyle.Icon != null;
			_icon.sprite = sectorStyle.Icon;
			_background.sprite = sectorStyle.Background;

			SetStyleToButton(_captureButton, sectorStyle);
			SetStyleToButton(_orderButton, sectorStyle);
		}

		private void SetStyleToButton(AButtonActionAttacher button, SectorStyle style)
		{
			button.Label.color = style.ButtonTextColor;
			button.Button.spriteState = style.ButtonTransition;
			(button.Button.targetGraphic as Image).sprite = style.ButtonDefault;
		}

		private void UpdateOrderButton()
		{
			if (_displaySector.IsCaptured == true)
			{
				ISectorOrderable sectorOrderable = GetSectorOrderable();

				if (sectorOrderable != null)
				{
					SectorOrder sectorOrder = sectorOrderable.GenerateSectorOrder();

					if (sectorOrder != null)
					{
						_orderButton.gameObject.SetActive(sectorOrder.IsAvailable);
						_orderButton.SectorOrder = sectorOrder;
					}
					else
					{
						_orderButton.gameObject.SetActive(false);
					}
				}
				else
				{
					_orderButton.gameObject.SetActive(false);
				}
			}
			else
			{
				_orderButton.gameObject.SetActive(false);
			}
		}

		private ISectorOrderable GetSectorOrderable()
		{
			ISectorOrderable[] sectorOrderables = _displaySector.FindObjectsInSectorOfType<ISectorOrderable>();

			if (sectorOrderables.Length > 1) throw new System.NotSupportedException("A sector cannot more than one ISectorOrderable.");

			if (sectorOrderables.Length > 0)
			{
				return sectorOrderables[0];
			}
			else
			{
				return null;
			}
		}

		private void UpdateCaptureButton()
		{
			_captureButton.gameObject.SetActive(!_displaySector.IsCaptured);
			_captureButton.Sector = _displaySector;
		}
		#endregion Methods
	}
}
