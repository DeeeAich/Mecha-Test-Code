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
    public string characterName;

    [Header("Text")]
    [TextArea]
    public string dialogueText;
    public float textSpeed = 0.1f;
    public float automaticSkipDelay;
    public bool isBold = false;
    public bool isItalic = false;

    /*
    [Header("Animation")]
    public bool triggerActionAnimation;
    public int actionAnimationID;

    
    [Header("Talking Audio")]
    public bool talkingAudio = false;
    public EventReference characterDialogue;
    */

    [Header("Entry Audio")]
    //public bool triggerEntryAudio;
    public EventReference entryAudioEvent;
    public float entryAudioEventLength = 1f;
    //public CharacterEmotion entryCharacterEmotion;

    /*
    [Header("Exit Audio")]
    public bool triggerExitAudio;
    public EventReference exitAudioEvent;
    public CharacterEmotion exitCharacterEmotion;

    [Header("Close Dialogue Audio")]
    public bool triggerCloseAudio;
    public EventReference closeAudioEvent;
    public CharacterEmotion closeCharacterEmotion;
    */
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

