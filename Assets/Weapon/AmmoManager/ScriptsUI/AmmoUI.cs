using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private AmmoManager _ammoManager;

    private TextMeshProUGUI _ammoText;

    private void Awake()
    {
        _ammoText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _ammoManager.onAmmoChanged += UpdateAmmoUI;
    }

    private void OnDisable()
    {
        _ammoManager.onAmmoChanged -= UpdateAmmoUI;
    }

    private void UpdateAmmoUI(int ammoInMag, int ammoReserve)
    {
        if (_ammoText != null)
        {
            _ammoText.text = $"{ammoInMag}/{ammoReserve}";
        }
    }
}
