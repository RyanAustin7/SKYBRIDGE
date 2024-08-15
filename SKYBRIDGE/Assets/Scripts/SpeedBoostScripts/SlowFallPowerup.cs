using UnityEngine;
using System.Collections;

public class SlowFallPowerup : MonoBehaviour
{
   [SerializeField] public float gravityReduction = 2.0f; 
    [SerializeField] public float jumpReduction = 5.0f;
    [SerializeField] public float duration = 5.0f; // Change to public
    [SerializeField] private float disableDuration = 6.0f;

    [SerializeField] private AudioSource slowFallSound;

    private MeshRenderer _meshRenderer;
    private Collider _collider;
    private Coroutine _deactivateCoroutine;
    private Coroutine _disableCoroutine;

    public bool IsActive { get; private set; }

    public void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
        IsActive = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RelativeMovement playerMovement = other.GetComponent<RelativeMovement>();
            if (playerMovement != null)
            {
                IsActive = true;
                playerMovement.StartSlowFall(gravityReduction, jumpReduction, duration);

                if (slowFallSound != null)
                {
                    slowFallSound.PlayOneShot(slowFallSound.clip);
                }

                if (_disableCoroutine != null)
                {
                    StopCoroutine(_disableCoroutine);
                }
                _disableCoroutine = StartCoroutine(DisableItemTemporarily());

                if (_deactivateCoroutine != null)
                {
                    StopCoroutine(_deactivateCoroutine);
                }
                _deactivateCoroutine = StartCoroutine(DeactivatePowerupAfterDuration(duration));
            }
        }
    }

    public IEnumerator DisableItemTemporarily()
    {
        _meshRenderer.enabled = false;
        _collider.enabled = false;

        yield return new WaitForSeconds(disableDuration);

        _meshRenderer.enabled = true;
        _collider.enabled = true;
    }

    public IEnumerator DeactivatePowerupAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        IsActive = false;
        ResetPowerup();
    }

    public void ResetPowerup()
    {
        if (_deactivateCoroutine != null)
        {
            StopCoroutine(_deactivateCoroutine);
        }
        if (_disableCoroutine != null)
        {
            StopCoroutine(_disableCoroutine);
        }
        _meshRenderer.enabled = true;
        _collider.enabled = true;
        IsActive = false;
    }
}
