using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace PUN2_Tanks_Game
{
    public enum Team
    {
        None,
        Ahly,
        Zamalek
    }

    public enum Role
    {
        None,
        DPS,
        Tank,
        Healer
    }
    
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject homePannel;
        [SerializeField] private GameObject lobbyPannel;
        [SerializeField] private GameObject preGameplayPannel;
        [SerializeField] private TMP_InputField playerName_IF;
        [SerializeField] private TMP_InputField roomName_IF;
        [SerializeField] private TMP_Dropdown playerTeam_DD;
        [SerializeField] private TMP_Dropdown playerClass_DD;
        [SerializeField] private Button startButton;

        private void Start()
        {
            homePannel.SetActive(true);
            lobbyPannel.SetActive(false);
            preGameplayPannel.SetActive(false);
            ToggleEvents(true);
        }
        
        private void OnDestroy()
        {
            ToggleEvents(false);
        }

        void ToggleEvents(bool @subscribe)
        {
            if (@subscribe)
            {
                NetworkingManager.Instance.OnConnectedToCloud += ShowLobby;
                NetworkingManager.Instance.OnPlayerJoinedRoom += ShowGameplayPannel;
                NetworkingManager.Instance.OnAllPlayersReady += SetStartButtonInteractability;
            }
            else
            {
                NetworkingManager.Instance.OnConnectedToCloud -= ShowLobby;
                NetworkingManager.Instance.OnConnectedToCloud -= ShowGameplayPannel;
                NetworkingManager.Instance.OnAllPlayersReady -= SetStartButtonInteractability;
            }
        }
        
        void ShowLobby()
        {
            homePannel.SetActive(false);
            lobbyPannel.SetActive(true);
            preGameplayPannel.SetActive(false);
            //Debug.Log("LobbyShown");
        }

        void ShowGameplayPannel()
        {
            if (PhotonNetwork.IsMasterClient == false)
            {
                startButton.GetComponentInChildren<TMP_Text>().text = "Ready";
            }
            else
            {
                startButton.interactable = false;
            }
            
            homePannel.SetActive(false);
            lobbyPannel.SetActive(false);
            preGameplayPannel.SetActive(true);
        }

        #region Buttons

            public void OnConnectClicked()
            {
                NetworkingManager.Instance.ConnectToCloud();
            }

            public void OnCreateRoomClicked()
            {
                if (string.IsNullOrEmpty(playerName_IF.text) || string.IsNullOrEmpty(roomName_IF.text)) 
                    return;

                NetworkingManager.Instance.UpdatePlayerStats(playerName_IF.text,
                    (Team)playerTeam_DD.value,
                    (Role)playerClass_DD.value
                );
                NetworkingManager.Instance.CreateRoom(roomName_IF.text);
            }
            
            public void OnJoinRoomClicked()
            {
                if (string.IsNullOrEmpty(playerName_IF.text) || string.IsNullOrEmpty(roomName_IF.text))
                    return;

                NetworkingManager.Instance.UpdatePlayerStats(playerName_IF.text,
                    (Team)playerTeam_DD.value,
                    (Role)playerClass_DD.value
                );
                NetworkingManager.Instance.JoinRoom(roomName_IF.text);
            }
            
            public void OnJoinRandomRoomClicked()
            {
                if (string.IsNullOrEmpty(playerName_IF.text))
                    return;

                NetworkingManager.Instance.UpdatePlayerStats(playerName_IF.text,
                    (Team)playerTeam_DD.value,
                    (Role)playerClass_DD.value
                );
                NetworkingManager.Instance.JoinRandomRoom();
            }

            public void OnStartGameClicked()
            {
                if (NetworkingManager.IsHost) 
                    NetworkingManager.Instance.LoadGamePlay();
                else
                {
                    PhotonHashtable playerReadyStats = new PhotonHashtable();
                    playerReadyStats.Add(Keys.Ready, true);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(playerReadyStats);
                    startButton.interactable = false;
                }
            }

            public void SetStartButtonInteractability(bool state)
            {
                startButton.interactable = state;
            }
        #endregion
        
    }
}
