using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] Transform[] points; // Array of points
    [SerializeField] float speed = 3.0f;
    [SerializeField] float shootInterval = 2.0f;
    [SerializeField] float waitTimeAtPoints = 2.0f; // Variable time to wait at each point
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Collider detectionTrigger; // Public variable for the trigger collider

    [SerializeField] AudioSource loopAudioSource; // Loops and always plays for the enemy
    [SerializeField] AudioSource shootAudioSource; // Plays when the enemy shoots
    [SerializeField] AudioSource playerDetectedAudioSource; // Plays when the player enters the detection zone

    private int currentPointIndex = 0;
    private bool playerInRange;
    private Transform playerTransform;
    private bool isShooting; // Flag to determine if the enemy should stop moving
    private bool isWaiting; // Flag to determine if the enemy is waiting at a point

    private void Start()
    {
        if (points.Length == 0)
        {
            Debug.LogError("No points assigned for the enemy to move to!");
            return;
        }

        StartCoroutine(ShootAtPlayer());

        if (detectionTrigger == null)
        {
            Debug.LogError("Detection Trigger is not assigned!");
        }
        else
        {
            // Assign the trigger events
            detectionTrigger.gameObject.AddComponent<TriggerHandler>().Initialize(this);
        }

        if (loopAudioSource != null)
        {
            loopAudioSource.loop = true;
            loopAudioSource.Play();
        }
    }

    private void Update()
    {
        if (!playerInRange && !isWaiting) // Move only if the player is not in range and not waiting
        {
            // Move towards the current target point
            float step = speed * Time.deltaTime;
            Transform targetPoint = points[currentPointIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);

            // Look in the direction of movement
            Vector3 directionToTarget = (targetPoint.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);

            // Check if the enemy reached the target point
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                StartCoroutine(WaitAtPoint()); // Start waiting coroutine
            }
        }
        else if (playerInRange) // Look at the player
        {
            if (playerTransform != null)
            {
                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
            }
        }
    }

    public void OnPlayerEnterTrigger(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger zone.");
            playerInRange = true;
            playerTransform = other.transform;
            isShooting = true; // Set the flag to true when the player is in range

            if (playerDetectedAudioSource != null)
            {
                playerDetectedAudioSource.PlayOneShot(playerDetectedAudioSource.clip);
            }
        }
    }

    public void OnPlayerExitTrigger(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger zone.");
            playerInRange = false;
            playerTransform = null;
            isShooting = false; // Reset the flag when the player leaves the range
        }
    }

    private IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            if (playerInRange && fireballPrefab != null && playerTransform != null)
            {
                Debug.Log("Shooting at player.");
                // Instantiate fireball in front of the enemy
                GameObject fireball = Instantiate(fireballPrefab, transform.position + transform.forward * 2f, Quaternion.identity);
                
                // Optionally, set fireball direction
                Fireball fireballScript = fireball.GetComponent<Fireball>();
                if (fireballScript != null)
                {
                    fireballScript.SetDirection(transform.forward); // Pass direction if needed
                }

                if (shootAudioSource != null)
                {
                    shootAudioSource.PlayOneShot(shootAudioSource.clip);
                }
            }
            yield return new WaitForSeconds(shootInterval);
        }
    }

    private IEnumerator WaitAtPoint()
    {
        isWaiting = true; // Set waiting flag
        yield return new WaitForSeconds(waitTimeAtPoints); // Wait for the specified time
        isWaiting = false; // Reset waiting flag
        currentPointIndex = (currentPointIndex + 1) % points.Length; // Move to the next point
    }
}

public class TriggerHandler : MonoBehaviour
{
    private EnemyAI enemyAI;

    public void Initialize(EnemyAI ai)
    {
        enemyAI = ai;
    }

    private void OnTriggerEnter(Collider other)
    {
        enemyAI.OnPlayerEnterTrigger(other);
    }

    private void OnTriggerExit(Collider other)
    {
        enemyAI.OnPlayerExitTrigger(other);
    }
}
