using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XInput;

namespace Game
{
    class GameController : Controller
    {

        //Sticks
        public double LeftStickX { get; set; }
        public double LeftStickY { get; set; }
        public double LeftStickDeadzone { get; set; }
        public double RightStickX { get; set; }
        public double RightStickY { get; set; }
        public double RightStickDeadzone { get; set; }

        //Buttons
        public GameControllerButton ButtonA { get; set; }
        public GameControllerButton ButtonB { get; set; }
        public GameControllerButton ButtonX { get; set; }
        public GameControllerButton ButtonY { get; set; }

        //Constructor
        public GameController(UserIndex userIndex, double leftDeadzone, double rightDeadzone) : base(userIndex)
        {
            LeftStickDeadzone = leftDeadzone;
            RightStickDeadzone = rightDeadzone;
            ButtonA = new GameControllerButton();
            ButtonB = new GameControllerButton();
            ButtonX = new GameControllerButton();
            ButtonY = new GameControllerButton();
        }

        //Get State
        public void UpdateState()
        {
            State state = base.GetState();
            //Buttons
            this.ButtonA.Update(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A));
            this.ButtonB.Update(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B)); 
            this.ButtonX.Update(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.X));
            this.ButtonY.Update(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Y));
            //Left stick
            this.LeftStickX = ControllerStickDeadzoned(state.Gamepad.LeftThumbX, this.LeftStickDeadzone);
            this.LeftStickY = ControllerStickDeadzoned(state.Gamepad.LeftThumbY, this.LeftStickDeadzone);
            //Right Stick
            this.RightStickX = ControllerStickDeadzoned(state.Gamepad.RightThumbX, this.RightStickDeadzone);
            this.RightStickY = ControllerStickDeadzoned(state.Gamepad.RightThumbY, this.RightStickDeadzone);
        }


        //Get the controller stick value with deadzoning applied
        private double ControllerStickDeadzoned(double controllerStickValue, double controllerStickDeadzone)
        {
            if (Math.Abs(controllerStickValue) < controllerStickDeadzone)
            {
                return 0.0;
            }
            else if (controllerStickValue >= controllerStickDeadzone)
            {
                return (controllerStickValue - controllerStickDeadzone) / (32768.0 - controllerStickDeadzone);
            }
            else
            {
                return (controllerStickValue + controllerStickDeadzone) / (32767.0 - controllerStickDeadzone);
            }
        }
    }
}
