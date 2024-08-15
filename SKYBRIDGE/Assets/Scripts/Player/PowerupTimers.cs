using UnityEngine;
using System.Collections;
using TMPro;

public class PowerupTimers : MonoBehaviour
{
    [SerializeField] private TMP_Text speedBoostCountdownText;
    [SerializeField] private TMP_Text slowFallCountdownText;

    private Coroutine speedBoostCoroutine;
    private Coroutine slowFallCoroutine;

    public void OnTriggerEnter(Collider other)
    {
        // Check for collision with SpeedBoost
        if (other.CompareTag("SpeedBoost"))
        {
            SpeedBoost speedBoost = other.GetComponent<SpeedBoost>();
            if (speedBoost != null)
            {
                StartSpeedBoostTimer(speedBoost.boostDuration);
            }
        }
        // Check for collision with SlowFall
        else if (other.CompareTag("SlowFall"))
        {
            SlowFallPowerup slowFall = other.GetComponent<SlowFallPowerup>();
            if (slowFall != null)
            {
                StartSlowFallTimer(slowFall.duration);
            }
        }
    }

    public void StartSpeedBoostTimer(float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }
        speedBoostCoroutine = StartCoroutine(UpdateCountdown(duration, speedBoostCountdownText));
    }

    public void StartSlowFallTimer(float duration)
    {
        if (slowFallCoroutine != null)
        {
            StopCoroutine(slowFallCoroutine);
        }
        slowFallCoroutine = StartCoroutine(UpdateCountdown(duration, slowFallCountdownText));
    }

    public IEnumerator UpdateCountdown(float duration, TMP_Text countdownText)
    {
        float timeRemaining = duration;
        countdownText.gameObject.SetActive(true); // Activate TMP
        while (timeRemaining > 0)
        {
            countdownText.text = Mathf.Ceil(timeRemaining).ToString(); // Update text
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        countdownText.text = "0"; // Set to zero when done
        countdownText.gameObject.SetActive(false); // Deactivate TMP
    }

    public void ResetPowerupTimer(TMP_Text countdownText)
    {
        if (countdownText != null)
        {
            countdownText.text = "0"; // Reset text
            countdownText.gameObject.SetActive(false); // Deactivate TMP
        }
    }
}
