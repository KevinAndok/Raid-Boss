using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Speed Up", menuName = "Spells/Speed Up", order = 1)]
public class SpeedUpSpell : Spell
{
    [Header("Spell Specific")]
    public float movespeedGain;
    public float attackSpeedGain;
    public float duration;

    public override void Execute(SpellCommand commandInfo)
    {
        commandInfo.caster.StartCoroutine(SpeedUp(commandInfo));

        commandInfo.caster.CommandList.RemoveAt(0);
    }

    IEnumerator SpeedUp(SpellCommand commandInfo)
    {
        var caster = commandInfo.caster;

        caster.moveSpeed += movespeedGain;
        caster.stats.attackSpeed -= attackSpeedGain;

        yield return new WaitForSeconds(duration);
        
        caster.moveSpeed -= movespeedGain;
        caster.stats.attackSpeed += attackSpeedGain;
    }
}
