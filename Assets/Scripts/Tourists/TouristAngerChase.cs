using UnityEngine;
using UnityEngine.AI;

public class TouristAngerChase : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] private GameObject touristObj;
    [SerializeField] private Transform target;

    [SerializeField] private GameObject touristNormalVIS;
    [SerializeField] private GameObject touristChaseVIS;

    [Header("Refs")]
    [SerializeField] private DeliveryCounter deliveryCounter;
    [SerializeField] private TouristRequestUI requestUI;

    public Transform originalPos;
    public GameObject Player;
    public GameObject PlayerModel;
    public Transform playerTransform;
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public float interestTimer = 5f;
    public float countdownDuration = 10f;
    public GameObject explodeDeath;

    private bool isChasing = false;
    private bool hasCaughtPlayer = false;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        playerTransform = Player.transform;
        target = Player.transform;
        ResetPosition();
    }

    private void OnEnable()
    {
        if (deliveryCounter != null)
        {
            deliveryCounter.OnRequestFailed += HandleRequestFailed;
        }
    }

    private void OnDisable()
    {
        if (deliveryCounter != null)
        {
            deliveryCounter.OnRequestFailed -= HandleRequestFailed;
        }
    }

    private void HandleRequestFailed()
    {
        isChasing = true;
        Debug.Log("Request failed! Chase locked and started!");
    }

    void Update()
    {
        // if we caught the player then freeze movement
        if (hasCaughtPlayer) return;

        // if chasing and didn't catch player yet then keep chasing
        if (isChasing)
        {
            interestTimer -= Time.deltaTime;
            if (interestTimer < 0)
            {
                ExecuteLoseInterest();
                return;
            }
            ChasePlayer();
            return;
        }

        // Arrival check at home base
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            // Stop agent rotation calculation so manual Slerp works
            _agent.updateRotation = false;

            // Rotates to face camera
            Quaternion worldBackwardRotation = Quaternion.LookRotation(-Vector3.forward, Vector3.up);
            touristObj.transform.rotation = Quaternion.Slerp(
                touristObj.transform.rotation,
                worldBackwardRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            // Keep agent rotation turned on if it is traveling back home
            _agent.updateRotation = true;
        }
    }

    void ChasePlayer()
    {
        if (playerTransform == null) return;
        if (requestUI != null) requestUI.HideFailIcon();

        // Force the agent to handle its own rotation when actively chasing
        _agent.updateRotation = true;
        if (touristChaseVIS != null && touristNormalVIS != null)
        {
            touristNormalVIS.SetActive(false);
            touristChaseVIS.SetActive(true);
        }

        if (Vector3.Distance(_agent.destination, Player.transform.position) > 0.1f)
        {
            _agent.SetDestination(Player.transform.position);
        }
    }

    private void ExecuteLoseInterest()
    {
        isChasing = false;
        Debug.Log("Lost Interest");
        if (touristChaseVIS != null && touristNormalVIS != null)
        {
            touristChaseVIS.SetActive(false);
            touristNormalVIS.SetActive(true);
        }
        ResetPosition();
    }

    private void ResetPosition()
    {
        interestTimer = countdownDuration;
        _agent.SetDestination(originalPos.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isChasing)
        {
            CaughtThePlayer();
        }
    }

    private void CaughtThePlayer()
    {
        hasCaughtPlayer = true;
        isChasing = false;
        Debug.Log("Game Over! The enemy caught the player!");
        if (PlayerModel != null && explodeDeath != null)
        {
            PlayerModel.SetActive(false);
            explodeDeath.SetActive(true);
        }
    }
}