using UnityEngine;

[CreateAssetMenu(fileName = "Restore Mana", menuName = "Spells/Restore Mana", order = 1)]
public class RestoreManaSpell : Spell
{
    [Header("Spell Specific")]
    public float manaAmount;

    public override void Execute(SpellCommand commandInfo)
    {
        commandInfo.target.RestoreMana(manaAmount, 0);

        commandInfo.caster.CommandList.RemoveAt(0);
    }
}
