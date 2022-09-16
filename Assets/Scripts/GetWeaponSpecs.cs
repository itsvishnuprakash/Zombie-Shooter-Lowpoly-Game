using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWeaponSpecs : MonoBehaviour
{
    // References
    [SerializeField] private Weapon weapon;
    //variables
    public int damage;
    public int maxAmmo;
    public int curretAmmo;
    public float timeBetweenShots;
    // Start is called before the first frame update
    void Start()
    {
        GetSpecs();
    }

    void GetSpecs()
    {
        damage=weapon.damage;
        curretAmmo=weapon.curretAmmo;
        maxAmmo=weapon.maxAmmo;
        timeBetweenShots=weapon.timeBetweenShots;
    }
}
