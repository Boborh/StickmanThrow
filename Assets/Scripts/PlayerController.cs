using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public StickmanController playerStickman;
    public LayerMask stickmanMask;
    public GameObject hintPanel;

    [SerializeField] private float inactivityDelay = 3f;

    private float inactivityTimer = 0f;
    private bool hintShown = false;

    private bool gameStarted = false;

    void Start()
    {
        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }
    }

    void Update()
    {
        if(!gameStarted)
            return;

        // If player doesn't have the ball, no hint logic
        if (!playerStickman.hasBall)
        {
            inactivityTimer = 0f;
            hintShown = false;

            if (hintPanel != null && hintPanel.activeSelf)
                hintPanel.SetActive(false);

            return;
        }

        // --- INPUT CHECK ---
        bool inputPressed = false;

        if (Mouse.current != null)
        {
            inputPressed |= Mouse.current.leftButton.wasPressedThisFrame;
        }

        if (Touchscreen.current != null)
        {
            inputPressed |= Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        }

        if (inputPressed)
        {
            // Prevent clicking through UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // Read input position
            Vector2 inputPosition = Vector2.zero;

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                inputPosition = Mouse.current.position.ReadValue();
            }
            else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                inputPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            }

            // Convert to world space
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(inputPosition);

            // Cast a ray to see if we clicked a stickman
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, stickmanMask);
            if (hit.collider != null)
            {
                StickmanController target = hit.collider.GetComponent<StickmanController>();
                if (target != null && target != playerStickman)
                {
                    // Player made a pass -> reset inactivity and hide hint
                    playerStickman.InitiatePass(target);
                    OnBallLost();
                    return;
                }
            }

            // Even if they clicked "nothing", treat it as activity -> reset timer & hide hint
            inactivityTimer = 0f;
            hintShown = false;
            if (hintPanel != null && hintPanel.activeSelf)
                hintPanel.SetActive(false);
        }

        // --- INACTIVITY TIMER ---

        inactivityTimer += Time.deltaTime;

        if (!hintShown && inactivityTimer >= inactivityDelay)
        {
            if (hintPanel != null)
                hintPanel.SetActive(true);

            hintShown = true;
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        inactivityTimer = 0f;
        hintShown = false;

        if (hintPanel != null && hintPanel.activeSelf)
            hintPanel.SetActive(false);
    }

    public void OnBallReceived()
    {
        // Player gets the ball -> reset inactivity and hide hint, timer starts again
        inactivityTimer = 0f;
        hintShown = false;

        if (hintPanel != null && hintPanel.activeSelf)
            hintPanel.SetActive(false);
    }

    public void OnBallLost()
    {
        // Player loses the ball -> no more hint, reset everything
        gameStarted = true;
        inactivityTimer = 0f;
        hintShown = false;

        if (hintPanel != null && hintPanel.activeSelf)
            hintPanel.SetActive(false);
    }
}