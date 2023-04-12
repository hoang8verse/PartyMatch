using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cube : MonoBehaviour
{
    [SerializeField]
    MeshRenderer materialCube;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void SetMaterial(Material _material)
    {
        materialCube.material = _material;
    }
}
