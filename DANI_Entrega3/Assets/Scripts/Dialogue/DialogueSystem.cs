using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    private DialogueManagerSo dialogue;

    [Header("UI")]
    [SerializeField]
    private GameObject dialoguePanel;

    [SerializeField]
    private TextMeshProUGUI speakerNameText;

    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [SerializeField]
    private GameObject imageBriefcase;

    [Header("Dialogue Settings")]
    [SerializeField]
    private int lineToShowImage = 1;

    [SerializeField]
    private float typingSpeed = 0.05f;

    private int currentLine;

    private bool isTyping;

    private bool isActive;

    private bool interactionConsumed;

    private bool canContinue;

    private PlayerInputHandler input;

    // =========================
    // UNITY
    // =========================

    private void Awake()
    {
        input =
            FindFirstObjectByType<PlayerInputHandler>();
    }

    private void OnEnable()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if (imageBriefcase != null)
        {
            imageBriefcase.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isActive)
            return;

        if (input == null)
            return;

        if (canContinue &&
            input.InteractPressed &&
            !interactionConsumed)
        {
            interactionConsumed = true;

            ContinueDialogue();
        }

        if (!input.InteractPressed)
        {
            interactionConsumed = false;
        }
    }

    // =========================
    // START DIALOGUE
    // =========================

    public void StartDialogue()
    {
        if (dialogue == null)
        {
            Debug.LogWarning(
                "NO DIALOGUE ASSIGNED");

            return;
        }

        currentLine = 0;

        isActive = true;

        canContinue = false;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        StartCoroutine(
            EnableContinueRoutine());

        StartCoroutine(
            ShowLine());
    }

    private IEnumerator EnableContinueRoutine()
    {
        yield return new WaitForSecondsRealtime(
            0.2f);

        canContinue = true;
    }

    // =========================
    // SET DIALOGUE
    // =========================

    public void SetDialogue(
        DialogueManagerSo newDialogue)
    {
        dialogue = newDialogue;
    }

    // =========================
    // CONTINUE
    // =========================

    private void ContinueDialogue()
    {
        if (!isActive)
            return;

        // =========================
        // SKIP TYPING
        // =========================

        if (isTyping)
        {
            StopAllCoroutines();

            dialogueText.text =
                dialogue.dialogueLines[currentLine].lineText;

            isTyping = false;

            ShowVisualForCurrentLine();

            StartCoroutine(
                EnableContinueRoutine());

            return;
        }

        // =========================
        // NEXT LINE
        // =========================

        currentLine++;

        if (currentLine <
            dialogue.dialogueLines.Length)
        {
            StartCoroutine(
                ShowLine());
        }
        else
        {
            EndDialogue();
        }
    }

    // =========================
    // SHOW LINE
    // =========================

    private IEnumerator ShowLine()
    {
        isTyping = true;

        dialogueText.text = "";

        speakerNameText.text = dialogue.dialogueLines[currentLine].speakerName;

        string line = dialogue.dialogueLines[currentLine].lineText;

        foreach (char character in line)
        {
            dialogueText.text += character;

            yield return new WaitForSecondsRealtime(
                typingSpeed);
        }

        isTyping = false;

        ShowVisualForCurrentLine();
    }

    // =========================
    // VISUALS
    // =========================

    private void ShowVisualForCurrentLine()
    {
        if (imageBriefcase != null)
        {
            imageBriefcase.SetActive(
                currentLine == lineToShowImage);
        }
    }

    // =========================
    // END DIALOGUE
    // =========================

    private void EndDialogue()
    {
        isActive = false;

        // APAGA EL PANEL VISUAL
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // APAGA EL OBJETO DEL SISTEMA
        gameObject.SetActive(false);

        UIManager.Instance.OnDialogueEnded();
    }
}