using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class WeaponChanger : MonoBehaviour
{
    [SerializeField] private Weapon[] _weapons;

    private Weapon currentWeapon;
    private int currentIndex;

    private void Awake()
    {
        currentIndex = 0;
        currentWeapon = _weapons[currentIndex];
    }

    private void OnEnable()
    {
        /*InputController.Instance.OnScroll += ChangeWeaponWScroll;
        InputController.Instance.OnNewWeapon += ChangeWeaponWNum;
        InputController.Instance.OnUseWeapon += WeaponUsed;*/
    }

    private void WeaponUsed()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsDialogueActive) return;

        //currentWeapon.OnUse();
    }

    private void ChangeWeaponWNum(int index)
    {
        if (UIManager.Instance != null && UIManager.Instance.IsDialogueActive) return;

        currentWeapon.gameObject.SetActive(false); //Desactivamos el arma actual
        currentIndex = index; //Se actualiza el índice

        currentWeapon = _weapons[currentIndex];
        currentWeapon.gameObject.SetActive(true);
    }

    private void ChangeWeaponWScroll(float direction)
    {
        if (UIManager.Instance != null && UIManager.Instance.IsDialogueActive) return;

        currentWeapon.gameObject.SetActive(false); //Desactivamos el arma actual

        if (direction > 0)
        {
            currentIndex = (currentIndex + 1) % _weapons.Length;
        }
        else
        {
            currentIndex = (currentIndex - 1 + _weapons.Length) % _weapons.Length; //Evitamos salirnos del array.
        }

        //Para actualizar, cambiamos el currentWeapon al nuevo y lo activamos.
        currentWeapon = _weapons[currentIndex];
        currentWeapon.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        /*InputController.Instance.OnScroll -= ChangeWeaponWScroll;
        InputController.Instance.OnNewWeapon -= ChangeWeaponWNum;
        InputController.Instance.OnUseWeapon -= WeaponUsed;*/
    }
}
