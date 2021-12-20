using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class GameControllerButton
    {
        public bool IsPressed { get; set; }
        public bool JustPressed { get; set; }

        public GameControllerButton()
        {
            IsPressed = false;
            JustPressed = JustPressed;
        }

        public void Update(bool state)
        {
            if (state)
            {
                if (IsPressed)
                {
                    JustPressed = false;
                } 
                else
                {
                    JustPressed = true;
                }
                IsPressed = true;
            }
            else
            {
                JustPressed = false;
                IsPressed = false;
            }
        }
    }
}
