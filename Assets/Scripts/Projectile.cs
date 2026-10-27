using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float knockbackForce = 4f;
    [SerializeField] private float lifeTime = 4f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Enemy") || other.GetComponent<StaticShooterEnemy>() != null)
        {
            return;
        }

        
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
        {
            player = other.GetComponentInParent<PlayerController>();
        }

        if (player != null || other.CompareTag("Player"))
        {
            if (player != null)
            {
                
                player.TakeDamage(damage);

               
                player.hitTime = 0.2f;
                player.hitForce = knockbackForce;
                player.hitFromRight = transform.position.x > player.transform.position.x;
            }

            
            Destroy(gameObject);
            return;
        }

        
        if (((1 << other.gameObject.layer) & LayerMask.GetMask("Ground")) != 0)
        {
            Destroy(gameObject);
        }
    }
}