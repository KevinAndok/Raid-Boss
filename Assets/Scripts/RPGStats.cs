using UnityEngine;

[System.Serializable]
public class RPGStats
{
    public float health, maxHealth;
    public float mana, maxMana;
    public float healthRegeneration, manaRegeneration;

    public float range;
    public float damage;
    public float attackSpeed;
    [Range(0, 1)] public float criticalStrike;

    [Range(0, .99f)] public float resistance;

    [HideInInspector] public float casting; //between 0 and 1; used by commands to display cast time
    [HideInInspector] public Coroutine castRoutine = null;
}
