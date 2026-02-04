using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public HealthUI healthUI;

    private SpriteRenderer spriteRenderer;

    public bool isDamageable = true;
    public float damageCooldown = 2.0f;
    public float initialFlashRedDuration = 0.1f;
    public float maxFlashRedDuration = 0.5f;
    public float flashRedMultiplier = 1.33f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (isDamageable)
        {
            currentHealth -= damage;
            healthUI.UpdateHearts(currentHealth);

            StartCoroutine(FlashRed());
            StartCoroutine(StartDamageCooldown());

            if (currentHealth <= 0)
            {
                EndManager.Instance.OpenDeathScreen();
            }
        }
    }

    private IEnumerator FlashRed()
    {
        float totalTime = 0f;

        for (float i = initialFlashRedDuration; i + totalTime < damageCooldown; i*=flashRedMultiplier)
        {
            i = Mathf.Min(i, maxFlashRedDuration);
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(i);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            totalTime += i;
        }
    }

    public IEnumerator StartDamageCooldown()
    {
        isDamageable = false;

        yield return new WaitForSeconds(damageCooldown);

        isDamageable = true;
    }
}
