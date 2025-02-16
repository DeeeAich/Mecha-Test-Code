using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DialogueInteraction", order = 2)]

public class DialogueInteraction : ScriptableObject
{
    public List<DialogueObject> dialogueObject;
}
