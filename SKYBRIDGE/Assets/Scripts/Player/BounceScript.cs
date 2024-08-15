using UnityEngine;

public class BounceScript : MonoBehaviour
{
    public float bounceForce = 20.0f; // Public variable for bounce force

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            RelativeMovement playerMovement = collision.gameObject.GetComponent<RelativeMovement>();
            if (playerMovement != null)
            {
                float actualBounceForce = bounceForce;
                if (playerMovement.IsSlowFallActive())
                {
                    actualBounceForce *= 0.1f; // Reduce bounce force if slow fall is active
                    Debug.Log("Slow fall active: Bounce force reduced to " + actualBounceForce);
                }
                else
                {
                    Debug.Log("Slow fall not active: Bounce force is " + actualBounceForce);
                }

                playerMovement.ApplyBounce(actualBounceForce);
            }
        }
    }
}
