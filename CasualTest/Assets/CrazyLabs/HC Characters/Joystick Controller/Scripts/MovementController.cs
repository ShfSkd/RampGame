using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrazyLabs
{
    public class MovementController : MonoBehaviour
    {
        public float movementSpeed = 4f;
        public float rotationSpeed = 8f;

        private Transform  _transformCached;
        private Quaternion _targetRotation;
        private Animator   _animator;

        private void Awake()
        {
            _targetRotation  = Quaternion.identity;
            _transformCached = transform;
            _animator        = GetComponent<Animator>();

            JoystickInputManager.OnJoystickMoved += Move;
        }

        private void OnDestroy()
        {
            JoystickInputManager.OnJoystickMoved -= Move;
        }

        void Update()
        {
            _transformCached.rotation = Quaternion.Lerp(_transformCached.rotation, _targetRotation, Time.deltaTime * rotationSpeed);
        }

        public void Move(Vector2 inputVector)
        {
            _transformCached.Translate((Vector3.right * inputVector.x + Vector3.forward * inputVector.y) * movementSpeed * Time.deltaTime, Space.World);

            if (inputVector.sqrMagnitude >= 0.1f)
                _targetRotation = Quaternion.Euler(0, Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg, 0);

            if (_animator)
                _animator.SetFloat("walkSpeed", inputVector.magnitude);
        }
    }
}