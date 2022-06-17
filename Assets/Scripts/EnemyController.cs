using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private bool grounded;                                  //  Flag to check if AI is on the ground
    private float ammoVelocity;                             //  Initial velocity (m/s) of each paintball
    private float ammo;                                     //  Ammo remaining
    private float maxAmmo;                                  //  Max ammo capacity
    private float ammoDamage;                               //  Damage dealt by each paintball
    private Rigidbody rb;                                   //  Rigidbody of the player model
    private Rigidbody rifle;                                //  Rigidbody of the weapon
    [SerializeField] int jumpHeight = 5;
    [SerializeField] int moveSpeed = 8;
    [SerializeField] int mouseSensitivity = 100;
    public Rigidbody paintball;                             //  Paintball rb
    public AudioSource audioPlayer;                         //  Audio source
    public AudioClip fireSound;                             //  Sound to play when paintball is fired
    public AudioClip ping;                                  //  :)
    public float score;
    public float health;
    public float maxHealth;
    public int wins;                                       //  Rounds won

    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        grounded = false;
        score = 0f;
        ammoVelocity = 100f;
        ammo = 1f;
        maxAmmo = ammo;
        ammoDamage = 100f;
        health = 100f;
        maxHealth = health;
        wins = 0;

        // Get the rigidbody component of the object this script is attached to
        rb = GetComponent<Rigidbody>();

        // Get the rigidbody of the rifle object
        rifle = GameObject.FindGameObjectWithTag("Rifle").GetComponent<Rigidbody>();
    }

    // Enter collision event handler
    private void OnCollisionEnter(Collision collision)
    {
        // Check for collision with the ground
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
        }
        // Check for collision with paintball
        if (collision.gameObject.tag == "Paintball")
        {
            // Subtract paintball damage from health
            health -= collision.gameObject.GetComponent<PaintballMgr>().damage;

            // Award player points for scoring a hit
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().score += collision.gameObject.GetComponent<PaintballMgr>().damage/10;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Check for collision with the ground
        if (collision.gameObject.tag == "Ground" && rb.velocity.y > 0)
        {
            grounded = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

        // Only check for input if the game is not locked
        if (!GameUtils.locked())
        {

        }
    }
}
