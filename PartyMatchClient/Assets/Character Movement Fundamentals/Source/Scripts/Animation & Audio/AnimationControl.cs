﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CMF
{
	//This script controls the character's animation by passing velocity values and other information ('isGrounded') to an animator component;
	public class AnimationControl : MonoBehaviour {
		public LevelManager levelmanager;
		public AudioClip[] audioHitList;
		Controller controller;
		Animator animator;
		Transform animatorTransform;
		Transform tr;
		public AudioSource audioSource;
		//Whether the character is using the strafing blend tree;
		public bool useStrafeAnimations = false;

		//Velocity threshold for landing animation;
		//Animation will only be triggered if downward velocity exceeds this threshold;
		public float landVelocityThreshold = 5f;

		private float smoothingFactor = 40f;
		Vector3 oldMovementVelocity = Vector3.zero;
		public bool stun=false;
		public Rigidbody rb;

		public Vector3 moveDirectionPush;
		Mover mover;
		void OnCollisionEnter(Collision other)
		{

			//if (other.gameObject.name == "AI")
			//{
			  

			//}

			if (other.gameObject.name == "AI")
			{
				audioSource.clip = audioHitList[Random.Range(0, audioHitList.Length)];
				audioSource.Play();
				moveDirectionPush = transform.position - other.transform.position;
				other.transform.GetComponent<Rigidbody>().AddForce(moveDirectionPush.normalized * 20);
				stun = true;
				animator.SetTrigger("stunned");
				
				StartCoroutine(waitbeforeforce());
			}
		}
		IEnumerator waitbeforeforce()
        {
			yield return new WaitForSeconds(0.1f);

			rb.AddForce(moveDirectionPush.normalized * 4000);

		}
		//Setup;
		void Awake () {
			audioSource = gameObject.GetComponent<AudioSource>();
			levelmanager.GetComponent<LevelManager>();

			rb = GetComponent<Rigidbody>();
			mover = GetComponent<Mover>();
			controller = GetComponent<Controller>();
			animator = GetComponentInChildren<Animator>();
			animatorTransform = animator.transform;

			tr = transform;
		}

		//OnEnable;
		void OnEnable()
		{
			//Connect events to controller events;
			controller.OnLand += OnLand;
			controller.OnJump += OnJump;
		}
	
		//OnDisable;
		void OnDisable()
		{
			//Disconnect events to prevent calls to disabled gameobjects;
			controller.OnLand -= OnLand;
			controller.OnJump -= OnJump;
		}
		IEnumerator waitbeforstun()
        {
			yield return new WaitForSeconds(0.5f);
			stun = false;
			print(stun);
		}
		//Update;
		void Update () {

			
				if (stun)
            {

				if (animator.GetCurrentAnimatorStateInfo(0).IsTag("getup"))
				{
					StartCoroutine(waitbeforstun());
				}
			}
			else
            {
				//Get controller velocity;
				Vector3 _velocity = controller.GetVelocity();

				//Split up velocity;
				Vector3 _horizontalVelocity = VectorMath.RemoveDotVector(_velocity, tr.up);
				Vector3 _verticalVelocity = _velocity - _horizontalVelocity;

				//Smooth horizontal velocity for fluid animation;
				_horizontalVelocity = Vector3.Lerp(oldMovementVelocity, _horizontalVelocity, smoothingFactor * Time.deltaTime);
				oldMovementVelocity = _horizontalVelocity;

				//animator.SetFloat("VerticalSpeed", _verticalVelocity.magnitude * VectorMath.GetDotProduct(_verticalVelocity.normalized, tr.up));
				//animator.SetFloat("HorizontalSpeed", _horizontalVelocity.magnitude);

				//If animator is strafing, split up horizontal velocity;
				if (useStrafeAnimations)
				{
					Vector3 _localVelocity = animatorTransform.InverseTransformVector(_horizontalVelocity);
					//animator.SetFloat("ForwardSpeed", _localVelocity.z);
					//animator.SetFloat("StrafeSpeed", _localVelocity.x);

				}
				if (levelmanager.winbool)
				{
					animator.Play("dance");
				}
				else
				{
					if (mover.IsGrounded() == true)
					{
						if (_velocity.magnitude > 0)
						{
							animator.SetBool("walk",true);

						}
						else
						{
							animator.SetBool("walk", false);

						}
					}
					//else
					//{
					//	animator.Play("jump");

					//}
				}


			}




			//Pass values to animator;
			//animator.SetBool("IsGrounded", controller.IsGrounded());
			//animator.SetBool("IsStrafing", useStrafeAnimations);
		}

	
		void OnLand(Vector3 _v)
		{
			//Only trigger animation if downward velocity exceeds threshold;
			if(VectorMath.GetDotProduct(_v, tr.up) > -landVelocityThreshold)
				return;

			animator.SetTrigger("OnLand");
		}

		void OnJump(Vector3 _v)
		{
			
		}
	}
}
