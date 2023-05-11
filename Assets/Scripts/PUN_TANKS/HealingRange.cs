using UnityEngine;

namespace PUN2_Tanks_Game
{
    public class HealingRange : MonoBehaviour
    {
        private bool _applyHealing;
        private float _totalHealingTime = 10;
        private NetworkPlayer _otherPlayer;

        public Team AffectingTeam { get; set; }
        
        private void Start()
        {
            transform.parent = null;
            transform.position = new Vector3(
                transform.position.x,
                0.01f,
                transform.position.z
            );
        }

        private void Update()
        {
            if (_totalHealingTime < 0) Destroy(gameObject, 2);
            
            _totalHealingTime -= Time.deltaTime;
            if (_applyHealing && _totalHealingTime > 0)
            {
                _otherPlayer.UpdatePlayerHealth(1f);
                //Debug.Log("Applying Heal Now");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<NetworkPlayer>()) return;
            
            _otherPlayer = other.GetComponent<NetworkPlayer>();
            if (other.tag.Equals("Player"))
            {
                // if any player has the same team as the spawner
                if ((Team)_otherPlayer.photonView.Owner.CustomProperties[Keys.Team] == AffectingTeam)
                {
                    //Apply healing
                    _applyHealing = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.GetComponent<NetworkPlayer>()) return;
            
            _otherPlayer = other.GetComponent<NetworkPlayer>();
            if ((Team)_otherPlayer.photonView.Owner.CustomProperties[Keys.Team] == AffectingTeam)
            {
                //Stop healing
                _applyHealing = false;
            }
        }
    }
}
