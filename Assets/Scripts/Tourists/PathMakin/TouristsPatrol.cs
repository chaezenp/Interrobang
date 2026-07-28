using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum EnemyState
{
    Patrolling,
    Following,
    ReturnHome
}

public class TouristPatrol : MonoBehaviour 
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform originalPos;
    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float stopAtDistance = 0.5f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float viewAngle = 100f;
    [SerializeField] private float losePlayerTime = 3f;

    private EnemyState _state = EnemyState.Patrolling;
    private int _currentPatrolIndex;
    private bool _isWaiting;
    private float timeSinceLostPlayer;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        GoToNextPatrolPoint();
    }

    
    void Update()
    {
        var distanceToPlayer = Vector3.Distance(player.position, transform.position);

        switch (_state)
        {
            case EnemyState.Patrolling:
                Patrol();
                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    _state = EnemyState.Following;
                }
                break;
            
            case EnemyState.Following:
                FollowPlayer();
                if (!CanSeePlayer())
                {
                    timeSinceLostPlayer += Time.deltaTime;
                    if (timeSinceLostPlayer > losePlayerTime)
                    {
                        _state = EnemyState.Patrolling;
                        GoToClosestPatrolPoint();
                    }
                }
                else
                {
                    timeSinceLostPlayer = 0f;
                }

                break;
        }
        
    }
    
    private void FollowPlayer()
    {
        _agent.SetDestination(player.position);
    }

    private void Patrol()
    {
        if (_isWaiting) return;
        if (!_agent.pathPending && _agent.remainingDistance <= stopAtDistance)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        _isWaiting = true;
        _agent.isStopped = true;

        yield return new WaitForSeconds(patrolWaitTime);

        _agent.isStopped = false;
        GoToNextPatrolPoint();
        _isWaiting = false;
    }

    private void GoToClosestPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        var closestIndex = 0;
        var closestDistance = float.MaxValue;

        for (var i = 0; i < patrolPoints.Length; i++)
        {
            var distance = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        _currentPatrolIndex = closestIndex;
        _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0 || originalPos == null) return;

        _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
        if (_currentPatrolIndex < patrolPoints.Length)
        {
            _currentPatrolIndex++;
        }    
    }

    private bool CanSeePlayer()
    {
        return isFacingPlayer() && HasClearPathToPlayer();
    }

    private bool isFacingPlayer()
    {
        var dirToPlayer = (player.position - transform.position).normalized;
        var angle = Vector3.Angle(transform.forward, dirToPlayer);
        return angle <= viewAngle / 2f;
    }

    private bool HasClearPathToPlayer()
    {
        var dirToPlayer = player.position - transform.position;
        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, dirToPlayer.magnitude))
        {
            return hit.transform == player;
        }

        return true;
    }

    private void ResetPosition()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= stopAtDistance)
        {
            // Reset index so it can patrol from the beginning
            _currentPatrolIndex = 0;
            
            _agent.isStopped = true;
        }
    }

private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, detectionRange);

    Vector3 leftBoundary = AngleToDirection(-viewAngle / 2f);
    Vector3 rightBoundary = AngleToDirection(viewAngle / 2f);

    Gizmos.color = Color.yellow;
    Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRange);
    Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRange);
}

private Vector3 AngleToDirection(float angleInDegrees)
{
    angleInDegrees += transform.eulerAngles.y;
    
    return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
}

}