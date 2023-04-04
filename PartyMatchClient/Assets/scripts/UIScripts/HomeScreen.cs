using TMPro;
using UnityEngine;

namespace UIElements
{
    public class HomeScreen : Screen
    {
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void CreateRoom()
        {
            MainMenu.instance.HostCreateNewRoom();
        }
        public void JoinRoom()
        {
            MainMenu.instance.UserJoinRoom();
        }
        public void SpectatorJoinRoom()
        {
            MainMenu.instance.SpectatorJoinRoom();
        }
    }
}