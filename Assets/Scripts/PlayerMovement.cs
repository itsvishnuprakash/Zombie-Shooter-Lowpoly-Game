using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    // Public Static instance created for acessing the public player properties by enemies
    public static PlayerMovement instance; 

    // References
    public Animator anim;
    public GameObject[] objectInHand;


    //public Transform orientation;
    public float walkSpeed;
    public float runSpeed;
    float horizontal;
    float vertical;
    float moveSpeed;
    int health=100;
    public float healthMultiplier=1f;

    //Weapon Variables
    public int damage;
    public int maxAmmo;
    public int curretAmmo=0;
    public float timeBetweenShots;
    public GameObject currentWeapon=null;

    //Equipments or collectables and maximum Capacity
    public int healthBoosts=0;
    public int pistolMags=0;
    public int aRMags=0;
    public bool hasKey=false;
    public int maxPistolMag=2;
    public int maxARMag=2;
    public int maxHealthBoosts=2;

    
    //Character controller for movement
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;

    //Camera
    public GameObject tpsCam;
    public GameObject aimCam;
    Transform camMain;

    bool isAiming=false;

    bool isDead=false;
    public bool hasWon=false;



    enum AnimationState
    {
        notEquiped,
        hasPistol,
        hasAR
    }
    AnimationState state;
    void Awake() 
    {
        if(instance!=this && instance!=null)
        {
            Destroy(this);
        }
        else
        {
            instance=this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        tpsCam.SetActive(true);
        aimCam.SetActive(false);

        RemoveObjectsInHand();

        camMain=Camera.main.transform;
        controller = gameObject.GetComponent<CharacterController>();
    }
    
    //Remove All Objects in Hand
    public void RemoveObjectsInHand()
    {
        //Updates the data in current weapon
        if(currentWeapon!=null)
        {
            currentWeapon.GetComponent<GetWeaponSpecs>().curretAmmo=curretAmmo;
            currentWeapon.transform.position=new Vector3(transform.position.x+1f,transform.position.y+1.3f,transform.position.z+1f);
            currentWeapon.SetActive(true);
            currentWeapon=null;
        }
        //make all weapon in hand not active
        for(int i=0;i<objectInHand.Length;i++)
        {
            objectInHand[i].SetActive(false);
        }
        state=AnimationState.notEquiped;
        UIManager.instance.ammoText.gameObject.SetActive(false);
        anim.SetInteger("state",(int)state);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead || hasWon)
        {
            return;
        }
       GetInput();
       
       Attack();


       UpdateHealthAndUI();


       
    }
    //For applying physics based movements and works independant of frames
    void FixedUpdate() 
    {
        if(isDead || hasWon)
        {
            return;
        }
        
        MovePlayer();
    }
    //For reading inputs and setting the corresponding speed when left shift is pressed
    void GetInput()
    {
        horizontal=Input.GetAxis("Horizontal");
        vertical=Input.GetAxis("Vertical");
        if(Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed=runSpeed;
        }
        else
        {
            moveSpeed=walkSpeed;
        }

        //pressing G to drop equiped item
       if(Input.GetKeyDown(KeyCode.G))
       {
            RemoveObjectsInHand();
       }

       //Aiming input making blending between aim cam and tps cam
       if(Input.GetKeyDown(KeyCode.Z))
       {
            var camRotation = camMain.transform.rotation;
            aimCam.transform.rotation=camRotation;
            
            transform.forward=tpsCam.transform.forward;
            tpsCam.SetActive(false);
            aimCam.SetActive(true);
            UIManager.instance.crossHair.SetActive(true);
            isAiming=true;
            

       }
       if(Input.GetKeyUp(KeyCode.Z))
       {
            var camRotation = camMain.transform.rotation;
            tpsCam.transform.rotation=camRotation;

            isAiming=false;
            tpsCam.transform.position=aimCam.transform.position;
            tpsCam.SetActive(true);
             aimCam.SetActive(false);
            UIManager.instance.crossHair.SetActive(false);
       }
    }
    //Applying movement vectors using the character controller component
    void MovePlayer()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(horizontal, 0, vertical);
        move=camMain.forward*move.z+camMain.right*move.x;
        move.y=0;
        move*=Time.deltaTime * moveSpeed;
        controller.Move(move);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        float speed=Vector3.Magnitude(move)*10;
        speed=Mathf.Clamp01(speed);
        anim.SetFloat("velocity",speed);

        //During aiming, set the rotation of player with camera
        if(isAiming)
        {
            var CharacterRotation = camMain.transform.rotation;
            CharacterRotation.x = 0;
            CharacterRotation.z = 0;
         
            transform.rotation = CharacterRotation;
        }
    }

    //Attack by different states
    void Attack()
    {
        switch(state)
        {
            case AnimationState.notEquiped:break;
            case AnimationState.hasPistol:
            if(Input.GetKeyDown(KeyCode.Mouse0) ||Input.GetKeyDown(KeyCode.X))
            {
                anim.SetTrigger("shoot");
                Shoot();
            }
            break;
            case AnimationState.hasAR:
            if(Input.GetKeyDown(KeyCode.Mouse0) ||Input.GetKeyDown(KeyCode.X))
            {
                anim.SetTrigger("shoot");
                StartCoroutine(ARShoot());
                
            }
            break;
            default:break;

        }
    }
    //Shooting with RayCasting
    void Shoot()
    {
        if(curretAmmo>0)
        {
            //RayCasting 
            curretAmmo--;

            UIManager.instance.ammoText.text=curretAmmo.ToString()+" / "+maxAmmo.ToString();

            RaycastHit hit;
            Vector2 screenCenterPoint=new Vector2(Screen.width/2,Screen.height/2);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

            if(Physics.Raycast(ray,out hit,Mathf.Infinity))
            {
                Debug.Log(hit.transform.gameObject.name);
                ZombieController zombieController=hit.transform.GetComponent<ZombieController>();
                if(zombieController!=null)
                {
                    zombieController.GetDamage(damage);
                }
            }
        }
    }

    //Coroutine for shooting with AR with a time gap between shoots
    IEnumerator ARShoot()
    {
        while(!Input.GetKeyUp(KeyCode.Mouse0) ||Input.GetKeyUp(KeyCode.X))
        {
            Shoot();
            yield return new WaitForSeconds(timeBetweenShots);
        }
        anim.SetTrigger("stopShoot");
    }


    //Called by enemy script on attacking player with corresponding damage as argument
    public void GetDamage(int damage)
    {
        health-=damage;

        //player will die when health reaches 0
        if(health<=0 && !isDead)
        {
            anim.SetTrigger("die");
            isDead=true;
            UIManager.instance.Fail();
        }
    }

    //Collect Equipments and also change between animation parameter states ..called from object detection script
    public void Equip(int equipment)
    {
        switch(equipment)
        {
            case 0: state=AnimationState.notEquiped;
            break;
            case 1:state=AnimationState.hasPistol;
            UIManager.instance.ammoText.gameObject.SetActive(true);
            objectInHand[equipment-1].SetActive(true);
            break;
            case 2:state=AnimationState.hasAR;
            UIManager.instance.ammoText.gameObject.SetActive(true);
            objectInHand[equipment-1].SetActive(true);
            break;
            default:break;
        }

        UIManager.instance.ammoText.text=curretAmmo.ToString()+" / "+maxAmmo.ToString();

        anim.SetInteger("state",(int)state);

    }

    //Updates health gradually with time and also use collectables with corresponding inputs
    void UpdateHealthAndUI()
    {
        if(health<100)
        {
           health+=(int)(healthMultiplier*Time.deltaTime);
        }

        //update on ui
        UIManager.instance.healthSlider.value=Mathf.Clamp01(health/100f);
        if(health<=30f)
        {
            UIManager.instance.fillHealthSlider.color=Color.red;
        }
        else
        {
            UIManager.instance.fillHealthSlider.color=UIManager.instance.initialColor;
        }
        int key=0;
        if(hasKey)
        {
            key=1;
        }

        //Updating details of collectables
        UIManager.instance.KeyText.text="Key :"+key+"/1";
        UIManager.instance.healthBoostsText.text="Med :"+healthBoosts.ToString()+"/"+maxHealthBoosts;
        UIManager.instance.pistolMagText.text="P Mags :"+pistolMags.ToString()+"/"+maxPistolMag;
        UIManager.instance.arMagText.text="AR Mags :"+aRMags.ToString()+"/"+maxARMag;

        //Using Collected items
        if(state==AnimationState.hasPistol && Input.GetKeyDown(KeyCode.R) &&pistolMags>0)
        {
            curretAmmo=maxAmmo;
            pistolMags--;
        }
        if(state==AnimationState.hasAR && Input.GetKeyDown(KeyCode.R) &&aRMags>0)
        {
            curretAmmo=maxAmmo;
            aRMags--;
        }
        if(Input.GetKeyDown(KeyCode.H) && healthBoosts>0)
        {
            health+=50;
            health=(int)Mathf.Clamp(health,0,100f);
        }
    }

    
}
