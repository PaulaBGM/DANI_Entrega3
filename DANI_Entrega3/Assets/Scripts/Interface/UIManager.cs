using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public bool IsDialogueActive { get; private set; } = false;

    [Header("Dialogue Panel")]
    [SerializeField] private DialogueManagerSo _initialDialogue;
    [SerializeField] private DialogueManagerSo _finalDialogue;
    [SerializeField] private DialogueSystem dialogueSystem;

    [Header("HUD")]
    [SerializeField] private GameObject hudInstance;
    [SerializeField] private float panelDuration;
    
    [Header("Finals")]
    [SerializeField] private GameObject missionCompletePanel;
    [SerializeField] private GameObject missionFailedPanel;

    private bool showMissionCompletePanel = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        hudInstance.SetActive(false);
        dialogueSystem.gameObject.SetActive(false);

        // Estado inicial: mostrar HUD, ocultar diálogo
       // ShowInitialDialogue();
    }

    public void ShowInitialDialogue()
    {
        if (hudInstance != null)
            hudInstance.SetActive(false);

        if (dialogueSystem.gameObject != null)
        {
            dialogueSystem.SetDialogue(_initialDialogue);

            dialogueSystem.gameObject.SetActive(true);
            dialogueSystem?.StartDialogue(); // Inicia el diálogo manualmente
            
            IsDialogueActive = true;
            
            // Pausar la escena
            Time.timeScale = 0f;
        }
    }

    public void ShowHUD()
    {
        if (hudInstance != null)
            hudInstance.SetActive(true);

        if (dialogueSystem.gameObject != null)
            dialogueSystem.gameObject.SetActive(false);
    }

    // Muestra el diálogo final
    public void ShowFinalDialogue()
    {
        hudInstance?.SetActive(false);
        dialogueSystem.gameObject.SetActive(true);

        // Le pasamos el nuevo diálogo al sistema
        dialogueSystem.SetDialogue(_finalDialogue);

        IsDialogueActive = true;
        showMissionCompletePanel = true; // marcar que después viene la victoria

        Time.timeScale = 0f;
        dialogueSystem.StartDialogue();
    }
    
    public void ShowMissionComplete()
    {
        dialogueSystem.gameObject?.SetActive(false);

        StartCoroutine(ShowPanelForSeconds(missionCompletePanel));
    }

    public void ShowMissionFailed()
    {
        hudInstance?.SetActive(false);
        dialogueSystem.gameObject?.SetActive(false);

        StartCoroutine(ShowPanelForSeconds(missionFailedPanel));
    }
    
    private IEnumerator ShowPanelForSeconds(GameObject panel)
    {
        // Pausar la escena
        Time.timeScale = 0f;
        
        panel?.SetActive(true);
        yield return new WaitForSecondsRealtime(panelDuration);
        panel?.SetActive(false);
        Cursor.lockState = CursorLockMode.None; //Bloquea el cursor

        SceneFlowManager.Instance.LoadMainMenu();
    }
    
    // Este método será llamado por el DialogueSystem al terminar el diálogo
    public void OnDialogueEnded()
    {
        ShowHUD();
        
        IsDialogueActive = false;
        
        if (showMissionCompletePanel)
        {
            showMissionCompletePanel = false;
            ShowMissionComplete(); // 👈 ahora muestra la victoria tras el diálogo
        }
        else
        {
            ShowHUD();
            Time.timeScale = 1f;
        }
    }
}