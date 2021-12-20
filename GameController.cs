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
        private double LeftStickDeadzone { get; set; }
        public double RightStickX { get; set; }
        public double RightStickY { get; set; }
        private double RightStickDeadzone { get; set; }

        //Triggers
        public double LeftTrigger { get; set; }
        private double LeftTriggerDeadzone { get; set; }
        public double RightTrigger { get; set; }
        private double RightTriggerDeadzone { get; set; }


        //Buttons
        public GameControllerButton ButtonA { get; set; }
        public GameControllerButton ButtonB { get; set; }
        public GameControllerButton ButtonX { get; set; }
        public GameControllerButton ButtonY { get; set; }
        public GameControllerButton LeftThumb { get; set; }
        public GameControllerButton RightThumb { get; set; }
        public GameControllerButton LeftShoulder { get; set; }
        public GameControllerButton RightShoulder { get; set; }


        //Constructor
        public GameController(UserIndex userIndex, double stickDeadzones, double triggerDeadzones) : base(userIndex)
        {
            //Sticks
            LeftStickDeadzone = stickDeadzones;
            RightStickDeadzone = stickDeadzones;

            //Triggers
            LeftTriggerDeadzone = triggerDeadzones;
            RightTriggerDeadzone = triggerDeadzones;

            //Buttons
            ButtonA = new GameControllerButton();
            ButtonB = new GameControllerButton();
            ButtonX = new GameControllerButton();
            ButtonY = new GameControllerButton();
            LeftThumb = new GameControllerButton();
            RightThumb = new GameControllerButton();
            LeftShoulder = new GameControllerButton();
            RightShoulder = new GameControllerButton();
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
            this.LeftThumb.Update(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb));
            this.RightThumb.Update(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb));
            this.LeftShoulder.Update(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder));
            this.RightShoulder.Update(state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder));

            //Sticks
            this.LeftStickX = DeadzonedAnalogInput(state.Gamepad.LeftThumbX, this.LeftStickDeadzone);
            this.LeftStickY = DeadzonedAnalogInput(state.Gamepad.LeftThumbY, this.LeftStickDeadzone);
            this.RightStickX = DeadzonedAnalogInput(state.Gamepad.RightThumbX, this.RightStickDeadzone);
            this.RightStickY = DeadzonedAnalogInput(state.Gamepad.RightThumbY, this.RightStickDeadzone);

            //Triggers
            this.LeftTrigger = DeadzonedAnalogInput(state.Gamepad.LeftTrigger, this.LeftTriggerDeadzone);
            this.RightTrigger = DeadzonedAnalogInput(state.Gamepad.RightTrigger, this.RightTriggerDeadzone);
        }


        //Get the controller stick value with deadzoning applied
        private double DeadzonedAnalogInput(double controllerStickValue, double controllerStickDeadzone)
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
