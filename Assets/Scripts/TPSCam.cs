using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCam : MonoBehaviour
{
    // References
    public Transform orientation;
    public Transform combatLookAt;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;

    public GameObject tPSCam;
    public GameObject combatCam;

    public float rotSpeed;

    float horizontal;
    float vertical;
    Vector3 inputDir;

    public CameraStyle currentStyle;
    //There are two styles for camera basic and combat
    public enum CameraStyle
    {
        basic,
        combat
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;
    }

    // Update is called once per frame
    void Update()
    {
        //rotate orientation
        Vector3 viewDir= player.position-new Vector3(transform.position.x,player.position.y,transform.position.z);
        orientation.forward=viewDir.normalized;

        //rotate player object
        // if(currentStyle==CameraStyle.basic)
        // {
        //     horizontal=Input.GetAxis("Horizontal");
        //     vertical=Input.GetAxis("Vertical");
        //     inputDir=orientation.forward*vertical+orientation.right*horizontal; 

        //     //smoothly changing player direction with input using slerp
        //     playerObj.forward=Vector3.Slerp(playerObj.forward,inputDir.normalized,Time.deltaTime*rotSpeed); 
        // }
        // else 
        if(currentStyle==CameraStyle.combat)
        {
            Vector3 dirToCombatLookAt= combatLookAt.position-new Vector3(transform.position.x,combatLookAt.position.y,transform.position.z);
            orientation.forward=dirToCombatLookAt.normalized;

            playerObj.forward=dirToCombatLookAt.normalized;

            // horizontal=Input.GetAxis("Horizontal");
            // vertical=Input.GetAxis("Vertical");
            // inputDir=orientation.forward*vertical+orientation.right*horizontal; 

            // //smoothly changing player direction with input using slerp
            // playerObj.forward=Vector3.Slerp(playerObj.forward,inputDir.normalized,Time.deltaTime*rotSpeed); 
        }

        //ChangeCameraStyle();
        
    }

    // To switch between camera styles
    void ChangeCameraStyle()
    {
        if (Input.GetMouseButtonDown(1))
        {
            currentStyle=CameraStyle.combat;
        }
        if(Input.GetMouseButtonUp(1))
        {
            currentStyle=CameraStyle.basic;
        }

        tPSCam.SetActive(false);
        combatCam.SetActive(false);

        if(currentStyle==CameraStyle.basic)
        {
            tPSCam.SetActive(true);
        }
        else if(currentStyle==CameraStyle.combat)
        {
            combatCam.SetActive(true);
        }

    }
}
