using System.Collections;
using UnityEngine;

public class WeaponController : Weapon
{
    [Header("References")]
    [SerializeField] private AimStateManager aimState;
    [SerializeField] private AmmunitionManager ammoManager;
    [SerializeField] private Camera cachedCamera;

    [Header("Projectiles")]
    [SerializeField] private GameObject bulletPrefab;
    //[SerializeField] private GameObject grenadePrefab;

    [Header("Effects")]
   // [SerializeField] private ParticleSystem muzzleParticles;

    [Header("Spawn")]
    [SerializeField] private Transform shootSpawn;

    [Header("Fire Rates")]
    [SerializeField] private float rifleFireRate = 0.15f;
    [SerializeField] private float pistolFireRate = 0.4f;
    [SerializeField] private float grenadeFireRate = 1.2f;

    /*[Header("Knife")]
    [SerializeField] private float knifeRange = 2f;
    [SerializeField] private int knifeDamage = 25;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Animator animator;*/

    [Header("Camera")]
    [SerializeField] private GameObject crosshairImage;
    [SerializeField] private GameObject normalCamera;
    [SerializeField] private GameObject pistolCamera;
    [SerializeField] private Transform camFollowPos_Pistol;
    [SerializeField] private Transform camFollowPos_Normal;

    private Coroutine shootingCoroutine;

    private Transform cameraTransform;

    private Ray ray;
    private RaycastHit hit;

    public WeaponType currentWeapon = WeaponType.Long;

    private void Awake()
    {
        cameraTransform = cachedCamera.transform;
    }

    private void Start()
    {
        if (ammoManager == null)
        {
            ammoManager = GetComponentInParent<AmmunitionManager>();
        }
    }

    private void Update()
    {
        HandleShooting();
        HandleAiming();

        if (currentWeapon != WeaponType.Knife)
        {
            AlignShootSpawnWithCamera();
        }
    }

    private void HandleShooting()
    {
        if (currentWeapon == WeaponType.Knife)
        {
            if (Input.GetMouseButtonDown(0))
            {
                KnifeAttack();
            }

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (shootingCoroutine == null)
            {
                shootingCoroutine =
                    StartCoroutine(AutoFireCoroutine());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (shootingCoroutine != null)
            {
                StopCoroutine(shootingCoroutine);
                shootingCoroutine = null;
            }
        }
    }

    IEnumerator AutoFireCoroutine()
    {
        WaitForSeconds waitRifle =
            new WaitForSeconds(rifleFireRate);

        WaitForSeconds waitPistol =
            new WaitForSeconds(pistolFireRate);

        WaitForSeconds waitGrenade =
            new WaitForSeconds(grenadeFireRate);

        while (true)
        {
            TryShoot();

            switch (currentWeapon)
            {
                case WeaponType.Short:
                    yield return waitPistol;
                    break;

                case WeaponType.GrenadeLauncher:
                    yield return waitGrenade;
                    break;

                default:
                    yield return waitRifle;
                    break;
            }
        }
    }

    private void HandleAiming()
    {
        if (currentWeapon == WeaponType.Knife)
        {
            crosshairImage.SetActive(false);
            return;
        }

        bool aiming = Input.GetMouseButton(1);

        crosshairImage.SetActive(aiming);

        pistolCamera.SetActive(aiming);
        normalCamera.SetActive(!aiming);

        if (aiming)
        {
            aimState.camFollowPos = camFollowPos_Pistol;
            aimState.ShootCamera();
        }
        else
        {
            aimState.camFollowPos = camFollowPos_Normal;
            aimState.NormalCamera();
        }
    }

    private void AlignShootSpawnWithCamera()
    {
        ray.origin = cameraTransform.position;
        ray.direction = cameraTransform.forward;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, 1000f))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint =
                ray.origin + (ray.direction * 1000f);
        }

        shootSpawn.forward =
            (targetPoint - shootSpawn.position).normalized;
    }

    private void TryShoot()
    {
        if (!ammoManager.UseAmmo(1))
            return;

        switch (currentWeapon)
        {
            case WeaponType.Long:
            case WeaponType.Short:
                SpawnBullet();
                break;

            case WeaponType.GrenadeLauncher:
                SpawnGrenade();
                break;
        }

       /* if (muzzleParticles != null)
        {
            muzzleParticles.Play();
        }*/
    }

    private void SpawnBullet()
    {
        Instantiate(
            bulletPrefab,
            shootSpawn.position,
            shootSpawn.rotation
        );
    }

    private void SpawnGrenade()
    {
      /*  Instantiate(
            grenadePrefab,
            shootSpawn.position,
            shootSpawn.rotation
        );*/
    }

    private void KnifeAttack()
    {
        /*if (animator != null)
        {
            animator.SetTrigger("Knife");
        }*/

        ray.origin = cameraTransform.position;
        ray.direction = cameraTransform.forward;

       /* if (Physics.Raycast(
            ray,
            out hit,
            knifeRange,
            enemyLayer,
            QueryTriggerInteraction.Ignore))
        {
            EnemyHealth enemy;

            if (hit.collider.TryGetComponent(
                out enemy))
            {
                enemy.TakeDamage(knifeDamage);
            }
        }*/
    }

    public void SetWeaponType(WeaponType weapon)
    {
        currentWeapon = weapon;
    }
}