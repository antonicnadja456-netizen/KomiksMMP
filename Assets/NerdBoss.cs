using System;
using System.Collections;
using UnityEngine;

public class NerdBoss : MonoBehaviour
{
    [TagField]
    public string playerTag;
    public LayerMask signLayer;

    public Rigidbody2D rb;
    public Animator animator;
    public AudioSource shootAudioSource;
    public AudioSource saiyanAudioSource;

    public Transform[] movePoints;
    public int currentPoint = 0;
    public float moveSpeed = 6;
    public float moveSmoothing = 0.5f;
    public bool canMove = false;
    public bool isRight = false;

    public int currentHealth = 4;
    public int maxHealth = 4;

    public GameObject[] signProjectiles;
    public GameObject target = null;
    public bool canAttack = true;
    public float cooldown = 2;
    public float projectileSpeed = 3;

    public bool battleStarted = false;
    public bool defeated = false;

    public HealthUI healthUI;
    public GameObject nerdText;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag(playerTag);
        healthUI.gameObject.SetActive(false);
        nerdText.SetActive(false);

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd += OnDialogueEnd;
        }
    }

    private void Update()
    {
        if (defeated) return;
        if (!battleStarted) return;

        if (Vector2.Distance(transform.position, movePoints[currentPoint].position) <= 0.01f * moveSpeed && canMove)
        {
            rb.linearVelocity = Vector2.zero;
            canMove = false;
            animator.ResetTrigger("Fly");
            animator.SetTrigger("FlyEnd");
            canAttack = true;
        }

        Attack();
    }

    private void FixedUpdate()
    {
        if (defeated) return;
        if (!battleStarted) return;
        if (!canMove) return;
        if (movePoints.Length == 0) return;

        Vector2 dir = (movePoints[currentPoint].position - transform.position).normalized;

        Vector2 desiredVelocity = dir * moveSpeed;
        if (dir != Vector2.zero)
        {
            rb.linearVelocity += moveSpeed * Time.fixedDeltaTime * dir;
        }

        rb.linearVelocity = desiredVelocity * (1f - moveSmoothing) + rb.linearVelocity * moveSmoothing;

        if (rb.linearVelocityX >= 0 && !isRight)
        {
            isRight = true;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (rb.linearVelocityX < 0 && isRight)
        {
            isRight = false;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    public void Attack()
    {
        if (!battleStarted) return;
        if (!canAttack) return;
        if (canMove) return;

        canAttack = false;

        // Shoot
        Vector2 direction = (target.transform.position - transform.position).normalized;
        GameObject projectile = Instantiate(signProjectiles[UnityEngine.Random.Range(0, signProjectiles.Length)], transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().SetVelocity(direction * projectileSpeed);

        shootAudioSource.Play();

        StartCoroutine(AttackCooldown());
    }

    public void OnDialogueEnd()
    {
        int value = DialogueManager.Instance.GoodAnswers;

        if (value < maxHealth)
        {
            StartBattle();
            currentHealth -= value;
            healthUI.UpdateHearts(currentHealth);
        }
        else
        {
            battleStarted = true;
            canMove = false;
            canAttack = false;
            currentPoint = 0;
            rb.linearVelocity = Vector2.zero;
            healthUI.SetMaxHearts(maxHealth);
            healthUI.UpdateHearts(currentHealth);
            TakeDamage(value);
        }
    }

    public void StartBattle()
    {
        battleStarted = true;
        canMove = false;
        canAttack = false;
        currentPoint = 0;
        rb.linearVelocity = Vector2.zero;
        healthUI.gameObject.SetActive(true);
        nerdText.SetActive(true);

        healthUI.SetMaxHearts(maxHealth);
        healthUI.UpdateHearts(currentHealth);

        animator.SetTrigger("Became Saian");
        saiyanAudioSource.Play();
    }

    public void SaianEnd()
    {
        animator.SetFloat("Saian", 1f);

        canMove = true;

        animator.ResetTrigger("Became Saian");
        animator.SetTrigger("Fly");
    }

    public void TakeDamage(int damage)
    {
        if (!battleStarted) return;

        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);
        if (currentHealth <= 0)
        {
            // DEFEATED
            Debug.Log("DEFEATED");
            defeated = true;
            EndManager.Instance.OpenWinScreen();
        }
        else
        {
            canMove = true;
            int newPoint = UnityEngine.Random.Range(0, movePoints.Length - 1);
            currentPoint = newPoint == currentPoint ? ((currentPoint + 1) % movePoints.Length) : newPoint;

            animator.ResetTrigger("FlyEnd");
            animator.SetTrigger("Fly");
        }
    }

    public IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(cooldown);

        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (defeated) return;
        if (!battleStarted) return;

        if (((1 << collision.gameObject.layer) & signLayer.value) != 0)
        {
            SignManager sm = collision.gameObject.GetComponent<SignManager>();
            if (!sm.flipped)
            {
                sm.Flip();
            }
        }
    }
}
