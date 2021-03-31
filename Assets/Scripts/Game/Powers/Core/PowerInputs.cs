﻿namespace Tartaros.Power
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Tartaros.Construction;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using static UnityEngine.InputSystem.InputAction;

    public class PowerInputs
    {
        private IMousePosition _mousePosition = null;
        private GameInputs _input = null;

        public PowerInputs()
        {
            _input = new GameInputs();
            _input.Construction.Enable();
            _mousePosition = new SetMousePositionWithPlane();
        }

        public event Action<CallbackContext> ValidatePerformed
        {
            add
            {
                _input.Construction.ValidateConstruction.performed += value;
            }

            remove
            {
                _input.Construction.ValidateConstruction.performed -= value;
            }
        }

        public Vector3 GetMousePosition()
        {
            return _mousePosition.GetPreviewPosition();
        }

        public bool IsValidatePerformed()
        {
            return _input.Construction.ValidateConstruction.phase == InputActionPhase.Performed;
        }

        public bool IsLeaveAndRefundPerformed()
        {
            return _input.Construction.ExitConstruction.phase == InputActionPhase.Performed;
        }
    }
}