using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using System.Linq;

namespace KinematicCharacterController.Walkthrough.PlayerCameraCharacterSetup
{
    public class MyPlayer : MonoBehaviour
    {
        public ExampleCharacterCamera OrbitCamera;
        public Transform CameraFollowPoint;
        public MyCharacterController Character;

        private Vector3 _lookInputVector = Vector3.zero;

        private void Start()
        {
            ControlFreak2.CFCursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            OrbitCamera.SetFollowTransform(CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            OrbitCamera.IgnoredColliders = Character.GetComponentsInChildren<Collider>().ToList();
        }

        private void Update()
        {
            if (ControlFreak2.CF2Input.GetMouseButtonDown(0))
            {
                ControlFreak2.CFCursor.lockState = CursorLockMode.Locked;
            }
        }

        private void LateUpdate()
        {
            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = ControlFreak2.CF2Input.GetAxisRaw("Mouse Y");
            float mouseLookAxisRight = ControlFreak2.CF2Input.GetAxisRaw("Mouse X");
            _lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (ControlFreak2.CFCursor.lockState != CursorLockMode.Locked)
            {
                _lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = -ControlFreak2.CF2Input.GetAxis("Mouse ScrollWheel");
    #if UNITY_WEBGL
            scrollInput = 0f;
    #endif

            // Apply inputs to the camera
            OrbitCamera.UpdateWithInput(Time.deltaTime, scrollInput, _lookInputVector);

            // Handle toggling zoom level
            if (ControlFreak2.CF2Input.GetMouseButtonDown(1))
            {
                OrbitCamera.TargetDistance = (OrbitCamera.TargetDistance == 0f) ? OrbitCamera.DefaultDistance : 0f;
            }
        }
    }
}