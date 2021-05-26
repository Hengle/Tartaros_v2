﻿namespace Tartaros.Gamemode.State
{
	using Tartaros.CameraSystem;
	using Tartaros.Dialogue;
	using Tartaros.Selection;
	using Tartaros.ServicesLocator;
	using UnityEngine;

	public class DialogueState : AGameState
	{

		#region Fields
		private int _currentSpeechIndex = -1;

		private readonly DialoguesSequence _dialogueSequence = null;
		private readonly Transform _cameraTarget = null;

		// SERVICES
		private readonly DialogueManager _dialogueManager = null;
		private readonly ISelection _currentSelection = null;
		private readonly RectangleSelectionInput _rectangleSelectionInput = null;
		private readonly ClickSelectionInput _clickSelectionInput = null;
		#endregion

		#region Ctor
		public DialogueState(GamemodeManager stateOwner, DialoguesSequence sequence, Transform cameraTarget) : base(stateOwner)
		{
			if (stateOwner is null) throw new System.ArgumentNullException(nameof(stateOwner));

			_dialogueSequence = sequence ?? throw new System.ArgumentNullException(nameof(sequence));
			_cameraTarget = cameraTarget ?? throw new System.ArgumentNullException(nameof(cameraTarget));

			_dialogueManager = Services.Instance.Get<DialogueManager>();
			_currentSelection = Services.Instance.Get<CurrentSelection>();
			_rectangleSelectionInput = GameObject.FindObjectOfType<RectangleSelectionInput>();
			_clickSelectionInput = GameObject.FindObjectOfType<ClickSelectionInput>();
		}
		#endregion

		#region Methods
		public override void OnStateEnter()
		{
			base.OnStateEnter();

			_currentSelection.Clear();
			_rectangleSelectionInput.enabled = false;
			_clickSelectionInput.enabled = false;

			PauseGame(true);

			ShowNextSpeech();

			if (_dialogueSequence.IsCameraTarget == true)
			{
				SetCameraFollowTargetMode(true);
			}

		}

		public override void OnStateExit()
		{
			base.OnStateExit();

			_rectangleSelectionInput.enabled = true;
			_clickSelectionInput.enabled = true;

			PauseGame(false);

			if (_dialogueSequence.IsCameraTarget == true)
			{
				SetCameraFollowTargetMode(false);
			}

			_dialogueManager.InvokeDialogueOver(new DialogueManager.DialogueOverArgs());
		}

		public void ShowNextSpeech()
		{
			if (IsThereSpeechToShow() == true)
			{
				_currentSpeechIndex++;

				Dialogue speech = _dialogueSequence.GetDialogue(_currentSpeechIndex);
				_dialogueManager.InvokeNewDialogueEvent(new DialogueManager.NextDialogueArgs(speech));
			}
			else
			{
				LeaveState();
			}
		}

		private bool IsThereSpeechToShow()
		{
			return _currentSpeechIndex + 1 < _dialogueSequence.DialoguesCount;
		}

		private void PauseGame(bool enablePause)
		{
			Time.timeScale = enablePause ? 0 : 1;

			if (Camera.main.TryGetComponent(out CameraController cameraController))
			{
				cameraController.UseUnscaledDeltaTime = enablePause;
			}
		}

		private void SetCameraFollowTargetMode(bool mode)
		{
			if (Camera.main.TryGetComponent<CameraController>(out CameraController cameraController))
			{
				if (_cameraTarget != null)
				{
					cameraController.SetCameraTarget(_cameraTarget);
					cameraController.SetCameraFollowTargetMode(mode);
				}
				else
				{
					Debug.LogWarning("there is no cameraTarget in DialogueManager");
				}
			}
			else
			{
				Debug.LogError("Dialogue State don't find cameraController");
			}
		}
	}
	#endregion
}