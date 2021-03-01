using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyLabs
{
    [CreateAssetMenu(menuName = ("CrazyLabs/Joystick Config"), fileName = "JoystickConfig")]
    public class JoystickConfig : ScriptableObject
    {
        public float joystickSize     = 0.075f;
        public float joystickDeadzone = 0.025f;
    }
}