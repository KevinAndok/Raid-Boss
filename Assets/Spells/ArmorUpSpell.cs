using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor Up", menuName = "Spells/Armor Up", order = 1)]
public class ArmorUpSpell : Spell
{
    [Header("Spell Specific")]
    [Range(0, 1)] public float armorGain;
    public float duration;

    public override void Execute(SpellCommand commandInfo)
    {
        commandInfo.caster.StartCoroutine(ArmorUp(commandInfo));

        commandInfo.caster.CommandList.RemoveAt(0);
    }

    IEnumerator ArmorUp(SpellCommand commandInfo)
    {
        commandInfo.caster.stats.resistance += armorGain;
        yield return new WaitForSeconds(duration);
        commandInfo.caster.stats.resistance -= armorGain;
    }
}
