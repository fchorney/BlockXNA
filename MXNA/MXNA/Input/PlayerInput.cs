using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MXNA.Input
{
    public interface IInputDevice
    {
        bool IsPressed(int code);
        bool IsHeld(int code);
        bool IsReleased(int code);
    }

    /// <summary>
    /// The PlayerInput provides user-defined action triggers binded to device input
    /// </summary>
    public class PlayerInput : GameComponent
    {

        private delegate bool Function();
        private Dictionary<string, Function> _ActionsPressed;
        private Dictionary<string, Function> _ActionsReleased;
        private Dictionary<string, Function> _ActionsHeld; 
        private PlayerIndex playerIndex;

        private KeyboardInput _Keyboard;
        private ControllerInput _Controller;

        /// <summary>
        /// Create a new PlayerInput instance. Monitors the input for a specified player.
        /// </summary>
        /// <param name="pIndex">Specified Player Index</param>
        public PlayerInput(PlayerIndex pIndex)
            : base(G.Game)
        {
            playerIndex = pIndex;
            _ActionsPressed = new Dictionary<string, Function>();
            _ActionsReleased = new Dictionary<string, Function>();
            _ActionsHeld = new Dictionary<string, Function>();
            _Keyboard = new KeyboardInput(pIndex);
            _Controller = new ControllerInput(pIndex);
        }

        #region Initialize & Update

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _Keyboard.Update(gameTime);
            _Controller.Update(gameTime);
        }

        #endregion

        public void BindAction(string actionCode, Keys key)
        {
            string keyCode = key.ToString();
            BindAction(actionCode, "Keys", keyCode);
        }

        public void BindAction(string actionCode, Buttons button)
        {
            string keyCode = button.ToString();
            BindAction(actionCode, "Buttons", keyCode);
        }

        public void BindAction(string actionCode, string enumName, string keyCode)
        {
            string ControlType = enumName;
            string ControlKey = keyCode;
            int codeValue;
            IInputDevice device;
            Type enumType;

            switch (ControlType)
            {
                case "Keys":
                    device = _Keyboard;
                    enumType = typeof(Keys);
                    break;
                case "Buttons":
                    device = _Controller;
                    enumType = typeof(Buttons);
                    break;
                default:
                    throw new Exception("Invalid input type");
            }

            codeValue = (int)Enum.Parse(Type.GetType(enumType.AssemblyQualifiedName), ControlKey, true);

            Function isPressedFunction = delegate() { return device.IsPressed(codeValue); };
            Function isReleasedFunction = delegate() { return device.IsReleased(codeValue); };
            Function isHeldFunction = delegate() { return device.IsHeld(codeValue); };

            if (_ActionsPressed.ContainsKey(actionCode))
                _ActionsPressed[actionCode] += isPressedFunction;
            else
                _ActionsPressed.Add(actionCode, isPressedFunction);

            if (_ActionsReleased.ContainsKey(actionCode))
                _ActionsReleased[actionCode] += isReleasedFunction;
            else
                _ActionsReleased.Add(actionCode, isReleasedFunction);

            if (_ActionsHeld.ContainsKey(actionCode))
                _ActionsHeld[actionCode] += isHeldFunction;
            else
                _ActionsHeld.Add(actionCode, isHeldFunction);

        }

        public void BindActionToMultiple(string actionCode, string enumName, string keyCodes)
        {
            string ControlType = enumName;
            string[] KeyCodes = keyCodes.Split(',');
            int[] ControlKeys = new int[KeyCodes.Length];

            IInputDevice device;
            Type enumType;

            switch (ControlType)
            {
                case "Keys":
                    device = _Keyboard;
                    enumType = typeof(Keys);
                    break;
                case "Buttons":
                    device = _Controller;
                    enumType = typeof(Buttons);
                    break;
                default:
                    throw new Exception("Invalid input type");
            }

            for (int i = 0; i < KeyCodes.Length; i++)
                ControlKeys[i] = ((int)Enum.Parse(Type.GetType(enumType.AssemblyQualifiedName), KeyCodes[i], true));

            Function isPressedFunction = delegate() {
                bool hasPressed = false;
                for (int i = 0; i < ControlKeys.Length; i++)
                {
                    if (!device.IsHeld(ControlKeys[i]))
                        return false;
                    if (device.IsPressed(ControlKeys[i]))
                        hasPressed = true;
                }
                return hasPressed;
            };

            Function isReleasedFunction = delegate() { for (int i = 0; i < ControlKeys.Length; i++) if (!device.IsReleased(ControlKeys[i])) return false; return true; };

            Function isHeldFunction = delegate() { for (int i = 0; i < ControlKeys.Length; i++) if (!device.IsHeld(ControlKeys[i])) return false; return true; };

            if (_ActionsPressed.ContainsKey(actionCode))
                _ActionsPressed[actionCode] += isPressedFunction;
            else
                _ActionsPressed.Add(actionCode, isPressedFunction);

            if (_ActionsReleased.ContainsKey(actionCode))
                _ActionsReleased[actionCode] += isReleasedFunction;
            else
                _ActionsReleased.Add(actionCode, isReleasedFunction);

            if (_ActionsHeld.ContainsKey(actionCode))
                _ActionsHeld[actionCode] += isHeldFunction;
            else
                _ActionsHeld.Add(actionCode, isHeldFunction);
        }

        public bool IsPressed(string actionCode)
        {
            return GetActionValue(actionCode, _ActionsPressed);
        }

        public bool IsReleased(string actionCode)
        {
            return GetActionValue(actionCode, _ActionsReleased);
        }

        public bool IsHeld(string actionCode)
        {
            return GetActionValue(actionCode, _ActionsHeld);
        }

        private bool GetActionValue(string actionCode, Dictionary<string,Function> actionList)
        {
            if (actionList.ContainsKey(actionCode))
            {
                Delegate[] dels = actionList[actionCode].GetInvocationList();
               
                for (int i = 0; i < dels.Length; i++)
                {
                    Function checkValue = dels[i] as Function;
                    if (checkValue()) return true;
                }
            } //else do we throw an error?
            return false;
        }
    }
}
