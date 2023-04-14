using CMF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : MonoBehaviour
{
    public Vector3 moveDirectionPush;

    public Animator playerAnim;
    Rigidbody rigidbody;
    public AudioSource audioSource;

    public Transform spawnhiteffectposition;

    public GameObject[] characters;
    public bool isMoving = false;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        playerAnim = GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }
    public Vector3 velocity = Vector3.zero;
    public Vector3 pos = Vector3.zero;
    public Quaternion rot = Quaternion.identity;

    public Transform target;
    public float speed = 5f;

    public float turnSpeed = 500f;
    //Current (local) rotation around the (local) y axis of this gameobject;
    float currentYRotation = 0f;

    //If the angle between the current and target direction falls below 'fallOffAngle', 'turnSpeed' becomes progressively slower (and eventually approaches '0f');
    //This adds a smoothing effect to the rotation;
    float fallOffAngle = 90f;
    void Update()
    {
        //if (Vector3.Distance(transform.position, pos) > 1f)
        //{
        //    float step = speed * Time.deltaTime;
        //    transform.position = Vector3.MoveTowards(transform.position, pos, step);
        //    transform.rotation = Quaternion.Lerp(transform.rotation, rot, step);
        //    playerAnim.SetBool("walk", true);
        //} 
        //else
        //{
        //    playerAnim.SetBool("walk", false);
        //}

        if (velocity.magnitude > 0)
        {
            isMoving = true;
            playerAnim.SetBool("walk", true);
            
            rigidbody.velocity = velocity;

            //Normalize velocity direction;
            //velocity.Normalize();

            //Get current 'forward' vector;
            Vector3 _currentForward = transform.forward;

            //Calculate (signed) angle between velocity and forward direction;
            float _angleDifference = VectorMath.GetAngle(_currentForward, velocity, transform.up);

            //Calculate angle factor;
            float _factor = Mathf.InverseLerp(0f, fallOffAngle, Mathf.Abs(_angleDifference));

            //Calculate this frame's step;
            float _step = Mathf.Sign(_angleDifference) * _factor * Time.deltaTime * turnSpeed;

            //Clamp step;
            if (_angleDifference < 0f && _step < _angleDifference)
                _step = _angleDifference;
            else if (_angleDifference > 0f && _step > _angleDifference)
                _step = _angleDifference;

            //Add step to current y angle;
            currentYRotation += _step;

            //Clamp y angle;
            if (currentYRotation > 360f)
                currentYRotation -= 360f;
            if (currentYRotation < -360f)
                currentYRotation += 360f;

            //Set transform rotation using Quaternion.Euler;
            transform.localRotation = Quaternion.Euler(0f, currentYRotation, 0f);
        }
        else
        {
            isMoving = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            playerAnim.SetBool("walk", false);

        }

    }

    public void SetVelocity(Vector3 _velocity)
    {
        velocity = _velocity;

    }
    public void SetAnimHit()
    {
        playerAnim.SetTrigger("hit");
    }
    public void SetAnimStunned()
    {
        playerAnim.SetTrigger("stunned");
    }

    public void SetActiveCharacter(int index)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (i == index)
            {
                characters[index].SetActive(true);
            }
            else
            {
                characters[i].SetActive(false);
            }

        }

    }

    //void FixedUpdate()
    //{
    //    if (Vector3.Distance(transform.position, pos) > 1f)
    //    {
    //        //transform.LookAt(idlerandomObject[ran1].transform);
    //        rigidbody.AddRelativeForce(Vector3.forward * 10 * Time.fixedDeltaTime, ForceMode.Force);
    //        playerAnim.SetBool("walk", true);
    //    }
    //    else
    //    {
    //        playerAnim.SetBool("walk", false);

    //        rigidbody.velocity = Vector3.zero;
    //        rigidbody.angularVelocity = Vector3.zero;

    //    }

    //}

    //void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.name.Contains("1") || other.gameObject.name.Contains("2") || other.gameObject.name.Contains("3") || other.gameObject.name.Contains("4") || other.gameObject.name.Contains("5") || other.gameObject.name.Contains("6") || other.gameObject.name.Contains("7") || other.gameObject.name.Contains("8") || other.gameObject.name.Contains("9") || other.gameObject.name.Contains("10") || other.gameObject.name.Contains("11") || other.gameObject.name.Contains("12") || other.gameObject.name.Contains("13") || other.gameObject.name.Contains("14") || other.gameObject.name.Contains("15") || other.gameObject.name.Contains("16"))
    //    {

    //        //code here } } 
    //        isgrounded = true;
    //        Debug.Log("Collided with floor");

    //    }
    //    else
    //    {
    //        isgrounded = false;

    //    }

    //    if(other.gameObject.tag=="dead")
    //    {
    //        Instantiate(deadEffect, new Vector3(spawnhiteffectposition.position.x, spawnhiteffectposition.position.y, spawnhiteffectposition.position.z), Quaternion.identity);

    //        Destroy(this.gameObject);
    //    }
    //    if (other.gameObject.name == "AI" || other.gameObject.name == "Player")
    //    {
    //        audioSource.clip = audioHitList[0];
    //        audioSource.Play();
    //        moveDirectionPush = rigidbody.transform.position - other.transform.position;
    //        rigidbody.AddForce(moveDirectionPush.normalized * 100);
    //        playerAnim.Play("hit");
    //        Instantiate(hiteffect, new Vector3(spawnhiteffectposition.position.x, spawnhiteffectposition.position.y, spawnhiteffectposition.position.z), Quaternion.identity);
    //        Instantiate(hiteffect1, new Vector3(spawnhiteffectposition.position.x, spawnhiteffectposition.position.y, spawnhiteffectposition.position.z), Quaternion.identity);


    //    }
    //}

    //void OnCollisionStay(Collision other)
    //{
    //    if (other.gameObject.name.Contains("1") || other.gameObject.name.Contains("2") || other.gameObject.name.Contains("3") || other.gameObject.name.Contains("4") || other.gameObject.name.Contains("5") || other.gameObject.name.Contains("6") || other.gameObject.name.Contains("7") || other.gameObject.name.Contains("8") || other.gameObject.name.Contains("9") || other.gameObject.name.Contains("10") || other.gameObject.name.Contains("11") || other.gameObject.name.Contains("12") || other.gameObject.name.Contains("13") || other.gameObject.name.Contains("14") || other.gameObject.name.Contains("15") || other.gameObject.name.Contains("16"))

    //    {
    //        isgrounded = true;

    //    }
    //    if(other.gameObject.name=="AI"  || other.gameObject.name == "Player")
    //    {
    //        moveDirectionPush = rigidbody.transform.position - other.transform.position;
    //        rigidbody.AddForce(moveDirectionPush.normalized * 100);
    //        playerAnim.Play("hit");
    //    }



    //}



    //void OnCollisionExit(Collision other)
    //{
    //    if (other.gameObject.name.Contains("1") || other.gameObject.name.Contains("2") || other.gameObject.name.Contains("3") || other.gameObject.name.Contains("4") || other.gameObject.name.Contains("5") || other.gameObject.name.Contains("6") || other.gameObject.name.Contains("7") || other.gameObject.name.Contains("8") || other.gameObject.name.Contains("9") || other.gameObject.name.Contains("10") || other.gameObject.name.Contains("11") || other.gameObject.name.Contains("12") || other.gameObject.name.Contains("13") || other.gameObject.name.Contains("14") || other.gameObject.name.Contains("15") || other.gameObject.name.Contains("16"))

    //    {
    //        isgrounded = false;

    //    }
    //}




}


