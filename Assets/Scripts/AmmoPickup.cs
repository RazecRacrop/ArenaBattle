using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int rifleAmmoBonus = 20;
    public int shotgunAmmoBonus = 5;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AddAmmo(rifleAmmoBonus, shotgunAmmoBonus);
            }
            Destroy(gameObject);
        }
    }
}