using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] float _speed = 30.0f;
    [SerializeField] float _knockbackForce = 100.0f;

    private Vector3 _direction;

    private void Start()
    {
        // Initialize direction based on passed value
        if (_direction == Vector3.zero)
        {
            _direction = transform.forward; // Default direction
        }

        // Start coroutine to destroy fireball after 7 seconds
        StartCoroutine(DestroyAfterTime(7.0f));
    }

    private void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CharacterController characterController = collision.gameObject.GetComponent<CharacterController>();

            if (characterController != null)
            {
                Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
                knockbackDirection.y = 0; // Zero out the Y component for horizontal knockback only
                if (characterController.TryGetComponent(out RelativeMovement relativeMovement))
                {
                    relativeMovement.ApplyKnockback(knockbackDirection * _knockbackForce);
                }
            }

            Destroy(gameObject); // Destroy the fireball after collision with the player
        }
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
