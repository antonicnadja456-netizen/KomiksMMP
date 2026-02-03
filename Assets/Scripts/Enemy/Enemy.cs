using System.Collections;
using System.Linq;
using UnityEngine;

public delegate void EnemyDeadEventHandler(GameObject enemy);

public class Enemy : MonoBehaviour
{
    public EnemyPatrol patrol;
    public Rigidbody2D rb;
    public Animator animator;
    public float patrolSpeed;
    public float followSpeed;
    public float moveSmoothing = 0.5f;
    public bool isRight = true;

    public AudioSource footstepsAudioSource;
    public AudioSource attackAudioSource;

    [TagField]
    public string playerTag;

    public GameObject followTarget;

    [Header("Health")]
    public int currentHealth;
    public int maxHealth = 1;

    [Header("Attack")]
    public GameObject attackPoint;
    public float attackRadius = 0.75f;
    public float damageRadius = 0.75f;
    public LayerMask playerLayer;
    public bool canAttack = true;
    public bool canMove = true;
    public float attackCooldown = 1f;
    public bool attacking = false;


    public event EnemyDeadEventHandler OnEnemyDead;

    void Start()
    {
        currentHealth = maxHealth;
        patrol.StartPatrol();
    }

    void FixedUpdate()
    {
        if (followTarget != null)
        {
            Collider2D[] player = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, playerLayer);

            if (player.Count() != 0)
            {
                canMove = false;
                PlayAttack();
            }
            else if (!attacking)
            {
                canMove = true;
            }

            float lastVelo = rb.linearVelocityX;

            float desiredVelocityX = 0;

            if (canMove)
            {
                float dirX = followTarget.transform.position.x - transform.position.x;
                dirX = dirX > 0 ? 1 : (dirX < 0) ? -1 : 0;

                desiredVelocityX = dirX * followSpeed;
                if (dirX != 0)
                {
                    rb.linearVelocityX += dirX * followSpeed * Time.fixedDeltaTime;
                }
            }

            rb.linearVelocityX = desiredVelocityX * (1f - moveSmoothing) + rb.linearVelocityX * moveSmoothing;

            if (lastVelo != rb.linearVelocityX)
            {
                if (lastVelo == 0)
                {
                    animator.ResetTrigger("RunStop");
                    animator.SetTrigger("RunStart");
                }
                else if (rb.linearVelocityX == 0)
                {
                    animator.ResetTrigger("RunStart");
                    animator.SetTrigger("RunStop");
                }
            }
        }

        if (rb.linearVelocityX > 0 && !isRight)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            isRight = true;
        }
        else if (rb.linearVelocityX < 0 && isRight)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            isRight = false;
        }

        if (!footstepsAudioSource.isPlaying && rb.linearVelocityX != 0)
        {
            footstepsAudioSource.Play();
        }
        else if (footstepsAudioSource.isPlaying && rb.linearVelocityX == 0)
        {
            footstepsAudioSource.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag) && followTarget == null)
        {
            followTarget = collision.gameObject;
            patrol.StopPatrol();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag) && followTarget != null)
        {
            followTarget = null;
            patrol.StartPatrol();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            OnEnemyDead?.Invoke(gameObject);
            Destroy(gameObject);
        }
    }

    private void PlayAttack()
    {
        if (canAttack)
        {
            attackAudioSource.Play();
            canAttack = false;
            StartCoroutine(StartAttackCooldown());

            animator.ResetTrigger("RunStart");
            animator.SetTrigger("RunStop");
            animator.SetTrigger("Attack");
            attacking = true;
        }
    }

    public void Attack()
    {
        Collider2D[] player = Physics2D.OverlapCircleAll(attackPoint.transform.position, damageRadius, playerLayer);

        foreach (Collider2D playerGameObject in player)
        {
            playerGameObject.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
        }
    }

    public void AttackEnd()
    {
        canMove = true;

        animator.ResetTrigger("RunStop");
        animator.ResetTrigger("Attack");
        attacking = false;
    }

    public IEnumerator StartAttackCooldown()
    {
        attackPoint.GetComponent<SpriteRenderer>().color = Color.yellow;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;

        attackPoint.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
