using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mirror_Tanks_Game
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [Header("Player Data")] 
        [SerializeField] private float maxHealth;
        [SerializeField] private float movementSpeed;

        [Header("Player Info")] 
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text playerTeam;
        [SerializeField] private Image hpImage;
        [SyncVar (hook = nameof(OnNameUpdated))] private string _playerName;
        [SyncVar (hook = nameof(OnHealthUpdated))] private float _health;
        [SyncVar (hook = nameof(OnTeamUpdated))] private int _playerTeam;
        [SyncVar] private int _playerRole;
        [SyncVar] private float _playerDamage;

        [Header("Shooting")] 
        [SerializeField] private Transform cannonRoot;
        [SerializeField] private float rotatingSpeed;
        [SerializeField] private Transform shootingPos;
        [SerializeField] private Bullet bulletPrefab;

        private Rigidbody _rigidbody;
        
        
        public int PlayerRole
        {
            get => _playerRole;
            set => _playerRole = value;
        }
        public int PlayerTeam
        {
            get => _playerTeam;
            set => _playerTeam = value;
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            
            if (isLocalPlayer && _rigidbody)
            {
                float x = Input.GetAxis("Horizontal");
                float z = Input.GetAxis("Vertical");
                
                bool rotateRight = Input.GetKey(KeyCode.RightArrow);
                bool rotateLeft = Input.GetKey(KeyCode.LeftArrow);

                bool shoot = Input.GetKeyDown(KeyCode.Space);
                
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
                    CmdShoot();
                }
            }
        }

        /**Overrides*/
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            //Execute command to server, update my name and this is my name
            CmdInitPlayer(
                NetworkingManager.Instance.PlayerName, 
                NetworkingManager.Instance.PlayerTeam,
                NetworkingManager.Instance.PlayerRole
                );
        }

        /**User-defined functions*/
        public void OnNameUpdated(string @oldVla, string @newVal)
        {
            playerName.text = @newVal;
        }

        public void OnHealthUpdated(float @oldVal, float @newVal)
        {
            hpImage.transform.localScale = new Vector3(newVal / maxHealth, 1, 1);
        }

        public void OnTeamUpdated(int @oldVal, int @newVal)
        {
            playerTeam.text = @newVal.ToString();
        }
        
        
        /**Commands*/
        [Command]
        public void CmdInitPlayer(string @playerName, int @playerTeam, int @playerRole)
        {
            _playerName = @playerName;
            _playerTeam = @playerTeam;
            _playerRole = @playerRole;
            
            if (_playerRole == 1)   //DPS
            {
                maxHealth = 100;
                movementSpeed = 15;
                _playerDamage = 35;
            }
            
            else if (_playerRole == 2)  //Tank
            {
                maxHealth = 250;
                movementSpeed = 8;
                _playerDamage = 15;
            }
            
            OnNameUpdated(string.Empty, _playerName);
            UpdateHealth(maxHealth);
            OnTeamUpdated(0, _playerTeam);
        }

        [Command]
        public void CmdShoot()
        {
            Debug.Log(_playerDamage);
            Bullet bullet = Instantiate(bulletPrefab, shootingPos.position, cannonRoot.rotation) as Bullet;
            bullet.PlayerTeam = _playerTeam;
            bullet.BulletDamage = _playerDamage;
            RpcShoot(shootingPos.position, cannonRoot.rotation, _playerTeam);
        }

        
        /**Server*/
        [Server]
        public void UpdateHealth(float @value, bool increment = false)
        {
            if (increment)
            {
                _health += @value;
            }
            else
            {
                _health = @value;
            }
            
            OnHealthUpdated(0, _health);
            
            if (_health == 0)
            {
                NetworkServer.Destroy(gameObject);
            }
        }
        
        
        /**ClientRPCs*/
        [ClientRpc]
        public void RpcShoot(Vector3 position, Quaternion rotation, int team)
        {
            if (!NetworkingManager.Instance.IsHost)
            {
                Instantiate(bulletPrefab, position, rotation);
            }
        }
    }
}
