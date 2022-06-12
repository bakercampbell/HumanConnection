/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDPlayerController : MonoBehaviour
{

    [Header("Player Settings")]
        [SerializeField]
        private float movementVelocity = 3f;
    
    [Header("Bullet Settings")]

        [SerializeField, Tooltip("Bullet Prefab to Shoot")]
        private GameObject bullet;
        [SerializeField, Tooltip("Bullet Direction and Position to Shoot in")]
        private Transform bulletDirection;

    #region Private Variables
        private TDActions controls;
        private bool canShoot = true;
        private Camera main;
        
    #endregion Private Variables

    private void Awake() {
        // Initialize input actions
        controls = new TDActions();
    }

    private void OnEnable() {
        // Enable input
        controls.Enable();
    }

    private void OnDisable() {
        // Disable input
        controls.Disable();
    }

    void Start()
    {
        main = Camera.main;
        // Event - When player presses shoot button call PlayerShoot()
        controls.Player.Shoot.performed += _ => PlayerShoot();
    }

    /// <summary>
    /// Shoot a bullet in the current direction + position
    /// </summary>
    private void PlayerShoot() {
        if (!canShoot) return;

        Vector2 mousePosition = controls.Player.MousePosition.ReadValue<Vector2>();
        mousePosition = main.ScreenToWorldPoint(mousePosition);
        GameObject g = Instantiate(bullet, bulletDirection.position, bulletDirection.rotation);
        g.SetActive(true);
        StartCoroutine(CanShoot());
    }

    /// <summary>
    /// Timer to press next bullet
    /// </summary>
    IEnumerator CanShoot() {
        canShoot = false;
        yield return new WaitForSeconds(.5f);
        canShoot = true;
    }

    void Update()
    {
        // Player Rotation Controls
        Vector2 mouseScreenPosition = controls.Player.MousePosition.ReadValue<Vector2>();
        Vector3 mouseWorldPosition = main.ScreenToWorldPoint(mouseScreenPosition);
        Vector3 targetDirection = mouseWorldPosition - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        // Player Movement Controls
        Vector3 movement = controls.Player.Movement.ReadValue<Vector2>() * movementVelocity;
        transform.position += movement * Time.deltaTime;
    }
}
