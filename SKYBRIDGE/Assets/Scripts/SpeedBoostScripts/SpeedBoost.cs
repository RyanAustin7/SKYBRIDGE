using UnityEngine;
using System.Collections;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField] public float boostAmount = 2.0f; // Can still be [SerializeField]
    [SerializeField] public float boostDuration = 5.0f; // Change to public
    [SerializeField] public float disableDuration = 6.0f;
    private MeshRenderer _meshRenderer;
    private Collider _collider;

    public void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
    }

    public void OnTriggerEnter(Collider other)
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

    public IEnumerator DisableItemTemporarily()
    {
        _meshRenderer.enabled = false;
        _collider.enabled = false;

        yield return new WaitForSeconds(disableDuration);

        _meshRenderer.enabled = true;
        _collider.enabled = true;
    }

    public void ResetPowerup()
    {
        // Any additional reset logic specific to the speed boost
    }
}
