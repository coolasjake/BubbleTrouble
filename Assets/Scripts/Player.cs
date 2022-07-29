using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player player1;
    public static Player player2;

    public bool isPlayer1 = true;

    public Rigidbody2D RB;
    public SpriteRenderer SR;
    public Sprite leftRight;
    public Sprite forwards;
    public GameObject grapplePre;
    public float speed = 1f;
    public float grappleSpeed = 2f;
    public float playerHeight = 1;
    public float shootDelay = 0.2f;

    private Grapple grapple;
    private bool grappleOut = false;
    private float shootTime = 0;

    void Awake()
    {
        if (isPlayer1)
            Player.player1 = this;
        else
            Player.player2 = this;

        if (RB == null)
            RB = GetComponent<Rigidbody2D>();

        if (SR == null)
            SR = GetComponent<SpriteRenderer>();

        if (grapple == null)
        {
            grapple = Instantiate(grapplePre, transform.parent).GetComponent<Grapple>();
            grapple.ownedByP1 = isPlayer1;
        }

        RetractGrapple();

        if (!Controller.twoPlayers && !isPlayer1)
            gameObject.SetActive(false);
        else if (Controller.twoPlayers && ((isPlayer1 && Controller.p1Lives == 0) || (!isPlayer1 && Controller.p2Lives == 0)))
            gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void ShootGrapple()
    {
        grappleOut = true;
        grapple.used = false;
        grapple.transform.position = transform.position + new Vector3(0, playerHeight, 0);
        grapple.gameObject.SetActive(true);
        grapple.RB.velocity = Vector2.up * grappleSpeed;
    }

    public void RetractGrapple()
    {
        grappleOut = false;
        grapple.gameObject.SetActive(false);
        grapple.RB.velocity = Vector2.zero;
    }

    public void Freeze()
    {
        RB.simulated = false;
        grapple.RB.simulated = false;
    }

    public void UnFreeze()
    {
        RB.simulated = true;
        grapple.RB.simulated = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Controller.Frozen || Controller.stopTimer)
            return;

        float motion = Input.GetAxis("p1Horizontal");
        if (!Controller.twoPlayers)
            motion = Input.GetAxis("p1Horizontal") + Input.GetAxis("p2Horizontal");
        else if (!isPlayer1)
            motion = Input.GetAxis("p2Horizontal");

        if (motion > 0 && Time.time > shootTime + shootDelay)
        {
            SR.sprite = leftRight;
            transform.localScale = new Vector3(1, 1, 1);
            RB.velocity = new Vector2(speed, 0);
        }
        else if (motion < 0 && Time.time > shootTime + shootDelay)
        {
            SR.sprite = leftRight;
            transform.localScale = new Vector3(-1, 1, 1);
            RB.velocity = new Vector2(-speed, 0);
        }
        else
        {
            SR.sprite = forwards;
            RB.velocity = new Vector2(0, 0);
        }

        if (!grappleOut)
        {
            bool fire = false;
            if (Controller.twoPlayers && isPlayer1 && Input.GetButtonDown("p1Fire"))
                fire = true;
            else if (Controller.twoPlayers && !isPlayer1 && Input.GetButtonDown("p2Fire"))
                fire = true;
            else if (!Controller.twoPlayers && (Input.GetButtonDown("p2Fire") || Input.GetButtonDown("p1Fire")))
                fire = true;
            if (fire)
            {
                ShootGrapple();
                RB.velocity = new Vector2(0, 0);
                shootTime = Time.time;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {

        }
        else if (collision.gameObject.CompareTag("Ball"))
        {
            Controller.singleton.PlayerHit(isPlayer1);
        }
    }
}
