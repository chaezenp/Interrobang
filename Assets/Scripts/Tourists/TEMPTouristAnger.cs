using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class TEMPTouristAnger : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject touristObj;
    [SerializeField] private Transform target;
    [SerializeField] private Transform originalPos;

    public Slider timerSlider;
    public GameObject failX;
    public Transform playerTransform;
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public float interestTimer = 5f;
    public float countdownDuration = 10f;
    public GameObject explodeDeath;

    private bool isChasing = false;
    private bool hasCaughtPlayer = false;
    private bool hasTriggered = false;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // if we caught the player then freeze movement
        if (hasCaughtPlayer) return;

        // if chasing and didnt catch player yet then keep chasing
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

        // if we already triggered the chase once then skip the slider checks
        if (hasTriggered) return;
        if (timerSlider == null) return;

        // trigger chase if slider is active and hits zero
        if (failX.gameObject.activeInHierarchy)
        {
            if (timerSlider.value <= 0f)
            {
                isChasing = true;
                hasTriggered = true;
                Debug.Log("Slider reached zero! Chase locked and started!");
            }
        }
    }

    void ChasePlayer()
    {
        if (playerTransform == null) return;
        if (failX != null) failX.gameObject.SetActive(false);

        // FIX: Force the agent to handle its own rotation when actively chasing
        _agent.updateRotation = true;

        if (Vector3.Distance(_agent.destination, Player.transform.position) > 0.1f)
        {
            _agent.SetDestination(Player.transform.position);
        }
    }

    private void ExecuteLoseInterest()
    {
        isChasing = false;
        Debug.Log("Lost Interest");
        ResetPosition();
    }

    private void ResetPosition()
    {
        interestTimer = countdownDuration;
        _agent.SetDestination(originalPos.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && timerSlider.value <= 0)
        {
            CaughtThePlayer();
        }
    }

    private void CaughtThePlayer()
    {
        hasCaughtPlayer = true;
        isChasing = false;
        Debug.Log("Game Over! The enemy caught the player!");
        if (Player != null && explodeDeath != null && !timerSlider.gameObject.activeInHierarchy)
        {
            Player.SetActive(false);
            explodeDeath.SetActive(true);
        }
    }
}
