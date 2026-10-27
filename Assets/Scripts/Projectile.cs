using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float knockbackStrength = 5f;
    [SerializeField] private float lifeTime = 4f;

    private Vector2 direction;

    private void Start()
    {
        
        Destroy(gameObject, lifeTime);
    }

    
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    private void Update()
    {
        
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(damage);

            
            player.hitFromRight = transform.position.x > player.transform.position.x;
            player.hitForce = knockbackStrength;
            player.hitTime = 0.3f;

            Destroy(gameObject); 
            Destroy(gameObject); 
        }
        
        else if (((1 << other.gameObject.layer) & LayerMask.GetMask("Ground")) != 0)
        {
            Destroy(gameObject);
        }
    }
}