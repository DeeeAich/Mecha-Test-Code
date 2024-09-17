using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI textBox;
    public RawImage profileImage;
    public GameObject dialogueBox;
    public Animator dialogueBoxAnimator;
    public Texture defaultProfileImage;
    public GameObject audioPrefab;
    public AudioClip transitionSound;
    public GameObject skipButton;
    public GameObject interactButton;

    private float smallFontSize = 25;
    private float mediumFontSize = 35;
    private float largeFontSize = 45;

    private float textDelay;
    private float slowTextDelay = 0.2f;
    private float mediumTextDelay = 0.125f;
    private float fastTextDelay = 0.05f;

    private float automaticSkipDelay;
    private float slowAutomaticSkipDelay = 2.5f;
    private float mediumAutomaticSkipDelay = 1.75f;
    private float fastAutomaticSkipDelay = 0.5f;

    [TextArea]
    public string currentText = "";
    [TextArea]
    public string finalText = "";

    public int currentInteraction = 1;
    public int currentLine = 0;
    private DialogueObject currnetDialogueObject;
    private bool isTyping;
    private bool isTransitioning;

    private List<DialogueObject> currentInteractionList;
    public List<DialogueObject> Interaction_1;
    public List<DialogueObject> Interaction_2;
    public List<DialogueObject> Interaction_3;


    public void DisplayNext(bool isFirstLine = false)
    {
        if (isFirstLine) currentLine = 0;

        switch (currentInteraction)
        {
            case 1:
                currentInteractionList = Interaction_1;
                if (currentLine == 0) { OpenDialogueBoxAndStart(); } else { TriggerNextDialogueTransition(); }
                break;
            case 2:
                currentInteractionList = Interaction_2;
                if (currentLine == 0) { OpenDialogueBoxAndStart(); } else { TriggerNextDialogueTransition(); }
                break;
            case 3:
                currentInteractionList = Interaction_3;
                if (currentLine == 0) { OpenDialogueBoxAndStart(); } else { TriggerNextDialogueTransition(); }
                break;
            default:
                return;
        }
    }
    void OpenDialogueBoxAndStart()
    {
        dialogueBox.SetActive(true);
        interactButton.SetActive(false);
        profileImage.texture = defaultProfileImage;
        textBox.text = "";
    }
    public void DialougeBoxOpenAnimationComplete()
    {
        TriggerNextDialogueTransition();
    }
    void TriggerNextDialogueTransition()
    {
        dialogueBoxAnimator.SetTrigger("transition");
        isTransitioning = true;
        PlaySFX(transitionSound);
        skipButton.SetActive(false);
    }
    public void TriggerNextDialogueFromTransitionAnimation()
    {
        DisplayNewDialogue(currentInteractionList[currentLine]);
        isTransitioning = false;
        skipButton.SetActive(true);
    }

    void DisplayNewDialogue(DialogueObject dialogueObject)
    {
        currnetDialogueObject = dialogueObject;

        if (dialogueObject.entranceAudio != null)
        {
            PlaySFX(dialogueObject.entranceAudio);
        }
        switch (dialogueObject.fontSize)
        {
            case FontSize.Small:
                textBox.fontSize = smallFontSize;
                break;
            case FontSize.Medium:
                textBox.fontSize = mediumFontSize;
                break;
            case FontSize.Large:
                textBox.fontSize = largeFontSize;
                break;
        }
        switch (dialogueObject.textSpeed)
        {
            case TextSpeed.Slow:
                textDelay = slowTextDelay;
                break;
            case TextSpeed.Medium:
                textDelay = mediumTextDelay;
                break;
            case TextSpeed.Fast:
                textDelay = fastTextDelay;
                break;
        }
        switch (dialogueObject.automaticSkipDelay)
        {
            case AutomaticSkipDelay.Slow:
                automaticSkipDelay = slowAutomaticSkipDelay;
                break;
            case AutomaticSkipDelay.Medium:
                automaticSkipDelay = mediumAutomaticSkipDelay;
                break;
            case AutomaticSkipDelay.Fast:
                automaticSkipDelay = fastAutomaticSkipDelay;
                break;
        }
        if (dialogueObject.isBold) textBox.fontStyle = FontStyles.Bold;
        if (dialogueObject.isItalic) textBox.fontStyle = FontStyles.Italic;

        finalText = dialogueObject.dialogueText + " ";
        profileImage.texture = dialogueObject.character;



        StartCoroutine(DisplayText(finalText));
        StartCoroutine(WaitForTextToEnd(finalText.Length * textDelay + 1f, automaticSkipDelay));

    }
    IEnumerator DisplayText(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            currentText = text.Substring(0, i);
            textBox.text = currentText;
            if (i > 1)
            {
                string lastChar = currentText.Substring(currentText.Length - 1);
                if (lastChar != " ")
                {
                    PlayTypingSFX(currnetDialogueObject.typingAudio);
                }
            }
            else
            {
                PlayTypingSFX(currnetDialogueObject.typingAudio);
            }
            yield return new WaitForSeconds(textDelay);
        }       
    }
    void PlayTypingSFX(AudioClip clip)
    {
        if (!isTyping) return;

        GameObject newSFX = Instantiate(audioPrefab);
        newSFX.GetComponent<AudioSource>().clip = clip;
        newSFX.GetComponent<AudioSource>().Play();
        Destroy(newSFX, clip.length);

    }
    void PlaySFX(AudioClip clip)
    {
        if (!dialogueBox.activeInHierarchy) return;

        GameObject newSFX = Instantiate(audioPrefab);
        newSFX.GetComponent<AudioSource>().clip = clip;
        newSFX.GetComponent<AudioSource>().Play();
        Destroy(newSFX, clip.length);
    }
    IEnumerator WaitForTextToEnd(float length, float automaticSkipDelay)
    {
        isTyping = true;
        yield return new WaitForSeconds(length);
        isTyping = false;
        Debug.Log("Finished");
        yield return new WaitForSeconds(automaticSkipDelay);
        currentLine++;
        if (currentLine < currentInteractionList.Count)
        { 
            DisplayNext();
        }
    }

    public void SkipCurrentText()
    {   
        if (isTransitioning) return;

        if (isTyping)
        {
            StopAllCoroutines();
            currentText = finalText;
            textBox.text = currentText;
            isTyping = false;
        }
        else
        {
            currentLine++;
            if (currentLine < currentInteractionList.Count)
            {
                DisplayNext();
            }
            else
            {
                currentLine = 0;
                currentInteraction++;
                CloseDialogue();
            }
        }
    }

    void CloseDialogue()
    {
        dialogueBoxAnimator.SetTrigger("exit");
        skipButton.SetActive(false);
    }
    public void DialougeBoxCloseAnimationComplete()
    {
        dialogueBox.SetActive(false);
        interactButton.SetActive(true);
    }
}
