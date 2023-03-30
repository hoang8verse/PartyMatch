using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyGameobject : MonoBehaviour
{
    public GameObject AI;
    void OnCollisionEnter(Collision other)
    {
     

        if (other.gameObject.tag == "dead")
        {

            Destroy(AI.gameObject);
        }
      
    }
}
