using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace CryosisEngine
{
    public class InputHandler
    {
        KeyboardState CurrentState { get; set; }

        GamePadState CurrentGamepadState { get; set; }

        public Dictionary<string, InputAction> KeyActions { get; set; }

        public void UpdateInputs(GameTime gameTime)
        {
            CurrentState = Keyboard.GetState();
            CurrentGamepadState = GamePad.GetState(0);

            float elapsedGameTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            foreach (InputAction action in KeyActions.Values)
                action.Update(CurrentState, CurrentGamepadState, elapsedGameTime);
        }

        public bool IsPressed(string action) => KeyActions[action].IsPressed;

        public bool IsNewPress(string action) => KeyActions[action].IsNewPress;
    }

    public class InputAction
    {
        public Keys Key { get; set; }

        public Buttons Button { get; set; }

        public int ElapsedFrames { get; set; }

        public float ElapsedTime { get; set; }

        public bool IsPressed { get; set; }

        public bool IsNewPress { get; set; }

        public InputAction (Keys key, Buttons button)
        {
            Key = key;
            Button = button;
        }

        public void Update(KeyboardState keyboardState, GamePadState gamepadState, float elapsedMilliseconds)
        {
            bool wasPressed = IsPressed;
            IsPressed = keyboardState.IsKeyDown(Key) || gamepadState.IsButtonDown(Button);
            IsNewPress = !wasPressed && IsPressed;

            if (IsNewPress)
            {
                ElapsedTime = elapsedMilliseconds;
                ElapsedFrames = 1;
            }
            else if (IsPressed)
            {
                ElapsedTime += elapsedMilliseconds;
                ElapsedFrames++;
            }
            else
            {
                ElapsedTime = ElapsedFrames = 0;
            }
        }
    }
}
