using UnityEditor.ShaderGraph.Internal;
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
    [SerializeField] private BaseCounter baseCounter;

    [SerializeField] private float xvalue = 90f;
    [SerializeField] private float zvalue = 0f;
    [SerializeField] private float spinSpeed = 75f;
    [SerializeField] private float spinTimer = 0;
    public bool canSpin = false;


    private void Start()
    {
        PlayerController.Instance.OnSelectedCounterChanged += OnSelectedCounterChanged;
    }

    private void LateUpdate()
    {
        Transform camTransform = Camera.main.transform;
        if (camTransform == null) return;

        switch (mode)
        {
            case Mode.CameraForwardFlat:
                float cameraYRotation = camTransform.eulerAngles.y;
                float cameraXRotation = camTransform.eulerAngles.x;
                
                transform.rotation = Quaternion.Euler(cameraXRotation + xvalue, cameraYRotation, spinTimer + zvalue);
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

    private void Update()
    {
        if (canSpin){
        spinTimer += Time.deltaTime * spinSpeed;
        if (spinTimer > 360)
        {
            spinTimer = 0f;
        }
        }
    }

    private void OnSelectedCounterChanged(object sender, PlayerController.OnSelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == baseCounter)
        {
            canSpin = true;
        }
        else
        {
            canSpin = false;
            spinTimer = 0f;
        }
    }
}
