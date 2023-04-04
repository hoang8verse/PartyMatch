using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : MonoBehaviour
{
    public Vector3 moveDirectionPush;
    private Animator playerAnim;
    Rigidbody rigidbody;
    public AudioSource audioSource;

    public Transform spawnhiteffectposition;

    // Start is called before the first frame update

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        playerAnim = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }
    public Vector3 pos = Vector3.zero;
    public Quaternion rot = Quaternion.identity;

    public Transform target;
    public float speed = 5f;

    void Update()
    {
        if (Vector3.Distance(transform.position, pos) > 1f)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, pos, step);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, step);
            playerAnim.SetBool("walk", true);
        } 
        else
        {
            playerAnim.SetBool("walk", false);
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


