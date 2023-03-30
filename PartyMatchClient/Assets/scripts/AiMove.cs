using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMove : MonoBehaviour
{
    public AudioClip[] audioHitList;


    Rigidbody m_Rigidbody;
    public float m_Speed = 5f;
    public int ran;
    public GameObject[] objects;
    public GameObject[] idlerandomObject;

    private Animator playerAnim;
    Rigidbody rigidbody;
    int ran1;
    bool isgrounded = true;
    RaycastHit hit;
    Vector3 dir = new Vector3(0, -1);
    bool gothit;
    public Transform hiteffect;
    public Transform hiteffect1;
    public Transform deadEffect;
    public AudioSource audioSource;

    public Transform spawnhiteffectposition;

    // Start is called before the first frame update

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        playerAnim = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame

    [ContextMenu("ToggleDead")]
    private void Update()
    {
        objects = GameObject.FindGameObjectsWithTag("go");
       
    }


    void FixedUpdate()
    {
        if (isgrounded == true)
        {
            if (objects.Length == 0)
            {
                if (Vector3.Distance(transform.position, idlerandomObject[ran1].transform.position) > 1f)
                {
                    transform.LookAt(idlerandomObject[ran1].transform);
                    rigidbody.AddRelativeForce(Vector3.forward * 10, ForceMode.Force);
                    playerAnim.SetBool("walk", true);
                }
                else
                {
                    playerAnim.SetBool("walk", false);

                    rigidbody.velocity = Vector3.zero;
                    rigidbody.angularVelocity = Vector3.zero;
                    ran1 = Random.Range(0, idlerandomObject.Length);

                }

            }
            else
            {
                if (objects.Length == 1)
                {
                    if (Vector3.Distance(transform.position, objects[0].transform.position) > 1f)
                    {
                        transform.LookAt(objects[0].transform);
                        rigidbody.AddRelativeForce(Vector3.forward * 10, ForceMode.Force);
                        playerAnim.SetBool("walk",true);
                    }
                    else
                    {
                        rigidbody.velocity = Vector3.zero;
                        rigidbody.angularVelocity = Vector3.zero;
                        playerAnim.SetBool("walk", false);

                    }


                }
                else if (objects.Length == 2)
                {
                    if (ran > 1)
                    {
                        ran = ran - 1;
                    }
                    if (Vector3.Distance(transform.position, objects[ran].transform.position) > 1f)
                    {
                        transform.LookAt(objects[ran].transform);
                        rigidbody.AddRelativeForce(Vector3.forward * 10, ForceMode.Force);
                        playerAnim.SetBool("walk", true);
                    }
                    else
                    {
                        rigidbody.velocity = Vector3.zero;
                        rigidbody.angularVelocity = Vector3.zero;
                        playerAnim.SetBool("walk", false);

                    }
                }
                else if (objects.Length == 3)
                {

                    if (Vector3.Distance(transform.position, objects[ran].transform.position) > 1f)
                    {
                        transform.LookAt(objects[ran].transform);
                        rigidbody.AddRelativeForce(Vector3.forward * 10, ForceMode.Force);
                        playerAnim.SetBool("walk", true);
                    }
                    else
                    {
                        rigidbody.velocity = Vector3.zero;
                        rigidbody.angularVelocity = Vector3.zero;
                        playerAnim.SetBool("walk", false);

                    }
                }
            }
        }
        else
        {
          //  rigidbody.AddRelativeForce(Vector3.down * 5, ForceMode.Force);

           
        }






    }
     
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Contains("1") || other.gameObject.name.Contains("2") || other.gameObject.name.Contains("3") || other.gameObject.name.Contains("4") || other.gameObject.name.Contains("5") || other.gameObject.name.Contains("6") || other.gameObject.name.Contains("7") || other.gameObject.name.Contains("8") || other.gameObject.name.Contains("9") || other.gameObject.name.Contains("10") || other.gameObject.name.Contains("11") || other.gameObject.name.Contains("12") || other.gameObject.name.Contains("13") || other.gameObject.name.Contains("14") || other.gameObject.name.Contains("15") || other.gameObject.name.Contains("16"))
        {

            //code here } } 
            isgrounded = true;
            Debug.Log("Collided with floor");

        }
        else
        {
            isgrounded = false;

        }

        if(other.gameObject.tag=="dead")
        {
            Instantiate(deadEffect, new Vector3(spawnhiteffectposition.position.x, spawnhiteffectposition.position.y, spawnhiteffectposition.position.z), Quaternion.identity);

            Destroy(this.gameObject);
        }
        if (other.gameObject.name == "AI" || other.gameObject.name == "Player")
        {
            audioSource.clip = audioHitList[0];
            audioSource.Play();
            moveDirectionPush = rigidbody.transform.position - other.transform.position;
            rigidbody.AddForce(moveDirectionPush.normalized * 100);
            playerAnim.Play("hit");
            Instantiate(hiteffect, new Vector3(spawnhiteffectposition.position.x, spawnhiteffectposition.position.y, spawnhiteffectposition.position.z), Quaternion.identity);
            Instantiate(hiteffect1, new Vector3(spawnhiteffectposition.position.x, spawnhiteffectposition.position.y, spawnhiteffectposition.position.z), Quaternion.identity);


        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.name.Contains("1") || other.gameObject.name.Contains("2") || other.gameObject.name.Contains("3") || other.gameObject.name.Contains("4") || other.gameObject.name.Contains("5") || other.gameObject.name.Contains("6") || other.gameObject.name.Contains("7") || other.gameObject.name.Contains("8") || other.gameObject.name.Contains("9") || other.gameObject.name.Contains("10") || other.gameObject.name.Contains("11") || other.gameObject.name.Contains("12") || other.gameObject.name.Contains("13") || other.gameObject.name.Contains("14") || other.gameObject.name.Contains("15") || other.gameObject.name.Contains("16"))

        {
            isgrounded = true;

        }
        if(other.gameObject.name=="AI"  || other.gameObject.name == "Player")
        {
            moveDirectionPush = rigidbody.transform.position - other.transform.position;
            rigidbody.AddForce(moveDirectionPush.normalized * 100);
            playerAnim.Play("hit");
        }
      


    }

   

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.name.Contains("1") || other.gameObject.name.Contains("2") || other.gameObject.name.Contains("3") || other.gameObject.name.Contains("4") || other.gameObject.name.Contains("5") || other.gameObject.name.Contains("6") || other.gameObject.name.Contains("7") || other.gameObject.name.Contains("8") || other.gameObject.name.Contains("9") || other.gameObject.name.Contains("10") || other.gameObject.name.Contains("11") || other.gameObject.name.Contains("12") || other.gameObject.name.Contains("13") || other.gameObject.name.Contains("14") || other.gameObject.name.Contains("15") || other.gameObject.name.Contains("16"))

        {
            isgrounded = false;

        }
    }
  
    public Vector3 moveDirectionPush;


}


