using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
	//This character movement input class is an example of how to get input from a keyboard to control the character;
    public class CharacterKeyboardInput : CharacterInput
    {
		public string horizontalInputAxis = "Horizontal";
		public string verticalInputAxis = "Vertical";
		public KeyCode jumpKey = KeyCode.Space;

		//If this is enabled, Unity's internal input smoothing is bypassed;
		public bool useRawInput = true;
		public bool isMoving = false;

		public override float GetHorizontalMovementInput()
		{

   //         Vector3 direction = this.transform.position - new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
			//Vector3 MoveVector = transform.TransformDirection(new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"))) * 100;
			//if (Input.touchCount > 0)
			//{
			//	Touch touch = Input.GetTouch(0);
			//	if (touch.phase == TouchPhase.Began)
			//	{
			//		// Touch began, do something
			//	}
			//	else if (touch.phase == TouchPhase.Moved)
			//	{
			//		Vector3 touchPosition = Input.GetTouch(0).position;
			//		touchPosition = cameraPlayer.ScreenToWorldPoint(touchPosition);
   //                 // Touch moved, do something else
   //                 MoveVector = transform.TransformDirection(new Vector3(touchPosition.x, 0, touchPosition.y)) * 100;
			//		float touchDelta = touch.deltaPosition.x;
			//		float mouseDelta = touchDelta * 1000;
			//		Debug.Log("GetHorizontalMovementInput =============  " + mouseDelta);
			//		return mouseDelta * 1000 * Time.fixedDeltaTime;
			//	}
			//	else if (touch.phase == TouchPhase.Ended)
			//	{
			//		// Touch ended, do another thing
			//	}
			//}
			//float angle = direction.normalized.x;
   //         Debug.Log("GetHorizontalMovementInput =============  " + angle);
   //         if (isMoving)
   //         {
   //             return MoveVector.x;
   //             //return Input.GetAxis("Mouse X") * 1000 * Time.fixedDeltaTime;
   //         }
   //         else
   //         {
   //             return 0;
   //         }

            if (useRawInput)
            {
				float _h = ControlFreak2.CF2Input.GetAxisRaw(horizontalInputAxis);
				Debug.Log("GetHorizontalMovementInput =============  " + _h);
				return _h;
			}
			else
				return ControlFreak2.CF2Input.GetAxis(horizontalInputAxis);
		}

		public override float GetVerticalMovementInput()
		{
   //         Vector3 direction = this.transform.position - new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
   //         float angle = direction.normalized.y;
			//Vector3 MoveVector = transform.TransformDirection(new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"))) * 100;
			
			//if (Input.touchCount > 0)
			//{
			//	Touch touch = Input.GetTouch(0);
			//	if (touch.phase == TouchPhase.Began)
			//	{
			//		// Touch began, do something
			//	}
			//	else if (touch.phase == TouchPhase.Moved)
			//	{
			//		// Touch moved, do something else
			//		Vector3 touchPosition = Input.GetTouch(0).position;
			//		touchPosition = cameraPlayer.ScreenToWorldPoint(touchPosition);
			//		// Touch moved, do something else
			//		MoveVector = transform.TransformDirection(new Vector3(touchPosition.x, 0, touchPosition.y)) * 100;
			//		float touchDelta = touch.deltaPosition.y;
			//		float mouseDelta = touchDelta * 1000;
			//		Debug.Log("GetVerticalMovementInput =============  " + mouseDelta);
			//		return mouseDelta * 1000 * Time.fixedDeltaTime;
			//	}
			//	else if (touch.phase == TouchPhase.Ended)
			//	{
			//		// Touch ended, do another thing
			//	}
			//}
			//if (isMoving)
   //         {
   //             return MoveVector.z;
   //             //return Input.GetAxis("Mouse Y") * 1000 * Time.fixedDeltaTime;
   //         }
   //         else
   //         {
   //             return 0;
   //         }

            if (useRawInput)
            {
				float _v = ControlFreak2.CF2Input.GetAxisRaw(verticalInputAxis);
				Debug.Log("GetVerticalMovementInput =============  " + _v);
				return _v;
			}
				
			else
				return ControlFreak2.CF2Input.GetAxis(verticalInputAxis);
		}

		public override bool IsJumpKeyPressed()
		{
			return ControlFreak2.CF2Input.GetKey(jumpKey);
		}

        private void Update()
        {
			//if (Input.touchCount > 0)
			//{
			//	Touch touch = Input.GetTouch(0);
			//	if (touch.phase == TouchPhase.Began)
			//	{
			//		// Touch began, do something
			//	}
			//	else if (touch.phase == TouchPhase.Moved)
			//	{
			//		// Touch moved, do something else
			//	}
			//	else if (touch.phase == TouchPhase.Ended)
			//	{
			//		// Touch ended, do another thing
			//	}
			//}
			//if (Input.GetMouseButtonDown(0)) // 0 : left , 1 : right, 2 : wheel
			//{
			//	Debug.Log(" ======================== GetMouseButtonDown ===========  ");
			//	isMoving = true;

			//}
			//else
			//if (Input.GetMouseButton(0)) // 0 : left , 1 : right, 2 : wheel
			//{
			//	isMoving = true;
			//	//anim.Play("Walk");            
			//	//Debug.Log(" ======================== GetMouseButton movingggggggggggggggggggggggggg ===========  ");
			//}
			//else
			//if (Input.GetMouseButtonUp(0))
			//{
			//	isMoving = false;
			//	Debug.Log(" ======================== GetMouseButtonUp dasdadadadad ===========  ");
			//}
		}
    }
}
