using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel; // Reference to the pause menu panel
    private bool isPaused = false;

    void Start()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        // Check if the player presses the P or Escape key
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        isPaused = !isPaused;

        // Enable or disable the pause menu panel based on the isPaused variable
        pauseMenuPanel.SetActive(isPaused);

        // Optionally, you can add any other logic here that you want to happen when pausing/unpausing
        // For example, showing the mouse cursor:
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    // Method to resume the game
    public void ResumeGame()
    {
        TogglePauseMenu();
    }

    // Method to reset the run (reload the current scene)
    public void ResetRun()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Method to quit to menu (load the Menu scene)
    public void QuitToMenu()
    {
        SceneManager.LoadScene("Menu");       // Replace "Menu" with the actual name of your menu scene
        Debug.Log("Clicked Quit to Menu Button");
    }
}
