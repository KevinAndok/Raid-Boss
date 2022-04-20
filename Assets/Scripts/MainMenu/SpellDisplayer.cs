using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellDisplayer : MonoBehaviour
{
    public Spell startingSpell;
    public Image image;
    public TextMeshProUGUI text;

    private void Awake()
    {
        SetSpellInformation(startingSpell);
    }

    public void SetSpellInformation(Spell spell)
    {
        image.sprite = spell.spellIcon;
        text.text = spell.description;
    }

}


