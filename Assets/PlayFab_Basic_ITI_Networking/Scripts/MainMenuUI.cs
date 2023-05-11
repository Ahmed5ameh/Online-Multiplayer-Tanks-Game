using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using WebSocketSharp;
using UnityEngine.UI;


namespace PlayFab_Basic
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Auth")] 
        [SerializeField] private GameObject authPanel;
        [SerializeField] private TMP_InputField customID_IF;
        
        [Header("Player Data")]
        [SerializeField] GameObject playerDataPanel;
        [SerializeField] TMP_Text playerName;
        [SerializeField] TMP_Text currencyValue;
        [SerializeField] TMP_InputField currencyAmount_IF;

        private void Start()
        {
            authPanel.SetActive(true);
            playerDataPanel.SetActive(false);
            PlayfabManager.Instance.onInventoryUpdated += UpdateCoins;
        }

        public void OnLoginClicked()
        {
            if (!string.IsNullOrEmpty(customID_IF.text))
            {
                PlayfabManager.Instance.LoginWithCustomID(customID_IF.text,
                    isNewlyCreated =>
                    {
                        if (isNewlyCreated)
                        { 
                            PlayfabManager.Instance.UpdateDisplayName(customID_IF.text);
                        }
                    }
                );
                authPanel.SetActive(false);
                playerDataPanel.SetActive(true);
            }
        }
        
        public void OnCoinsClicked()
        {
            if (int.TryParse(currencyAmount_IF.text, out int coinsValue))
            {
                PlayfabManager.Instance.AddVirtualCurrency(PlayfabManager.VC_Gold, coinsValue);
            }

        }

        public void UpdateCoins()
        {
            currencyValue.text= PlayfabManager.Instance.ownedCoins.ToString();   
        }
        
        
    }
}
