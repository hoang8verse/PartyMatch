using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using System.Linq;

namespace KinematicCharacterController.Walkthrough.ClimbingLadders
{
    public class MyPlayer : MonoBehaviour
    {
        public ExampleCharacterCamera OrbitCamera;
        public Transform CameraFollowPoint;
        public MyCharacterController Character;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private void Start()
        {
            ControlFreak2.CFCursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            OrbitCamera.SetFollowTransform(CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            OrbitCamera.IgnoredColliders.Clear();
            OrbitCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            if (ControlFreak2.CF2Input.GetMouseButtonDown(0))
            {
                ControlFreak2.CFCursor.lockState = CursorLockMode.Locked;
            }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = ControlFreak2.CF2Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = ControlFreak2.CF2Input.GetAxisRaw(MouseXInput);
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (ControlFreak2.CFCursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = -ControlFreak2.CF2Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            // Apply inputs to the camera
            OrbitCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // Handle toggling zoom level
            if (ControlFreak2.CF2Input.GetMouseButtonDown(1))
            {
                OrbitCamera.TargetDistance = (OrbitCamera.TargetDistance == 0f) ? OrbitCamera.DefaultDistance : 0f;
            }
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = ControlFreak2.CF2Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = ControlFreak2.CF2Input.GetAxisRaw(HorizontalInput);
            characterInputs.CameraRotation = OrbitCamera.Transform.rotation;
            characterInputs.JumpDown = ControlFreak2.CF2Input.GetKeyDown(KeyCode.Space);
            characterInputs.CrouchDown = ControlFreak2.CF2Input.GetKeyDown(KeyCode.C);
            characterInputs.CrouchUp = ControlFreak2.CF2Input.GetKeyUp(KeyCode.C);
            characterInputs.ClimbLadder = ControlFreak2.CF2Input.GetKeyUp(KeyCode.E);

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }
    }
}