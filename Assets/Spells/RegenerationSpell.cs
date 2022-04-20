using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Regeneration", menuName = "Spells/Regeneration", order = 1)]
public class RegenerationSpell : Spell
{
    [Header("Spell Specific")]
    public float healthRegenerationBonus;
    public float manaRegenerationBonus;

    public float duration;

    public override void Execute(SpellCommand commandInfo)
    {
        commandInfo.caster.StartCoroutine(Regeneration(commandInfo));

        commandInfo.caster.CommandList.RemoveAt(0);
    }

    IEnumerator Regeneration(SpellCommand commandInfo)
    {
        commandInfo.target.stats.healthRegeneration += healthRegenerationBonus;
        commandInfo.target.stats.manaRegeneration += manaRegenerationBonus;
        yield return new WaitForSeconds(duration);
        commandInfo.target.stats.healthRegeneration -= healthRegenerationBonus;
        commandInfo.target.stats.manaRegeneration -= manaRegenerationBonus;
    }
}
