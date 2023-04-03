using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] characters;
    // Start is called before the first frame update
    void Awake()
    {
        //if (!PlayerPrefs.HasKey("selectedCharacter"))
        //{
        //    characters[0].SetActive(true);
        //    PlayerPrefs.SetInt("selectedCharacter", 0);
        //}
        //else
        //{
        //   characters[PlayerPrefs.GetInt("selectedCharacter")].SetActive(true);

        //}

    }
    public void SetActiveCharacter(int index)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if(i == index)
            {
                characters[index].SetActive(true);
            } else
            {
                characters[i].SetActive(false);
            }
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
