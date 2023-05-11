using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror_Tanks_Game
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float bulletDamage = 15;
        private Rigidbody _rigidbody;
        private int _playerTeam;
        
        /**Properties*/
        public float BulletDamage
        {
            get => bulletDamage;
            set => bulletDamage = value;
        }
        
        public int PlayerTeam
        {
            get => _playerTeam;
            set => _playerTeam = value;
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.velocity = transform.forward * speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (NetworkingManager.Instance.IsServer && other.CompareTag("Player") && 
                other.GetComponent<NetworkPlayer>().PlayerTeam != PlayerTeam)
            {
                other.GetComponent<NetworkPlayer>().UpdateHealth(-BulletDamage, true);
                // Debug.Log("A RANDOM BULLET JUST SMASHED SOMEONE");
                // Debug.Log(BulletDamage);
            }

            else if (other.GetComponent<NetworkPlayer>().PlayerTeam == PlayerTeam)
            {
                //TODO: MAKE IT BYPASS HIM
            }
            
            Destroy(gameObject);
        }
    }
}
