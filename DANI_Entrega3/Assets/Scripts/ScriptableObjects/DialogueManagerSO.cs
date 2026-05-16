using UnityEngine;

[CreateAssetMenu(fileName = "DialogueManagerSO", menuName = "Scriptable Objects/DialogueMnagerSO")]

public class DialogueManagerSo : ScriptableObject
{
    //Creamos una clase interna que represente a cada línea del diálogo.
    [System.Serializable]
    public class DialogueLines
    {
        public string speakerName;
        [TextArea(3, 10)] public string lineText;
    }
    
    public DialogueLines[] dialogueLines;
}
