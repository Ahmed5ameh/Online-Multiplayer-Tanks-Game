using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Mirror_Tanks_Game
{
    public class NetworkingManager : NetworkManager
    {
        private static NetworkingManager _instance;     //Singelton
        //private List<Player> _netPlayers;
        
        
        /**Properties*/
        public static NetworkingManager Instance        //Singelton
        {
            get => _instance;
            private set => _instance = value;
        }
        public bool IsClient { get; private set; }
        public bool IsServer { get; private set; }
        public bool IsHost => IsClient && IsServer;
        public string PlayerName { get; private set; }
        public int PlayerTeam { get; set; }
        public int PlayerRole { get; set; }

        /**Awake*/
        public override void Awake()
        {
            base.Awake();
            _instance ??= this;     //Singelton
        }
        
        /**Start*/
        public override void Start()
        {
            base.Start();
            //_netPlayers = new List<Player>();
        }
        
        /**Overrides*/
        public override void OnStartClient()
        {
            IsClient = true;
        }

        public override void OnStartServer()
        {
            IsServer = true;
        }
        
        /**Functions*/
        public void SetPlayerName(string playerName)
        {
            PlayerName = playerName;
        }

        public void SetPlayerToTeam(int team)
        {
            PlayerTeam = team;
        }

        public void SetPlayerRole(int role)
        {
            PlayerRole = role;
        }
    }
}
