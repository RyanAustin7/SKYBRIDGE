using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadSceneOnTrigger : MonoBehaviour
{
    private bool playerInside = false;
    private float timeInside = 0f;
    public float timeRequired = 2f; // Time required inside the trigger zone
    public string sceneToLoad = "GameplayScene"; // Name of the scene to load
    public TextMeshProUGUI counterText; // Reference to the TMP counter
    private bool isCounting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            timeInside = 0f; // Reset the timer
            isCounting = true;
            counterText.gameObject.SetActive(true); // Enable the TMP counter
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            timeInside = 0f; // Reset the timer
            isCounting = false;
            counterText.gameObject.SetActive(false); // Disable the TMP counter
        }
    }

    private void Update()
    {
        if (isCounting && playerInside)
        {
            timeInside += Time.deltaTime;
            float progress = Mathf.Clamp01(timeInside / timeRequired);
            int displayValue = Mathf.RoundToInt(progress * 100);
            counterText.text = displayValue.ToString();

            if (timeInside >= timeRequired)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
