using UnityEngine;

namespace Pickups
{
    public class HealthPickup : Pickup
    {
        [SerializeField] private float healAmount = 10f;
        public override void OnTriggerEnter2D(Collider2D other)
        {
            var playerHealth = other.GetComponent<Player.PlayerHealth>();
            
            if (playerHealth != null)
            {
                Debug.Log("Player picked up health");
                OnPickup();
                playerHealth.GainHealth(healAmount);
            }
        }
    }
}