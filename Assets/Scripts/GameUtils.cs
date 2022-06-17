using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUtils : MonoBehaviour
{
    private static int maxRounds;                           //  Maximum number of rounds
    private static bool Lock;                               //  Lock control of game objects such as player and enemy
    private static GameObject playerSpawn;                  //  Player spawner
    private static GameObject enemySpawn;                   //  Enemy spawner
    private static GameObject player;                       //  Player object
    private static GameObject enemy;                        //  Enemy object
    private static PlayerController playerCtrl;             //  Player controller
    private static EnemyController enemyCtrl;               //  Enemy controller
    public static int round;                                //  Current round
    public static IEnumerator wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set target fps to 60
        Application.targetFrameRate = 60;

        // Set starting round number
        round = 1;
        Lock = false;

        // Set max number of rounds
        maxRounds = 42;

        // Get player and enemy objects
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        playerCtrl = player.GetComponent<PlayerController>();
        enemyCtrl = enemy.GetComponent<EnemyController>();

        // Set spawner objects and make them invisible
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
        playerSpawn.GetComponent<MeshRenderer>().enabled = false;
        enemySpawn = GameObject.FindGameObjectWithTag("EnemySpawn");
        enemySpawn.GetComponent<MeshRenderer>().enabled = false;


        // Move player and enemy to their spawn points
        Debug.Log("Player position before move: " + player.transform.position);
        respawn();
        Debug.Log("Player position after move: " + player.transform.position);

    }

    private static void respawn()
    {
        // Move to spawn points
        player.transform.position = playerSpawn.transform.position;
        enemy.transform.position = enemySpawn.transform.position;

        // Ensure mesh renderers are enabled (visible)
        player.GetComponent<MeshRenderer>().enabled = true;
        enemy.GetComponent<MeshRenderer>().enabled = true;

        // Reset player/enemy health
        playerCtrl.health = playerCtrl.maxHealth;
        enemyCtrl.health = enemyCtrl.maxHealth;
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
    public static void nextRound()
    {
        // Wait before starting the next round
        wait(3f);
        round++;

        // Check if game is over
        if(round < maxRounds + 1)
        {
            // TODO: Add logic to start next round
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

    //  Enable the 'Buy period' where the player and AI can purchase upgrades
    private static void buyPeriod()
    {
        float waitTime = 15f; // 15s buy period

        while(waitTime > 0)
        {
            wait(1f);
            waitTime--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!locked())
        {
            checkForRoundEnd();
        }
       
    }
}