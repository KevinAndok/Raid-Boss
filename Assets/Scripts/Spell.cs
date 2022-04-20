using UnityEngine;

public abstract class Spell : ScriptableObject
{
    public Sprite spellIcon;
    public string spellName;

    public string description;

    public float cooldown = 1;
    public float range = 0;

    public float manacost = 0;

    public float castTime = 0;

    [Header("Dependancies")]
    public bool requireTarget;
    public bool canBeCanceled;
    public bool useRange;
    public bool dontCancelAttack;

    [Space(5)]
    public bool canTargetDead;
    public bool canTargetLiving;

    [Space(5)]
    public bool targetEnemies;
    public bool targetAllies;
    public bool targetGround;

    public abstract void Execute(SpellCommand commandInfo);
}
