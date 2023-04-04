using UnityEngine;
using TMPro;

namespace UIElements
{
    public class Screen : MonoBehaviour
    {
        public GameObject Panel => this.gameObject;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void OnShow()
        {
            Panel.SetActive(false);
        }
        public virtual void OnHide()
        {
            Panel.SetActive(false);
        }
    }
}