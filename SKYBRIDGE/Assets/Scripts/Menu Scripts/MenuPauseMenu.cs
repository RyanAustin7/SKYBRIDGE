using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuPauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject controlsPanel;
    public GameObject areYouSurePanel;

    public Button closeButton;
    public Button controlsButton;
    public Button quitGameButton;

    public Button controlsBackButton;
    public Button areYouSureYesButton;
    public Button areYouSureNoButton;

    private bool isPaused = true;

    public void Start()
    {
        pauseMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);
        areYouSurePanel.SetActive(false);

        closeButton.onClick.AddListener(CloseMenu);
        controlsButton.onClick.AddListener(OpenControlsPanel);
        quitGameButton.onClick.AddListener(OpenAreYouSurePanel);

        controlsBackButton.onClick.AddListener(CloseControlsPanel);
        areYouSureYesButton.onClick.AddListener(QuitGame);
        areYouSureNoButton.onClick.AddListener(CloseAreYouSurePanel);

        UpdateCursorState();

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuPanel.activeSelf)
            {
                CloseMenu();
            }
            else
            {
                OpenPauseMenu();
            }
        }
        
        UpdateCursorState();
 
    }

    public void OpenPauseMenu()
    {
        pauseMenuPanel.SetActive(true);
        controlsPanel.SetActive(false); // Ensure other panels are closed
        areYouSurePanel.SetActive(false); // Ensure other panels are closed
        isPaused = true;
    }

    public void CloseMenu()
    {
        pauseMenuPanel.SetActive(false);
        controlsPanel.SetActive(false); // Ensure other panels are closed
        areYouSurePanel.SetActive(false); // Ensure other panels are closed
        isPaused = false;
    }

    public void OpenControlsPanel()
    {
        pauseMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
        areYouSurePanel.SetActive(false); // Ensure other panels are closed
    }

    public void CloseControlsPanel()
    {
        controlsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void OpenAreYouSurePanel()
    {
        pauseMenuPanel.SetActive(false);
        areYouSurePanel.SetActive(true);
        controlsPanel.SetActive(false); // Ensure other panels are closed
    }

    public void CloseAreYouSurePanel()
    {
        areYouSurePanel.SetActive(false);
        pauseMenuPanel.SetActive(true);  
    }

    public void QuitGame()
    {
        // If running in the Unity editor, stop play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running in a build, quit the application
        Application.Quit();
#endif
    }

   public void UpdateCursorState()
    {
        if (pauseMenuPanel.activeSelf || controlsPanel.activeSelf || areYouSurePanel.activeSelf)
        {
            Debug.Log("Cursor should be unlocked and visible");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.Log("Cursor should be locked and invisible");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
