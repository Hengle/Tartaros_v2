﻿namespace Tartaros.Wave
{
	using Tartaros.Dialogue;
	using Tartaros.ServicesLocator;
	using Tartaros.SoundsSystem;
	using UnityEngine.Networking;

	public class WaitDeathOfSpawnedEnemiesWaveState : AWaveSpawnerState
	{
		#region Fields
		private readonly WavesEnemiesStillAliveManager _stillAliveManager = null;
		private readonly MusicManager _musicManager = null;
		private readonly DialogueManager _dialogueManager = null;
		#endregion Fields

		#region Ctor
		public WaitDeathOfSpawnedEnemiesWaveState(EnemiesWavesManager stateOwner, WavesEnemiesStillAliveManager stillAliveManager) : base(stateOwner)
		{
			_stillAliveManager = stillAliveManager;

			_musicManager = Services.Instance.Get<MusicManager>();
			_dialogueManager = Services.Instance.Get<DialogueManager>();
		}
		#endregion Ctor

		#region Methods
		public override void OnStateEnter()
		{
			base.OnStateEnter();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();

			_musicManager.CurrentMusic = MusicManager.MusicPhase.Construction;

			var currentWave = _stateOwner.CurrentWave;

			if (currentWave.LaunchDialogueWhenWaveOver)
			{
				_dialogueManager.EnterDialogueState(currentWave.DialogueID);
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			if (IsEverySpawnedEnemiesWaveDead())
			{
				if (_stateOwner.CurrentWaveIndex < _stateOwner.LastWaveIndex)
				{
					_stateOwner.InvokeWaveFinish();
					_stateOwner.WaveFSM.CurrentState = new WaveCooldownState(_stateOwner);
				}
				else
				{
					_stateOwner.InvokeWaveFinish();
					_stateOwner.InvokeGameFinished();
					_stateOwner.WaveFSM.CurrentState = new WaveFinishedState(_stateOwner);
				}
			}
		}

		private bool IsEverySpawnedEnemiesWaveDead()
		{
			return (_stillAliveManager.GetStillAliveEnemiesCount() == 0);
		}
		#endregion Methods
	}
}