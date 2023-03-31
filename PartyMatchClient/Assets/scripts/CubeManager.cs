using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public static CubeManager instance;

    public GameObject[] CubeMeshs;
    public List<GameObject> cubemesh2 = new List<GameObject>();

    public Material targetmateriel;
    public Material[] materiellist;
    public GameObject boardMateriel;
    public Material[] nogomateriel;

    public Material blue;
    public Vector3[] spawnpoints;
    public Quaternion[] spawnpointsR;
    public GameObject cam1;
    public GameObject cam2;


    int target;
    int ran1;
    int ran2;
    int ran3;

    public bool backPosition;
    public LevelManager levelmanager;

    public GameObject counter;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        levelmanager.GetComponent<LevelManager>();

        for (int i = 0; i < CubeMeshs.Length; i++)
        {
            spawnpoints[i] = CubeMeshs[i].transform.position;
            spawnpointsR[i] = CubeMeshs[i].transform.localRotation;

        }
       // counter.SetActive(true);

        //Invoke("selectCube", 3);
        Invoke("SendRequestRandomTarget", 3);
    }

    public void SendRequestRandomTarget()
    {
        int _target = Random.Range(0, materiellist.Length);
        int _ran1 = Random.Range(0, CubeMeshs.Length);
        int _ran2 = Random.Range(0, CubeMeshs.Length);
        int _ran3 = Random.Range(0, CubeMeshs.Length);
        SocketClient.instance.OnRequestRandomTarget(_target, _ran1, _ran2, _ran3);
    }

    public void PerformCube(int _target, int _ran1, int _ran2, int _ran3)
    {
        target = _target;
        ran1 = _ran1;
        ran2 = _ran2;
        ran3 = _ran3;
        selectCube();
    }

    void selectCube()
    {
        if (!levelmanager.winbool)
        {
            //targetmateriel = materiellist[Random.Range(0, materiellist.Length)];
            targetmateriel = materiellist[target];
            boardMateriel.GetComponent<MeshRenderer>().material = targetmateriel;

            cubemesh2.Clear();
            //ran1 = Random.Range(0, CubeMeshs.Length);
            //ran2 = Random.Range(0, CubeMeshs.Length);
            //ran3 = Random.Range(0, CubeMeshs.Length);

            for (int i = 0; i < CubeMeshs.Length; i++)
            {
                cubemesh2.Add(CubeMeshs[i]);
            }

            CubeMeshs[ran1].GetComponent<MeshRenderer>().material = targetmateriel;
            CubeMeshs[ran2].GetComponent<MeshRenderer>().material = targetmateriel;
            CubeMeshs[ran3].GetComponent<MeshRenderer>().material = targetmateriel;

            cubemesh2.Remove(CubeMeshs[ran1]);
            cubemesh2.Remove(CubeMeshs[ran2]);
            cubemesh2.Remove(CubeMeshs[ran3]);

            for (int i = 0; i < cubemesh2.Count; i++)
            {


                cubemesh2[i].GetComponent<MeshRenderer>().material = nogomateriel[Random.Range(0, nogomateriel.Length)];
            }

            CubeMeshs[ran1].transform.tag = "go";
            CubeMeshs[ran2].transform.tag = "go";
            CubeMeshs[ran3].transform.tag = "go";





            print("selected number is " + ran1);
            print("selected number is " + ran2);
            print("selected number is " + ran3);

            if (LevelManager.instance.winbool)
            {
                counter.SetActive(false);



            }
            else
            {
                StartCoroutine(cubefall());


            }

        }
           


    }

    IEnumerator cubefall()
    {
        yield return new WaitForSeconds(1);

        counter.SetActive(false);

        yield return new WaitForSeconds(3);


        foreach (GameObject item in CubeMeshs)
        {
            if (item.name== CubeMeshs[ran1].name || item.name == CubeMeshs[ran2].name || item.name == CubeMeshs[ran3].name)
            {
                print("not falling "+item.name);

            }else

            {
                item.GetComponent<Rigidbody>().isKinematic = false;
                item.GetComponent<Rigidbody>().useGravity = true;
                print("falling");
            }
        }
        for (int i = 0; i < CubeMeshs.Length; i++)

        {
            if(CubeMeshs[i].transform.position == spawnpoints[i])
            {
                backPosition = true;

            }



        }
        yield return new WaitForSeconds(3);
        StartCoroutine(resteCube());
        

    }

   IEnumerator resteCube()
    {

        yield return new WaitForSeconds(3);

        foreach (GameObject item in CubeMeshs)
        {
            item.GetComponent<MeshRenderer>().material = blue;

            print("materiel is " + blue);
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Rigidbody>().useGravity = false;
            CubeMeshs[ran1].transform.tag = "ngo";
            CubeMeshs[ran2].transform.tag = "ngo";
            CubeMeshs[ran3].transform.tag = "ngo";

            if (ran1 == ran2)
            {
                if (ran2 > CubeMeshs.Length && ran2 > 0)
                {

                    CubeMeshs[ran2 - 1].transform.tag = "ngo";


                }
                else
                {
                    CubeMeshs[ran2 + 1].transform.tag = "ngo";
                }
            }
            else
            {
                CubeMeshs[ran2].transform.tag = "ngo";

            }

            backPosition = false;

        }
        yield return new WaitForSeconds(3.5f);
       
        if (LevelManager.instance.winbool)
        {
            counter.SetActive(false);

        }
        else
        {

            counter.SetActive(true);
            yield return new WaitForSeconds(4);

            SendRequestRandomTarget();
            //selectCube();
        }
        //  StartCoroutine(SmoothTranlation());

    }
    private void FixedUpdate()
    {
        if(!backPosition)
        {
            for (int i = 0; i < CubeMeshs.Length; i++)
            {
                CubeMeshs[i].transform.position = Vector3.Lerp(CubeMeshs[i].transform.position, spawnpoints[i], Time.deltaTime * 10);
                CubeMeshs[i].transform.rotation = Quaternion.Lerp(CubeMeshs[i].transform.localRotation, spawnpointsR[i], Time.deltaTime * 10);


            }
        }
        if(LevelManager.instance.winbool)
        {
            cam1.SetActive(false);
            cam2.SetActive(true);
            return;
        }
    }
    IEnumerator SmoothTranlation()
 
        {

        counter.SetActive(true);


        for (int i = 0; i < CubeMeshs.Length; i++)

        {

            while (CubeMeshs[i].transform.position != spawnpoints[i])
                    {

                backPosition = false;
            }

        }

        yield return null;
        Invoke("selectCube", 3);
    }
      
            
                   
            

  
    

        

     
           


       

        // Update is called once per frame
   
}
