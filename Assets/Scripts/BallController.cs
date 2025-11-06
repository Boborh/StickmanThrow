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

        //Give the ball a random spin each throw
        rotationSpeed = Random.Range(180f, 720f); //degrees per second
        rotationSpeed *= Random.value > 0.5 ? 1 : -1; //random direction
    }

    void Update()
    {
        if (moving && target != null)
        {
            //Move ball toward target
            transform.position = Vector2.MoveTowards(transform.position,
                                                     target.transform.position,
                                                     speed * Time.deltaTime);

            //Apply spin
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            //Check if target reached
            if (Vector2.Distance(transform.position, target.transform.position) < 0.1f)
            {
                moving = false;
                target.ReceiveBall(this);
            }
        }
    }
}
