﻿namespace Tartaros.Orders
{
	using Tartaros;
	using Tartaros.Entities;
	using Tartaros.ServicesLocator;
	using Tartaros.UI.HoverPopup;
	using UnityEngine;

	public class SelfKillOrder : Order
	{
		public static Sprite Icon => Services.Instance.Get<IconsDatabase>().Data.SelfKillIcon;

		public SelfKillOrder(Entity entity) : base(Icon, () => entity.Kill(false), Services.Instance.Get<HoverPopupsDatabase>().Database.SelfKill)
		{
			if (entity is null) throw new System.ArgumentNullException(nameof(entity));
		}
	}
}
