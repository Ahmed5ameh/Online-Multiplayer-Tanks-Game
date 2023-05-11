
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

namespace PUN2_Tanks_Game
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float bulletDamage;
        private Rigidbody _rigidbody;
        private int OwnerActorNr;
        private Player _shooterPlayer;

        [SerializeField] private Team shooterTeam;
        [SerializeField] private Role shooterRole;
        
        
        /**Properties*/
        public Player ShooterPlayer
        {
            get => _shooterPlayer;
            set => _shooterPlayer = value;
        }
        
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.velocity = transform.forward * speed;
            
            bulletDamage = (float)_shooterPlayer.CustomProperties[Keys.Damage];
            
            shooterTeam = (Team)_shooterPlayer.CustomProperties[Keys.Team];
            
            shooterRole = (Role)_shooterPlayer.CustomProperties[Keys.Role];
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                NetworkPlayer networkPlayer = other.GetComponent<NetworkPlayer>();
                Player otherPlayer = other.GetComponent<PhotonView>().Owner;
                //otherPlayer.CustomProperties.TryGetValue(Keys.Team, out object @Team);

                if (shooterTeam != (Team)otherPlayer.CustomProperties[Keys.Team])
                {
                    //Apply damage
                    networkPlayer.UpdatePlayerHealth(-bulletDamage);
                    Destroy(gameObject);
                }
                else
                {
                    if (shooterRole == Role.Healer)
                    {
                        networkPlayer.UpdatePlayerHealth(bulletDamage);
                    }
                }
            }
        }

        public void Init(int @OwnerActorNumber)
        {
            OwnerActorNr = @OwnerActorNumber;
            List<Photon.Realtime.Player>
                players = new List<Photon.Realtime.Player>(Photon.Pun.PhotonNetwork.PlayerList);
            //NetworkingManager.Instance.Log($"{players.Find(x=>x.ActorNumber == OwnerActorNr).NickName} is Shooting");
        }
        
    }
}
