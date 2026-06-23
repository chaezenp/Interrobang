using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _jumpSpeed = 3f;
    [SerializeField] private float _rotateSpeed = 20f;


    private PlayerInputController _playerInputController;
    private GroundController _groundController;
    private Rigidbody _rb;
    private bool _JumpTriggered;

    private void Awake()
    {
        _playerInputController = GetComponent<PlayerInputController>();
        _groundController = GetComponent<GroundController>();
        _rb = GetComponent<Rigidbody>();

        _playerInputController.OnJumpButtonPressed += JumpButtonPressed;
    }

    private void Update()
    {
        Vector3 velocity = new Vector3(
            _playerInputController.MovementInputVector.x, 0,
            _playerInputController.MovementInputVector.y) 
            * _speed;

        velocity.y = _rb.linearVelocity.y;

        if(_JumpTriggered)
        {
            velocity.y = _jumpSpeed;
            _JumpTriggered = false;
        }

        _rb.linearVelocity = velocity;

        //Rotate the player model to face movement direction
        //Not = vec3 zero so it doesnt snap back to forward
        if (velocity != Vector3.zero)
        {
            //what direction character is facing by movement
            Quaternion targetRotation = Quaternion.LookRotation(velocity);

            //smooth rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);        }    
    }

    private void JumpButtonPressed()
    {
        if (_groundController.IsGrounded)
        {
        _JumpTriggered = true;
        }    
    }
}
