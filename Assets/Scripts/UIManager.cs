using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    // Singleton simplu pentru a fi accesat ușor din PlayerController
    public static UIManager Instance;

    [Header("--- UI Arme & Muniție ---")]
    public TextMeshProUGUI weaponText;
    public TextMeshProUGUI ammoText;
    public Image weaponIcon; // Opțional: dacă ai iconițe pentru arme

    [Header("--- UI Avertizări ---")]
    public GameObject meleeWarningPanel; // Un panel roșu semi-transparent sau un text mare
    public TextMeshProUGUI outOfAmmoText;

    [Header("--- Culori Feedback ---")]
    public Color normalColor = Color.white;
    public Color alertColor = Color.red;

    private Coroutine flashCoroutine;

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(this); 
        }
    }

    void Start()
    {
        if (meleeWarningPanel != null) meleeWarningPanel.SetActive(false);
    }

    // Funcție apelată din PlayerController ori de câte ori se schimbă arma sau se trage
    public void UpdateWeaponUI(PlayerController.WeaponType currentWeapon, int rifleAmmo, int shotgunAmmo)
    {

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        if (outOfAmmoText != null) outOfAmmoText.gameObject.SetActive(false);

        switch (currentWeapon)
        {
            case PlayerController.WeaponType.Rifle:
                if(weaponText != null) weaponText.text = "WEAPON: RIFLE";
                if(ammoText != null) { ammoText.text = rifleAmmo.ToString(); ammoText.color = normalColor; }
                if(meleeWarningPanel != null) meleeWarningPanel.SetActive(false);
                break;

            case PlayerController.WeaponType.Shotgun:
                if(weaponText != null) weaponText.text = "WEAPON: SHOTGUN";
                if(ammoText != null) { ammoText.text = shotgunAmmo.ToString(); ammoText.color = normalColor; }
                if(meleeWarningPanel != null) meleeWarningPanel.SetActive(false);
                break;

            case PlayerController.WeaponType.Melee:
                if(weaponText != null) weaponText.text = "WEAPON: SWORD";
                if(ammoText != null) { ammoText.text = "∞"; ammoText.color = alertColor; }
                
                if (rifleAmmo <= 0 && shotgunAmmo <= 0)
                {
                    if(meleeWarningPanel != null) meleeWarningPanel.SetActive(true);
                    flashCoroutine = StartCoroutine(FlashOutOfAmmoText());
                }
                else
                {
                    if(meleeWarningPanel != null) meleeWarningPanel.SetActive(false);
                }
                break;
        }
    }

    // Efect vizual simplu pentru a atrage atenția jucătorului
    private IEnumerator FlashOutOfAmmoText()
    {
        if (outOfAmmoText == null) yield break;

        outOfAmmoText.text = "OUT OF AMMO! MELEE ACTIVE!";
        outOfAmmoText.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2f); // Afișăm mesajul 2 secunde
        
        outOfAmmoText.gameObject.SetActive(false);
    }
}