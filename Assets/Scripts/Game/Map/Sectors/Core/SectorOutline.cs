﻿namespace Tartaros.Map
{
	using Sirenix.OdinInspector;
	using System.Collections.Generic;
	using System.Linq;
	using Tartaros.Map;
	using Tartaros.Selection;
	using UnityEngine;

	public class SectorOutline : MonoBehaviour, ISelectionEffect
	{
		#region Fields
		private const string EDITOR_GROUP_COLORS = "Colors";

		[SerializeField]
		private SectorOutlineData _data = null;

		[SerializeField]
		private float _defaultOutlineHeightOffset = 0.1f;

		[SerializeField]
		private float _capturedOutlineHeightOffset = 0.3f;

		[SerializeField]
		private float _selectedOutlineHeightOffset = 0.5f;

		[SerializeField]
		private LineRenderer _lineRenderer = null;

		[SerializeField]
		private Sector _sector = null;

		[FoldoutGroup(EDITOR_GROUP_COLORS)]
		[SerializeField]
		private Color _selectedOutlineColor = Color.magenta;

		[FoldoutGroup(EDITOR_GROUP_COLORS)]
		[SerializeField]
		private Color _capturedOutlineColor = Color.red;

		[FoldoutGroup(EDITOR_GROUP_COLORS)]
		[SerializeField]
		private Color _uncapturedOutlineColor = Color.grey;

		private bool _selected = false;
		#endregion Fields

		#region Methods		
		private void Start()
		{
			if (_data != null)
			{
				_capturedOutlineColor = _data.CapturedSectorsColor;
				_uncapturedOutlineColor = _data.UnCapturedSectorsColor;
				_selectedOutlineColor = _data.CapturedSectorSelectedColor;
			}
			else
			{
				Debug.LogWarningFormat("There is no SectorOutlineData in {0}", this);
			}

			SetupLinePoints();
			SetUnselectedColor();
		}

		private void OnEnable()
		{
			_sector.Captured -= OnSectorCaptured;
			_sector.Captured += OnSectorCaptured;
		}

		private void OnSectorCaptured(object sender, CapturedArgs e)
		{
			UpdateColor();
		}

		private void UpdateColor()
		{
			if (_selected == true)
			{
				SetSelectedColor();
			}
			else
			{
				SetUnselectedColor();
			}
		}

		private void SetLineRendererHeight(float height)
		{
			for (int i = 0; i < _lineRenderer.positionCount; i++)
			{
				var linePos = _lineRenderer.GetPosition(i);
				var newLinePos = new Vector3(linePos.x, height, linePos.z);

				_lineRenderer.SetPosition(i, newLinePos);
			}
		}

		private void SetSelectedColor()
		{
			_lineRenderer.SetColor(_selectedOutlineColor);
			SetLineRendererHeight(_selectedOutlineHeightOffset);
		}

		private void SetUnselectedColor()
		{
			if (_sector.IsCaptured == true)
			{
				_lineRenderer.SetColor(_capturedOutlineColor);
				SetLineRendererHeight(_capturedOutlineHeightOffset);
			}
			else
			{
				_lineRenderer.SetColor(_uncapturedOutlineColor);
				SetLineRendererHeight(_defaultOutlineHeightOffset);
			}
		}

		private void SetupLinePoints()
		{
			Vector3[] positions = _sector.GetPointsWrappedSnappedToGround()
				.Select(x => x + Vector3.up * _defaultOutlineHeightOffset)
				.ToArray();

			_lineRenderer.positionCount = positions.Length;
			_lineRenderer.SetPositions(positions);

			_lineRenderer.startWidth = 0.5f;
		}

		void ISelectionEffect.OnSelected()
		{
			SetSelectedColor();

			// TODO TF: (refactor) it's strange to have boolean; we should get the selectable of ISelectionEffect
			_selected = true;
		}

		void ISelectionEffect.OnUnselected()
		{
			SetUnselectedColor();
			_selected = false;
		}
		#endregion Methods
	}
}
