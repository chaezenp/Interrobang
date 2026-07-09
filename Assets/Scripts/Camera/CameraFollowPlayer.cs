using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{

    public Camera MainCamera;
    public Transform playerTransform;
    public Vector3 offset;

    [SerializeField] private float smoothtime = 0.3f;
    [SerializeField] private Vector3 currentVelocity = Vector3.zero;
    public void Update()
    {
        if (MainCamera == null) return;

        Vector3 targetposition = playerTransform.position + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetposition, ref currentVelocity, smoothtime);


    }

}
