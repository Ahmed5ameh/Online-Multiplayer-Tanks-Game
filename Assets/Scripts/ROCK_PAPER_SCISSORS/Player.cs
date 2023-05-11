using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace ROCK_PAPER_SCISSORS
{
    public class Player : NetworkBehaviour
    {
        private GameplayUIManager _gameplayUIManager;
        [SyncVar(hook = nameof(OnNameUpdated))] private string _playerName;
        [SyncVar(hook = nameof(OnMoveUpdated))] private RPS_Action _playerMove;
        [SyncVar] private int _score;
        [SyncVar(hook = nameof(OnUIUpdate))] private int _updateUI;


        /**Properties*/
        public RPS_Action PlayerMove => _playerMove;

        private void Awake()
        {
            _gameplayUIManager = FindObjectOfType<GameplayUIManager>();
            NetworkingManager.Instance.AddPlayer(this);
        }

        /**Overrides*/
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            CmdUpdateMyName(NetworkingManager.Instance.PlayerName);
        }
        
        void OnNameUpdated(string oldValue, string newValue)
        {
            _gameplayUIManager.SetPlayerName(isLocalPlayer, newValue);
        }

        void OnMoveUpdated(RPS_Action oldValue, RPS_Action newValue)
        {
            Debug.Log($"{_playerName}'s move is: {newValue}");
        }

        void OnUIUpdate(int @old, int @new)
        {
            _gameplayUIManager.ResetUI(isLocalPlayer, @new);
        }

        
        /**Commands*/
        [Command]
        void CmdUpdateMyName(string name)
        {
            _playerName = name;
        }

        [Command]
        void CmdUpdateUI(int @newScore)
        {
            _updateUI = @newScore;
        }
        
        [Command]
        public void CmdUpdateMove(RPS_Action move)
        {
            _playerMove = move;
            if (NetworkingManager.Instance.AllPlayersMadeMove())
            {
                RPCToggleActions(false);
                StartCoroutine(NetworkingManager.Instance.CalculateResult());
                //_gameplayUIManager.ResetUI(_score);
                CmdUpdateUI(_score);
                RPCToggleActions(true);
            }
        }

        /**ClientRPCs*/
        [ClientRpc]
        public void RPCToggleActions(bool enable)
        {
            _gameplayUIManager.ToggleActions(enable);
            if (!enable)
            {
                _gameplayUIManager.PlaySFX(SFXType.DrumRoll);
            }
        }
        

        /**TargetRPCs*/
        [TargetRpc]
        public void TargetSetResult(EndResult result)
        {
            _gameplayUIManager.PlaySFX(SFXType.Crash);
            Debug.Log(result);
        }

        [TargetRpc]
        // public void TargetStartNewRound()
        // {
        //     _gameplayUIManager.
        // }

        /**Server*/
        [Server]
        public void SetScore(bool increment)
        {
            if (increment) _score++;
            else _score--;
        }

        // [Server]
        // public void StartNewRound()
        // {
        //     _playerMove = RPS_Action.None;
        //     TargetStartNewRound();
        // }

    }

}

