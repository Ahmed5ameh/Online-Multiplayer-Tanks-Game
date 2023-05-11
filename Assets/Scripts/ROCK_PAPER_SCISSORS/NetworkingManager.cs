using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace ROCK_PAPER_SCISSORS
{
    public class NetworkingManager : NetworkManager
    {
        private static NetworkingManager _instance;     //Singelton
        private List<Player> _netPlayers;

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
        public Player LocalPlayer => _netPlayers.Find(x => x.isLocalPlayer);
        
        
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
            _netPlayers = new List<Player>();
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
        
        public void SetPlayerName(string playerName)
        {
            PlayerName = playerName;
        }

        /**Functions*/
        public void AddPlayer(Player addedPlayer)
        {
            if (!_netPlayers.Contains(addedPlayer))
            {
                _netPlayers.Add(addedPlayer);
            }
        }

        public bool AllPlayersMadeMove()
        {
            return !_netPlayers.Exists(x => x.PlayerMove == RPS_Action.None) && _netPlayers.Count == 2;
        }

        public IEnumerator CalculateResult()
        {
            RPS_Action p1Move = _netPlayers[0].PlayerMove;
            RPS_Action p2Move = _netPlayers[1].PlayerMove;
            EndResult p1Result = EndResult.Lose;
            EndResult p2Result = EndResult.Lose;

            if (p1Move == p2Move)
            {
                p1Result = p2Result = EndResult.Draw;
            }
            else
            {
                p1Result = p1Move switch    //Pattern Matching Switch
                {
                    RPS_Action.Rock => p2Move == RPS_Action.Scissors ? EndResult.Win : EndResult.Lose,
                    RPS_Action.Paper => p2Move == RPS_Action.Rock ? EndResult.Win : EndResult.Lose,
                    RPS_Action.Scissors => p2Move == RPS_Action.Paper ? EndResult.Win : EndResult.Lose,
                    _ => EndResult.Lose
                };
                p2Result = p1Result == EndResult.Win ? EndResult.Lose : EndResult.Win;
                _netPlayers[0].SetScore(p1Result == EndResult.Win);
                _netPlayers[1].SetScore(p2Result == EndResult.Win);
            }
            //TODO: Send result to each player
            _netPlayers[0].TargetSetResult(p1Result);
            _netPlayers[1].TargetSetResult(p2Result);

            yield return new WaitForSeconds(2);
            //_netPlayers.ForEach();
        }
    }
}

