using UnityEngine;

public class ShadowProjector : MonoBehaviour
{
    [SerializeField] private GameObject shadowPrefab; // Assign your shadow or object prefab in the inspector
    [SerializeField] private float maxRaycastDistance = 10.0f; // Max distance to cast the ray
    [SerializeField] private float shadowOffset = 0.01f; // Offset to prevent z-fighting
    [SerializeField] private float minDistanceFromGround = 0.1f; // Minimum distance to show the shadow
    [SerializeField] private Color debugRayColor = Color.red; // Color for debugging raycasts
    [SerializeField] private LayerMask ignoreLayers; // Layer mask to specify layers where shadows should be ignored
    [SerializeField] private LayerMask additionalIgnoreLayer; // Additional layer to ignore

    private GameObject shadowInstance; // Instance of the shadow or object

    private void Start()
    {
        // Instantiate the shadow object at the start, but keep it inactive
        shadowInstance = Instantiate(shadowPrefab);
        shadowInstance.SetActive(false);

        ignoreLayers |= additionalIgnoreLayer;
    }

    private void FixedUpdate()
    {
        // Raycast downwards from the player
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxRaycastDistance))
        {
            // Check if the hit object is on a layer that should be ignored
            if ((ignoreLayers & (1 << hit.collider.gameObject.layer)) != 0)
            {
                shadowInstance.SetActive(false);
                return;
            }

            // Calculate the distance from the player to the ground
            float distanceToGround = hit.distance;

            // Activate and position the shadow object if the distance is greater than the minimum threshold
            if (distanceToGround >= minDistanceFromGround)
            {
                shadowInstance.SetActive(true);
                shadowInstance.transform.position = hit.point + Vector3.up * shadowOffset; // Slight offset to prevent z-fighting
                shadowInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal); // Align with ground slope

                // Debugging lines
                Debug.DrawRay(transform.position, Vector3.down * maxRaycastDistance, debugRayColor);
            }
            else
            {
                // Deactivate the shadow object if the distance is less than the minimum threshold
                shadowInstance.SetActive(false);
            }
        }
        else
        {
            // Deactivate the shadow object if no ground is detected
            shadowInstance.SetActive(false);
        }
    }
}
