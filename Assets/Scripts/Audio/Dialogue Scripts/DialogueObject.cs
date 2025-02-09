using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

    [Header("Audio")]
    public bool triggerActionAnimation;
    public int actionAnimationID;
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