using System.ComponentModel.Design;
using TMPro;
using UnityEngine;

public class StickmanController : MonoBehaviour
{
    public bool hasBall = false;
    public BallController ball;
    public Animator animator;
    public Transform handPoint;

    private StickmanController pendingTarget;

    public TMP_Text nameReference;
    public string fixedName;

    public int ballsReceived = 0;

    [SerializeField] private int maxPasses = 10;

    //shared flag for all stickman
    private static bool gameEnded = false;

    //reference to UI
    private UIManager uiManager;

    void Start()
    {
        nameReference.text = fixedName;
        uiManager = FindObjectOfType<UIManager>();
    }

    public void InitiatePass(StickmanController receiver)
    {
        if (gameEnded) return;
        if (!hasBall) return;

        pendingTarget = receiver;
        animator.SetTrigger("Throw"); // animation will look like throwing
        hasBall = false;

        // After short delay, release the ball
        Invoke(nameof(ReleaseBall), 0.15f);
    }

    private void ReleaseBall()
    {
        if (gameEnded) return;

        if (ball != null && pendingTarget != null)
        {
            ball.PassTo(pendingTarget);
            pendingTarget = null;
        }
    }

    public void ReceiveBall(BallController receivedBall)
    {
        if (gameEnded) return;

        ball = receivedBall;
        hasBall = true;
        ballsReceived++;
        GameRules.TotalPasses++;
        if (GameRules.PlayerRef != null && this == GameRules.PlayerRef)
        {
            GameRules.PlayerPassesReceived++;
        }
        Debug.Log($"TOTAL PASSES = {GameRules.TotalPasses} (receiver: {fixedName})");

        //Check against the maximum passes
        if (GameRules.TotalPasses >= maxPasses)
        {
            gameEnded = true;

            if(uiManager != null)
            {
                uiManager.ShowGameOver();
            }
        }

        animator.SetTrigger("Catch");
        ball.transform.SetParent(handPoint);
        ball.transform.localPosition = Vector3.zero; // snap to hand point

        if (CompareTag("Player"))
        {
            FindObjectOfType<PlayerController>().OnBallReceived();
        }

        if(uiManager != null)
        {
            uiManager.UpdateHUD(this);
        }
    }
}
