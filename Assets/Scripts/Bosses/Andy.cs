using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/*
    TODO: destroy platforms mechanic
    TODO: reward player mechanic
*/

[System.Serializable]
public class Andy : Boss
{
    public float platformlowerTime;

    public GameObject platformPrefab;
    public float platformSize;
    public int gridX, gridY;

    public float lavaDamage;

    public GameObject boundaries;

    public GameObject bombPrefab;
    public int bombTimer;
    public float bombFrequencyTimer;
    public float bombFrequencyDifference;
    public Coroutine bombSpawnCoroutine;

    List<LavaPlatform> allPlatforms = new List<LavaPlatform>();
    float lavaDamageTime = .5f;

    public override void InitiateBattleield()
    {
        base.InitiateBattleield();

        Camera.main.GetComponent<CameraBackgroundColorPulsator>().Pulsate();

        Instantiate(boundaries);

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                var platform = Instantiate(platformPrefab, transform.position + new Vector3(x * platformSize - (gridX - 1) * platformSize / 2, y * platformSize - (gridY - 1) * platformSize / 2, 0), Quaternion.identity);
                allPlatforms.Add(platform.GetComponent<LavaPlatform>());
                allPlatforms[allPlatforms.Count - 1].enabled = false;
                platform.transform.parent = transform;
            }
        }
    }

    public override void DifficultySetup()
    {
        base.DifficultySetup();

        float difficulty = DifficultyCalculation;

        bombTimer = (int)((float)bombTimer * (1f / (float)difficulty)); //might be too hard
        bombFrequencyTimer = bombFrequencyTimer / difficulty;
        lavaDamage *= difficulty;
        platformlowerTime *= (2f / (float)difficulty);
    }

    public override void StartFight()
    {
        foreach (LavaPlatform platform in allPlatforms) platform.enabled = true;

        StartCoroutine(SpawnBomb(bombFrequencyTimer + Random.Range(0, bombFrequencyDifference)));
    }

    public override void UpdateBehaviour()
    {
        lavaDamageTime -= Time.fixedDeltaTime;

        if (lavaDamageTime <= 0)
        {
            List<Hero> damagedHeroes = new List<Hero>();

            foreach (LavaPlatform platform in allPlatforms) 
            {
                if (!platform.safe)
                {
                    foreach (Hero h in platform.heroesInside)
                    {
                        if (!damagedHeroes.Contains(h))
                        {
                            h.TakeDamage(lavaDamage, .5f);
                            damagedHeroes.Add(h);
                        }
                    }
                }
            }

            lavaDamageTime = .5f;
        }
    }

    IEnumerator SpawnBomb(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        int randomSquareIndex = Random.Range(0, allPlatforms.Count);
        var bomb = Instantiate(bombPrefab, allPlatforms[randomSquareIndex].transform.position, Quaternion.identity).GetComponent<Enemy>();
        bomb.CreateCommand(new WaitCommand
        {
            type = CommandTypes.Wait,
            waitTime = bombTimer,
            canBeCancelled = false,
            position = bomb.transform.position
        });
        int timer = bombTimer;

        do
        {
            FloatingText.CreateFloatingText(bomb.transform.position, timer.ToString(), FloatingText.TextColor.white);
            yield return new WaitForSeconds(1);
            timer--;
        } while (!bomb.IsDead && timer > 0);

        if (!bomb.IsDead && timer == 0)
        {
            allPlatforms[randomSquareIndex].destroyed = true;
            allPlatforms.RemoveAt(randomSquareIndex);
        }

        Destroy(bomb.statusBars.gameObject);
        Destroy(bomb.gameObject);

        StartCoroutine(SpawnBomb(bombFrequencyTimer + Random.Range(-bombFrequencyDifference, bombFrequencyDifference)));
    }

}
