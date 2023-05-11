using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Mirror_Tanks_Game
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField playerName_IF;
        [SerializeField] private TMP_Dropdown team_Dd;
        [SerializeField] private TMP_Dropdown role_Dd;
 
        public void StartServer()
        {
            NetworkingManager.Instance.StartServer();
        }

        public void StartHost()
        {
            if (!string.IsNullOrEmpty(playerName_IF.text))
            {
                NetworkingManager.Instance.SetPlayerName(playerName_IF.text);
                NetworkingManager.Instance.SetPlayerToTeam(team_Dd.value);
                NetworkingManager.Instance.SetPlayerRole(role_Dd.value);
                NetworkingManager.Instance.StartHost();
            }
        }

        public void StartClient()
        {
            if (!string.IsNullOrEmpty(playerName_IF.text))
            {
                NetworkingManager.Instance.SetPlayerName(playerName_IF.text);
                NetworkingManager.Instance.SetPlayerToTeam(team_Dd.value);
                NetworkingManager.Instance.SetPlayerRole(role_Dd.value);
                NetworkingManager.Instance.StartClient();
            }
        }

        // private void Update()
        // {
        //     Debug.Log(team_Dd.value);
        // }
    }
}
