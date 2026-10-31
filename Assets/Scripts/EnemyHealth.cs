using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 30f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"<color=yellow>{gameObject.name}</color> recibió daño. Vida restante: {currentHealth}");

       
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            StartCoroutine(FlashRedRoutine(sr));
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator FlashRedRoutine(SpriteRenderer sr)
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
    }

    private void Die()
    {
        Debug.Log($"<color=red>{gameObject.name} ha sido derrotado.</color>");
        Destroy(gameObject);
    }
}