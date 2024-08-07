using UnityEngine;
using System.Collections;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField] private float boostAmount = 2.0f; // Amount to increase the movement speed
    [SerializeField] private float boostDuration = 5.0f; // Duration of the speed boost
    [SerializeField] private float disableDuration = 6.0f; // Duration to disable the item

    private MeshRenderer _meshRenderer;
    private Collider _collider;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RelativeMovement playerMovement = other.GetComponent<RelativeMovement>();
            if (playerMovement != null)
            {
                playerMovement.StartSpeedBoost(boostAmount, boostDuration);
                StartCoroutine(DisableItemTemporarily());
            }
        }
    }

    private IEnumerator DisableItemTemporarily()
    {
        // Disable the mesh renderer and collider
        _meshRenderer.enabled = false;
        _collider.enabled = false;

        // Wait for the disable duration
        yield return new WaitForSeconds(disableDuration);

        // Re-enable the mesh renderer and collider
        _meshRenderer.enabled = true;
        _collider.enabled = true;
    }
}
