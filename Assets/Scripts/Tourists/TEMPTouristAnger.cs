using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class TEMPTouristAnger : MonoBehaviour
{
    public Slider timerSlider;       
    public GameObject failX;

    public Transform playerTransform; 
    public GameObject Player;
    public float moveSpeed = 5f;
    public GameObject explodeDeath;

    private bool isChasing = false;
    private bool hasCaughtPlayer = false;

    private bool hasTriggered = false;

    void Update()
    {
        // if we caught the player then freeze movement
        if (hasCaughtPlayer) return;

        // if chasing and didnt catch player yet then keep chasing
        if (isChasing)
        {
            ChasePlayer();
            return;
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

        // Move towards the player's position
        Vector3 targetPos = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // Turn to face the player
        transform.LookAt(targetPos);
    }

        private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && timerSlider.value <= 0)
        {
            CaughtThePlayer();
        }
    }

        void CaughtThePlayer()
    {
        hasCaughtPlayer = true;
        isChasing = false;
        Debug.Log("Game Over! The enemy caught the player!");
        
        if(Player != null && explodeDeath != null && !timerSlider.gameObject.activeInHierarchy)
            {
            Player.SetActive(false);
            explodeDeath.SetActive(true);
        }
    }
}
