using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    private GameObject player;
    private GameObject enemy;
    private PlayerController playerCtrl;
    private EnemyController enemyCtrl;
    public List<Upgrade> upgrades;

    // Start is called before the first frame update
    void Start()
    {
        // Get player and enemy objects
        player = GameObject.FindGameObjectWithTag("Player");
        playerCtrl = player.GetComponent<PlayerController>();
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemyCtrl = enemy.GetComponent<EnemyController>();

        // Create upgrades list
        upgrades = new List<Upgrade>(5)
        {
            new Upgrade("max ammo", 1, 100, playerCtrl.maxAmmo, 1),
            new Upgrade("max health", 2, 110, playerCtrl.maxHealth, 2),
            new Upgrade("damage", 3, 150, playerCtrl.ammoDamage, 1.5f),
            new Upgrade("movement speed", 4, 120, playerCtrl.moveSpeed, 1.2f),
            new Upgrade("richochet chance", 5, 200, 0.05f, 1.1f)
        };
        
        foreach(Upgrade u in upgrades)
        {
            Debug.Log(u.Name + ": " + u.PlayerValue);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if buy period is enabled
        if(GameUtils.BuyEnabled)
        {
            // Check for upgrade selection
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                // Purchase max ammo upgrade for player
                upgrades[0].playerBuy();
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                // Purchase max health upgrade for player
                upgrades[1].playerBuy();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                // Purchase ammo damage upgrade for player
                upgrades[2].playerBuy();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                // Purchase movement speed upgrade for player
                upgrades[3].playerBuy();
            }
            //else if (Input.GetKeyDown(KeyCode.Alpha5))
            //{
            //    // Purchase max health upgrade for player
            //    upgrades[4].playerBuy();
            //}
        }
    }


    public class Upgrade
    {
        private string name;                    //  Name of the upgrade
        private int index;                      //  Upgrade index (value of key pressed)
        private int playerPurchased;            //  Number purchased by the player so far
        private int enemyPurchased;             //  Number purchased by the enemy so far
        private float cost;                     //  Starting cost
        private float value;                    //  Starting value
        private float playerCost;               //  Current cost for player
        private float enemyCost;                //  Current cost for enemy
        private float playerValue;              //  Current value of this upgrade for the player
        private float enemyValue;               //  Current value of this upgrade for the enemy
        private float scalingFactor;            //  Scaling factor for cost and value updates
        private PlayerController playerCtrl;    //  Player controller

        // Main constructor
        public Upgrade(string upgradeName, int idx = -1, float startCost = 0, float startValue = 0, float scaleFactor = 1)
        {
            // Initialize variables
            name = upgradeName;
            cost = startCost;
            value = startValue;
            playerCost = startCost;
            enemyCost = startCost;
            playerValue = startValue;
            enemyValue = startValue;
            scalingFactor = scaleFactor;
            playerPurchased = 0;
            enemyPurchased = 0;
            index = idx;

            playerCtrl = GameObject.Find("Player").GetComponent<PlayerController>();
        }

        // Purchase upgrade for player
        public void playerBuy()
        {
            // Check if player has enough score to buy upgrade
            if(playerCtrl.score < playerCost)
            {
                // TODO: Play error sound effect
                Debug.Log("ERROR in Upgrades.playerBuy(): Broke detected!");
                return;
            }

            // Subtract cost from player score and increment number purchased
            playerCtrl.score -= playerCost;
            playerPurchased++;

            // Compute rate of change for upgrade
            float rate = scalingFactor;

            if(playerPurchased > 1)
            {
                rate = scalingFactor * playerPurchased;
            }
            

            // Compute update to cost and value for the player
            playerCost = cost * rate;
            playerValue = value * rate;

            // Select the correct attribute to upgrade
            switch (index)
            {
                // Ammo capacity
                case 1:
                    playerCtrl.maxAmmo = playerValue;

                    if(playerPurchased > 1)
                    {
                        Debug.Log("Upgraded player " + name + " from " + (value * scalingFactor * Mathf.Abs(playerPurchased - 1)) + " to " + playerValue + "!");
                    }
                    else
                    {
                        Debug.Log("Upgraded player " + name + " from " + value + " to " + playerValue + "!");
                    }
                    
                    break;

                // Max health
                case 2:
                    playerCtrl.maxHealth = playerValue;
                    if (playerPurchased > 1)
                    {
                        Debug.Log("Upgraded player " + name + " from " + (value * scalingFactor * Mathf.Abs(playerPurchased - 1)) + " to " + playerValue + "!");
                    }
                    else
                    {
                        Debug.Log("Upgraded player " + name + " from " + value + " to " + playerValue + "!");
                    }
                    break;

                // Damage
                case 3:
                    playerCtrl.ammoDamage = playerValue;
                    if (playerPurchased > 1)
                    {
                        Debug.Log("Upgraded player " + name + " from " + (value * scalingFactor * Mathf.Abs(playerPurchased - 1)) + " to " + playerValue + "!");
                    }
                    else
                    {
                        Debug.Log("Upgraded player " + name + " from " + value + " to " + playerValue + "!");
                    }
                    break;

                // Movement speed
                case 4:
                    playerCtrl.moveSpeed = playerValue;
                    if (playerPurchased > 1)
                    {
                        Debug.Log("Upgraded player " + name + " from " + (value * scalingFactor * Mathf.Abs(playerPurchased - 1)) + " to " + playerValue + "!");
                    }
                    else
                    {
                        Debug.Log("Upgraded player " + name + " from " + value + " to " + playerValue + "!");
                    }
                    break;

                // Richochet
                case 5:
                    // TODO: Implement richocets
                    break;
            }

        }

        //  GETTERS  //
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int PlayerPurchased
        {
            get { return playerPurchased; }
        }
        public int EnemyPurchased
        {
            get { return enemyPurchased; }
        }
        public float PlayerCost
        {
            get { return playerCost; }
        }
        public float EnemyCost
        {
            get { return enemyCost; }
        }
        public float PlayerValue
        {
            get { return playerValue; }
        }
        public float EnemyValue
        {
            get { return enemyValue; }
        }
        public float ScalingFactor
        {
            get { return scalingFactor; }
        }
    }
}

