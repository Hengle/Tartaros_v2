﻿namespace Tartaros.Orders
{
	using System;
	using System.Collections;
	using Tartaros.Entities;
	using Tartaros.Gamemode;
	using Tartaros.OrderGiver;
	using Tartaros.ServicesLocator;
	using Tartaros.UI.HoverPopup;
	using UnityEngine;

	public class MoveAgressivelyOrder : Order
	{
		private static Sprite Icon => Services.Instance.Get<IconsDatabase>().Data.MoveAgressivelyIcon;
		public MoveAgressivelyOrder(EntityMovement entityMovement) : base(Icon, Services.Instance.Get<HoverPopupsDatabase>().Database.MoveAggressively)
		{
			if (entityMovement is null) throw new ArgumentNullException(nameof(entityMovement));

			_executeAction = Create(entityMovement, this);
		}
		private static Action Create(EntityMovement entityMovement, Order order)
		{
			return () =>
			{
				GamemodeManager gamemodeManager = Services.Instance.Get<GamemodeManager>();

				CatchRightClickState state = new CatchRightClickState(gamemodeManager, OrderMoveToPositionUnderCursor(entityMovement), order);

				gamemodeManager.SetState(state);
			};
		}

		private static Action OrderMoveToPositionUnderCursor(EntityMovement entityMovement)
		{
			return () =>
			{
				var userErrorLogger = Services.Instance.Get<UserErrorsLogger>();
				RaycastHit hit;
				bool gameObjectUnderCursor = MouseHelper.GetHitUnderCursor(out hit);
				Vector3 position = hit.point;

				if (entityMovement.CanMoveToPoint(position))
				{
					entityMovement.GetComponent<IOrderMoveAggresivellyReceiver>().MoveAggressively(position);
				}
				else
				{
					userErrorLogger.Log("MovePoint isn't valable");
				}
			};
		}
	}
}