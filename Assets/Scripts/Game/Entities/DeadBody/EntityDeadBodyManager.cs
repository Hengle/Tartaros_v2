﻿namespace Tartaros.Entities
{
	using Sirenix.OdinInspector;
	using UnityEngine;

	public class EntityDeadBodyManager : AEntityBehaviour
	{
		#region Fields
		[SerializeField, Required] private GameObject _model = null;
		[SerializeField, Required] private AnimationClip _deathAnimation = null;
		[SerializeField] private DeadbodyData _deadbodyData = null;
		#endregion Fields

		#region Methods
		private void OnEnable()
		{
			Entity.EntityKilled -= EntityKilled;
			Entity.EntityKilled += EntityKilled;
		}

		private void OnDisable()
		{
			Entity.EntityKilled -= EntityKilled;
		}

		private void EntityKilled(object sender, Wave.KilledArgs e)
		{
			InstanciateDeadbody();
		}

		private void InstanciateDeadbody()
		{
			if (_model == null)
			{
				Debug.LogWarning("MODEL NULL");
			}

			//GameObject deadbody = Instantiate(_model, _model.transform.position, _model.transform.rotation, null);
			GameObject deadbody = _model;
			deadbody.transform.parent = null;
			deadbody.GetOrAddComponent<Deadbody>().Data = _deadbodyData;

			if (deadbody.TryGetComponent(out AnimationInstancing.AnimationInstancing animationInstancing))
			{
				this.ExecuteAfterFrame(() => animationInstancing.PlayAnimation(_deathAnimation.name));
			}
			else
			{
				Debug.LogWarningFormat("The deadbody {0} must have a component AnimationInstancing.");
			}
		}
		#endregion Methods
	}
}