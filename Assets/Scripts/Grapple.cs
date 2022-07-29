using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    public bool ownedByP1 = true;
    public Rigidbody2D RB;
    [HideInInspector()]
    public bool used = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ceiling"))
        {
            RB.velocity = Vector2.zero;
            if (ownedByP1)
                Player.player1.RetractGrapple();
            else
                Player.player2.RetractGrapple();
        }
        else if (collision.gameObject.CompareTag("Ball"))
        {
            if (!used)
            {
                used = true;
                if (ownedByP1)
                    Player.player1.RetractGrapple();
                else
                    Player.player2.RetractGrapple();
                collision.gameObject.GetComponent<Ball>().Pop(ownedByP1);
            }
        }
    }
}
