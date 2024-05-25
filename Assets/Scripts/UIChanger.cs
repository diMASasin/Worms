using Configs;
using UnityEngine;
using UnityEngine.UI;

public class UIChanger : MonoBehaviour
{
    [SerializeField] private UIConfig _config;

    [SerializeField] private Image _weaponSelectorImage;
    [SerializeField] private Image _turnTimerImage;
    [SerializeField] private Image _globalTimerImage;
    [SerializeField] private Image _windViewImage;
    
    private void OnValidate()
    {
        
    }
    
    [ContextMenu("Update")]
    public void UpdateUI()
    {
        if(_config.WeaponSelectorSprite != null) _weaponSelectorImage.sprite = _config.WeaponSelectorSprite;
        if(_config.TurnTimerSprite != null) _turnTimerImage.sprite = _config.TurnTimerSprite;
        if(_config.GlobalTimerSprite != null) _globalTimerImage.sprite = _config.GlobalTimerSprite;
        if(_config.WindViewSprite != null) _windViewImage.sprite = _config.WindViewSprite;
    }
}