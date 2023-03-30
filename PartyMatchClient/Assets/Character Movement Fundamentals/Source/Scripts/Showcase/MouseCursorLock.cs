using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
	//This script provides simple mouse cursor locking functionality;
	public class MouseCursorLock : MonoBehaviour {

		//Whether to lock the mouse cursor at the start of the game;
		public bool lockCursorAtGameStart = true;

		//Key used to unlock mouse cursor;
		public KeyCode unlockKeyCode = KeyCode.Escape;

		//Key used to lock mouse cursor;
		public KeyCode lockKeyCode = KeyCode.Mouse0;

		//Start;
		void Start () {

			if(lockCursorAtGameStart)
			{
				ControlFreak2.CFCursor.lockState = CursorLockMode.Locked;
				ControlFreak2.CFCursor.visible = false;
			}
				
		}
		
		//Update;
		void Update () {

			if(ControlFreak2.CF2Input.GetKeyDown(unlockKeyCode))
			{
				ControlFreak2.CFCursor.lockState = CursorLockMode.None;
				ControlFreak2.CFCursor.visible = true;
			}
				
			if(ControlFreak2.CF2Input.GetKeyDown(lockKeyCode))
			{
				ControlFreak2.CFCursor.lockState = CursorLockMode.Locked;
				ControlFreak2.CFCursor.visible = false;
			}	
		}
	}
}
