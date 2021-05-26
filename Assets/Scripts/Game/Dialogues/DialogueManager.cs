﻿namespace Tartaros.Dialogue
{
	using System;
	using Tartaros.Gamemode;
	using Tartaros.Gamemode.State;
	using Tartaros.ServicesLocator;
	using Tartaros.Wave;
	using UnityEngine;

	public class DialogueManager : MonoBehaviour
	{
		#region Fields
		[SerializeField] private DialoguesData _data = null;
		[SerializeField] private Transform _cameraTarget = null;

		private int _indexDialogue = 0;

		private GamemodeManager _gamemodeManager = null;
		#endregion Fields

		#region Events
		public class NextDialogueArgs : EventArgs
		{
			public readonly Dialogue speech = null;

			public NextDialogueArgs(Dialogue speech)
			{
				this.speech = speech;
			}
		}
		public event EventHandler<NextDialogueArgs> NewDialogue = null;

		public class DialogueOverArgs : EventArgs { }
		public event EventHandler<DialogueOverArgs> DialogueOver = null;

		public class CameraMoveStartArgs : EventArgs { }
		public event EventHandler<CameraMoveStartArgs> CameraMoveStart = null;

		public class CameraMoveEndArgs : EventArgs { }
		public event EventHandler<CameraMoveEndArgs> CameraMoveEnd = null;
		#endregion Events

		#region Methods
		private void Awake()
		{
			_gamemodeManager = Services.Instance.Get<GamemodeManager>();
		}

		public void EnterDialogueState(string dialogueID)
		{
			_gamemodeManager.SetState(new DialogueState(_gamemodeManager, _data.GetDialoguesSequence(dialogueID), _cameraTarget));
			_indexDialogue++;
		}

		public void ShowNextLine()
		{
			if (_gamemodeManager.CurrentState is DialogueState dialogueState)
			{
				dialogueState.ShowNextSpeech();
			}
			else
			{
				throw new NotSupportedException("Cannot show next line, game is not in a dialogue state.");
			}
		}

		public void InvokeNewDialogueEvent(NextDialogueArgs args) => NewDialogue?.Invoke(this, args);
		public void InvokeDialogueOver(DialogueOverArgs args) => DialogueOver?.Invoke(this, args);
		#endregion Methods
	}
}