using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace PUN2_Tanks_Game
{
    public class NetworkingManager : MonoBehaviourPunCallbacks
    {
        private static NetworkingManager _instance;     //Singelton
        
        [Header("Logging")] 
        [SerializeField] private TMP_Text Log_txt;

        private Dictionary<int, NetworkPlayer> _players;    //ActorNr, NetworkPlayer

        /**Events*/
        public event Action OnConnectedToCloud;
        public event Action OnPlayerJoinedRoom;
        public event Action<bool> OnAllPlayersReady;

        /**Properties*/
        public static NetworkingManager Instance => _instance;
        public static bool IsHost => PhotonNetwork.IsMasterClient;
        
        /**Awake*/
        private void Awake()
        {
            _instance ??= this;     //Singelton
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _players = new Dictionary<int, NetworkPlayer>();
        }

        #region Userdefiened Functions

            public void ConnectToCloud()
            {
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.GameVersion = "v1.0";
                PhotonNetwork.ConnectUsingSettings();
                Log("Connecting...");
            }

            public void UpdatePlayerStats(string @playerName, Team @playerTeam, Role @playerRole)
            {
                PhotonNetwork.NickName = @playerName;

                PhotonHashtable playerCustomProperties = new();
                playerCustomProperties.Add(Keys.Team, @playerTeam);
                playerCustomProperties.Add(Keys.Role, @playerRole);

                switch (@playerRole)
                {
                    case Role.DPS:
                        playerCustomProperties.Add(Keys.Hp, 100f);
                        playerCustomProperties.Add(Keys.MaxHp, 100f);
                        playerCustomProperties.Add(Keys.Damage, 20f);
                        playerCustomProperties.Add(Keys.MovementSpeed, 10f);
                        playerCustomProperties.Add(Keys.HealingAmount, 0f);
                        break;
                    
                    case Role.Tank:
                        playerCustomProperties.Add(Keys.Hp, 300f);
                        playerCustomProperties.Add(Keys.MaxHp, 300f);
                        playerCustomProperties.Add(Keys.Damage, 5f);
                        playerCustomProperties.Add(Keys.MovementSpeed, 5f);
                        playerCustomProperties.Add(Keys.HealingAmount, 0f);
                        break;
                    
                    case Role.Healer:
                        playerCustomProperties.Add(Keys.Hp, 100f);
                        playerCustomProperties.Add(Keys.MaxHp, 100f);
                        playerCustomProperties.Add(Keys.Damage, 20f);
                        playerCustomProperties.Add(Keys.MovementSpeed, 10f);
                        playerCustomProperties.Add(Keys.HealingAmount, 7.5f);
                        break;
                }
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerCustomProperties);
            }

            public void CreateRoom(string @roomName)
            {
                RoomOptions roomOptions = new RoomOptions
                {
                    MaxPlayers = 4, 
                    BroadcastPropsChangeToAll = true
                };
                
                PhotonNetwork.CreateRoom(@roomName, roomOptions);
                Log($"Creating Room {@roomName}");
            }
            
            public void JoinRoom(string @roomName)
            {
                PhotonNetwork.JoinRoom(@roomName);
                Log($"Joinning Room {@roomName}");
            }
            
            public void JoinRandomRoom()
            {
                PhotonNetwork.JoinRandomRoom();
                Log("Joining Random Room");
            }

            public void LoadGamePlay()
            {
                PhotonNetwork.LoadLevel(1);
            }

            public void AddPlayer(int @actorNr, NetworkPlayer @netPlayer)
            {
                if (!_players.ContainsKey(@actorNr))
                {
                    _players.Add(@actorNr, @netPlayer);
                }
            }

            public NetworkPlayer GetPlayer(int @actorNr)
            {
                return _players[@actorNr];
            }

        #endregion

        #region Callbacks

            public override void OnConnectedToMaster()
            {
                base.OnConnectedToMaster();
                Log("Connected To Photon Cloud");
                OnConnectedToCloud?.Invoke();
            }

            public override void OnJoinedRoom()
            {
                base.OnJoinedRoom();
                OnPlayerJoinedRoom?.Invoke();
                Log($"Joined Room {PhotonNetwork.CurrentRoom.Name}");

                foreach (var @player in PhotonNetwork.PlayerListOthers)
                {
                    Log($"Player {@player.NickName}");
                }

                PhotonHashtable playerReadyStats = new();
                playerReadyStats.Add(Keys.Ready, false);
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerReadyStats);
            }

            public override void OnPlayerEnteredRoom(Player newPlayer)
            {
                base.OnPlayerEnteredRoom(newPlayer);
                Log($"Player {newPlayer.NickName} entered the room");
            }

            public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
            {
                base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
                if (targetPlayer != null && changedProps.ContainsKey(Keys.Ready))
                {
                    bool AllPlayersReady = true;
                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        var readyStats = targetPlayer.CustomProperties[Keys.Ready];
                        if (!player.IsMasterClient && readyStats != null)
                        {
                            if ((bool)readyStats == false) 
                                AllPlayersReady = false;
                        }
                    }

                    if (PhotonNetwork.IsMasterClient)
                    {
                        OnAllPlayersReady?.Invoke(AllPlayersReady);
                    }
                }
            }

        #endregion

        #region Logging

            public void Log(string @msg)
            {
                Log_txt.text = $"-{@msg}\n{Log_txt.text}";
            }

            public void ClearLog()
            {
                Log_txt.text = string.Empty;
            }

        #endregion
    }
}
