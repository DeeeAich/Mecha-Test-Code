using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public RawImage profileImage;
    public Animator dialogueBoxAnimator;
    public Animator npcAnimator;
    public GameObject dialogueBox;

    public bool inRange;
    public GameObject interactionPrompt;
    public GameObject interactionCamera;

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

    public int currentInteractionNum = 0;
    public int currentLine = 0;
    private bool isTyping;

    public List<DialogueInteraction> Interaction;




    /////////////////////////////////////////////////// OPEN
    void OpenDialogueBoxAndStart()
    {
        dialogueBox.SetActive(true);
        interactionCamera.SetActive(true);
        interactionPrompt.SetActive(false);
        isTyping = true;
        dialogueText.text = "";
        npcAnimator.SetBool("Talking", true);
        StopAllCoroutines();
        StartCoroutine(OpenDialogueWait());
    }
    IEnumerator OpenDialogueWait()
    {
        yield return new WaitForSeconds(0.5f);
        DisplayNext(true);
    }


    public void DisplayNext(bool isFirstLine = false)
    {
        if (isFirstLine) currentLine = 0;

        DisplayNewDialogue(Interaction[currentInteractionNum].dialogueObject[currentLine]);
    }
    void DisplayNewDialogue(DialogueObject dialogueObject)
    {
        profileImage.texture = dialogueObject.character;
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
        if (dialogueObject.isBold) dialogueText.fontStyle = FontStyles.Bold;
        if (dialogueObject.isItalic) dialogueText.fontStyle = FontStyles.Italic;

        finalText = dialogueObject.dialogueText + " ";


        if (dialogueObject.triggerActionAnimation) 
        {
            npcAnimator.SetInteger("ActionID", dialogueObject.actionAnimationID);
            npcAnimator.SetTrigger("Action");
        }


        StopAllCoroutines();

        StartCoroutine(DisplayText(finalText));
        StartCoroutine(WaitForTextToEnd(finalText.Length * textDelay + 1f, automaticSkipDelay));

    }


    IEnumerator DisplayText(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            currentText = text.Substring(0, i);
            dialogueText.text = currentText;
            if (i > 1)
            {
                string lastChar = currentText.Substring(currentText.Length - 1);

            }
            yield return new WaitForSeconds(textDelay);
        }       
    }
    IEnumerator WaitForTextToEnd(float length, float automaticSkipDelay)
    {
        isTyping = true;
        yield return new WaitForSeconds(length);
        isTyping = false;
        Debug.Log("Finished");
        yield return new WaitForSeconds(automaticSkipDelay);
        currentLine++;
        if (currentLine < Interaction[currentInteractionNum].dialogueObject.Count)
        { 
            DisplayNext(false);
        }
        else
        {
            CloseDialogue();
            currentInteractionNum++;
        }
    }


    /////////////////////////////////////////////////// CLOSE

    void CloseDialogue()
    {
        dialogueBoxAnimator.SetTrigger("Close");
        npcAnimator.SetBool("Talking", false);
        StopAllCoroutines();
        StartCoroutine(CloseDialogueWait());
    }
    IEnumerator CloseDialogueWait()
    {
        yield return new WaitForSeconds(0.5f);
        dialogueBox.SetActive(false);
        interactionCamera.SetActive(false);
    }




    /////////////////////////////////////////////////// Interaction

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
            interactionPrompt.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            interactionPrompt.SetActive(false);
            dialogueBox.SetActive(false);
            interactionCamera.SetActive(false);
            npcAnimator.SetBool("Talking", false);
            npcAnimator.SetTrigger("Exit");
            isTyping = false;
            StopAllCoroutines();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(isTyping==false)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    OpenDialogueBoxAndStart();
                }
            }
        }
    }

}
