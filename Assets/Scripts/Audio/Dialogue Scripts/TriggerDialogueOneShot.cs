using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogueOneShot : MonoBehaviour
{
    public DialogueObject specificDialogueOneShot;
    public void TriggerSpecificDialogueOneShot()
    {
        AudioManager.instance.dialogueOneLiner.TriggerOneShot(specificDialogueOneShot);
    }

    public void TriggerLevelStartOneLiner()
    {
        AudioManager.instance.dialogueOneLiner.TriggerLevelStartOneLiner();
    }
    public void TriggerPlayerDeathOneLiner()
    {
        AudioManager.instance.dialogueOneLiner.TriggerPlayerDeathOneLiner();
    }
    public void TriggerBossDeathOneLiner()
    {
        AudioManager.instance.dialogueOneLiner.TriggerBossDeathOneLiner();
    }
}
