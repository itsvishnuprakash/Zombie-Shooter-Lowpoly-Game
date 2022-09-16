using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetection : MonoBehaviour
{
    // Reference
    [SerializeField] private PlayerMovement playerMovement;
    GameObject obj;


    private void OnTriggerStay(Collider other) {
        if(other.gameObject.CompareTag("AR"))
        {
            obj=other.gameObject;
            Equip();
        }
        else if(other.gameObject.CompareTag("Pistol"))
        {
            obj=other.gameObject;
            Equip();
        }
        else if(other.gameObject.CompareTag("Health"))
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                other.gameObject.SetActive(false);
                playerMovement.healthBoosts++;
                playerMovement.healthBoosts=(int)Mathf.Clamp(playerMovement.healthBoosts,0,playerMovement.maxHealthBoosts);
            }
        }
        else if(other.gameObject.CompareTag("AmmoPistol"))
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                other.gameObject.SetActive(false);
                playerMovement.pistolMags++;
                playerMovement.pistolMags=(int)Mathf.Clamp(playerMovement.pistolMags,0,playerMovement.maxPistolMag);
            }
        }
        else if(other.gameObject.CompareTag("AmmoAR"))
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                other.gameObject.SetActive(false);
                playerMovement.aRMags++;
                playerMovement.aRMags=(int)Mathf.Clamp(playerMovement.aRMags,0,playerMovement.maxARMag);
            }
        }
        else if(other.gameObject.CompareTag("Key"))
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                other.gameObject.SetActive(false);
                playerMovement.hasKey=true;
            }
        }
        


    }

    //equiping guns and getting datas
    void Equip()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            playerMovement.RemoveObjectsInHand(); 

            obj.SetActive(false);
            playerMovement.currentWeapon=obj;
            
            GetWeaponSpecs specs=obj.GetComponent<GetWeaponSpecs>();
            if(specs!=null)
            {
                playerMovement.damage=specs.damage;
                playerMovement.maxAmmo=specs.maxAmmo;
                playerMovement.curretAmmo=specs.curretAmmo;
                playerMovement.timeBetweenShots=specs.timeBetweenShots;
            }

            if(obj.CompareTag("Pistol"))
            {
                playerMovement.Equip(1);
            }
            else if(obj.CompareTag("AR"))
            {
                playerMovement.Equip(2);
            }
        }
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Finish") )
        {
            if(!playerMovement.hasKey)
            {
                UIManager.instance.ShowInstruction();
            }
            else
            {
                UIManager.instance.Win();
                playerMovement.hasWon=true;
            }
        }
    }
}
