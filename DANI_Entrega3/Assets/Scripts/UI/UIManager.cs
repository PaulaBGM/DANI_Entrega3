using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public bool IsDialogueActive { get; private set; }

    [Header("Dialogue")]
    [SerializeField] private DialogueManagerSo initialDialogue;
    [SerializeField] private DialogueManagerSo finalDialogue;

    [SerializeField]
    private DialogueSystem dialogueSystem;

    [Header("HUD")]
    [SerializeField] private GameObject hudInstance;

    [Header("Menus")]
    [SerializeField] private GameObject gameOverMenu;

    [SerializeField]
    private GameObject missionCompletePanel;

    [SerializeField]
    private GameObject missionFailedPanel;

    [SerializeField]
    private float panelDuration = 3f;

    [Header("Stored Weapons")]
    [SerializeField] private GameObject fullCirclePistol;

    [SerializeField] private GameObject fullCircleRifle;

    [SerializeField] private GameObject fullCircleGrenade;

    [Header("Active Weapon")]
    [SerializeField] private GameObject middleCircleKnife;

    [SerializeField] private GameObject middleCirclePistol;

    [SerializeField] private GameObject middleCircleRifle;

    [SerializeField] private GameObject middleCircleGrenade;

    private WeaponManager weaponManager;

    private bool showMissionCompletePanel;

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

    private void OnEnable()
    {
        PlayerBehavior.OnGameOver += ShowMissionFailed;
    }

    private void OnDisable()
    {
        PlayerBehavior.OnGameOver -= ShowMissionFailed;
    }

    private void Start()
    {
        weaponManager =
            FindFirstObjectByType<WeaponManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetupInitialState();

        ShowInitialDialogue();
    }

    private void Update()
    {
        if (weaponManager != null)
        {
            UpdateWeaponUI();
        }
    }

    // =====================================================
    // INITIAL SETUP
    // =====================================================

    private void SetupInitialState()
    {
        hudInstance?.SetActive(false);

        dialogueSystem?.gameObject.SetActive(false);

        gameOverMenu?.SetActive(false);

        missionCompletePanel?.SetActive(false);

        missionFailedPanel?.SetActive(false);
    }

    // =====================================================
    // DIALOGUE
    // =====================================================

    public void ShowInitialDialogue()
    {
        if (initialDialogue == null)
        {
            ShowHUD();
            return;
        }

        hudInstance?.SetActive(false);

        dialogueSystem.gameObject.SetActive(true);

        dialogueSystem.SetDialogue(initialDialogue);

        dialogueSystem.StartDialogue();

        IsDialogueActive = true;

        Time.timeScale = 0f;
    }

    public void ShowFinalDialogue()
    {
        if (finalDialogue == null)
        {
            ShowMissionComplete();
            return;
        }

        hudInstance?.SetActive(false);

        dialogueSystem.gameObject.SetActive(true);

        dialogueSystem.SetDialogue(finalDialogue);

        dialogueSystem.StartDialogue();

        IsDialogueActive = true;

        showMissionCompletePanel = true;

        Time.timeScale = 0f;
    }

    public void OnDialogueEnded()
    {
        IsDialogueActive = false;

        dialogueSystem.gameObject.SetActive(false);

        Time.timeScale = 1f;

        if (showMissionCompletePanel)
        {
            showMissionCompletePanel = false;

            ShowMissionComplete();
        }
        else
        {
            ShowHUD();
        }
    }

    // =====================================================
    // HUD
    // =====================================================

    public void ShowHUD()
    {
        hudInstance?.SetActive(true);
    }

    public void HideHUD()
    {
        hudInstance?.SetActive(false);
    }

    // =====================================================
    // GAME OVER
    // =====================================================

    public void ShowMissionFailed()
    {
        StartCoroutine(ShowMissionFailedCoroutine());
    }

    private IEnumerator ShowMissionFailedCoroutine()
    {
        Time.timeScale = 0f;

        HideHUD();

        yield return new WaitForSecondsRealtime(1f);

        missionFailedPanel?.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSecondsRealtime(panelDuration);

        SceneFlowManager.Instance.LoadMainMenu();
    }

    // =====================================================
    // MISSION COMPLETE
    // =====================================================

    public void ShowMissionComplete()
    {
        StartCoroutine(ShowMissionCompleteCoroutine());
    }

    private IEnumerator ShowMissionCompleteCoroutine()
    {
        Time.timeScale = 0f;

        HideHUD();

        yield return new WaitForSecondsRealtime(1f);

        missionCompletePanel?.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSecondsRealtime(panelDuration);

        SceneFlowManager.Instance.LoadMainMenu();
    }

    // =====================================================
    // WEAPON UI
    // =====================================================

    private void UpdateWeaponUI()
    {
        if (weaponManager == null)
            return;

        DisableAllWeaponUI();

        WeaponType currentWeapon =
            weaponManager.CurrentWeaponType;

        // ACTIVE

        switch (currentWeapon)
        {
            case WeaponType.Knife:

                middleCircleKnife.SetActive(true);

                break;

            case WeaponType.Pistol:

                middleCirclePistol.SetActive(true);

                break;

            case WeaponType.Rifle:

                middleCircleRifle.SetActive(true);

                break;

            case WeaponType.GrenadeLauncher:

                middleCircleGrenade.SetActive(true);

                break;
        }

        // STORED

        if (weaponManager.HasWeapon(WeaponType.Pistol) &&
            currentWeapon != WeaponType.Pistol)
        {
            fullCirclePistol.SetActive(true);
        }

        if (weaponManager.HasWeapon(WeaponType.Rifle) &&
            currentWeapon != WeaponType.Rifle)
        {
            fullCircleRifle.SetActive(true);
        }

        if (weaponManager.HasWeapon(WeaponType.GrenadeLauncher) &&
            currentWeapon != WeaponType.GrenadeLauncher)
        {
            fullCircleGrenade.SetActive(true);
        }
    }

    private void DisableAllWeaponUI()
    {
        // STORED

        fullCirclePistol.SetActive(false);

        fullCircleRifle.SetActive(false);

        fullCircleGrenade.SetActive(false);

        // ACTIVE

        middleCircleKnife.SetActive(false);

        middleCirclePistol.SetActive(false);

        middleCircleRifle.SetActive(false);

        middleCircleGrenade.SetActive(false);
    }

    // =====================================================
    // BUTTONS
    // =====================================================

    public void StartGame(string sceneName)
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
