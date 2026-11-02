using UnityEngine;

public class HammerPickUp : MonoBehaviour
{
    [Header("Configuración del Martillo")]
    [SerializeField] private float damageBonus = 15f; 
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null) player = other.GetComponent<PlayerController>();

        if (player != null || other.CompareTag("Player"))
        {
            if (player != null)
            {
                
                player.UpgradeDamage(damageBonus);
            }

            Debug.Log("<color=gold>¡Martillo recogido! Daño de ataque aumentado.</color>");

            
            gameObject.SetActive(false);
        }
    }
}