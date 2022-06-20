using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUtils : MonoBehaviour
{
    private static int maxRounds;                           //  Maximum number of rounds
    private static float buyTime;                           //  Time in seconds of buy period
    private static bool Lock;                               //  Lock control of game objects such as player and enemy
    private static bool mouseLock;                          //  Lock control of the mouse looking
    private static bool buyEnabled;                         //  Flag for controlling the buy period at round start
    private static GameObject playerSpawn;                  //  Player spawner
    private static GameObject enemySpawn;                   //  Enemy spawner
    private static GameObject player;                       //  Player object
    private static GameObject enemy;                        //  Enemy object
    private static PlayerController playerCtrl;             //  Player controller
    private static EnemyController enemyCtrl;               //  Enemy controller
    private static Rigidbody playerRb;                      //  Player rigidbody
    private static Rigidbody enemyRb;                       //  Enemy rb
    public static int round;                                //  Current round
    public static IEnumerator wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set target fps
        Application.targetFrameRate = 144;

        // Set starting variable states
        round = 1;
        Lock = false;
        buyEnabled = false;
        buyTime = 12f;          //  12s for buy period

        // Set max number of rounds
        maxRounds = 42;

        // Get player and enemy objects
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        playerCtrl = player.GetComponent<PlayerController>();
        enemyCtrl = enemy.GetComponent<EnemyController>();
        playerRb = player.GetComponent<Rigidbody>();
        enemyRb = enemy.GetComponent<Rigidbody>();

        // Set spawner objects and make them invisible
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
        playerSpawn.GetComponent<MeshRenderer>().enabled = false;
        enemySpawn = GameObject.FindGameObjectWithTag("EnemySpawn");
        enemySpawn.GetComponent<MeshRenderer>().enabled = false;


        // Move player and enemy to their spawn points
        respawn();
    }

    // Credit to Roixo from Unity forums: https://answers.unity.com/questions/893966/how-to-find-child-with-tag.html
    // Find the child game object with a given tag
    public static GameObject FindChildObjectWithTag(GameObject parent, string tag)
    {
        // Get parent transform
        Transform tf = parent.transform;

        // Iterate through child objects
        for (int i = 0; i < tf.childCount; i++)
        {
            // Check if child's tag matches tag given
            if (tf.GetChild(i).gameObject.tag == tag)
            {
                return tf.GetChild(i).gameObject;
            }

        }

        return null;
    }

    private static void respawn()
    {
        // Face towards buy wall
        GameObject buyWall = GameObject.Find("BuyWall");
        playerSpawn.transform.LookAt(buyWall.transform);

        // Move to spawn points
        player.transform.position = playerSpawn.transform.position;
        enemy.transform.position = enemySpawn.transform.position;

        // Increment max ammo capacities
        playerCtrl.maxAmmo++;
        //enemyCtrl.maxAmmo++;
        

        // Get first person camera object
        GameObject fpCamera = FindChildObjectWithTag(player, "FirstPersonCamera");
        fpCamera.transform.LookAt(buyWall.transform);
        // TODO: Add code to do this for enemy


        // Ensure mesh renderers are enabled (visible)
        player.GetComponent<MeshRenderer>().enabled = true;
        enemy.GetComponent<MeshRenderer>().enabled = true;

        // Reset player/enemy health and ammo
        playerCtrl.health = playerCtrl.maxHealth;
        enemyCtrl.health = enemyCtrl.maxHealth;
        playerCtrl.ammo = playerCtrl.maxAmmo;
        //enemyCtrl.ammo = enemyCtrl.maxAmmo;

        // Check if buy period should be enabled
        if(round > 1)
        {
            buyEnabled = true;
            Lock = true;
        }
    }

    private void checkForRoundEnd()
    {
        // Check if player is dead
        if(playerCtrl.health <= 0)
        {
            // Set lock
            Lock = true;

            // Make player invisible
            player.GetComponent<MeshRenderer>().enabled = false;

            // Add score to enemy
            enemyCtrl.score += playerCtrl.maxHealth;
            enemyCtrl.wins++;

            // Award player some points at the end of the round
            playerCtrl.score += Mathf.Sqrt(playerCtrl.maxHealth);

            // Move to next round
            nextRound();
        }
        // Check if AI is dead
        if (enemyCtrl.health <= 0)
        {
            // Set round lock
            Lock = true;

            // Make enemy invisible
            enemy.GetComponent<MeshRenderer>().enabled = false;

            // Add score to player
            playerCtrl.score += enemyCtrl.maxHealth;
            playerCtrl.wins++;

            // Award enemy some points at the end of the round
            enemyCtrl.score += Mathf.Sqrt(enemyCtrl.maxHealth);

            // Move to next round
            nextRound();
        }
    }

    /*
     * Start the next round
     */
    public static void nextRound(bool skipWait = false)
    {
        // Zero-out player and enemy velocities
        playerRb.velocity = new Vector3(0, 0, 0);
        enemyRb.velocity = new Vector3(0, 0, 0);

        if(!skipWait)
        {
            // Wait before starting the next round
            wait(3f);
        }
        
        round++;

        // Check if game is over
        if(round < maxRounds + 1)
        {
            Debug.Log("Round " + round + "!");
            respawn();
            printStats();
            wait(2f);
            Lock = false;
        }
        else
        {
            Lock = true;
            Debug.Log("Game over!");
            Application.Quit();
        }
    }

    // Print player and enemy stats
    public static void printStats()
    {
        Debug.Log("Player score: " + player.GetComponent<PlayerController>().score);
        Debug.Log("Player wins: " + player.GetComponent<PlayerController>().wins);
        Debug.Log("Enemy score: " + enemy.GetComponent<EnemyController>().score);
        Debug.Log("Enemy wins: " + enemy.GetComponent<EnemyController>().wins);
    }

    public static bool locked()
    {
        return Lock;
    }

    public static bool mouseLocked()
    {
        return mouseLock;
    }

    public static void lockMouse(bool state)
    {
        mouseLock = state;
    }

    public static GameObject getPlayerObject()
    {
        return player;
    }

    public static GameObject getEnemyObject()
    {
        return enemy;
    }

    //  Enable the 'Buy period' where the player and AI can purchase upgrades
    private static void buyPeriod()
    {
        buyEnabled = true;
        Lock = true;

        // TODO: Handle upgrade logic
    }

    public static bool BuyEnabled
    {
        get { return buyEnabled; }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if game is currently in pre-round buy period
        if(buyEnabled)
        {
            if(buyTime == 12f)
            {
                Debug.Log("BUY PERIOD STARTED!");
            }

            // Decrement buyTime and call buyPeriod function
            buyTime -= Time.deltaTime;

            // Check if buy time has expired
            if(buyTime > 0)
            {
                buyPeriod();
            }
            else
            {
                // End buy period and reset timer
                buyEnabled = false;
                Lock = false;
                buyTime = 12f;
                Debug.Log("BUY PERIOD ENDED!");
            }
        }

        // Ensure game is not locked
        if(!locked())
        {
            // Check for round end condition
            checkForRoundEnd();
        }
       
    }
}