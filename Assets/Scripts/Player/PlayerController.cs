using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IItemObjectParent
{
    public static PlayerController Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotateSpeed = 20f;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform ItemObjectHoldPoint;

    private ItemObject itemObject;

    //Temp variable for test pickup
    private ClearCounter3 activeCounter;
    //Real Clear Counter for interactions
    private BaseCounter selectedCounter;

    private DeliveryCounter deliveryCounter;


    private PlayerInputController _playerInputController;
    private Vector3 lastInteractDir;

    private Rigidbody _rb;

    private void Awake()
    {
        _playerInputController = GetComponent<PlayerInputController>();
        _rb = GetComponent<Rigidbody>();
        if (Instance != null)
        {
            Debug.LogError("More than one player instance ERROR");
        }
        Instance = this;
    }
    
    private void Start()
    {
        _playerInputController.OnInteractAction += OnInteractAction;
        _playerInputController.OnAltButtonAction += OnAltButtonAction;
        _playerInputController.OnInteractHoldAction += OnInteractHoldAction;
        _playerInputController.OnInteractHoldCanceled += OnInteractHoldCanceled;

    }

    private void OnInteractHoldCanceled(object sender, EventArgs e)
    {
        if (!HahaluGameManager.Instance.IsGamePlaying()) return;

        //Debug.Log("Button released. Holding down state stopped.");
        if(selectedCounter != null)
        {
            selectedCounter.InteractHoldRelease(this);
        }
    }

    private void OnInteractHoldAction(object sender, EventArgs e)
    {
        if (!HahaluGameManager.Instance.IsGamePlaying()) return;

        //Debug.Log("Holding down state active.");
        if(selectedCounter != null)
        {
            selectedCounter.InteractHold(this);
        }
    }

    private void OnAltButtonAction(object sender, EventArgs e)
    {
        if (!HahaluGameManager.Instance.IsGamePlaying()) return;

        if(selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }    
    }

    private void OnInteractAction(object sender, EventArgs e)
    {
        if (!HahaluGameManager.Instance.IsGamePlaying()) return;

        if(selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }

    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        Vector3 velocity = new Vector3(_playerInputController.MovementInputVector.x, 0, _playerInputController.MovementInputVector.y) * _speed;
 
        velocity.y = _rb.linearVelocity.y;

        _rb.linearVelocity = velocity;
        Vector2 inputVector = _playerInputController.GetMovementVectorNormalized();

        //Rotate the player model to face movement direction
        //!= vec3 zero so it doesnt snap back to forward
        if (velocity != Vector3.zero)
        {
            //what direction character is facing by movement
            Quaternion targetRotation = Quaternion.LookRotation(velocity);

            //smooth rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
        }

        //collisions
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        float movedistance = _speed * Time.deltaTime;
        float playerRadius = 0.5f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, movedistance);
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = _playerInputController.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
            if (moveDir != Vector3.zero)
            {
            lastInteractDir = moveDir;
            }
            float interactDistance = 1f;
            //Make the raycast hit higher because if left on default it would not hit counter
            Vector3 rayOrigin = transform.position + new Vector3(0f, 1f, 0f); 
            Debug.DrawRay(rayOrigin, lastInteractDir * interactDistance, Color.red);
            //Raycast to hit counter/Interactables

            Vector3 boxHalfExtents = new Vector3(0.1f, 5f, 0.1f);
            // idk box cast test :P


            //old ray cast //if (Physics.Raycast(rayOrigin, lastInteractDir, out RaycastHit raycastHit, interactDistance, counterLayerMask, QueryTriggerInteraction.Collide))

            if (Physics.BoxCast(rayOrigin, boxHalfExtents, lastInteractDir, out RaycastHit raycastHit, Quaternion.identity, interactDistance, counterLayerMask, QueryTriggerInteraction.Collide))
            
            {
              
                if(raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
                {
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
                }
                else
            {
                SetSelectedCounter(null);
            }
            }
            else
        {
            SetSelectedCounter(null);
        }
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

        public Transform GetItemObjectFollowTransform()
    {
        return ItemObjectHoldPoint;
    }

    public void SetItemObject(ItemObject itemObject)
    {
        //can add item grab animation
        //Debug.Log("Playing pick up anim");
        this.itemObject = itemObject;
    }

    public ItemObject GetItemObject()
    {
        return itemObject;
    }

    public void ClearItemObject()
    {
        itemObject = null;
    }

    public bool HasItemObject()
    {
        return itemObject != null;
    }
}
