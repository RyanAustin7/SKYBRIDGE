using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] Transform _target;

    [SerializeField] float _rotSpeed = 1.5f;
    [SerializeField] float _vertSpeed = 1.0f; // Speed for vertical movement

    float _rotY;
    float _rotX; // Pitch rotation for vertical movement
    Vector3 _offset;

    private void Start()
    {
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _rotY = transform.eulerAngles.y;
        _rotX = transform.eulerAngles.x;
        _offset = _target.position - transform.position;
    }

    private void LateUpdate()
    {
        // Mouse input for horizontal and vertical rotation
        _rotY += Input.GetAxis("Mouse X") * _rotSpeed;
        _rotX -= Input.GetAxis("Mouse Y") * _vertSpeed;

        // Clamp the vertical rotation to prevent flipping
        _rotX = Mathf.Clamp(_rotX, -10f, 35f);

        Quaternion rotation = Quaternion.Euler(_rotX, _rotY, 0);
        transform.position = _target.position - (rotation * _offset);

        transform.LookAt(_target);
    }
}
