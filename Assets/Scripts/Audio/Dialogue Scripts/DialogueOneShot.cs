using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMOD.Studio;

public class DialogueOneShot : MonoBehaviour
{
    public Animator npcInteractionAnimator;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogueText;
    public RawImage characterProfile;

    bool isTalking;
    bool canClose;
    DialogueObject currentDialogueObject;
    EventInstance currentDialogueInstance;

    [TextArea]
    public string currentText = "";
    [TextArea]
    public string finalText = "";


    

    private void FixedUpdate()
    {
        if (!IsPlaying(currentDialogueInstance) && isTalking && canClose)
        {
            StartCoroutine(CloseUI());
        }
    }

    public DialogueInteraction levelStartLines;
    public DialogueInteraction playerDeathLines;
    public DialogueInteraction bossKillLines;

    public void TriggerLevelStartOneLiner()
    {
        int num = Random.Range(0, levelStartLines.dialogueObject.Count);
        TriggerOneLiner(levelStartLines.dialogueObject[num]);
    }
    public void TriggerPlayerDeathOneLiner()
    {
        int num = Random.Range(0, playerDeathLines.dialogueObject.Count);
        TriggerOneLiner(playerDeathLines.dialogueObject[num]);
    }
    public void TriggerBossDeathOneLiner()
    {
        int num = Random.Range(0, bossKillLines.dialogueObject.Count);
        TriggerOneLiner(bossKillLines.dialogueObject[num]);
    }






    public void TriggerOneLiner(DialogueObject dialogueObject)
    {
        if (isTalking) { return; }

        isTalking = true;
        npcInteractionAnimator.SetBool("Open", true);
        currentDialogueObject = dialogueObject;

        characterNameText.text = currentDialogueObject.characterName;
        finalText = currentDialogueObject.dialogueText + " ";
        characterProfile.texture = currentDialogueObject.character;

        StartCoroutine(OpenUI());
    }
    IEnumerator OpenUI()
    {
        yield return new WaitForSeconds(0.5f);
        TriggerAudio();
    }
    void TriggerAudio()
    {     
        
        currentDialogueInstance = FMODUnity.RuntimeManager.CreateInstance(currentDialogueObject.entryAudioEvent);
        currentDialogueInstance.start();


        //AudioManager.instance.PlayOneShotSFX(currentDialogueObject.entryAudioEvent,transform.position);
        StartCoroutine(DisplayText(finalText));

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
            yield return new WaitForSeconds(0.01f);

            if (i == text.Length - 1)
            {
                UnityEngine.Debug.Log("finished typing");
                canClose = true;
            }
        }
    }
    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }
    IEnumerator CloseUI()
    {
        yield return new WaitForSeconds(currentDialogueObject.automaticSkipDelay);
        npcInteractionAnimator.SetBool("Open", false);
        yield return new WaitForSeconds(0.5f);
        isTalking = false;
        canClose = false;
    }


}
