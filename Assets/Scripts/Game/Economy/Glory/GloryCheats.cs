﻿namespace Tartaros.Glory
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Tartaros.Economy;
	using Tartaros.ServicesLocator;
	using TF.CheatsGUI;

	public static class GloryCheats
	{
		[Cheat]
		public static void GiveTenGlory()
		{
			GiveGlory(10);
		}

		[Cheat]
		public static void GiveGlory(int amount)
		{
			IPlayerGloryWallet gloryWallet = Services.Instance.Get<IPlayerGloryWallet>();
			gloryWallet.AddAmount(amount);
		}
	}
}