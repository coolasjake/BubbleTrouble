using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public LevelController LC;
    public Rigidbody2D RB;
    public SpriteRenderer SR;
    public bool startMovingRight = true;
    public int size = 1;
    private static float baseBounceVelocity = 5f;
    private static float sizeVelMultiplier = 0.7f;
    private static float baseHorizontalVelocity = 1.8f;

    
    public bool deleteConfirmation = false;

    // Start is called before the first frame update
    void Start()
    {
        if (RB == null)
            RB = GetComponent<Rigidbody2D>();

        float sign = (startMovingRight) ? 1 : -1;
        RB.velocity += new Vector2(baseHorizontalVelocity * sign, 0);
        RB.angularVelocity = 1;

        UpdateSize();
    }

    public void Initialize(int oldSize, bool right)
    {
        size = oldSize - 1;
        transform.localScale = new Vector3(size, size, size);
        RB.velocity = new Vector2(0, baseBounceVelocity);
        startMovingRight = right;
    }

    public void UpdateSize()
    {
        transform.localScale = new Vector3(size, size, size);
    }

    private float CalculateHeight()
    {
        float V = baseBounceVelocity + (size - 1) * sizeVelMultiplier;
        return (V * V) / (2 * Physics.gravity.magnitude);
    }

    public void InspectorUpdateSize()
    {
        transform.localScale = new Vector3(size, size, size);

        transform.localPosition = new Vector3(transform.localPosition.x, CalculateHeight(), 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Freeze()
    {
        RB.simulated = false;
    }

    public void UnFreeze()
    {
        RB.simulated = true;
    }

    public void Pop(bool p1)
    {
        if (size == 1)
            Instantiate(Controller.statPopPre, transform.position, transform.rotation);
        else
        {
            //Create 2 smaller balls
            Ball b1 = Instantiate(gameObject, transform.position, transform.rotation, LC.transform).GetComponent<Ball>();
            Ball b2 = Instantiate(gameObject, transform.position, transform.rotation, LC.transform).GetComponent<Ball>();
            //Give them base velocity up and horizontal velocity in opposite directions
            b1.Initialize(size, true);
            b2.Initialize(size, false);

            Controller.singleton.balls.Add(b1);
            Controller.singleton.balls.Add(b2);
        }
        //Add score
        if (p1)
            Controller.singleton.P1AddScore(size * 10);
        else
            Controller.singleton.P2AddScore(size * 10);

        //Destroy ball
        Controller.singleton.balls.Remove(this);
        Controller.singleton.BallDestroyed();
        Destroy(gameObject);
    }

    private void CeilingPop()
    {
        //Add bonus score?
        Controller.singleton.P1AddScore(size * 30);
        Controller.singleton.P2AddScore(size * 30);
        Instantiate(Controller.statComboPre, transform.position, transform.rotation);
        Controller.singleton.balls.Remove(this);
        Controller.singleton.BallDestroyed();
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Lose Life
        }
        else if (collision.gameObject.CompareTag("Floor"))
        {
            float x = RB.velocity.x;
            RB.velocity = new Vector2(x, baseBounceVelocity + (size - 1) * sizeVelMultiplier);
        }
        else if (collision.gameObject.CompareTag("Ceiling"))
        {
            CeilingPop();
        }

        //Ensure horizontal velocity is always consistent
        float y = RB.velocity.y;
        float sign = (RB.velocity.x > 0) ? 1 : -1;
        RB.velocity = new Vector2(baseHorizontalVelocity * sign, y);
    }
}
