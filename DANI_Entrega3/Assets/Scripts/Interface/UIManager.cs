using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public bool IsDialogueActive
    {
        get;
        private set;
    }

    [Header("Dialogue")]
    [SerializeField]
    private DialogueManagerSo initialDialogue;

    [SerializeField]
    private DialogueSystem dialogueSystem;

    [Header("HUD")]
    [SerializeField]
    private GameObject hudInstance;

    [Header("Final Panels")]
    [SerializeField]
    private GameObject missionCompletePanel;

    [SerializeField]
    private GameObject missionFailedPanel;

    private bool showMissionCompletePanel;

    // =========================
    // UNITY
    // =========================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(
                gameObject);
        }
        else
        {
            Destroy(gameObject);

            return;
        }
    }

    private void Start()
    {
        if (hudInstance != null)
        {
            hudInstance.SetActive(false);
        }

        if (missionCompletePanel != null)
        {
            missionCompletePanel.SetActive(false);
        }

        if (missionFailedPanel != null)
        {
            missionFailedPanel.SetActive(false);
        }

        ShowInitialDialogue();
    }

    // =========================
    // INITIAL DIALOGUE
    // =========================

    public void ShowInitialDialogue()
    {
        if (dialogueSystem == null)
            return;

        if (hudInstance != null)
        {
            hudInstance.SetActive(false);
        }

        dialogueSystem.gameObject.SetActive(true);

        dialogueSystem.SetDialogue(
            initialDialogue);

        dialogueSystem.StartDialogue();

        IsDialogueActive = true;

        Time.timeScale = 0f;
    }

    // =========================
    // FINAL DIALOGUE
    // =========================

    public void ShowFinalDialogue()
    {
        showMissionCompletePanel = true;

        OnDialogueEnded();
    }

    // =========================
    // SHOW HUD
    // =========================

    public void ShowHUD()
    {
        if (hudInstance != null)
        {
            hudInstance.SetActive(true);
        }

        if (dialogueSystem != null)
        {
            dialogueSystem.gameObject.SetActive(false);
        }
    }

    // =========================
    // DIALOGUE ENDED
    // =========================

    public void OnDialogueEnded()
    {
        IsDialogueActive = false;

        // =========================
        // FINAL FLOW
        // =========================

        if (showMissionCompletePanel)
        {
            if (missionCompletePanel != null)
            {
                missionCompletePanel.SetActive(true);
            }

            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;

            return;
        }

        // =========================
        // NORMAL FLOW
        // =========================

        ShowHUD();

        Time.timeScale = 1f;
    }

    // =========================
    // MISSION FAILED
    // =========================

    public void ShowMissionFailed()
    {
        Debug.Log(
            "MISSION FAILED");

        if (hudInstance != null)
        {
            hudInstance.SetActive(false);
        }

        if (dialogueSystem != null)
        {
            dialogueSystem.gameObject.SetActive(false);
        }

        if (missionFailedPanel != null)
        {
            missionFailedPanel.SetActive(true);
        }

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        Time.timeScale = 0f;
    }
}