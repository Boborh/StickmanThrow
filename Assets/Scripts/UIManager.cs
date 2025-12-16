using TMPro;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header ("Modals")]
    public GameObject welcomeModal;
    public GameObject informedConsent;
    public TMP_InputField nameInput;
    public StickmanController playerStickman;
    public TMP_Text warningMsg;
    public GameObject gameOverModal;
    public TMP_Text gameOverText;

    [System.Serializable]
    public class HUDInterface
    {
        public GameObject root;
        public TMP_Text nameText;
        public TMP_Text scoreText;
        public StickmanController stickman;
    }

    [Header("Panels")]
    public HUDInterface[] huds;

    [Header("Passwords")]
    public TMP_InputField passwordInput;
    const string PASSWORD_NORMAL = "alfa123";
    const string PASSWORD_DECAY = "omega321";
    
    [Header("Loading")]
    public GameObject loadingModal;
    public RectTransform loadingIcon;     // drag LoadingIcon here
    [SerializeField] private float loadingSeconds = 4f;  // 3–5
    [SerializeField] private float iconDegreesPerSecond = 180f;
    private Coroutine loadingRoutine;

    void Start()
    {
        ShowWelcomeModal();
        HideHUDs();
        SetStickmanNameLabelsVisible(false);

        if(gameOverModal != null)
        {
            gameOverModal.SetActive(false);
        }

        if (loadingModal != null)
        {
            loadingModal.SetActive(false);
        }
        
    }

    // --- HUD Control ---
    public void UpdateHUD(StickmanController stickman)
    {
        foreach (var hud in huds)
        {
            if (hud.stickman == stickman)
            {
                hud.nameText.text = stickman.fixedName;
                hud.scoreText.text = $"Skóre: {stickman.ballsReceived}";
                break;
            }
        }
    }

    private void InitializeHUDs()
    {
        foreach (var hud in huds)
        {
            if (hud.stickman != null)
            {
                //Enable all panels
                hud.root.SetActive(true);

                //Fill in text
                hud.nameText.text = hud.stickman.fixedName;
                hud.scoreText.text = $"Skóre: {hud.stickman.ballsReceived}";
            }
        }
    }

    private void HideHUDs()
    {
        foreach (var hud in huds)
        {
            if (hud.nameText != null)
                //Hide all panels
                hud.root.SetActive(false);
        }
    }

    // --- Modal logic ---
    public void ShowWelcomeModal() => welcomeModal.SetActive(true);

    public void HideWelcomeModal()
    {
        welcomeModal.SetActive(false);
        ShowInfromedConsentModal();
    }

    public void ShowInfromedConsentModal()
    {
        informedConsent.SetActive(true);

        //hide warning
        warningMsg.gameObject.SetActive(false);

        //add listener: every time player types, call OnNameChanged
        nameInput.onValueChanged.AddListener(OnNameChanged);
        passwordInput.onValueChanged.AddListener(OnNameChanged);
    }

    public void HideInformedConsentModal()
    {
        informedConsent.SetActive(false);

        //remove listener
        nameInput.onValueChanged.RemoveListener(OnNameChanged);
        passwordInput.onValueChanged.RemoveListener(OnNameChanged);

        InitializeHUDs();

        var playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.StartGame();

            if (playerStickman.hasBall)
            {
                playerController.OnBallReceived();
            }
        }
    }

    public void ConfirmName()
    {
        string playerName = nameInput.text.Trim();
        string password = passwordInput.text.Trim().ToLowerInvariant();

        // Check for empty fields
        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(password))
        {
            warningMsg.text = "Pole nesmie byť prázdne!";
            warningMsg.gameObject.SetActive(true);
            return;
        }

        // Assign player name
        playerStickman.nameReference.text = playerName;
        playerStickman.fixedName = playerName;

        // Handle game mode selection based on password
        if (password == PASSWORD_NORMAL)
        {
            GameRules.CurrentMode = GameRules.GameMode.Normal;
        }
        else if (password == PASSWORD_DECAY)
        {
            GameRules.CurrentMode = GameRules.GameMode.PlayerChanceDecreases;
        }
        else
        {
            // Invalid password
            warningMsg.text = "Nesprávne heslo!";
            warningMsg.gameObject.SetActive(true);
            return;
        }

        // Reset global counter and references
        GameRules.Reset();
        GameRules.PlayerRef = playerStickman;

        // Close modal & start game
        StartLoadingAndStartGame();
    }

    private void OnNameChanged(string value)
    {
        //If the player typed something, hide warning immediately
        if (warningMsg.gameObject.activeSelf && !string.IsNullOrEmpty(value.Trim()))
        {
            warningMsg.gameObject.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        // Hide HUD panels
        HideHUDs();

        // Hide in-world stickman name labels
        SetStickmanNameLabelsVisible(false);

        // Show game over modal
        if (gameOverModal != null)
            gameOverModal.SetActive(true);

        // Freeze game
        Time.timeScale = 0f;
    }

    public void StartLoadingAndStartGame()
    {
        // prevent double-clicks / multiple coroutines
        if (loadingRoutine != null)
            StopCoroutine(loadingRoutine);

        loadingRoutine = StartCoroutine(LoadingThenStartGame());
    }

    private IEnumerator LoadingThenStartGame()
    {
        // Hide informed consent modal
        informedConsent.SetActive(false);

        // Remove listeners (same as your HideInformedConsentModal)
        nameInput.onValueChanged.RemoveListener(OnNameChanged);
        passwordInput.onValueChanged.RemoveListener(OnNameChanged);

        //keep HUD hidden during loading
        HideHUDs();
        SetStickmanNameLabelsVisible(false);

        // Show loading modal
        if (loadingModal != null)
            loadingModal.SetActive(true);

        float t = 0f;

        // rotate icon for loadingSeconds using real time (not affected by timeScale)
        while (t < loadingSeconds)
        {
            t += Time.unscaledDeltaTime;

            if (loadingIcon != null)
                loadingIcon.Rotate(0f, 0f, -iconDegreesPerSecond * Time.unscaledDeltaTime);

            yield return null;
        }

        // Hide loading modal
        if (loadingModal != null)
            loadingModal.SetActive(false);

        loadingRoutine = null;

        // Show HUD and continue the game
        SetStickmanNameLabelsVisible(true);
        InitializeHUDs();

        //gameStarted gate
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.StartGame();

            if (playerStickman.hasBall)
                playerController.OnBallReceived();
        }
    }

    private void SetStickmanNameLabelsVisible(bool visible)
    {
        var stickmen = FindObjectsOfType<StickmanController>(true); // include inactive
        foreach (var s in stickmen)
        {
            if (s != null && s.nameReference != null)
                s.nameReference.gameObject.SetActive(visible);
        }
    }
}