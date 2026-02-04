using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public delegate void OnJumpStartedEventHandler();

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveSmooting = 0.5f;
    public bool isFacingRight = false;
    public float jumpPower = 10f;
    public bool isJumping = false;
    Vector2 movementInput = Vector2.zero;
    public bool isGrounded = false;

    public AudioSource footstepsAudioSource;
    public AudioSource kickAudioSource;
    public AudioSource jumpAudioSource;
    public AudioSource landAudioSource;

    [Header("Gravity")]
    public float baseGravity = 1f;
    public float maxFallSpeed = 5f;
    public float fallGravityMult = 1.1f;

    [Header("Attack")]
    public GameObject attackPoint;
    public float attackRadius = 0.75f;
    public LayerMask enemiesLayer;
    public LayerMask signsLayer;
    public LayerMask nerdsLayer;
    public LayerMask spawnersLayer;
    public bool canAttack = true;
    public float attackCooldown = 2f;

    [Header("Knockback")]
    public float signKnockbackForce = 10f;
    public float knockback = 0f;
    public float knockbackSmoothTime = 1f;
    private float knockbackVelocity = 0;
    private float epsilon = 5f;

    [Header("Stunt")]
    public float stuntCooldown = 2;

    public Rigidbody2D rb;

    public Animator animator;

    public event OnJumpStartedEventHandler OnJumpStarted;
    private CapsuleCollider2D capsuleCollider;

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        InputManager.Instance.OnInteractPressed += OnInteract;
        InputManager.Instance.OnAttackPressed += OnAttack;
        InputManager.Instance.OnJumpPressed += OnJump;
        InputManager.Instance.OnMove += OnMove;

        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();
    }

    private void FixedUpdate()
    {
        Vector2 currentMoventInput = movementInput;
        if (animator.GetBool("IsAttacking"))
        {
            currentMoventInput = Vector2.zero;
        }

        float desiredVelocityX = currentMoventInput.x * moveSpeed;
        if (currentMoventInput.x != 0)
        {
            rb.linearVelocityX += currentMoventInput.x * moveSpeed * Time.fixedDeltaTime;
        }

        if (Mathf.Abs(knockback) > 0)
        {
            knockback = Mathf.SmoothDamp(knockback, 0f, ref knockbackVelocity, knockbackSmoothTime);
            if (Mathf.Abs(knockback) < epsilon)
            {
                knockback = 0f;
            }

            rb.linearVelocityX += knockback;
            desiredVelocityX += knockback;
        }

        rb.linearVelocityX = desiredVelocityX * (1f - moveSmooting) + rb.linearVelocityX * moveSmooting;

        Gravity();

        bool previousGround = isGrounded;
        isGrounded = Physics2D.CapsuleCast(capsuleCollider.bounds.center, capsuleCollider.size, CapsuleDirection2D.Vertical, capsuleCollider.transform.rotation.eulerAngles.z, Vector2.down, 0.05f);
        animator.SetBool("IsGrounded", isGrounded);
        if (previousGround != isGrounded)
        {
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Landing");
        }

        if (isGrounded && animator.GetBool("Landed"))
        {
            if (!footstepsAudioSource.isPlaying && movementInput.x != 0)
            {
                footstepsAudioSource.Play();
            }
            else if (footstepsAudioSource.isPlaying && movementInput.x == 0)
            {
                footstepsAudioSource.Stop();
            }
        }
    }

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallGravityMult;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    void FlipSprite()
    {
        Vector2 currentMoventInput = movementInput;
        if (animator.GetBool("IsAttacking"))
        {
            currentMoventInput = Vector2.zero;
        }

        if (isFacingRight && currentMoventInput.x < 0f || !isFacingRight && currentMoventInput.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    isJumping = false;
    //}

    public void Attack()
    {
        kickAudioSource.Play();

        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, enemiesLayer);

        foreach (Collider2D enemyGameObject in enemy)
        {
            if (!enemyGameObject.isTrigger)
            {
                enemyGameObject.gameObject.GetComponent<Enemy>().TakeDamage(1);
            }
        }

        Collider2D[] signs = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, signsLayer);

        foreach (Collider2D signGameObject in signs)
        {
            if (!signGameObject.gameObject.GetComponent<SignManager>().Ruin())
            {
                Knockback(signGameObject.gameObject.transform, signKnockbackForce);
            }
        }

        Collider2D[] nerds = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, nerdsLayer);

        foreach (Collider2D nerdsGameObject in nerds)
        {
            nerdsGameObject.gameObject.GetComponent<NerdBoss>().TakeDamage(1);
        }

        Collider2D[] spawners = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, spawnersLayer);

        foreach (Collider2D spawnerGameObject in spawners)
        {
            spawnerGameObject.gameObject.GetComponent<EnemySpawner>().Close();
        }

        attackPoint.GetComponent<SpriteRenderer>().color = Color.yellow;

        StartCoroutine(AttackCooldown());
    }

    public void EndAttack()
    {
        animator.ResetTrigger("Attack");
        animator.SetBool("IsAttacking", false);
    }

    public IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;

        attackPoint.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void Knockback(Transform source, float knockbackForce)
    {
        Vector2 direction = new Vector2(transform.position.x - source.position.x, 0).normalized;
        knockback = direction.x * knockbackForce;
    }

    public void Stunt()
    {
        InputManager.Instance.DisablePlayer();
        StartCoroutine(StuntCooldown());
    }

    public IEnumerator StuntCooldown()
    {
        yield return new WaitForSeconds(stuntCooldown);

        InputManager.Instance.EnablePlayer();
    }

    #region [ Input ]

    public void OnMove(Vector2 move)
    {
        movementInput = move;
        if (move.x != 0)
        {
            animator.ResetTrigger("RunStop");
            animator.SetTrigger("RunStart");
        }
        else
        {
            animator.ResetTrigger("RunStart");
            animator.SetTrigger("RunStop");
        }
    }

    public void OnJump()
    {
        if (isGrounded)
        {
            jumpAudioSource.Play();
            rb.linearVelocityY = jumpPower;
            isGrounded = false;
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("Landed", false);
            animator.ResetTrigger("RunStart");
            animator.ResetTrigger("RunStop");
            animator.SetTrigger("Jump");
            OnJumpStarted?.Invoke();
        }
    }

    public void OnLanded()
    {
        landAudioSource.Play();
        animator.ResetTrigger("Landing");
        animator.SetBool("Landed", true);
        if (movementInput.x != 0)
        {
            animator.ResetTrigger("RunStop");
            animator.SetTrigger("RunStart");
        }
    }

    public void OnAttack()
    {
        if (canAttack && isGrounded && animator.GetBool("Landed") && movementInput.x == 0)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("IsAttacking", true);
            canAttack = false;
        }
    }

    public void OnInteract()
    {
    }

    #endregion
}
