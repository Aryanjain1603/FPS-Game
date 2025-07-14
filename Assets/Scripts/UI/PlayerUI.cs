using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image damageEffectOverlay;

    private Coroutine hideOverlayRoutine;

    private void Start()
    {
        // Only show the UI for the local player
        if (!photonView.IsMine)
        {
            GetComponent<Canvas>().enabled = false;
            return;
        }

        PlayerController.OnDamage += UpdateHealthUI;

        // Ensure the overlay starts disabled
        damageEffectOverlay.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        PlayerController.OnDamage -= UpdateHealthUI;
    }

    /// <summary>
    /// Called by the PlayerController when this player takes damage.
    /// </summary>
    private void UpdateHealthUI(int currentHealth)
    {
        healthText.text = currentHealth.ToString();

        // Flash the overlay any time health drops
        if (currentHealth < 100)   // adjust the threshold or remove if‑check as needed
            ShowDamageEffect();
    }

    /// <summary>
    /// Shows the red overlay and starts / restarts the 1‑second timer.
    /// </summary>
    private void ShowDamageEffect()
    {
        damageEffectOverlay.gameObject.SetActive(true);

        // If a previous timer is still running, cancel it
        if (hideOverlayRoutine != null)
            StopCoroutine(hideOverlayRoutine);

        hideOverlayRoutine = StartCoroutine(HideDamageEffectAfterDelay(1f));
    }

    /// <summary>
    /// Waits for the given delay, then hides the overlay.
    /// </summary>
    private IEnumerator HideDamageEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        damageEffectOverlay.gameObject.SetActive(false);
        hideOverlayRoutine = null;
    }
}