using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PartyCreation : MonoBehaviour
{
    public static List<HeroRole> roles = new List<HeroRole>();
    public static int difficulty;

    public UIButton tankButton, meleeButton, rangedButton, healerButton;
    public UIButton[] partyPositions;
    public TextMeshProUGUI difficultyText;

    private void Awake()
    {
        difficulty = 0;
        roles = new List<HeroRole>();
    }

    public void AddPartyMember(UIButton button)
    {
        if (roles.Count < partyPositions.Length)
        {
            int position = roles.Count;

            Image image = partyPositions[position].GetComponent<Image>();

            image.color = Color.white;
            image.sprite = button.GetComponent<Image>().sprite;
            partyPositions[position].transform.DOMove(partyPositions[position].transform.position, .1f).From(button.transform.position);

            if (button == tankButton) roles.Add(HeroRole.Tank);
            else if (button == meleeButton) roles.Add(HeroRole.Melee);
            else if (button == rangedButton) roles.Add(HeroRole.Ranged);
            else if (button == healerButton) roles.Add(HeroRole.Healer);

            UpdateDifficultyText();
        }
    }

    public void RemovePartyMember(int index)
    {
        partyPositions[index].GetComponent<Image>().color = Color.clear;
        roles.RemoveAt(index);
        UpdatePartyPositions();
        UpdateDifficultyText();
    }

    void UpdatePartyPositions()
    {
        for (int i = 0; i < partyPositions.Length; i++)
        {
            if (i < roles.Count)
            {
                Image currentImage = partyPositions[i].GetComponent<Image>();
                if (currentImage.color == Color.clear)
                {
                    Image nextImage = partyPositions[i + 1].GetComponent<Image>();

                    currentImage.sprite = nextImage.sprite;
                    currentImage.color = Color.white;
                    nextImage.color = Color.clear;

                    partyPositions[i].transform.DOMove(partyPositions[i].transform.position, .1f).From(partyPositions[i + 1].position);
                }
            }
            else
            {
                partyPositions[i].GetComponent<Image>().color = Color.clear;
            }
        }
    }

    void UpdateDifficultyText()
    {
        difficulty = (int)Mathf.Ceil((float) roles.Count / 2);

        if (difficulty == 0) difficultyText.text = "Difficulty";
        if (difficulty == 1) difficultyText.text = "Easy";
        if (difficulty == 2) difficultyText.text = "Normal";
        if (difficulty == 3) difficultyText.text = "Hard";
        if (difficulty == 4) difficultyText.text = "Insane";
    }
}
