using UnityEngine;

public class RotatingStage : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30.0f; // Speed of rotation

    private void Update()
    {
        // Rotate around the X axis
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }
}
