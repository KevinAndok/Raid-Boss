using UnityEngine;

[CreateAssetMenu(fileName = "Stealth", menuName = "Spells/Stealth", order = 1)]
public class StealthSpell : Spell
{
    [Header("Spell Specific")]
    public int aggroLoss;

    public override void Execute(SpellCommand commandInfo)
    {
        foreach (Enemy enemy in Player.allEnemyCharacters) 
        {
            if (enemy.aggroPoints.Count > 0) 
            {
                foreach(Enemy.AggroPoints ap in enemy.aggroPoints)
                {
                    if (ap.character == commandInfo.caster) 
                    {
                        ap.points = Mathf.Clamp(ap.points - aggroLoss, 0, Mathf.Infinity);
                    }
                }
            }
        }

        commandInfo.caster.CommandList.RemoveAt(0);
    }

}
