using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class ItemPickup : MonoBehaviour
{
    [SerializeField] private InputActionReference Interact;

    [SerializeField] private Transform handHoldPoint;
    [SerializeField] private Collider grabRangeTrigger;
    public string objectiveTag = "Tourist";

    private bool isHoldingItem = false;
    private bool _interactTriggered;
    private Vector3 originalScale;
    //private PlayerInputController _playerInputController;
    //private ItemWorldObject heldItem;
    private IInteractable closestInteractableCounter;
    public bool CanGrab { get; private set; }
    public GameObject TargetObject { get; private set; }
    private GameObject _itemInRange;
    private GameObject _grabbedItem;

    private void Start()
    {
        originalScale = transform.localScale; 
    }

    private void OnEnable()
    {
        if (Interact != null) Interact.action.Enable();
    }

    private void OnDisable()
    {
        if (Interact != null) Interact.action.Disable();
    }

    private void Update()
    {
        if (Interact != null && Interact.action.WasPressedThisFrame())
        {
            if (closestInteractableCounter != null)
            {
                closestInteractableCounter.Interact(this);
            }
            
            if (CanGrab && !isHoldingItem)
            {
            
            if (_grabbedItem == null && _itemInRange != null)
            {
                GrabObject(_itemInRange);
            }
            
            else if (_grabbedItem != null)
            {
                DropObject();
            }
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable counter))
        {
            closestInteractableCounter = counter;
        }

        if (_grabbedItem == null && other.isTrigger && !isHoldingItem) 
        {
            Debug.Log($"An item entered the grab range: {other.name}");
            CanGrab = true;
            _itemInRange = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable counter))
        {
            if (closestInteractableCounter == counter)
            {
                closestInteractableCounter = null;
            }
        }

        if (other.gameObject == _itemInRange) 
        {
            Debug.Log($"An item exit the grab range: {other.name}");
            CanGrab = false;
            _itemInRange = null;
        }
        

    }

    private void GrabObject(GameObject item)
    {
        _grabbedItem = item;

        if (_grabbedItem.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Attach the object to the hand anchor
        _grabbedItem.transform.SetParent(handHoldPoint);
        
        // Snap to the hand's position and rotation (Optional: adjust if needed)
        _grabbedItem.transform.localPosition = Vector3.zero;
        _grabbedItem.transform.localRotation = Quaternion.identity;
    }

    private void DropObject()
    {
        if (_grabbedItem.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Detach from player
        _grabbedItem.transform.SetParent(null);
        _grabbedItem = null;
    }

    // public bool IsHoldingItem() => heldItem != null;
    // public ItemWorldObject GetHeldItem() => heldItem;

    // public void GiveItem(ItemWorldObject item)
    // {
    //     heldItem = item;
    //     item.transform.parent = handHoldPoint;
    //     item.transform.localPosition = Vector3.zero;
    //     item.transform.localRotation = Quaternion.identity;
    // }

    // public ItemWorldObject TakeItem()
    // {
    //     ItemWorldObject itemToReturn = heldItem;
    //     heldItem = null;
    //     return itemToReturn;
    // }

}
