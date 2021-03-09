using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
//using SharpDX.Direct3D11;
using SharpDX.DirectInput;
using SharpDX.Windows;

namespace Para_1
{
    class InputController : IDisposable
    {
        private DirectInput _directInput;

        private Keyboard _keyboard;
        private KeyboardState _keyboardState;
        private bool _keyboardUpdate = false;
        public bool KeyboardUpdate { get => _keyboardUpdate; }
        private bool _keyboardAcquired;

        private Mouse _mouse;
        private MouseState _mouseState;
        private bool _mouseUpdate = false;
        public bool MouseUpdate { get => _mouseUpdate; }
        private bool _mouseAcquired;

        private bool[] _mouseButtons = new bool[8];
        public bool[] MouseButtons { get => _mouseButtons; }

        private int _mouseRelativePositionX; 
        public int MouseRelativePositionX { get => _mouseRelativePositionX; }

        private int _mouseRelativePositionY;
        public int MouseRelativePositionY { get => _mouseRelativePositionY; }

        private int _mouseRelativePositionZ;
        public int MouseRelativePositionZ { get => _mouseRelativePositionZ; }

        private bool _keyLeftPrevios;
        private bool _keyLeftCurrent;
        private bool _keyLeft; 
        public bool KeyLeft { get => _keyLeft; }

        private bool _keyRightPrevios;
        private bool _keyRightCurrent;
        private bool _keyRight;
        public bool KeyRight { get => _keyRight; }
        private bool _keyUpPrevios;
        private bool _keyUpCurrent;
        private bool _keyUp;
        public bool KeyUp { get => _keyUp; }
        private bool _keyDownPrevios;
        private bool _keyDownCurrent;
        private bool _keyDown;
        public bool KeyDown { get => _keyDown; }


        public bool KeyW { get => _keyboardState.IsPressed(Key.W); }
        public bool KeyA { get => _keyboardState.IsPressed(Key.A); }
        public bool KeyS { get => _keyboardState.IsPressed(Key.S); }
        public bool KeyD { get => _keyboardState.IsPressed(Key.D); }

        public InputController(RenderForm renderForm)
        {
            _directInput = new DirectInput();

            _keyboard = new Keyboard(_directInput);
            _keyboard.SetCooperativeLevel(renderForm.Handle, 
                CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);

            AcquireKeyboard();
            _keyboardState = new KeyboardState();

            _mouse = new Mouse(_directInput);
            _mouse.SetCooperativeLevel(renderForm.Handle,
                CooperativeLevel.Foreground | CooperativeLevel.NonExclusive);
            
            AcquireMouse();
            _mouseState = new MouseState();
        }

        private void AcquireKeyboard()
        {
            try
            {
                _keyboard.Acquire();
                _keyboardAcquired = true;
            }
            catch(SharpDXException e)
            {
                if (e.ResultCode.Failure)
                    _keyboardAcquired = false;
            }
        }
        private void AcquireMouse()
        {
            try
            {
                _mouse.Acquire();
                _mouseAcquired = true;
            }
            catch (SharpDXException e)
            {
                if (e.ResultCode.Failure)
                    _mouseAcquired = false;
            }
        }

        private bool TriggerByKeyUp(Key key, ref bool previos, ref bool current)
        {
            previos = current;
            current = _keyboardState.IsPressed(key);
            return previos && !current;
        }

        private bool TriggerByKeyDown(Key key, ref bool previos, ref bool current)
        {
            previos = current;
            current = _keyboardState.IsPressed(key);
            return !previos && current;
        }

        private void ProcessKeyboardState()
        {
            _keyLeft = TriggerByKeyDown(Key.Left, ref _keyLeftPrevios, ref _keyLeftCurrent);
            _keyRight = TriggerByKeyDown(Key.Right, ref _keyRightPrevios, ref _keyRightCurrent);
            _keyDown = TriggerByKeyDown(Key.Down, ref _keyDownPrevios, ref _keyDownCurrent);
            _keyUp = TriggerByKeyDown(Key.Up, ref _keyUpPrevios, ref _keyUpCurrent);
        }

        public void UpdateKeyboardState()
        {
            if (!_keyboardAcquired) AcquireKeyboard();
            ResultDescriptor resultCode = ResultCode.Ok;
            try
            {
                _keyboard.GetCurrentState(ref _keyboardState);
                ProcessKeyboardState();
                _keyboardUpdate = true;
            }
            catch(SharpDXException e)
            {
                resultCode = e.Descriptor;
                _keyboardUpdate = false;
            }
            if(resultCode == ResultCode.InputLost || resultCode == ResultCode.NotAcquired)
                _keyboardUpdate = false;
        }

        private void ProcessMouseState()
        {
            for (int i = 0; i <= 7; ++i)
                _mouseButtons[i] = _mouseState.Buttons[i];
            _mouseRelativePositionX = _mouseState.X;
            _mouseRelativePositionY = _mouseState.Y;
            _mouseRelativePositionZ = _mouseState.Z;
        }

        public void UpdateMouseState()
        {
            if (!_mouseAcquired) AcquireMouse();
            ResultDescriptor resultCode = ResultCode.Ok;
            try
            {
                _mouse.GetCurrentState(ref _mouseState);
                ProcessMouseState();
                _mouseUpdate = true;
            }
            catch (SharpDXException e)
            {
                resultCode = e.Descriptor;
                _mouseUpdate = false;
            }
            if (resultCode == ResultCode.InputLost || resultCode == ResultCode.NotAcquired)
                _mouseUpdate = false;
        }

        public void Dispose()
        {
            _mouse.Unacquire();
            Utilities.Dispose(ref _mouse);
            _keyboard.Unacquire();
            Utilities.Dispose(ref _keyboard);
            Utilities.Dispose(ref _directInput);
        }
    }
}
