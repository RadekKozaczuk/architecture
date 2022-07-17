using System;
using JetBrains.Annotations;
using Shared.Interfaces;
using UI.Config;
using UnityEngine;
using UnityEngine.InputSystem;
// ReSharper disable EventUnsubscriptionViaAnonymousDelegate

namespace UI.Controllers
{
    /// <summary>
    /// InputHandler parses player's input and combines it into one universal set of commands.
    /// Requires PlayerInput component on scene with InputActionAsset same as one given in UIInstaller.cs
    /// </summary>
    [UsedImplicitly]
    class InputController : ICustomUpdate
    {
        static readonly UIConfig _uiConfig;

        // Float
        readonly InputAction _movementHorizontal;
        readonly InputAction _movementVertical;

        // Vec2
        readonly InputAction _mousePointer;

        // Buttons
        readonly InputAction _esc;
        readonly InputAction _use;
        
        InputController()
        {
            // TODO: to be fixed
            /*InputActionAsset inputActionAsset = _uiConfig.InputActionAsset;
            InputActionMap inputActionMap = inputActionAsset.FindActionMap(InputScheme.MapName);

            // Floats
            _movementHorizontal = inputActionMap.FindAction(InputScheme.MovementHorizontalName);
            _movementVertical = inputActionMap.FindAction(InputScheme.MovementVerticalName);

            // Vec2
            _mousePointer = inputActionMap.FindAction(InputScheme.MousePointerName);

            // Buttons
            _esc = inputActionMap.FindAction(InputScheme.EscName);
            _use = inputActionMap.FindAction(InputScheme.UseName);*/
        }

        public void CustomUpdate()
        {
            // TODO: uncomment when above is fixed
            /*// value updates
            MovementHorizontalValueUpdate();
            MovementVerticalValueUpdate();
            MovementValueUpdate();
            MousePointerValueUpdate();

            // button states updates
            EscButtonUpdate();
            UseButtonUpdate();*/
        }

#region Custom System Actions
        // read values 
        Action<float> _movementHorizontalValueUpdate;
        Action<float> _movementVerticalValueUpdate;

        Action<Vector2> _pointerPositionValueUpdate;
        Action<Vector2> _movementValueUpdate;

        // on pressed
        Action _useButtonPressed;
        Action _escButtonPressed;

        // on button up
        Action _useButtonUp;
        Action _escButtonUp;

        bool _aimState, _fireState, _reloadState, _useState, _escState;
#endregion

#region Add Action
        internal void AddUseActionPerformed(Action onButtonDown) => _use.performed += x => onButtonDown();

        internal void AddEscActionPerformed(Action onButtonDown) => _esc.performed += x => onButtonDown();

        internal void AddUseActionUp(Action onButtonUp) => _useButtonUp += onButtonUp;

        internal void AddEscActionUp(Action onButtonUp) => _escButtonUp += onButtonUp;

        internal void AddUseActionPressed(Action onButtonPressed) => _useButtonPressed += onButtonPressed;

        internal void AddEscActionPressed(Action onButtonPressed) => _escButtonPressed += onButtonPressed;

        internal void AddVector2ActionValueChange(Action<Vector2> readValueAction) => _pointerPositionValueUpdate += readValueAction;

        internal void AddMovementActionValueChange(Action<Vector2> readValueAction) => _movementValueUpdate += readValueAction;
#endregion

#region Remove Action
        internal void RemoveUseActionPerformed(Action onButtonDown) => _use.performed -= x => onButtonDown();

        internal void RemoveEscActionPerformed(Action onButtonDown) => _esc.performed -= x => onButtonDown();

        internal void RemoveEscActionUp(Action onButtonUp) => _escButtonUp -= onButtonUp;

        internal void RemoveUseActionPressed(Action onButtonPressed) => _useButtonPressed -= onButtonPressed;

        internal void RemoveEscActionPressed(Action onButtonPressed) => _escButtonPressed -= onButtonPressed;

        internal void RemoveVector2ActionValueChange(Action<Vector2> readValueAction) => _pointerPositionValueUpdate -= readValueAction;

        internal void RemoveMovementActionValueChange(Action<Vector2> readValueAction) => _movementValueUpdate -= readValueAction;
#endregion

#region Value Update
        void EscButtonUpdate()
        {
            if (_esc.IsPressed())
                _escButtonPressed?.Invoke();

            if (!_esc.IsPressed() && _escState != _esc.IsPressed())
                _escButtonUp?.Invoke();

            _escState = _esc.IsPressed();
        }

        void UseButtonUpdate()
        {
            if (_use.IsPressed())
                _useButtonPressed?.Invoke();

            if (!_use.IsPressed() && _useState != _use.IsPressed())
                _useButtonUp?.Invoke();

            _useState = _use.IsPressed();
        }

        void MovementHorizontalValueUpdate() => _movementHorizontalValueUpdate?.Invoke(_movementHorizontal.ReadValue<float>());

        void MovementVerticalValueUpdate() => _movementVerticalValueUpdate?.Invoke(_movementVertical.ReadValue<float>());

        void MousePointerValueUpdate() => _pointerPositionValueUpdate?.Invoke(_mousePointer.ReadValue<Vector2>());

        void MovementValueUpdate() =>
            _movementValueUpdate?.Invoke(new Vector2(_movementHorizontal.ReadValue<float>(), _movementVertical.ReadValue<float>()));
#endregion
    }
}