using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject gameHUD;

    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI instructionsText;
    
    [SerializeField] private TextMeshProUGUI victoryText; 

    [Header("Game Systems")]
    [SerializeField] private CursorAgentMovement raceManager;
    [SerializeField] private CameraRail cameraRail;

    private bool isGameStarted;

    private void Awake()
    {
        ValidateReferences();
    }

    private void Start()
    {
        InitializeMenu();
    }

    private void ValidateReferences()
    {
        Debug.Assert(startMenuPanel != null, "StartMenuPanel not assigned");
        Debug.Assert(gameHUD != null, "GameHUD not assigned");
        Debug.Assert(startButton != null, "StartButton not assigned");
        Debug.Assert(instructionsText != null, "InstructionsText not assigned");
        Debug.Assert(raceManager != null, "RaceManager not assigned");
        Debug.Assert(cameraRail != null, "CameraRail not assigned");
        Debug.Assert(victoryText != null, "VictoryText not assigned");
    }

    private void InitializeMenu()
    {
        isGameStarted = false;

        startMenuPanel.SetActive(true);
        gameHUD.SetActive(false);
        
       
        victoryText.text = ""; 

        raceManager.SetGameActive(false);
        cameraRail.SetGameActive(false);

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(StartGame);

        SetInstructionsText();
    }

  
    public void OnAgentReachedGoal(string agentName)
    {
        if (victoryText != null)
        {
            victoryText.text = agentName + " Win!";
          
            raceManager.SetGameActive(false);
            Debug.Log("Winner: " + agentName);
        }
    }

    public void StartGame()
    {
        if (isGameStarted) return;

        isGameStarted = true;
        startMenuPanel.SetActive(false);
        gameHUD.SetActive(true);
        victoryText.text = ""; 

        raceManager.SetGameActive(true);
        cameraRail.SetGameActive(true);
    }

    private void SetInstructionsText()
    {
        instructionsText.text =
            "CONTROLS\n\n" +
            "Agent Control:\n" +
            "• Left Mouse Button - Set destination\n\n" +
            "Camera Control:\n" +
            "• W / ↑ - Move forward\n" +
            "• S / ↓ - Move backward\n\n" +
            "OBJECTIVE:\n" +
            "Guide both agents to the goal\n" +
            "Avoid moving obstacles\n\n" +
            "Click START to begin";
    }
}