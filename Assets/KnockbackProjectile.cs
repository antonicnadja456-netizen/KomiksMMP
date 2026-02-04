using UnityEngine;

public class KnockbackProjectile : Projectile
{
    protected override void Start()
    {
        base.Start();
    }

    public override void SetVelocity(Vector2 velocity)
    {
        base.SetVelocity(velocity);
        if (velocity.normalized.x < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            PlayerMovement pm = collision.gameObject.GetComponent<PlayerMovement>();
            pm.Knockback(transform, pm.signKnockbackForce);
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
            Destroy(gameObject);
        }
    }
}
