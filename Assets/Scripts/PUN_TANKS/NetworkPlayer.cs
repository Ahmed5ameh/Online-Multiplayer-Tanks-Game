using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace PUN2_Tanks_Game
{
    
    
    public class NetworkPlayer : MonoBehaviourPun, IPunObservable
    {   
        [Header("Player Data")] 
        [SerializeField] private float maxHealth;
        [SerializeField] private float movementSpeed;

        [Header("Player Info")]
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text playerTeam;
        [SerializeField] private TMP_Text playerRole;
        [SerializeField] private Image hpImage;
        private string _playerName;
        private float _health;
        private float _playerDamage;

        [Header("Shooting")] 
        [SerializeField] private Transform cannonRoot;
        [SerializeField] private float rotatingSpeed;
        [SerializeField] private Transform shootingPos;
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private HealingRange healingRangePrefab;
        private Rigidbody _rigidbody;
        
        private void Start()
        {
            playerName.text = photonView.Owner.NickName;
            photonView.RPC(nameof(RPC_UpdateUI), RpcTarget.All);

            if (photonView.IsMine)
            {
                _rigidbody = GetComponent<Rigidbody>();
                UpdatePlayerHealth(maxHealth);
                UpdateHealthUI();
            }
            NetworkingManager.Instance.AddPlayer(photonView.OwnerActorNr, this);
            movementSpeed = (float)photonView.Owner.CustomProperties[Keys.MovementSpeed];
        }
        private void Update()
        {
            if (photonView.IsMine && _rigidbody)
            {
                float x = Input.GetAxis("Horizontal");
                float z = Input.GetAxis("Vertical");
                
                bool rotateRight = Input.GetKey(KeyCode.RightArrow);
                bool rotateLeft = Input.GetKey(KeyCode.LeftArrow);
                bool shoot = Input.GetKeyDown(KeyCode.Space);
                bool spawnHealingRange = Input.GetKeyDown(KeyCode.F1);
                
                _rigidbody.MovePosition(_rigidbody.position + new Vector3(
                    x * movementSpeed * Time.deltaTime,
                    0,
                    z * movementSpeed * Time.deltaTime
                ));

                if (rotateRight)
                {
                    cannonRoot.Rotate(0, rotatingSpeed * Time.deltaTime, 0);
                }

                if (rotateLeft)
                {
                    cannonRoot.Rotate(0, -rotatingSpeed * Time.deltaTime, 0);
                }

                if (shoot)
                {
                    photonView.RPC(nameof(RPC_Shoot), RpcTarget.All);
                }

                if (spawnHealingRange && (Role)photonView.Owner.CustomProperties[Keys.Role] == Role.Healer)
                {
                    photonView.RPC(nameof(RPC_SpawnHealingRange), RpcTarget.All);
                }
            }
        }
        
        // public void UpdateHealth(float @value, bool @increment = false, int @affectingPlayer = -1)
        // {
        //     if (@increment)
        //     {
        //         _health += @value;
        //     }
        //     else
        //     {
        //         _health = @value;
        //     }
        //
        //     if (_health <= 0)
        //     {
        //         var killer = NetworkingManager.Instance.GetPlayer(@affectingPlayer);
        //         NetworkingManager.Instance.Log($"{killer.photonView.Owner.NickName} killed you.");
        //         PhotonNetwork.Destroy(gameObject);
        //     }
        // }

        public void UpdatePlayerHealth(float damage)
        {
            if (photonView.Owner.IsLocal)
            {
                PhotonHashtable hpHash = new();
                float health = (float)photonView.Owner.CustomProperties[Keys.Hp];
                health += damage;
                health = Mathf.Clamp(health, 0, maxHealth);
                _health = health;
                hpHash[Keys.Hp] = health;
                photonView.Owner.SetCustomProperties(hpHash);
            }
        }

        void UpdateHealthUI()
        {
            _health = (float)photonView.Owner.CustomProperties[Keys.Hp];
            maxHealth = (float)photonView.Owner.CustomProperties[Keys.MaxHp];
            hpImage.transform.localScale = new Vector3(_health / maxHealth, 1, 1);
        }

        [PunRPC]
        void RPC_Shoot()
        {
            Bullet bullet = Instantiate(bulletPrefab, shootingPos.position, cannonRoot.rotation);
            bullet.Init(photonView.OwnerActorNr);
            bullet.ShooterPlayer = photonView.Owner;
        }

        [PunRPC]
        void RPC_SpawnHealingRange()
        {
            HealingRange healingRange = Instantiate(healingRangePrefab, transform.position, Quaternion.identity);
            healingRange.AffectingTeam = (Team)photonView.Owner.CustomProperties[Keys.Team];
        }

        [PunRPC]
        void RPC_UpdateUI()
        {
            // Setting Teams
            if ((Team)photonView.Owner.CustomProperties[Keys.Team] == Team.Ahly)
            {
                playerTeam.text = "Ahlawy";
                playerRole.text = photonView.Owner.CustomProperties[Keys.Role].ToString();
                gameObject.GetComponent<Renderer>().material.color = Color.red;
            }
            
            else if((Team)photonView.Owner.CustomProperties[Keys.Team] == Team.Zamalek)
            {
                playerTeam.text = "Zmalkawy";
                playerRole.text = photonView.Owner.CustomProperties[Keys.Role].ToString();
                gameObject.GetComponent<Renderer>().material.color = Color.cyan;
            }
            
            // Setting Roles
            if ((Role)photonView.Owner.CustomProperties[Keys.Role] == Role.DPS)
                playerRole.text = "DPS";
            
            else if ((Role)photonView.Owner.CustomProperties[Keys.Role] == Role.Tank)
                playerRole.text = "Tank";
            
            else if ((Role)photonView.Owner.CustomProperties[Keys.Role] == Role.Healer)
                playerRole.text = "Healer";
            
            else playerRole.text = string.Empty;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            stream.Serialize(ref _health);
            UpdateHealthUI();
        }
        
    }
}
