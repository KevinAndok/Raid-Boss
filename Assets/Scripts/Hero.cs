using UnityEngine;

public enum HeroRole { Unassigned, Tank, Melee, Ranged, Healer };

public class Hero : Character
{
    public Spell[] spells;
    public HeroRole role;

    [HideInInspector] public float[] spellCooldowns;

    public new void Awake()
    {
        base.Awake();

        spellCooldowns = new float[spells.Length];
        for (int i = 0; i < spellCooldowns.Length; i++) spellCooldowns[i] = -999;
    }

    public new void Update()
    {
        base.Update();

        if (role == HeroRole.Unassigned)
        {
            if (IsDead)
            {
                Destroy(statusBars.gameObject);
                Destroy(gameObject);
            }
            if (CommandList.Count == 0)
            {
                Destroy(statusBars.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
