using UnityEngine;

public class Projectile : MonoBehaviour
{
    [TagField]
    public string playerTag;
    public float liveTime = 10f;
    public Rigidbody2D rb;

    protected virtual void Start()
    {
        Destroy(gameObject, liveTime);
    }

    public virtual void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }
}

public class StuntProjectile : Projectile
{
    protected override void Start()
    {
        base.Start();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            collision.gameObject.GetComponent<PlayerMovement>().Stunt();
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
            Destroy(gameObject);
        }
    }
}
