using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class ClearCounter3 : MonoBehaviour
 {
//     //Might reuse some fucntions from here
//     // This is not the real Clear counter script
//     //It has been taken over by pickup/drop functions so we have 
//     //something for presentations
//     //Temp Variables
//     [SerializeField] GameObject thisOBJ;
//     public bool isHoldingitem = false;
//     public bool canGive = false;
//     public bool isTourist = false;
//     public bool needMultiplePress = false;
//     [SerializeField] Transform handHoldpoint;
//     [SerializeField] Transform fillHoldPoint;

//     [SerializeField] Collider myCollider;

//     public string objectiveTag = "Tourists";
    
//     public string StepTag = "FillStation";

//     [SerializeField] GameObject BaseModel;
//     [SerializeField] GameObject FilledModel;

//     [SerializeField] GameObject touristQuestion;
//     [SerializeField] GameObject touristRequest;
//     [SerializeField] GameObject touristCountdown;
//     public UIButtonTrigger _UiButton;
//     public int tourist2COunt = 0;
//     public int buttonPressesNeeded = 3;
//     [SerializeField] TouristsTimer TT; 


//     //Temp start
//     private void Start()
//     {
//         //thisOBJ = GetComponent<GameObject>();
//         //myCollider = GetComponent<SphereCollider>();
//         //_UiButton = GetComponent<UIButtonTrigger>();


//     }

//     private void Update()
//     {
// GameObject obj = GameObject.Find("TEMPSunscreen(Clone)");

// if (obj != null)
// {

//     //Debug.Log("Found the instantiated object via name!");
//     TT._item = obj;
//     TT.spawnedinYET = true;
//     canGive = true;
// }

//     }

//     public void Interact()
//     {
//         //Debug.Log("INTERACTS");
//         if (thisOBJ != null)
//         {
//         if (isTourist)
//             {
//                 TalkToTourist();
//             } 
        
//         if (!isHoldingitem && !isTourist)
//         {
//             Debug.Log("Picking");
//             TempPickup();
//         }
//         else if (canGive)
//             {
//                 if(needMultiplePress)
//                 {
//                     multiplePresses();
//                 }
//                 else
//                 {
//                     Destroy(gameObject);
//                 }
                
//             }
        
//         else if (isHoldingitem)
//         {
//             Debug.Log("Dropping");
//             TempDropItem();
//         }
//     }

//     if (TT._item != null)
//         {
//             if (canGive)
//             {
//                 if (needMultiplePress)
//                 {
//                     multiplePresses();
//                 }
//                 else { Destroy(TT._item); }
//             }
//         }
//     }

// //Temperary pickup and drop script for presentation whitebox
//     private void TempPickup()
//     {
//         Debug.Log("Picked UP");
//         isHoldingitem = true;
//         myCollider.isTrigger = true;
//         if (thisOBJ.TryGetComponent<Rigidbody>(out Rigidbody rb))
//         {
//             rb.isKinematic = true;
//             rb.useGravity = false;
//         }

//         // Attach the object to the hand anchor
//         thisOBJ.transform.SetParent(handHoldpoint);
        
//         // Snap to the hand's position and rotation (Optional: adjust if needed)
//         thisOBJ.transform.localPosition = Vector3.zero;
//         thisOBJ.transform.localRotation = Quaternion.identity;
//     }

//     private void TempDropItem()
//     {
//         isHoldingitem = false;
//         myCollider.isTrigger = false;
//         if (thisOBJ.TryGetComponent<Rigidbody>(out Rigidbody rb))
//         {
//             rb.isKinematic = false;
//             rb.useGravity = true;
//         }

//         // Detach from player
//         thisOBJ.transform.SetParent(null);
//         Debug.Log("Dropped");

//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (isHoldingitem){
//         if (other.CompareTag(StepTag))
//         {
//         thisOBJ.transform.SetParent(fillHoldPoint);
        
//         // Snap to the hand's position and rotation (Optional: adjust if needed)
//         thisOBJ.transform.localPosition = Vector3.zero;
//         thisOBJ.transform.localRotation = Quaternion.identity;
//         isHoldingitem = false;
//         BaseModel.SetActive(false);
//         FilledModel.SetActive(true);
//         }
//         if (other.CompareTag(objectiveTag))
//         {
//             canGive = true;
//                     Debug.Log("Check1: " + needMultiplePress);

//         }
//         }
//     }

//     //function to talk to tourist
//     private void TalkToTourist()
//     {
//         touristQuestion.SetActive(false);
//         _UiButton.isEnabled = false;
//         touristRequest.SetActive(true);
//         touristCountdown.SetActive(true);
//         Debug.Log("check3 " +needMultiplePress);

//     }

//     private void multiplePresses()
//     {
//         Debug.Log("All good");
//         tourist2COunt = tourist2COunt +1;
//         if (tourist2COunt > buttonPressesNeeded)
//         {
//             Destroy(TT._item);
//         }
//     }

 }
