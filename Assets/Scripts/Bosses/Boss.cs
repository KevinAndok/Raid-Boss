using UnityEngine;

public abstract class Boss : MonoBehaviour
{
    public GameObject bossPrefab;
    public RPGStats[] bossStats;
    public Vector2[] bossPositions;

    public float DifficultyCalculation => 1 + ((float)PartyCreation.difficulty / 2f);

    public virtual void DifficultySetup()
    {
        float difficulty = DifficultyCalculation;

        foreach (RPGStats stats in bossStats) 
        {
            stats.health *= difficulty;
            stats.maxHealth *= difficulty;
            stats.healthRegeneration *= difficulty;
            stats.damage *= difficulty;
        }
    }
    public virtual void InitiateBattleield()
    {
        //create bosses
        for (int i = 0; i < bossStats.Length; i++)
        {
            var boss = Instantiate(bossPrefab, bossPositions[i], Quaternion.identity).GetComponent<Enemy>();
            boss.stats = bossStats[i];
            Player.instance.bosses.Add(boss);
            Player.allEnemyCharacters.Add(boss);
        }
    }
    public abstract void StartFight();
    public abstract void UpdateBehaviour();
}
