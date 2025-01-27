﻿namespace Tartaros.Entities.State
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	using Tartaros.Entities;
	using Tartaros.Entities.Attack;

	public class StateAttack : AEntityState
	{
		private readonly IAttackable _target = null;
		private readonly EntityAttack _entityAttack = null;
		private readonly EntityMovement _entityMovement = null;
		private readonly Animator _animator = null;

		private AnimatorClipInfo _attackClip;

		public StateAttack(Entity stateOwner, IAttackable target) : base(stateOwner)
		{
			_target = target;

			if (_target == null)
			{
				Debug.LogErrorFormat("The attack target of {0} is null. It is not a expected behaviour.", stateOwner);
			}

			_entityMovement = stateOwner.GetComponent<EntityMovement>();
			_entityAttack = stateOwner.GetComponent<EntityAttack>();
			_animator = stateOwner.GetComponentInChildren<Animator>();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();

			StopMovement();
			_entityAttack.StopAttacking();
		}

		public override void OnUpdate()
		{
			if (IsTargetDead() == true)
			{
				_stateOwner.GetComponent<EntityFSM>().MarkCurrentStateAsFinish();
			}
			else
			{
				if (_entityAttack.IsInRange(_target) == true)
				{
					_entityAttack.StartAttacking();
					StopMovement();
					_entityAttack.CastAttackIfPossible(_target);

					if(_animator != null)
					{
						AnimatorClipInfo[] animatorClipInfos = _animator.GetCurrentAnimatorClipInfo(0);

						if (animatorClipInfos.Length > 0)
						{
							_attackClip = animatorClipInfos[0];
						}
					}
				}
				else
				{
					_entityAttack.StopAttacking();

					if (IsCurrentAttackAnimationFinish() == true)
					{
						MoveToTarget();
					}
				}
			}
		}

		private bool IsTargetDead()
		{
			return _target == null || _target.IsAlive == false || _target.IsInterfaceDestroyed<IAttackable>();
		}

		private void StopMovement()
		{
			if (_entityMovement != null)
			{
				_entityMovement.StopMovement();
			}
		}

		private bool IsCurrentAttackAnimationFinish()
		{
			if (_animator == null) return true;

			AnimatorClipInfo currentClip = _animator.GetCurrentAnimatorClipInfo(0)[0];

			return _attackClip.clip == null || (_attackClip.clip != null && currentClip.clip.name != _attackClip.clip.name);
		}

		private void MoveToTarget()
		{
			if (_entityMovement != null)
			{
				_entityMovement.MoveToPoint(_target.Transform.position);
			}
		}
	}
}