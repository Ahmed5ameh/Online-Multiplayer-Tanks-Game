using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ROCK_PAPER_SCISSORS
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField playerName_IF;

        public void StartServer()
        {
            NetworkingManager.Instance.StartServer();
        }

        public void StartHost()
        {
            if (!string.IsNullOrEmpty(playerName_IF.text))
            {
                NetworkingManager.Instance.SetPlayerName(playerName_IF.text);
                NetworkingManager.Instance.StartHost();
            }
        }

        public void StartClient()
        {
            if (!string.IsNullOrEmpty(playerName_IF.text))
            {
                NetworkingManager.Instance.SetPlayerName(playerName_IF.text);
                NetworkingManager.Instance.StartClient();
            }
        }
    }
}