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

    private Coroutine hintRoutine;

    void Update()
    {
        if (!playerStickman.hasBall)
            return;

        // Check for mouse or touch input
        bool inputPressed = Mouse.current.leftButton.wasPressedThisFrame;
        if (Touchscreen.current != null)
        {
            inputPressed |= Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        }

        if (!inputPressed)
            return;

        // Prevent clicking through UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Read input position
        Vector2 inputPosition = Vector2.zero;

        if (Mouse.current.leftButton.wasPressedThisFrame)
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
                playerStickman.InitiatePass(target);
                OnBallLost();
            }
        }
    }

    public void OnBallReceived()
    {
        //Stop any existing hint timer
        if (hintRoutine != null)
        {
            StopCoroutine(hintRoutine);
        }

        hintRoutine = StartCoroutine(HintAfterDelay());
    }

    private IEnumerator HintAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        if (playerStickman.hasBall)
        {
            hintPanel.SetActive(true);
        }
    }
    
    public void OnBallLost()
    {
        if (hintRoutine != null)
        {
            StopCoroutine(hintRoutine);
        }

        hintPanel.SetActive(false);
    }
}