using UnityEngine;

public class ItemRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 30.0f; // Speed of rotation around the Y axis
    [SerializeField] private float floatAmplitude = 0.2f; // Amplitude of the up and down movement
    [SerializeField] private float floatSpeed = 1.0f; // Speed of the up and down movement
    [SerializeField] private float zRotationSpeed = 0.0f; // Speed of rotation around the Z axis (default 0)

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        // Rotate around the Z axis
        if (zRotationSpeed != 0.0f)
        {
            transform.Rotate(Vector3.forward, zRotationSpeed * Time.deltaTime);
        }

        // Rotate around the Y axis
        if (rotationSpeed != 0.0f)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        // Move up and down
        float newY = _startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
