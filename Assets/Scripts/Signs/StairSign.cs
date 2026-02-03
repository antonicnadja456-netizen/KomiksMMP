using UnityEngine;

public class StairSign : MonoBehaviour
{
    public SignManager sign;
    public GameObject stairs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sign.OnFlip += OnFlip;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnFlip(bool flipped)
    {
        if (sign.flipX)
        {
            stairs.transform.localScale = new Vector3(-1 * stairs.transform.localScale.x, stairs.transform.localScale.y, stairs.transform.localScale.z);
        }
        if (sign.flipY)
        {
            stairs.transform.localScale = new Vector3(stairs.transform.localScale.x, -1 * stairs.transform.localScale.y, stairs.transform.localScale.z);
        }
    }
}
