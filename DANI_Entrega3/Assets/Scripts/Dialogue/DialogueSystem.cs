using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    private DialogueManagerSo dialogue;

    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject imageBriefcase;
    [SerializeField] private int lineToShowImage = 1;
    [SerializeField] private float typingSpeed = 0.05f;

    private int currentLine = 0;
    private bool isTyping = false;
    private bool isActive = false;

    private void OnEnable()
    {
        imageBriefcase.SetActive(false);

        if (InputController.Instance != null)
            InputController.Instance.OnContinueDialogue += ContinueDialogue;
    }

    public void StartDialogue()
    {
        currentLine = 0;
        isActive = true;
        StartCoroutine(ShowLine());
    }

    public void SetDialogue(DialogueManagerSo newDialogue)
    {
        dialogue = newDialogue;
    }

    private void ContinueDialogue()
    {
        if (!isActive) return;

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = dialogue.dialogueLines[currentLine].lineText;
            isTyping = false;
            ShowVisualForCurrentLine();
        }
        else
        {
            currentLine++;
            if (currentLine < dialogue.dialogueLines.Length)
            {
                StartCoroutine(ShowLine());
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private IEnumerator ShowLine()
    {
        isTyping = true;
        dialogueText.text = "";
        speakerNameText.text = dialogue.dialogueLines[currentLine].speakerName;

        foreach (char character in dialogue.dialogueLines[currentLine].lineText)
        {
            dialogueText.text += character;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;

        //Activa la imagen a mostrar en el diálogo
        ShowVisualForCurrentLine();
    }

    private void ShowVisualForCurrentLine()
    {
        // En este ejemplo, activamos el maletín solo en la segunda línea (índice 1)
        if (imageBriefcase != null)
            imageBriefcase.SetActive(currentLine == lineToShowImage);
    }

    private void EndDialogue()
    {
        isActive = false;
        UIManager.Instance.OnDialogueEnded(); // Notifica al UIManager
    }

    private void OnDisable()
    {
        if (InputController.Instance != null)
            InputController.Instance.OnContinueDialogue -= ContinueDialogue;
    }
}