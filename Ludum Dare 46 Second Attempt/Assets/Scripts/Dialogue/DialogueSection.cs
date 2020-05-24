using UnityEngine;

[System.Serializable]
public class DialogueSection
{
    public int id;

    [TextArea]
    public string speech; 
    public DialogueOption[] options;
}
