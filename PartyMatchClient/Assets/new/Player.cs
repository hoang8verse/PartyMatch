using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
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

    bool isRunOnMobile = false;
    private Vector2 PlayerMouseInput;
    bool isMoving = false;
    public Camera cameraPlayer;
    public Transform planeTransform;
    public Transform tranformModel;

    void Start()
    {
        //audioSource = gameObject.GetComponent<AudioSource>();

        playerAnim = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        CheckDevice();
    }
    void CheckDevice()
    {
        if (Application.isMobilePlatform)
        {
            Debug.Log("Running on a mobile device.");
            isRunOnMobile = true;
        }
        else
        {
            Debug.Log("Running on a non-mobile device.");
            isRunOnMobile = false;
        }
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
            //if (objects.Length == 0)
            //{
            //    if (Vector3.Distance(transform.position, idlerandomObject[ran1].transform.position) > 1f)
            //    {
            //        transform.LookAt(idlerandomObject[ran1].transform);
            //        rigidbody.AddRelativeForce(Vector3.forward * 10, ForceMode.Force);
            //        playerAnim.SetBool("walk", true);
            //    }
            //    else
            //    {
            //        playerAnim.SetBool("walk", false);

            //        rigidbody.velocity = Vector3.zero;
            //        rigidbody.angularVelocity = Vector3.zero;
            //        ran1 = Random.Range(0, idlerandomObject.Length);

            //    }

            //}
            //else
            //{
            //    if (objects.Length == 1)
            //    {
            //        if (Vector3.Distance(transform.position, objects[0].transform.position) > 1f)
            //        {
            //            transform.LookAt(objects[0].transform);
            //            rigidbody.AddRelativeForce(Vector3.forward * 10, ForceMode.Force);
            //            playerAnim.SetBool("walk",true);
            //        }
            //        else
            //        {
            //            rigidbody.velocity = Vector3.zero;
            //            rigidbody.angularVelocity = Vector3.zero;
            //            playerAnim.SetBool("walk", false);

            //        }


            //    }
            //    else if (objects.Length == 2)
            //    {
            //        if (ran > 1)
            //        {
            //            ran = ran - 1;
            //        }
            //        if (Vector3.Distance(transform.position, objects[ran].transform.position) > 1f)
            //        {
            //            transform.LookAt(objects[ran].transform);
            //            rigidbody.AddRelativeForce(Vector3.forward * 10, ForceMode.Force);
            //            playerAnim.SetBool("walk", true);
            //        }
            //        else
            //        {
            //            rigidbody.velocity = Vector3.zero;
            //            rigidbody.angularVelocity = Vector3.zero;
            //            playerAnim.SetBool("walk", false);

            //        }
            //    }
            //    else if (objects.Length == 3)
            //    {

            //        if (Vector3.Distance(transform.position, objects[ran].transform.position) > 1f)
            //        {
            //            transform.LookAt(objects[ran].transform);
            //            rigidbody.AddRelativeForce(Vector3.forward * 10, ForceMode.Force);
            //            playerAnim.SetBool("walk", true);
            //        }
            //        else
            //        {
            //            rigidbody.velocity = Vector3.zero;
            //            rigidbody.angularVelocity = Vector3.zero;
            //            playerAnim.SetBool("walk", false);

            //        }
            //    }
            //}
        }
        else
        {
          //  rigidbody.AddRelativeForce(Vector3.down * 5, ForceMode.Force);

           
        }

        if (isRunOnMobile)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0); // trying to get the second touch input

                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log(" ======================== GetTouch mobile  down   ===========  ");
                    
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    //Debug.Log(" ================= GetTouch mobile  movingggggggg ===========  ");
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {

                    Debug.Log(" ======================== GetTouch mobile  uppppppppp ===========  ");
                }
            }
        }
        else
        {
            PlayerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            Debug.Log(" ======================== PlayerMouseInput ===========  " + PlayerMouseInput);
            if (Input.GetMouseButtonDown(0)) // 0 : left , 1 : right, 2 : wheel
            {
                Debug.Log(" ======================== GetMouseButtonDown ===========  ");
                isMoving = true;

            }
            else
            if (Input.GetMouseButton(0)) // 0 : left , 1 : right, 2 : wheel
            {
                //anim.Play("Walk");            
                //Debug.Log(" ======================== GetMouseButton movingggggggggggggggggggggggggg ===========  ");
            }
            else
            if (Input.GetMouseButtonUp(0))
            {
                isMoving = false;
                Debug.Log(" ======================== GetMouseButtonUp dasdadadadad ===========  ");
            }
        }

        if (isMoving)
        {
            Ray ray = cameraPlayer.ScreenPointToRay(Input.mousePosition);

            //Plane plane = new Plane(Vector3.up, planeTransform.position);
            //float distanceToPlane;
            //if (plane.Raycast(ray, out distanceToPlane))
            //{
            //    Vector3 hitPoint = ray.GetPoint(distanceToPlane);
            //    transform.position = Vector3.Lerp(transform.position, hitPoint, 10 * Time.deltaTime);
            //}

            Vector2 PlayerMovementInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput);
            //rig.velocity = new Vector3(MoveVector.x, rig.velocity.y, MoveVector.z) + currentGroundAdjustmentVelocity;
            float step = 12 * Time.deltaTime;
            //transform.position = Vector3.MoveTowards(transform.position, new Vector3(MoveVector.x, 0, MoveVector.z), step);
            tranformModel.LookAt(new Vector3(Input.mousePosition.x, tranformModel.position.y, Input.mousePosition.y));
            //rb.velocity = Vector3.forward*_speed;
            tranformModel.Translate(Vector3.forward * 1 * Time.fixedDeltaTime);
            Debug.Log(" ========================  PlayerMovementInput " + PlayerMovementInput);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit) && Input.GetKey(KeyCode.Mouse0))
            //{
            //    Vector3 _Pos = hit.point;
            //    Debug.Log(" ========================  _Pos " + _Pos);
            //    tranformModel.LookAt(new Vector3(_Pos.x, tranformModel.position.y, _Pos.z));
            //    //rb.velocity = Vector3.forward*_speed;
            //    tranformModel.Translate(Vector3.forward * 1 * Time.fixedDeltaTime);
            //}

            //transform.LookAt(objectPos);
            rigidbody.AddRelativeForce(Vector3.forward * 10, ForceMode.Force);
            playerAnim.SetBool("walk", true);
        } 
        else
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            playerAnim.SetBool("walk", false);
        }


        //if (Vector3.Distance(transform.position, objects[0].transform.position) > 1f)
        //{
        //    transform.LookAt(objects[0].transform);
        //    rigidbody.AddRelativeForce(Vector3.forward * 10, ForceMode.Force);
        //    playerAnim.SetBool("walk", true);
        //}
        //else
        //{
        //    rigidbody.velocity = Vector3.zero;
        //    rigidbody.angularVelocity = Vector3.zero;
        //    playerAnim.SetBool("walk", false);

        //}





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
            audioSource.clip = audioHitList[Random.Range(0, audioHitList.Length)];
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


