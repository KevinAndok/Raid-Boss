using UnityEngine;

[CreateAssetMenu(fileName = "Taunt", menuName = "Spells/Taunt", order = 1)]
public class TauntSpell : Spell
{
    [Header("Spell Specific")]
    public float aggroGain;

    public override void Execute(SpellCommand commandInfo)
    {
        if (!commandInfo.target || commandInfo.target.GetType() != typeof(Enemy)) return;
        ((Enemy)commandInfo.target).AddAggro(commandInfo.caster, aggroGain);

        commandInfo.caster.CommandList.RemoveAt(0);
    }

}
