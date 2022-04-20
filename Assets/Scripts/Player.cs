using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Player : MonoBehaviour
{
    public static Player instance;

    public static Hero[] selectedCharacters = new Hero[0];

    public static List<Hero> allHeroCharacters = new List<Hero>();
    public static List<Enemy> allEnemyCharacters = new List<Enemy>();

    public static int controllingCharacter = -1;

    public Color controllingCharacterHighlightColor = Color.white;

    public Vector2 heroStartPosition;
    public GameObject tankPrefab, meleePrefab, rangedPrefab, healerPrefab;

    public List<Enemy> bosses;
    public static Hero CurrentControllingCharacter => GetControllingHero();

    public LayerMask characterCollisionLayers;

    public GameObject gameOverScreen;
    public TextMeshProUGUI finalText;

    [Space(10)]
    public GameObject statusBarPrefab;
    public Color healthBarColor;
    public Color manaBarColor;
    public Color CastBarColor;

    private void Awake()
    {
        instance = this;

        allHeroCharacters = new List<Hero>();
        allEnemyCharacters = new List<Enemy>();
        selectedCharacters = new Hero[0];
        controllingCharacter = -1;

        //used for testing
        if (PartyCreation.roles.Count == 0) 
            PartyCreation.roles = new List<HeroRole>() { HeroRole.Tank, HeroRole.Melee, HeroRole.Ranged, HeroRole.Healer };

        CreateHeroes();
    }

    private void Update()
    {
        if (selectedCharacters.Length > 0)
            if (Input.GetKeyDown(KeyCode.Tab))
                SelectCharacter((controllingCharacter + 1) % selectedCharacters.Length);

        if (!gameOverScreen.activeInHierarchy)
        {
            if (GameOverCheck(allEnemyCharacters.ToArray()))
            {
                finalText.text = "Raid successful!";
                gameOverScreen.SetActive(true);
                gameOverScreen.transform.DOScale(1, .25f).From(0);
            }
            else if (GameOverCheck(allHeroCharacters.ToArray()))
            {
                finalText.text = "Raid failed!";
                gameOverScreen.SetActive(true);
                gameOverScreen.transform.DOScale(1, .25f).From(0);
            }
        }
    }

    public bool GameOverCheck(Character[] list)
    {
        bool gameOver = true;

        foreach (Character i in list)
        {
            if (!i.IsDead)
            {
                gameOver = false;
                break;
            }
        }

        return gameOver;
    }

    public static void AssignSelectedCharacters(List<Hero> selection)
    {
        foreach (Hero h in selectedCharacters) if (!selection.Contains(h)) h.selected = false;
        if (selection.Count > 0) selectedCharacters = selection.ToArray();
        else selectedCharacters = new Hero[0];
        foreach (Hero h in selectedCharacters) if (!h.selected) h.selected = true;
        SelectCharacter(0);
    }

    static void SelectCharacter(int character)
    {
        controllingCharacter = character;

        for (int i = 0; i < selectedCharacters.Length; i++)
        {
            if (i == character) selectedCharacters[i].selectionIndicatorShape.Color = instance.controllingCharacterHighlightColor;
            else selectedCharacters[i].selectionIndicatorShape.Color = Color.white;
        }

        GameUI.UpdateSpellIcons();
    }

    static Hero GetControllingHero()
    {
        return selectedCharacters.Length > 0 ? selectedCharacters[controllingCharacter] : null;
    }

    public Vector3[] GetHeroSpawnPositions(int numberOfHeroes)
    {
        Vector3[] positions = new Vector3[numberOfHeroes];
        int rows = (int)Mathf.Ceil((float)numberOfHeroes / 4);

        int[] heroesInRows = new int[2];

        //TODO: put this in a for loop ffs
        heroesInRows[0] = numberOfHeroes > 4 ? 4 : numberOfHeroes;
        heroesInRows[1] = numberOfHeroes - 4;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < heroesInRows[y]; x++)
            {
                positions[4 * y + x] = new Vector3(
                    heroStartPosition.x - (float)heroesInRows[y] / 2 * 1.2f + x * 1.2f + .6f,
                    heroStartPosition.y - 1.8f * y * Mathf.Sign(-heroStartPosition.y),
                    0);
            }
        }

        return positions;
    }

    public void CreateHeroes()
    {
        Vector3[] positions = GetHeroSpawnPositions(PartyCreation.roles.Count);

        for (int i = 0; i < positions.Length; i++)
        {
            switch (PartyCreation.roles[i])
            {
                case HeroRole.Tank:
                    allHeroCharacters.Add(Instantiate(tankPrefab, positions[i], Quaternion.identity).GetComponent<Hero>());
                    break;
                case HeroRole.Melee:
                    allHeroCharacters.Add(Instantiate(meleePrefab, positions[i], Quaternion.identity).GetComponent<Hero>());
                    break;
                case HeroRole.Ranged:
                    allHeroCharacters.Add(Instantiate(rangedPrefab, positions[i], Quaternion.identity).GetComponent<Hero>());
                    break;
                case HeroRole.Healer:
                    allHeroCharacters.Add(Instantiate(healerPrefab, positions[i], Quaternion.identity).GetComponent<Hero>());
                    break;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        foreach(Vector3 pos in GetHeroSpawnPositions(7)) Gizmos.DrawWireSphere(pos, .5f);
    }
#endif
}
