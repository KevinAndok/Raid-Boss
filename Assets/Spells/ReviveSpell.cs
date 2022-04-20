using UnityEngine;

[CreateAssetMenu(fileName = "Revive", menuName = "Spells/Revive", order = 1)]
public class ReviveSpell : Spell
{
    [Header("Spell Specific")]
    [Range(0, 1)] public float healthPercentage;

    public override void Execute(SpellCommand commandInfo)
    {
        RPGStats targetStats = commandInfo.target.stats;
        RPGStats casterStats = commandInfo.caster.stats;

        float healAmount = casterStats.maxHealth * healthPercentage;
        healAmount = casterStats.health > healAmount ? healAmount : casterStats.health;

        float heal = Mathf.Clamp(targetStats.health + healAmount, 0, targetStats.maxHealth) - targetStats.health;

        commandInfo.caster.TakeDamage(healAmount, 0);

        FloatingText.CreateFloatingText(commandInfo.target.transform.position, heal.ToString(), FloatingText.TextColor.green);
        targetStats.health = Mathf.Clamp(targetStats.health + healAmount, 0, targetStats.maxHealth);

        foreach (Enemy e in Player.allEnemyCharacters) e.AddAggro(commandInfo.caster, 5);

        commandInfo.caster.CommandList.RemoveAt(0);
    }
}
