using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Execute", menuName = "Spells/Execute", order = 1)]
public class ExecuteSpell : Spell
{
    [Header("Spell Specific")]
    [Range(0, 1)] public float healthPercentage;
    public float nonExecuteDamage;
    public bool canExecuteBoss;

    public override void Execute(SpellCommand commandInfo)
    {
        RPGStats targetStats = commandInfo.target.stats;
        float damage;

        if ((Player.instance.bosses.Contains(commandInfo.target) && !canExecuteBoss) || targetStats.health / targetStats.maxHealth > healthPercentage) damage = nonExecuteDamage;
        else damage = targetStats.health + 1;

        if (commandInfo.target.GetType() == typeof(Enemy)) ((Enemy)commandInfo.target).AddAggro(commandInfo.caster, 3);
        commandInfo.target.TakeDamage(damage, 0);

        commandInfo.caster.CommandList.RemoveAt(0);
    }
}
