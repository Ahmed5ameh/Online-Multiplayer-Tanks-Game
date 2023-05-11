using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum RPS_Action
{
    None = 0,
    Rock,
    Paper,
    Scissors,
}

public enum SFXType
{
    DrumRoll,
    Crash,
}

public enum EndResult
{
    Win,
    Lose,
    Draw,
}

namespace ROCK_PAPER_SCISSORS
{
    public class GameplayUIManager : MonoBehaviour
    {
        [Header("Player Info")]
        [SerializeField] private TMP_Text p1Name_txt;
        [SerializeField] private TMP_Text p1Score_txt;
        [SerializeField] private TMP_Text p2Name_txt;
        [SerializeField] private TMP_Text p2Score_txt;

        [Header("Actions")] 
        [SerializeField] private CanvasGroup _actionsCanvasGroup;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _drumRoll;
        [SerializeField] private AudioClip _crash;
        

        private void Start()
        {
            p1Score_txt.text = p2Score_txt.text = "0";
            ToggleActions(true);
        }

        public void SetPlayerName(bool isLocal, string name)
        {
            if (isLocal)
            {
                p1Name_txt.text = name;
            }
            else
            {
                p2Name_txt.text = name;
            }
        }

        public void OnActionClicked(int moveValue)
        {
            if (!NetworkingManager.Instance.IsClient) return;
            
            RPS_Action move = (RPS_Action) moveValue;
            // Submit Action to Local Player
            NetworkingManager.Instance.LocalPlayer.CmdUpdateMove(move);
        }

        public void ToggleActions(bool enable)
        {
            _actionsCanvasGroup.interactable = enable;
        }

        public void PlaySFX(SFXType type)
        {
            _audioSource.Stop();
            _audioSource.clip = type switch
            {
                SFXType.DrumRoll => _drumRoll,
                SFXType.Crash => _crash,
                _ => _crash
            };
            _audioSource.Play();
        }

        public void ResetUI(bool @isLocal, int @Score)
        {
            if (@isLocal)
            {
                p1Score_txt.text = @Score.ToString();
            }
            else
            {
                p2Score_txt.text = @Score.ToString();
            }
        }
    }
}

