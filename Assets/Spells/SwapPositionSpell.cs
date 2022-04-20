using UnityEngine;

[CreateAssetMenu(fileName = "Swap Position", menuName = "Spells/Swap Position", order = 1)]
public class SwapPositionSpell : Spell
{
    public override void Execute(SpellCommand commandInfo)
    {
        Vector2 targetDestination = commandInfo.caster.transform.position;
        Vector2 casterDestination = commandInfo.target.transform.position;

        commandInfo.target.transform.position = targetDestination;
        commandInfo.caster.transform.position = casterDestination;

        commandInfo.caster.CommandList.RemoveAt(0);
    }

}
