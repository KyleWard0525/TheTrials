using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    private bool grounded;                                  //  Flag to check if player is on the ground
    private float mouseX;                                   //  Mouse X value
    private float mouseY;                                   //  Mouse Y value
    private float ammoVelocity;                             //  Initial velocity (m/s) of each paintball
    private float ammoDamage;                               //  Damage dealt by each paintball
    private Rigidbody rb;                                   //  Rigidbody of the player model
    private Rigidbody rifle;                                //  Rigidbody of the weapon
    private Camera firstPersonCam;                          //  First person camera object
    private Camera thirdPersonCam;                          //  Third person camera object
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
    public float ammo;                                     //  Ammo remaining
    public float maxAmmo;                                  //  Max ammo capacity

    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        grounded = false;
        mouseX = 0f;
        mouseY = 0f;
        score = 0f;
        ammoVelocity = 100f;
        ammo = 1f;
        maxAmmo = ammo;
        ammoDamage = 100f;
        health = 100f;
        maxHealth = health;
        wins = 0;

        // Get camera objects
        firstPersonCam = GameObject.FindGameObjectWithTag("FirstPersonCamera").GetComponent<Camera>();
        thirdPersonCam = GameObject.FindGameObjectWithTag("ThirdPersonCamera").GetComponent<Camera>();

        // Get the rigidbody component of the object this script is attached to
        rb = GetComponent<Rigidbody>();

        // Get the rigidbody of the rifle object
        rifle = GameObject.FindGameObjectWithTag("Rifle").GetComponent<Rigidbody>();

        // Confine cursor to within game window
        Cursor.lockState = CursorLockMode.Locked;
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
        if(collision.gameObject.tag == "Paintball")
        {
            // Subtract paintball damage from health
            health -= collision.gameObject.GetComponent<PaintballMgr>().damage;
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

    // Check for cheat code inputs
    void checkForCheatCodes()
    {
        // Add ammo (CTRL + T)
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
        {
            ammo += 10;

            Debug.Log("Player ammo count increased to " + ammo + "!");
        }
    }

    // Handle player input
    void checkForInput()
    {
        // Get horizontal and vertical inputs (-1 to 1)
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        // Move player based on axial inputs
        if(grounded)
        {
            rb.velocity = new Vector3(hInput * moveSpeed, rb.velocity.y, vInput * moveSpeed);
        }
        else
        {
            rb.velocity = new Vector3(hInput * moveSpeed, rb.velocity.y-9.81f/60.0f, vInput * moveSpeed);
        }


        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.y);
        }
        // Sprint
        if (Input.GetKey(KeyCode.LeftShift) && grounded)
        {
            rb.velocity = new Vector3(hInput * moveSpeed * 2, rb.velocity.y, vInput * moveSpeed * 2);
        }

        // Check for fire input (LMB) from player
        if (Input.GetKeyDown(KeyCode.Mouse0) && ammo > 0)
        {
            // Play shoot sound effect
            audioPlayer.PlayOneShot(fireSound);

            // Decrement ammo count 
            ammo--;

            // I had to do it 
            if (ammo == 0)
            {
                audioPlayer.PlayOneShot(ping, 0.65f);
            }

            // Create a clone of the paintball object at the spawner position
            Rigidbody projectile = Instantiate(paintball, rifle.transform.position, firstPersonCam.transform.rotation);

            // Set the paintball damage amount
            projectile.GetComponent<PaintballMgr>().damage = ammoDamage;

            // Move paintball object into rifle barrel
            projectile.transform.position += rifle.transform.up * 0.25f;

            // Apply forward velocity relative to the direction the weapon is facing and the rotation of the camera
            projectile.velocity = (rifle.transform.forward + firstPersonCam.transform.forward) * ammoVelocity;

            // Wait for paintball to leave the barrel before applying gravity
            GameUtils.wait(0.5f);

            // Apply gravity to projectile
            projectile.useGravity = true;
        }

        // Check for player rotation with mouse movement
        mouseLook();

        // Rotate player direction relative to transform rotation
        rb.velocity = transform.rotation * rb.velocity;

        checkForCheatCodes();
    }

    // Rotate player model relative to where the mouse is pointing
    void mouseLook()
    {
        // Update mouseX input value and horizontal rotation
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Clamp mouseY
        mouseY = Mathf.Clamp(mouseY, -25f, 45f);
        
        // Rotate camera within limits
        firstPersonCam.transform.localRotation = Quaternion.Euler(-mouseY, 0f, 0f);  
        transform.rotation = Quaternion.Euler(0f, mouseX, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Only check for input if the game is not locked
        if(!GameUtils.locked())
        {
            checkForInput();
        }
        
    }

}
