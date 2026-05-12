using UnityEngine;
using UnityEngine.InputSystem;

public class CheckInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private GameObject interactionMark;

    [SerializeField] private bool isFinalNPC;

    private bool playerInRange;

    // =====================================================
    // UNITY
    // =====================================================

    private void Start()
    {
        if (interactionMark != null)
        {
            interactionMark.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerInRange)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    // =====================================================
    // INTERACTION
    // =====================================================

    private void Interact()
    {
        if (interactionMark != null)
        {
            interactionMark.SetActive(false);
        }

        // NPC FINAL

        if (isFinalNPC)
        {
            UIManager.Instance.ShowFinalDialogue();
        }
        else
        {
            UIManager.Instance.ShowInitialDialogue();
        }

        Destroy(gameObject);
    }

    // =====================================================
    // TRIGGER
    // =====================================================

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (interactionMark != null)
        {
            interactionMark.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (interactionMark != null)
        {
            interactionMark.SetActive(false);
        }
    }
}