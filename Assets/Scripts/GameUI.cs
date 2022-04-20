using UnityEngine;
using Shapes;

public class GameUI : MonoBehaviour
{
    public static GameUI instance;

    public GameObject spellIconBackgroundPrefab;
    public GameObject spellIconPrefab;

    public float backgroundMargin = 10;

    static Vector2 spellIconSize;
    static int selectedCharacterSpellCount;
    static Rectangle iconBackground;
    static SpellIconController[] iconPool;
    static Hero _displayedHero;

    static Vector2 CalculateSpellBackgroundSize => new Vector2((spellIconSize.x + 2 * instance.backgroundMargin) * selectedCharacterSpellCount, spellIconSize.y + 2 * instance.backgroundMargin);

    private void Start()
    {
        if (!instance) instance = this;
        CreateSpellIconPool();
        spellIconSize = spellIconPrefab.GetComponent<RectTransform>().sizeDelta;
    }

    public static void UpdateSpellIcons()
    {
        var heroInControl = Player.CurrentControllingCharacter;

        if (heroInControl != null)
        {
            if (_displayedHero != heroInControl)
            {
                iconBackground.gameObject.SetActive(true);
                selectedCharacterSpellCount = heroInControl.spells.Length;

                var iconBGSize = CalculateSpellBackgroundSize;
                iconBackground.Width = iconBGSize.x;

                for (int i = 0; i < iconPool.Length; i++)
                {
                    iconPool[i].SetHero(heroInControl);
                    iconPool[i].gameObject.SetActive(i < selectedCharacterSpellCount);
                    iconPool[i].rectTransform.anchoredPosition = new Vector2(i * (2 * instance.backgroundMargin + spellIconSize.x) + instance.backgroundMargin - (iconBGSize.x / 2) + (spellIconSize.x / 2), 10);
                    if (heroInControl.spells.Length > i) iconPool[i].SetIcon(heroInControl);
                }

                _displayedHero = Player.CurrentControllingCharacter;
            }
        }
        else HideSpellUI();
    }

    static void HideSpellUI()
    {
        _displayedHero = null;
        iconBackground.gameObject.SetActive(false);
    }

    void CreateSpellIconPool()
    {
        iconPool = new SpellIconController[CommandController.instance.spellHotkeys.Length];

        iconBackground = Instantiate(spellIconBackgroundPrefab, transform).GetComponent<Rectangle>();

        for (int i = 0; i < iconPool.Length; i++)
        {
            iconPool[i] = Instantiate(spellIconPrefab, iconBackground.transform).GetComponent<SpellIconController>();

            iconPool[i].SetIconShortcut(CommandController.instance.spellHotkeys[i].ToString());
            iconPool[i].SetSpellindex(i);
        }

        HideSpellUI();
    }
}
