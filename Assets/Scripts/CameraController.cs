using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    // Camera objects
    Camera firstPerson;
    Camera thirdPerson;


    // Start is called before the first frame update
    void Start()
    {
        // Get camera objects
        firstPerson = GameObject.FindGameObjectWithTag("FirstPersonCamera").GetComponent<Camera>();
        thirdPerson = GameObject.FindGameObjectWithTag("ThirdPersonCamera").GetComponent<Camera>();

        // Start game in first person mode
        firstPerson.enabled = true;
        thirdPerson.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for input to change camera view
        if (Input.GetKeyDown(KeyCode.V))
        {
            firstPerson.enabled = !firstPerson.enabled;
            thirdPerson.enabled = !thirdPerson.enabled;
        }
    }
}
