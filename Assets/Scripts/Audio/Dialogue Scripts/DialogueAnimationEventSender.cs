using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class DialogueAnimationEventSender : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public void DialougeBoxOpenAnimationComplete()
    {
        dialogueManager.DialougeBoxOpenAnimationComplete();
    }

    public void TriggerNextDialogueFromTransitionAnimation()
    {
        dialogueManager.TriggerNextDialogueFromTransitionAnimation();
    }
    public void DialougeBoxCloseAnimationComplete()
    {
        dialogueManager.DialougeBoxCloseAnimationComplete();
    }

}
