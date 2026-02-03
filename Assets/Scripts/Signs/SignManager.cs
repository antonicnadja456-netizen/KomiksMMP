using UnityEngine;
using UnityEngine.Audio;

public delegate void SignFlippedEventHandler(bool flipped);
public delegate void SignRuinedEventHandler();

[RequireComponent(typeof(Collider2D))]
public class SignManager : MonoBehaviour
{
    [TagField]
    public string playerTag;
    public GameObject interactionObj;

    public AudioSource audioSource;

    public SpriteRenderer toFlip;
    public bool flipX = false;
    public bool flipY = false;
    public bool ruinable = false;

    public bool flipped = false;
    public bool ruined = false;
    public bool showInteraction = false;

    public event SignFlippedEventHandler OnFlip;
    public event SignRuinedEventHandler OnRuined;

    private void Start()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogError("Nie ma input managera");
        }
        else
        {
            InputManager.Instance.OnInteractPressed += OnInteract;
        }
        interactionObj.SetActive(showInteraction);
        toFlip.GetComponent<Rigidbody2D>().simulated = false;
        foreach (var col in toFlip.GetComponents<Collider2D>())
        {
            col.enabled = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag) && !showInteraction && !ruined)
        {
            showInteraction = !showInteraction;
            interactionObj.SetActive(showInteraction);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag) && showInteraction && !ruined)
        {
            showInteraction = !showInteraction;
            interactionObj.SetActive(showInteraction);
        }
    }

    public bool Ruin()
    {
        audioSource.Play();
        if (!ruinable) return false;

        ruined = true;
        showInteraction = false;
        interactionObj.SetActive(showInteraction);

        Rigidbody2D rb = toFlip.GetComponent<Rigidbody2D>();
        rb.simulated = true;
        rb.WakeUp();
        foreach (var col in toFlip.GetComponents<Collider2D>())
        {
            col.enabled = true;
        }
        OnRuined?.Invoke();
        return true;
    }

    public void OnInteract()
    {
        if (showInteraction && !ruined)
        {
            Flip();
        }
    }

    public void Flip()
    {
        if (flipX)
        {
            toFlip.flipX = !toFlip.flipX;
        }
        if (flipY)
        {
            toFlip.flipY = !toFlip.flipY;
        }
        flipped = !flipped;
        OnFlip?.Invoke(flipped);
    }
}
