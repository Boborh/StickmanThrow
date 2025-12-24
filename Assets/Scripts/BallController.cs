using UnityEngine;

public class BallController : MonoBehaviour
{
    public float speed = 8f;
    public float rotationSpeed = 0f;

    private StickmanController target;
    private bool moving = false;

    public void PassTo(StickmanController receiver)
    {
        target = receiver;
        transform.SetParent(null);
        moving = true;

        // Give the ball a random spin each throw
        rotationSpeed = Random.Range(180f, 720f);
        rotationSpeed *= Random.value > 0.5f ? 1f : -1f;
    }

    void Update()
    {
        if (moving && target != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.transform.position,
                speed * Time.deltaTime
            );

            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, target.transform.position) < 0.1f)
            {
                moving = false;
                target.ReceiveBall(this);
            }
        }
    }
}