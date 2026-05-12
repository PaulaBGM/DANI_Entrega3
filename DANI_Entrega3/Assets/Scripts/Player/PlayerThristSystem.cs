using System;
using UnityEngine;

public class PlayerThristSystem : MonoBehaviour
{
    [SerializeField] private EventManagerSO eventManager;
    
    [SerializeField] private float maxThrist = 100f;
    [SerializeField] private float thristDecreaseRate = 0.3f;
    [SerializeField] private float healthDamageDehydrated = 5f;
    
    private PlayerMain playerMain;
    
    private float currentThrist;

    private void Start()
    {
        currentThrist = maxThrist;
        playerMain = GetComponentInParent<PlayerMain>();
        
        UpdateUI();
    }

    private void Update()
    {
        currentThrist -= Time.deltaTime * thristDecreaseRate;
        currentThrist = Mathf.Clamp(currentThrist, 0f, maxThrist);

        if (currentThrist <= 0f)
        {
            currentThrist = 0;
            playerMain.NotifiesDehydrated(healthDamageDehydrated * Time.deltaTime);
        }
        
        UpdateUI();
    }
    
    public void Drink(float amount)
    {
        currentThrist = Mathf.Min(currentThrist + amount, maxThrist);
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        // 🔹 Opcional: notificar a la UI si usas tu EventManagerSO
        if (eventManager != null)
            eventManager.PlayerNotifiesThirstChanged(currentThrist, maxThrist);
    }

    public float GetThirstPercent() => currentThrist / maxThrist;
}
