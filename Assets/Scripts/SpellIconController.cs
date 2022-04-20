using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellIconController : MonoBehaviour
{
    public RectTransform rectTransform;

    [SerializeField] TextMeshProUGUI _shortcutText;
    [SerializeField] TextMeshProUGUI _cooldownText;

    Image _image;
    Hero _hero;
    int _spellIndex = 0;
    int lastCooldown = -10;

    public void SetSpellindex(int i) => _spellIndex = i;

    public void SetIconShortcut(string s) => _shortcutText.text = s;

    public void SetHero(Hero h)
    {
        if (_hero != h) _hero = h;
    }

    public void SetIcon(Hero hero) => _image.sprite = hero.spells[_spellIndex].spellIcon;

    private void Awake()
    {
        _image = GetComponent<Image>();
        //rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        var cooldown = (int)Mathf.Ceil(Mathf.Clamp(_hero.spells[_spellIndex].cooldown - (Time.time - _hero.spellCooldowns[_spellIndex]), 0, 999));

        if (cooldown != lastCooldown) 
        {
            if (cooldown == 0) _cooldownText.text = "";
            else _cooldownText.text = cooldown.ToString();
        }

        lastCooldown = cooldown;
    }

}
