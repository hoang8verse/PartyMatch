using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class CubeManager : Singleton<CubeManager>
{
    public GameObject[] CubeMeshs;
    public List<GameObject> cubemesh2 = new List<GameObject>();

    public Material targetmateriel;
    public Material[] materiellist;
    public GameObject boardMateriel;
    public Material[] nogomateriel;

    public Material blue;
    public Vector3[] spawnpoints;
    public Quaternion[] spawnpointsR;
    public GameObject cam2;


    int m_target;
    int m_ran1;
    int m_ran2;
    int m_ran3;
    byte[] m_rans;

    public bool backPosition;  

    public GameObject counter;

    public int m_round = 0;
    public int RoundCount => m_round;
    public bool IsInitialized { set; get; } = false;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < CubeMeshs.Length; i++)
        {
            spawnpoints[i] = CubeMeshs[i].transform.position;
            spawnpointsR[i] = CubeMeshs[i].transform.localRotation;

        }
        LevelManager.Instance.textRound.text = "VÒNG " + (m_round + 1).ToString();

        IsInitialized = true;
        boardMateriel.SetActive(false);
    }

    public void SendRequestRandomTarget()
    {
        int _target = Random.Range(0, materiellist.Length);
        int _ran1 = Random.Range(0, CubeMeshs.Length);
        int _ran2 = Random.Range(0, CubeMeshs.Length);
        int _ran3 = Random.Range(0, CubeMeshs.Length);
        byte[] _rans = new byte[CubeMeshs.Length];

        m_target = _target;

        for (int i = 0; i < CubeMeshs.Length; i++)
            _rans[i] = (byte)GetRandomMaterialCube();            
       
        SocketClient.instance.OnRequestRandomTarget(_target, _ran1, _ran2, _ran3, _rans);
    }

    public void PerformCube(int _target, int _ran1, int _ran2, int _ran3, byte[] _rans)
    {
        m_target = _target;
        m_ran1 = _ran1;
        m_ran2 = _ran2;
        m_ran3 = _ran3;
        m_rans = _rans;
        selectCube();
    }

    int GetRandomMaterialCube()
    {
        int rand = 0;
        do
        {
            rand = Random.Range(0, nogomateriel.Length);
        } while (rand == m_target);
 

        return rand;
    }
    void selectCube()
    {
        if (!LevelManager.Instance.winbool)
        {
            boardMateriel.SetActive(true);
            //targetmateriel = materiellist[Random.Range(0, materiellist.Length)];
            targetmateriel = materiellist[m_target];
            boardMateriel.GetComponent<MeshRenderer>().material = targetmateriel;

            cubemesh2.Clear();
            //ran1 = Random.Range(0, CubeMeshs.Length);
            //ran2 = Random.Range(0, CubeMeshs.Length);
            //ran3 = Random.Range(0, CubeMeshs.Length);

            for (int i = 0; i < CubeMeshs.Length; i++)
            {
                cubemesh2.Add(CubeMeshs[i]);
            }

            //CubeMeshs[ran1].GetComponent<MeshRenderer>().material = targetmateriel;
            //CubeMeshs[ran2].GetComponent<MeshRenderer>().material = targetmateriel;
            //CubeMeshs[ran3].GetComponent<MeshRenderer>().material = targetmateriel;

            CubeMeshs[m_ran1].GetComponent<Cube>().SetMaterial(targetmateriel);
            CubeMeshs[m_ran2].GetComponent<Cube>().SetMaterial(targetmateriel);
            CubeMeshs[m_ran3].GetComponent<Cube>().SetMaterial(targetmateriel);


            for (int i = 0; i < cubemesh2.Count; i++)
            {
                if(i != m_ran1 && i != m_ran2 && i != m_ran3)
                        cubemesh2[i].GetComponent<Cube>().SetMaterial(nogomateriel[m_rans[i]]);
            }

            cubemesh2.Remove(CubeMeshs[m_ran1]);
            cubemesh2.Remove(CubeMeshs[m_ran2]);
            cubemesh2.Remove(CubeMeshs[m_ran3]);

            CubeMeshs[m_ran1].transform.tag = "go";
            CubeMeshs[m_ran2].transform.tag = "go";
            CubeMeshs[m_ran3].transform.tag = "go";

            print("selected number is " + m_ran1);
            print("selected number is " + m_ran2);
            print("selected number is " + m_ran3);

            if (LevelManager.Instance.winbool)
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
        //yield return new WaitForSeconds(1);

        counter.SetActive(false);

        yield return new WaitForSeconds(3);
        SocketClient.instance.OnCubeFall();


        yield return new WaitForSeconds(3);
        StartCoroutine(resetCube());
        

    }

    public void PerformCubeFall()
    {
        foreach (GameObject item in CubeMeshs)
        {
            if (item.name == CubeMeshs[m_ran1].name || item.name == CubeMeshs[m_ran2].name || item.name == CubeMeshs[m_ran3].name)
            {
                print("not falling " + item.name);

            }
            else

            {
                item.GetComponent<Rigidbody>().isKinematic = false;
                item.GetComponent<Rigidbody>().useGravity = true;
                print("falling");
            }
        }
        for (int i = 0; i < CubeMeshs.Length; i++)

        {
            if (CubeMeshs[i].transform.position == spawnpoints[i])
            {
                backPosition = true;

            }

        }
    }

    public void SetPlayerWin()
    {
        //if (round >= 5)
        {
            LevelManager.Instance.SetPlayerWin();
            SocketClient.instance.OnPlayerWin();
        }
    }
   IEnumerator resetCube()
    {
        m_round++;       
        SocketClient.instance.OnRoundPass(m_round);      

        yield return new WaitForSeconds(3);

        SocketClient.instance.OnCubeReset();
        boardMateriel.SetActive(false);
        LevelManager.Instance.textRound.text = "VÒNG " + (m_round + 1).ToString();

        yield return new WaitForSeconds(3.5f);
        
        if (LevelManager.Instance.winbool)
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

    public void PerformCubeReset()
    {
        foreach (GameObject item in CubeMeshs)
        {
            //item.GetComponent<MeshRenderer>().material = blue;
            item.GetComponent<Cube>().SetMaterial(blue);

            print("materiel is " + blue);
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Rigidbody>().useGravity = false;
            CubeMeshs[m_ran1].transform.tag = "ngo";
            CubeMeshs[m_ran2].transform.tag = "ngo";
            CubeMeshs[m_ran3].transform.tag = "ngo";

            if (m_ran1 == m_ran2)
            {
                if (m_ran2 > CubeMeshs.Length && m_ran2 > 0)
                {

                    CubeMeshs[m_ran2 - 1].transform.tag = "ngo";


                }
                else
                {
                    CubeMeshs[m_ran2 + 1].transform.tag = "ngo";
                }
            }
            else
            {
                CubeMeshs[m_ran2].transform.tag = "ngo";

            }

            backPosition = false;

        }

        // check new round already to play
        //SocketClient.instance.OnRoundAlready();
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
        if(LevelManager.Instance.winbool)
        {
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
