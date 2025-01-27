﻿namespace Tartaros.Economy
{
	using System;
	using UnityEngine;

	public class GloryWallet : IGloryWallet
	{
		#region Fields
		private int _gloryAmount = 0;
		private int _maxGloryAmount = 220;
		#endregion Fields

		#region Properties
		event EventHandler<GloryAmountChangedArgs> AmountChanged = null;
		event EventHandler<GloryAmountChangedArgs> IGloryWallet.AmountChanged { add => AmountChanged += value; remove => AmountChanged -= value; }
		#endregion Properties

		#region Methods
		void IGloryWallet.AddAmount(int amount)
		{
			_gloryAmount += amount;
			_gloryAmount = Mathf.Clamp(_gloryAmount, 0, _gloryAmount);

			AmountChanged?.Invoke(this, new GloryAmountChangedArgs());
		}

		void IGloryWallet.Spend(int price)
		{
			if (_gloryAmount - price > 0)
			{
				_gloryAmount -= price;
				AmountChanged?.Invoke(this, new GloryAmountChangedArgs());
			}
		}

		bool IGloryWallet.CanSpend(int price)
		{
			return _gloryAmount - price >= 0;
		}

		int IGloryWallet.GetAmount()
		{
			return _gloryAmount;
		}
		#endregion Methods
	}
}