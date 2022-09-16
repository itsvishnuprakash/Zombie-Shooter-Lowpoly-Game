
using UnityEngine;


[CreateAssetMenu(fileName = "Zombie", menuName = "ScriptableObjetcs/Zombie", order = 0)]
public class Zombie : ScriptableObject 
{
    public int points;
    public int damage;
    public int health;
    public float range;
    public bool acidAttacking;
    public float speed=3f;
}

