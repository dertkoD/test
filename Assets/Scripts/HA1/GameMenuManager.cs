using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject endMenuPanel;

    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI instructionsText;
    [SerializeField] private TextMeshProUGUI victoryText; 
    
    [Header("Events")]
    [SerializeField] private GameOverActionChannelSO gameOverAction;

    [Header("Game Systems")]
    [SerializeField] private CursorAgentMovement raceManager;
    [SerializeField] private CameraRail cameraRail;
    
    [Header("Names")]
    [SerializeField] private string agent1Name = "Swat";
    [SerializeField] private string agent2Name = "Anime";

    private bool isGameStarted;
    private string sceneName;

    private void OnEnable()
    {
        if (gameOverAction)
            gameOverAction.OnEvent += OnGameOver;
    }

    private void OnDisable()
    {
        if (gameOverAction)
            gameOverAction.OnEvent -= OnGameOver;
    }

    private void Start()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        isGameStarted = false;

        startMenuPanel.SetActive(true);
        endMenuPanel.SetActive(false);
        
        victoryText.text = ""; 

        raceManager.SetGameActive(false);
        cameraRail.SetGameActive(false);

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(StartGame);
        
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(RestartGame);

        SetInstructionsText();
    }

    public void OnGameOver(int winnerAgentId)
    {
        string winner =
            winnerAgentId == 1 ? agent1Name :
            winnerAgentId == 2 ? agent2Name :
            $"Agent {winnerAgentId}";

        victoryText.text = winner + " Win!";
        
        endMenuPanel.SetActive(true);
        raceManager.SetGameActive(false);
        cameraRail.SetGameActive(false);
    }

    public void StartGame()
    {
        if (isGameStarted) return;

        isGameStarted = true;
        startMenuPanel.SetActive(false);
        victoryText.text = ""; 

        raceManager.SetGameActive(true);
        cameraRail.SetGameActive(true);
    }

    public void RestartGame()
    {
        sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    private void SetInstructionsText()
    {
        instructionsText.text =
            "CONTROLS\n\n" +
            "Agent Control:\n" +
            "• Left Mouse Button - Set destination for swat agent\n" +
            "• Right Mouse Button - Set destination for anime agent\n\n" +
            "Camera Control:\n" +
            "• W / ↑ - Move forward\n" +
            "• S / ↓ - Move backward\n\n" +
            "OBJECTIVE:\n" +
            "Guide both agents to pick up weapons\n" +
            "Let them shoot each other!\n" +
            "The agent left standing wins\n\n" +
            "Click START to begin";
    }
}