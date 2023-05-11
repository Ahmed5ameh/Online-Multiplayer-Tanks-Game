using UnityEngine;
using Photon.Pun;

namespace PUN2_Tanks_Game
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform[] spawnPositions;

        private void Start()
        {
            Vector3 spawnPos = spawnPositions[PhotonNetwork.LocalPlayer.ActorNumber - 1].position;
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
        }
    }
}
