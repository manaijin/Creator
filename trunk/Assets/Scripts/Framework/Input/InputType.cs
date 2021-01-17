using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class InputType
    {
        /// <summary>
        /// 按键状态
        /// </summary>
        public enum KeyState
        {
            isPressed = 0,
            wasPressedThisFrame = 1,
            wasReleasedThisFrame = 2,
            wasDoublePressedThisFrame = 4,
        }

        /// <summary>
        /// 鼠标按键
        /// </summary>
        public enum MouseButton
        {
            leftButton = 0,
            middleButton = 1,
            rightButton = 2,
            backButton = 3,
            forwardButton = 4,
        }
    }
}
