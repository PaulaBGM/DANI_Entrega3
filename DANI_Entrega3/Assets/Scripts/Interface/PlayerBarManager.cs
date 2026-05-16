using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBarManager : MonoBehaviour
{
    [SerializeField] private EventManagerSO _eventManagerSo;

    [SerializeField] private Image healthBar;
    [SerializeField] private Image thristBar;

    [SerializeField] private Image ammoGunBar;
    [SerializeField] private Image ammoGrenadeBar;
    [SerializeField] private TextMeshProUGUI ammoGunText;
    [SerializeField] private TextMeshProUGUI ammoGrenadeText;

    private void OnEnable()
    {
        _eventManagerSo.OnPlayerDamaged += UpdateHealthBar;
        _eventManagerSo.OnPlayerHealed += UpdateHealthBar;

        _eventManagerSo.OnPlayerUseAmmoGun += UpdateGunAmmo;
        _eventManagerSo.OnAmmoGunGetted += UpdateGunAmmo;
        _eventManagerSo.OnPlayerUseGrenade += UpdateGrenadeAmmo;
        _eventManagerSo.OnAmmoGrenadeGetted += UpdateGrenadeAmmo;
        
        _eventManagerSo.OnPlayerThirstChanged += UpdateThirstBar;
    }

    private void UpdateThirstBar(float current, float max) => thristBar.fillAmount = current / max;

    private void UpdateHealthBar(float current, float max) => healthBar.fillAmount = current / max;

    private void UpdateGunAmmo(float current, float max) => UpdateAmmoBar(ammoGunBar, ammoGunText, current, max);

    private void UpdateGrenadeAmmo(float current, float max) => UpdateAmmoBar(ammoGrenadeBar, ammoGrenadeText, current, max);

    private void UpdateAmmoBar(Image bar, TextMeshProUGUI text, float current, float max)
    {
        bar.fillAmount = current / max;
        text.text = current.ToString("00");
    }
    
    private void OnDisable()
    {
        _eventManagerSo.OnPlayerDamaged -= UpdateHealthBar;
        _eventManagerSo.OnPlayerHealed -= UpdateHealthBar;

        _eventManagerSo.OnPlayerUseAmmoGun -= UpdateGunAmmo;
        _eventManagerSo.OnAmmoGunGetted -= UpdateGunAmmo;
        _eventManagerSo.OnPlayerUseGrenade -= UpdateGrenadeAmmo;
        _eventManagerSo.OnAmmoGrenadeGetted -= UpdateGrenadeAmmo;
        
        _eventManagerSo.OnPlayerThirstChanged -= UpdateThirstBar;
    }
}