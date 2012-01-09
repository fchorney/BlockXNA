using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FrequencyAnalyzer.Modules
{
    class PlayerInput
    {
#if XBOX
        private GamePadState prevState;
        private GamePadState currState;
#elif WINDOWS
        private KeyboardState prevState;
        private KeyboardState currState;
#endif
        private PlayerIndex playerIndex;

        /// <summary>
        /// Creates a new instance of the PlayerInput object. 
        /// </summary>
        /// <param name="pIndex">Which player to poll input from.</param>
        public PlayerInput(PlayerIndex pIndex)
        {
#if XBOX
            prevState = GamePad.GetState(playerIndex);
#elif WINDOWS
            prevState = Keyboard.GetState(playerIndex); 
#endif
            currState = prevState;

            playerIndex = pIndex;
        }

        /// <summary>
        /// Poll player input.
        /// </summary>
        public void Poll()
        {
            prevState = currState;
#if XBOX
            currState = GamePad.GetState(playerIndex);
#elif WINDOWS
            currState = Keyboard.GetState(playerIndex);
#endif
        }

#if XBOX      
        #region XBox360
        public bool isPressed(Buttons button)
        {
            switch(button)
            {
                case Buttons.A:
                    if (prevState.Buttons.A == ButtonState.Released && currState.Buttons.A == ButtonState.Pressed)
                    {
                        return true;
                    }
                    break;
                case Buttons.B:
                    if (prevState.Buttons.A == ButtonState.Released && currState.Buttons.B == ButtonState.Pressed)
                    {
                        return true;
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }
        #endregion
#elif WINDOWS
        #region Windows

        /// <summary>
        /// Check if a key has been pressed
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>Returns true if key has been pressed</returns>
        public bool isPressed(Keys key)
        {
            if (prevState.IsKeyUp(key) && currState.IsKeyDown(key))
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Check if a key has been released
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>Returns true if key has been released</returns>
        public bool isReleased(Keys key)
        {
            if (prevState.IsKeyDown(key) && currState.IsKeyUp(key))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if a key is currently held
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>Returns true if key is held</returns>
        public bool isHeld(Keys key)
        {
            if (currState.IsKeyDown(key))
            {
                return true;
            }
            return false;
        }

        #endregion
#endif
    }
}
