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

    void Start()
    {
        nameReference.text = fixedName;
    }

    public void InitiatePass(StickmanController receiver)
    {
        if (!hasBall) return;
        pendingTarget = receiver;
        animator.SetTrigger("Throw"); // animation will look like throwing
        hasBall = false;

        // After short delay, release the ball
        Invoke(nameof(ReleaseBall), 0.15f);
    }

    private void ReleaseBall()
    {
        if (ball != null && pendingTarget != null)
        {
            ball.PassTo(pendingTarget);
            pendingTarget = null;
        }
    }

    public void ReceiveBall(BallController receivedBall)
    {
        ball = receivedBall;
        hasBall = true;
        ballsReceived++;
        GameRules.TotalPasses++;
        animator.SetTrigger("Catch");
        ball.transform.SetParent(handPoint);
        ball.transform.localPosition = Vector3.zero; // snap to hand point

        if (CompareTag("Player"))
        {
            FindObjectOfType<PlayerController>().OnBallReceived();
        }

        FindObjectOfType<UIManager>().UpdateHUD(this);
    }
}
