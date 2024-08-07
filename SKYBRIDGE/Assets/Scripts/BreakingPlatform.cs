using UnityEngine;
using System.Collections;

public class BreakingPlatform : MonoBehaviour
{
    private Renderer platformRenderer;
    private Collider platformCollider;


    [SerializeField] private float reactivationDelay = 1.7f; // Time before the platform reactivates

    private void Start()
    {
        platformRenderer = GetComponent<Renderer>();
        platformCollider = GetComponent<Collider>();

    }

    public IEnumerator DeactivateAndReactivateCoroutine()
    {
        platformRenderer.enabled = false;
        platformCollider.enabled = false;
        // Wait for the specified reactivation delay
        yield return new WaitForSeconds(reactivationDelay);
        ReactivatePlatform();
    }

    public void ReactivatePlatform()
    {
        // Reactivate the platform at its initial position
        platformRenderer.enabled = true;
        platformCollider.enabled = true;
    }
}
