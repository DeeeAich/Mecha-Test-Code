using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionTextPopUpUI : MonoBehaviour
{
    public bool open;
    public bool close;
    public string _name;
    public TextMeshProUGUI nameText;

    [TextArea]
    public string finalDescriptionText = "";
    private string currentDescriptionText = "";
    public TextMeshProUGUI descriptionText;

    public rewardRarity rarity;
    public Color noneColor;
    public Color commonColor;
    public Color rareColor;
    public Color legendaryColor;

    public Animator anim;

    public void PassInRewardInfo(string rewardName, string rewardDescription, rewardRarity rewardRarity)
    {
        _name = rewardName;
        finalDescriptionText = rewardDescription;
        rarity = rewardRarity;
    }

    private void LateUpdate()
    {
        if (open)
        {
            open = false;
            OpenDescriptionPopUpUI();
        }
        if (close)
        {
            close = false;
            CloseDescriptionPopUpUI();
        }
    }

    public void OpenDescriptionPopUpUI()
    {
        anim.SetBool("Open", true);
        nameText.text = _name;
        descriptionText.text = "";
        switch (rarity)
        {
            case rewardRarity.none:
                nameText.color = noneColor;
                break;
            case rewardRarity.common:
                nameText.color = commonColor;
                break;
            case rewardRarity.rare:
                nameText.color = rareColor;
                break;
            case rewardRarity.legendary:
                nameText.color = legendaryColor;
                break;
        }
        StopAllCoroutines();
        StartCoroutine(DisplayText(finalDescriptionText));
    }


    IEnumerator DisplayText(string text)
    {
        yield return new WaitForSeconds(.25f);
        for (int i = 0; i < text.Length; i++)
        {
            currentDescriptionText = text.Substring(0, i);
            descriptionText.text = currentDescriptionText;
            yield return new WaitForSeconds(0.005f);
        }
    }
    public void CloseDescriptionPopUpUI()
    {
        anim.SetBool("Open", false);
    }

}

public enum rewardRarity
{
    none,
    common,
    rare,
    legendary
}
