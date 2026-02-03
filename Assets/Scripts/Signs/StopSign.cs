using UnityEngine;

public class StopSign : MonoBehaviour
{
    public SignManager sign;

    public Sprite stopSprite;
    public Sprite potsSprite;

    public GameObject stopCollider;
    public GameObject pots;

    private void Start()
    {
        sign.OnFlip += OnFlipped;
        sign.OnRuined += OnRuined;
        stopCollider.SetActive(true);
        pots.SetActive(false);
        sign.toFlip.sprite = stopSprite;
    }

    void OnRuined()
    {
        stopCollider.SetActive(false);
        pots.SetActive(false);
        sign.toFlip.sprite = stopSprite;
    }

    void OnFlipped(bool flipped)
    {
        stopCollider.SetActive(!flipped);
        pots.SetActive(flipped);
        if (!flipped)
        {
            sign.toFlip.sprite = stopSprite;
        }
        else
        {
            sign.toFlip.sprite = potsSprite;
        }
    }
}
