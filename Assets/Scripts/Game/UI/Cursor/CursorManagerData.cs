﻿namespace Tartaros.UI.Cursor
{
	using Sirenix.OdinInspector;
	using System.Collections;
	using UnityEngine;

	public class CursorManagerData : SerializedScriptableObject
	{
		[SerializeField, PreviewField] private Texture2D _defaultCursor = null;
		[SerializeField, PreviewField] private Texture2D _defaultCursorHoverable = null;
		[SerializeField, PreviewField] private Texture2D _defaultCursorHoverableEnemy = null;
		[SerializeField, PreviewField] private Texture2D _constructionCursor = null;
		[SerializeField, PreviewField] private Texture2D _constructionInvalideCursor = null;
		[SerializeField, PreviewField] private Texture2D _addAllyInSelectionCursor = null;
		[SerializeField, PreviewField] private Texture2D _orderAttackCursor = null;
		[SerializeField, PreviewField] private Texture2D _orderMoveCursor = null;
		[SerializeField, PreviewField] private Texture2D _orderCantMoveCursor = null;
		[SerializeField, PreviewField] private Texture2D _orderMoveAndAttackCursor = null;
		[SerializeField, PreviewField] private Texture2D _orderPatrolCursor = null;
		[SerializeField, PreviewField] private Texture2D _powerCursor = null;

		public Texture2D DefaultCursor => _defaultCursor;
		public Texture2D DefaultCursorHoverable => _defaultCursorHoverable;
		public Texture2D DefaultCursorHoverableEnemy => _defaultCursorHoverableEnemy;
		public Texture2D ConstructionCursor => _constructionCursor;
		public Texture2D ConstructionInvalideCursor => _constructionInvalideCursor;
		public Texture2D AddAllyInSelectionCursor => _addAllyInSelectionCursor;
		public Texture2D OrderAttackCursor => _orderAttackCursor;
		public Texture2D OrderMoveCursor => _orderMoveCursor;
		public Texture2D OrderCantMoveCursor => _orderCantMoveCursor;
		public Texture2D OrderMoveAndAttackCursor => _orderMoveAndAttackCursor;
		public Texture2D OrderPatrolCursor => _orderPatrolCursor;
		public Texture2D PowerCursor => _powerCursor;
	}
}