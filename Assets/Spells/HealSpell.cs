using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Spells/Heal Target", order = 1)]
public class HealSpell : Spell
{
    [Header("Spell Specific")]
    public float healAmount;

    public override void Execute(SpellCommand commandInfo)
    {
        commandInfo.target.RestoreHealth(healAmount, 0);

        foreach (Enemy e in Player.allEnemyCharacters) e.AddAggro(commandInfo.caster, 2);

        commandInfo.caster.CommandList.RemoveAt(0);
    }
}
