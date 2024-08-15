using UnityEngine;
using System.Collections;

public class RespawnManager : MonoBehaviour
{
    private Vector3 lastRespawnPosition;
    private CharacterController charController;
    private GameObject lastRespawnFlag;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material greenMaterial;
    private RelativeMovement playerMovement;
    [Header("Audio")]
    [SerializeField] private AudioSource flagSound;
    [Header("Powerups")]
    [SerializeField] public SlowFallPowerup slowFallPowerup;
    [SerializeField] public SpeedBoost speedBoost;

    private void Start()
    {
        lastRespawnPosition = transform.position;
        charController = GetComponent<CharacterController>();
        playerMovement = GetComponent<RelativeMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RespawnPoint"))
        {
            lastRespawnPosition = other.transform.position;
            Debug.Log("Collided with respawn point. New respawn position: " + lastRespawnPosition);

            if (lastRespawnFlag != null && lastRespawnFlag != other.gameObject)
            {
                UpdateFlagColor(lastRespawnFlag, redMaterial);
            }

            if (lastRespawnFlag != other.gameObject)
            {
                UpdateFlagColor(other.gameObject, greenMaterial);

                if (flagSound != null)
                {
                    flagSound.PlayOneShot(flagSound.clip);
                }
            }

            lastRespawnFlag = other.gameObject;
        }
        else if (other.CompareTag("DeathRespawner"))
        {
            Debug.Log("Collided with Death Respawner. Respawning at: " + lastRespawnPosition);
            StartCoroutine(HandleRespawn());
        }
    }

    private IEnumerator HandleRespawn()
    {
        if (charController != null)
        {
            charController.enabled = false;
        }

        yield return new WaitForSeconds(0.1f);

        transform.position = lastRespawnPosition;
        Debug.Log("Player position set to: " + transform.position);

        if (charController != null)
        {
            charController.enabled = true;
        }

        if (playerMovement != null)
        {
            playerMovement.ResetSpeedBoost();
            playerMovement.StopSlowFall(); // Directly stop slow fall on the player
        }

        if (slowFallPowerup != null)
        {
            slowFallPowerup.ResetPowerup(); // Ensure slow fall is stopped
        }

        if (speedBoost != null)
        {
            speedBoost.ResetPowerup(); // Ensure speed boost is stopped
        }
    }

    private void UpdateFlagColor(GameObject respawnPoint, Material material)
    {
        Transform flagTransform = respawnPoint.transform.Find("Flag");
        if (flagTransform != null)
        {
            Renderer flagRenderer = flagTransform.GetComponent<Renderer>();
            if (flagRenderer != null)
            {
                flagRenderer.material = material;
            }
        }
    }
}
