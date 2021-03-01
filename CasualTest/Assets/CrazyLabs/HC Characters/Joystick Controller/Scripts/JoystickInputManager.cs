using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyLabs
{
    public class JoystickInputManager : MonoBehaviour
    {
        public static Action<Vector2> OnJoystickMoved = delegate { };

        public JoystickConfig _config;

        private Vector3 _mouseStartPosition;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouseStartPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                HandleJoystickControls();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnJoystickMoved.Invoke(Vector2.zero);
            }
        }

        private void HandleJoystickControls()
        {
            float vectorLength = (Input.mousePosition - _mouseStartPosition).magnitude / Screen.height;
            float value = Mathf.InverseLerp(0, _config.joystickSize - _config.joystickDeadzone, Mathf.Clamp01(vectorLength - _config.joystickDeadzone));

            OnJoystickMoved.Invoke((Input.mousePosition - _mouseStartPosition).normalized * value);
        }
    }
}