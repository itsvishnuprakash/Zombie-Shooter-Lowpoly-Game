
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjetcs/Weapon", order = 0)]
public class Weapon : ScriptableObject 
{
    public int damage;
    public int maxAmmo;
    public float timeBetweenShots;
    public int curretAmmo;
}