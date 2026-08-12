using UnityEngine;

public class LookAtCameraIcons : MonoBehaviour
{
    private enum Mode
    {
        CameraForwardFlat,
        LookAtYAxisOnly,
        CameraForwardFull
    }

    [SerializeField] private Mode mode = Mode.CameraForwardFlat;

    private void LateUpdate()
    {
        Transform camTransform = Camera.main.transform;
        if (camTransform == null) return;

        switch (mode)
        {
            case Mode.CameraForwardFlat:
                float cameraYRotation = camTransform.eulerAngles.y;
                
                transform.rotation = Quaternion.Euler(0f, cameraYRotation, 0f);
                break;

            case Mode.LookAtYAxisOnly:
                Vector3 targetPos = camTransform.position;
                targetPos.y = transform.position.y; 
                Vector3 lookDir = transform.position - targetPos; 
                
                if (lookDir.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(lookDir);
                }
                break;

            case Mode.CameraForwardFull:
                transform.rotation = camTransform.rotation;
                break;
        }
    }
}
