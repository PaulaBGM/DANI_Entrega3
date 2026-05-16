using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerDamageFlash : MonoBehaviour
{
    [SerializeField] private Image damageImage;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float maxAlpha = 0.5f;
    
    private Coroutine flashCoroutine;
    
    [SerializeField] private EventManagerSO _eventManagerSo;

    private void Awake()
    {
        damageImage =  GetComponent<Image>();
    }

    private void OnEnable()
    {
        _eventManagerSo.OnPlayerDamaged += Flash;

    }

    private void Flash(float arg1, float arg2)
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        //Subimos el alpha hasta el maxAlpha
        float elapsed = 0f;
        Color color = damageImage.color;

        while (elapsed < flashDuration / 2)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0, maxAlpha, elapsed / (flashDuration / 2));
            
            damageImage.color = color;
            yield return null;
        }
        
        //Bajamos el alpha de nuevo
        elapsed = 0f;
        while (elapsed < flashDuration / 2)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(maxAlpha, 0, elapsed / (flashDuration / 2));
            damageImage.color = color;
            yield return null;
        }

        color.a = 0;
        damageImage.color = color;
        flashCoroutine = null;
    }

    private void OnDisable()
    {
        _eventManagerSo.OnPlayerDamaged -= Flash;
    }
}
