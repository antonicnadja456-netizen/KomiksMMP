using UnityEngine;
using Unity.Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float offsetBase = 5f;
    [SerializeField] private float offset = 5f;
    [SerializeField] private float smoothTime = 0.5f;
    [SerializeField] private float velocity = 0f;
    [SerializeField] private PlayerMovement playerMovement;
    
    public CinemachinePositionComposer positionComposer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        positionComposer = GetComponent<CinemachinePositionComposer>();
    }

    private void FixedUpdate()
    {
        if (playerMovement.isFacingRight)
        {
            offset = Mathf.SmoothDamp(offset, offsetBase, ref velocity, smoothTime);
            
        }
        else
        {
            offset = Mathf.SmoothDamp(offset, -offsetBase, ref velocity, smoothTime);
        }

        positionComposer.TargetOffset = new Vector3(offset,0,0);
    }
}
