using UnityEngine;
using System.Collections;
using TMPro;

public class Coin : MonoBehaviour
{
    [SerializeField] private float reappearanceTime = 20.0f;
    [SerializeField] private int timeReduction = 10;
    [SerializeField] private TextMeshProUGUI flashText; // Reference to the TMP element to flash
    [SerializeField] private float flashDuration = 1.0f; // Duration to keep the TMP element enabled

    private TimerManager timerManager;
    private MeshRenderer meshRenderer;
    private Collider coinCollider;

    private void Start()
    {
        timerManager = FindObjectOfType<TimerManager>();
        if (timerManager == null)
        {
            Debug.LogError("TimerManager not found in the scene.");
        }

        meshRenderer = GetComponent<MeshRenderer>();
        coinCollider = GetComponent<Collider>();
        if (meshRenderer == null || coinCollider == null)
        {
            Debug.LogError("Coin is missing MeshRenderer or Collider component.");
        }

        // Ensure the flashText is initially disabled
        if (flashText != null)
        {
            flashText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Flash TextMeshPro element is not assigned.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (timerManager != null)
            {
                timerManager.SubtractTime(timeReduction);
                timerManager.FlashTimerOnCoinCollection(); // Flash timer with coin color
            }

            // Flash the TMP element
            if (flashText != null)
            {
                StartCoroutine(FlashTMP());
            }

            HideCoin();
            StartCoroutine(ReappearAfterDelay());
        }
    }

    private void HideCoin()
    {
        if (meshRenderer != null) meshRenderer.enabled = false;
        if (coinCollider != null) coinCollider.enabled = false;
    }

    private void ShowCoin()
    {
        if (meshRenderer != null) meshRenderer.enabled = true;
        if (coinCollider != null) coinCollider.enabled = true;
    }

    private IEnumerator ReappearAfterDelay()
    {
        Debug.Log("Coin will reappear after " + reappearanceTime + " seconds.");
        yield return new WaitForSeconds(reappearanceTime);
        ShowCoin();
        Debug.Log("Coin reappeared.");
    }

    private IEnumerator FlashTMP()
    {
        // Enable the TMP element
        flashText.gameObject.SetActive(true);

        // Wait for the flash duration
        yield return new WaitForSeconds(flashDuration);

        // Disable the TMP element
        flashText.gameObject.SetActive(false);
    }
}
