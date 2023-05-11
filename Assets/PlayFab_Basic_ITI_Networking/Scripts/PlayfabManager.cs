using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.EconomyModels;

namespace PlayFab_Basic
{
    public class PlayfabManager : MonoBehaviour
    {
        private static PlayfabManager _instance;        //Singelton
        public int ownedCoins;
        
        
        /**Keys*/
        public const string VC_Gold = "GD";
        
        /**Events*/
        public Action onInventoryUpdated;
        
        
        /**Properties*/
        public static PlayfabManager Instance => _instance;

        
        
        /**Awake*/
        private void Awake()
        {
            _instance ??= this;
        }

        public void LoginWithCustomID(string @customID, Action<bool> @onSuccess = null, Action @onFail = null)
        {
            Debug.Log("Login With Custom ID");
            LoginWithCustomIDRequest request = new LoginWithCustomIDRequest
            {
                TitleId = PlayFabSettings.TitleId,
                CustomId = @customID,
                CreateAccount = true
            };
            PlayFabClientAPI.LoginWithCustomID(request,
                result =>
                {
                    Debug.Log($"Logged with {result.PlayFabId}");
                    onSuccess?.Invoke(result.NewlyCreated);
                    UpdateInventory();
                },
                err =>
                {
                    Debug.LogError($"Failed to log in due to {err.ErrorMessage}");
                    onFail?.Invoke();
                }
            );
        }
        
        public void UpdateDisplayName(string @playerName, Action @onSuccess = null, Action @onFail = null)
        {
            UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = @playerName
            };
            PlayFabClientAPI.UpdateUserTitleDisplayName(request,
                result =>
                {
                    Debug.Log($"Logged with {result.DisplayName}");
                    onSuccess?.Invoke();  
                },
                err =>
                {
                    Debug.LogError($"Failed to log in due to {err.ErrorMessage}");
                    onFail?.Invoke();
                }
            );
        }
        
        public void AddVirtualCurrency(string currencyCode, int amount, Action onSuccess = null, Action onFail = null)
        {
            AddUserVirtualCurrencyRequest request = new AddUserVirtualCurrencyRequest()
            {
                VirtualCurrency = currencyCode,
                Amount = amount

            };
            PlayFabClientAPI.AddUserVirtualCurrency
            (request, result =>
                {

                    Debug.Log("Added currency to " + currencyCode);
                    onSuccess?.Invoke();
                    UpdateInventory();
                },
                err =>
                {
                    Debug.LogError("Failed add coins due to " + err.ErrorMessage);
                    onFail?.Invoke();
                });
        }

        public void UpdateInventory(Action onSuccess = null, Action onFail = null)
        {
            GetUserInventoryRequest request = new GetUserInventoryRequest();

            PlayFabClientAPI.GetUserInventory(request,
                result =>
                {
                    ownedCoins = result.VirtualCurrency[VC_Gold];
                    onInventoryUpdated?.Invoke();
                    Debug.Log("Updated inventory");
                    onSuccess?.Invoke();
                },
                err =>
                {
                    Debug.LogError("Failed to Update inventory" + err.ErrorMessage);
                    onFail?.Invoke();
                });
        }
    }
}
