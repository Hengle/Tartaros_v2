﻿namespace Tartaros.Entities.Attack
{
	using Sirenix.OdinInspector;
	using System;
	using Tartaros.Entities;
	using Tartaros.Entities.Detection;
	using Tartaros.OrderGiver;
	using Tartaros.Orders;
	using UnityEngine;

	[RequireComponent(typeof(EntityDetection), typeof(EntityFSM), typeof(Entity))]
	[DisallowMultipleComponent]
	public partial class EntityAttack : AEntityBehaviour, IOrderAttackReceiver, IOrderable
	{
		#region Fields
		[Title("Settings")]
		[SerializeField] private Vector3 _projectileSpawnPoint = Vector3.up;
		[Title("References")]
		[SerializeField] private float _inflictDamageDelay = 0f;
		[SerializeField] private InflictDamageAnimationEvent _inflictDamageAnimationEvent = null;
		[SerializeField] private Transform _turretTransform = null;
		[SerializeField, FoldoutGroup("Sounds")] private AudioSource _attackAudioSource = null;
		[SerializeField, FoldoutGroup("Sounds")] private AudioClip[] _attackAudioClips = null;

		private EntityAttackData _entityAttackData = null;
		private EntityDetection _entityDetection = null;
		private EntityFSM _entityFSM = null;
		private float _lastTimeAttack = 0;

		private IAttackable _target = null;
		private bool _isAttacking = false;
		#endregion

		#region Properties
		public EntityAttackData EntityAttackData { get => _entityAttackData; set => _entityAttackData = value; }
		public float AttackRange => _entityAttackData.AttackRange;
		public Vector3 ProjectileSpawnWorldPoint
		{
			get => transform.InverseTransformPoint(_projectileSpawnPoint);
			set => _projectileSpawnPoint = transform.TransformPoint(value);
		}
		#endregion Properties

		#region Events
		public class AttackCastedArgs : EventArgs { }
		public event EventHandler<AttackCastedArgs> AttackCasted = null;

		public class StartAttackArgs : EventArgs { }
		public event EventHandler<StartAttackArgs> StartAttack = null;

		public class StopAttackArgs : EventArgs { }
		public event EventHandler<StopAttackArgs> StopAttack = null;
		#endregion Events

		#region Methods
		private void Start()
		{
			_entityFSM = GetComponent<EntityFSM>();
			_entityDetection = GetComponent<EntityDetection>();

			_entityAttackData = Entity.GetBehaviourData<EntityAttackData>();

			if (_entityDetection == null)
			{
				Debug.LogError(string.Format("Missing EntityDetection component on \"{0}\". Add detection behaviour on entity data.", name), gameObject);
			}
		}

		private void OnEnable()
		{
			if (_inflictDamageAnimationEvent != null)
			{
				_inflictDamageAnimationEvent.InflictDamageAnimationPlayed -= InflictDamageAnimation;
				_inflictDamageAnimationEvent.InflictDamageAnimationPlayed += InflictDamageAnimation;
			}
		}

		private void OnDisable()
		{
			if (_inflictDamageAnimationEvent != null)
			{
				_inflictDamageAnimationEvent.InflictDamageAnimationPlayed -= InflictDamageAnimation;
			}
		}

		public void StartAttacking()
		{
			if (_isAttacking == true) return;

			StartAttack?.Invoke(this, new StartAttackArgs());
			_isAttacking = true;
		}

		public void StopAttacking()
		{
			if (_isAttacking == false) return;

			StopAttack?.Invoke(this, new StopAttackArgs());
			_isAttacking = false;
		}

		public void CastAttackIfPossible(IAttackable target)
		{
			if (target == null) return;

			if (IsInRange(target) == false) return;

			if (CanAttackCooldown() == false) return;

			if (_entityAttackData.AttackMode == null)
			{
				Debug.LogErrorFormat("Please set an attack mode in data of {0}.", name);
				return;
			}

			_target = target;

			_lastTimeAttack = Time.time;
			LookAt(target);

			if (DoInflictDamageOnSpecificKey() == false)
			{
				if (_inflictDamageDelay > 0)
				{
					this.ExecuteAfterTime(_inflictDamageDelay, InflictDamageToTarget);
				}
				else
				{
					InflictDamageToTarget(); // inflict damage now
				}
			}


			AttackCasted?.Invoke(this, new AttackCastedArgs());
		}

		private bool DoInflictDamageOnSpecificKey()
		{
			return _inflictDamageAnimationEvent != null;
		}

		private void InflictDamageAnimation(object sender, InflictDamageAnimationEvent.InflictDamageAnimationPlayedArgs e)
		{
			InflictDamageToTarget();
		}

		private void InflictDamageToTarget()
		{
			if (_target.IsInterfaceDestroyed() == false)
			{
				PlayAttackSound();

				_entityAttackData.AttackMode.Attack(transform, _target);
			}
		}

		private void PlayAttackSound()
		{
			if (_attackAudioSource != null)
			{
				if (_attackAudioClips.Length > 0)
				{
					_attackAudioSource.PlayOneShot(_attackAudioClips.GetRandom());
				}
				else
				{
					Debug.LogWarning("There is an attack audio source referenced in EntityAttack, but there is no sound to play.", gameObject);
				}
			}
		}

		private void LookAt(IAttackable target)
		{
			if (target != null)
			{
				if (_turretTransform == null)
				{
					transform.forward = (target.Transform.position - transform.position);
				}
				else
				{
					_turretTransform.forward = (target.Transform.position - transform.position);
				}
			}
		}

		public void TryOrderAttackNearestOpponent()
		{
			IAttackable attackable = _entityDetection.GetNearestAttackableOpponentInDetectionRange();

			if (attackable != null)
			{
				(this as IOrderAttackReceiver).Attack(attackable);
			}
		}

		public bool IsInRange(IAttackable target)
		{
			return _entityDetection.IsInAttackRange(target);
		}

		public bool CanAttackCooldown()
		{
			return Time.time > _lastTimeAttack + _entityAttackData.SecondsBetweenAttacks;
		}

		void IOrderAttackReceiver.Attack(IAttackable target)
		{
			if (target != null)
			{
				_entityFSM.OrderAttack(target);
			}
		}

		void IOrderAttackReceiver.AttackAdditive(IAttackable target)
		{
			_entityFSM.EnqueueOrderAttack(target);
		}

		Order[] IOrderable.GenerateOrders()
		{
			Order[] orders = new Order[]
			{
				new AttackOrder(Entity)
			};

			return orders;
		}
		#endregion
	}

#if UNITY_EDITOR
	public partial class EntityAttack
	{
		private void OnDrawGizmos()
		{
			if (_entityAttackData != null)
			{
				Editor.HandlesHelper.DrawWireCircle(transform.position, Vector3.up, AttackRange, Color.red);
			}
		}
	}
#endif
}