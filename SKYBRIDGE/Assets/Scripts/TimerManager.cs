using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI finalTimesText; // New TMP for final times display
    [SerializeField] private Color flashColor = Color.green;
    [SerializeField] private float flashDuration = 1.0f;
    [SerializeField] private List<Transform> respawnFlags;
    [SerializeField] private AudioSource finalFlagSound; // Assign this in the inspector


    private float timeElapsed;
    private bool timerRunning = false; // Timer is not running at start
    private Color originalColor;
    private Dictionary<Transform, float> flagTimes;
    private bool[] flagsReached;

    private void Start()
    {
        originalColor = timerText.color;
        flagTimes = new Dictionary<Transform, float>();
        flagsReached = new bool[respawnFlags.Count];

        for (int i = 0; i < respawnFlags.Count; i++)
        {
            flagTimes.Add(respawnFlags[i], -1f); // Initialize with -1 to indicate not reached
        }

        finalTimesText.gameObject.SetActive(false); // Ensure the final times TMP is disabled initially
    }

    private void Update()
    {
        if (timerRunning)
        {
            timeElapsed += Time.deltaTime;
            timerText.text = FormatTime(timeElapsed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TimerStart"))
        {
            StartTimer();
        }
        else if (other.CompareTag("RespawnPoint"))
        {
            int flagIndex = respawnFlags.IndexOf(other.transform);

            if (flagIndex != -1 && !flagsReached[flagIndex])
            {
                flagsReached[flagIndex] = true;
                flagTimes[other.transform] = timeElapsed;
                StartCoroutine(FlashTimerColor());

                if (flagIndex == respawnFlags.Count - 1)
                {
                    StopTimer();
                }
            }
        }
    }

    private void StartTimer()
    {
        if (!timerRunning)
        {
            timerRunning = true;
            timeElapsed = 0f;
            timerText.text = FormatTime(timeElapsed);
        }
    }

    private IEnumerator FlashTimerColor()
    {
        timerText.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        timerText.color = originalColor;
    }

    private void StopTimer()
    {
        timerRunning = false;
        DisplayFinalTimes();

        // Unlock and show the cursor when the timer stops
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void DisplayFinalTimes()
    {
        string finalTimes = "Final Splits:\n";
        float totalTime = 0f;

        for (int i = 0; i < respawnFlags.Count; i++)
        {
            if (flagTimes[respawnFlags[i]] != -1f)
            {
                float splitTime;
                if (i == 0)
                {
                    // For the first flag, the split time is the same as the time at the first flag
                    splitTime = flagTimes[respawnFlags[i]];
                }
                else
                {
                    // For other flags, the split time is the difference between the current and previous flag times
                    splitTime = flagTimes[respawnFlags[i]] - flagTimes[respawnFlags[i - 1]];
                }

                finalTimes += $"Flag {i + 1} Split: {FormatTime(splitTime)}\n";
                totalTime = flagTimes[respawnFlags[i]]; // Update total time to the last flag reached
            }
            else
            {
                finalTimes += $"Flag {i + 1}: SKIPPED\n";
            }
        }

        finalTimes += $"\nTotal Time: {FormatTime(totalTime)}";

        finalTimesText.text = finalTimes;
        finalTimesText.gameObject.SetActive(true); // Enable the final times TMP object

        finalFlagSound.PlayOneShot(finalFlagSound.clip);
    }

    private string FormatTime(float time)
    {
        int hours = Mathf.FloorToInt(time / 3600F);
        int minutes = Mathf.FloorToInt((time % 3600F) / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int centiseconds = Mathf.FloorToInt((time * 100F) % 100F);

        string formattedTime = "";

        if (hours > 0)
        {
            formattedTime += $"{hours}:{minutes:00}:{seconds:00}.{centiseconds:00}";
        }
        else if (minutes > 0)
        {
            formattedTime += $"{minutes}:{seconds:00}.{centiseconds:00}";
        }
        else
        {
            formattedTime += $"{seconds}.{centiseconds:00}";
        }

        return formattedTime;
    }
}
