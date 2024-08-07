using UnityEngine;
using System.Collections;

public class RespawnManager : MonoBehaviour
{
    private Vector3 lastRespawnPosition;
    private CharacterController charController;
    private GameObject lastRespawnFlag;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material greenMaterial;
    private RelativeMovement playerMovement; // Reference to RelativeMovement

    private void Start()
    {
        // Initialize with the player's starting position
        lastRespawnPosition = transform.position;
        charController = GetComponent<CharacterController>();
        playerMovement = GetComponent<RelativeMovement>(); // Get the RelativeMovement component
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RespawnPoint"))
        {
            // Update the last respawn position when contacting a respawn point
            lastRespawnPosition = other.transform.position;
            Debug.Log("Collided with respawn point. New respawn position: " + lastRespawnPosition);

            // Update flag colors
            UpdateFlagColor(other.gameObject, greenMaterial);
            if (lastRespawnFlag != null && lastRespawnFlag != other.gameObject)
            {
                UpdateFlagColor(lastRespawnFlag, redMaterial);
            }
            lastRespawnFlag = other.gameObject;
        }
        else if (other.CompareTag("DeathRespawner"))
        {
            // Trigger respawn logic
            Debug.Log("Collided with Death Respawner. Respawning at: " + lastRespawnPosition);
            StartCoroutine(HandleRespawn());
        }
    }

    private IEnumerator HandleRespawn()
    {
        // Disable the CharacterController
        if (charController != null)
        {
            charController.enabled = false;
        }

        // Wait for a short period
        yield return new WaitForSeconds(0.1f);

        // Move the player to the last respawn position
        transform.position = lastRespawnPosition;
        Debug.Log("Player position set to: " + transform.position);

        // Re-enable the CharacterController
        if (charController != null)
        {
            charController.enabled = true;
        }

        // Reset the speed boost
        if (playerMovement != null)
        {
            playerMovement.ResetSpeedBoost();
        }
    }

    private void UpdateFlagColor(GameObject respawnPoint, Material material)
    {
        // Find the flag child object and change its material
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
