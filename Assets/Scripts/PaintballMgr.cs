using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Paintball manager script
 */
public class PaintballMgr : MonoBehaviour
{
    private float tof;                              //  Time of flight before destroying object
    public AudioSource audioPlayer;                 //  For playing audio
    public AudioClip impactSound;                   //  Sound to play when paintball impacts an object
    public float damage = 100;                      //  Damage dealt

    // Start is called before the first frame update
    void Start()
    {
        tof = 300;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Rifle")
        {
            Debug.Log("\nPaintball has collided with " + collision.gameObject.tag + "!");

            // Play impact sound effect
            audioPlayer.PlayOneShot(impactSound);

            //  Destroy this paintball instance
            Destroy(gameObject, 0.2f);
        }
        
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag != "Rifle")
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Decrement time of flight
        tof--;

        if(tof <= 0)
        {
            Destroy(gameObject);
        }
    }
}
