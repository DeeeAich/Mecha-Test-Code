using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD;
using FMOD.Studio;
using FMODUnity;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DialogueObject", order = 1)]
public class DialogueObject : ScriptableObject
{
    [Header("Profile Image")]
    public Texture character;

    [Header("Text")]
    [TextArea]
    public string dialogueText;
    public TextSpeed textSpeed;
    public AutomaticSkipDelay automaticSkipDelay;
    public bool isBold = false;
    public bool isItalic = false;

    [Header("Animation")]
    public bool triggerActionAnimation;
    public int actionAnimationID;


    [Header("Talking Audio")]
    public bool talkingAudio = true;
    public EventReference characterDialogue;


    [Header("Entry Audio")]
    public bool triggerEntryAudio;
    public EventReference entryAudioEvent;
    public CharacterEmotion entryCharacterEmotion;


    [Header("Exit Audio")]
    public bool triggerExitAudio;
    public EventReference exitAudioEvent;
    public CharacterEmotion exitCharacterEmotion;

}

public enum TextSpeed
{
    Medium,
    Slow,
    Fast
}
public enum AutomaticSkipDelay
{
    Medium,
    Slow,
    Fast
}

public enum CharacterEmotion
{
    Hello,
    Happy,
    Sad,
    Angry,
    Surprise,
    Laugh,
    Snore,
    Bye
}

