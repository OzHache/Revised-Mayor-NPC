using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStatUI : MonoBehaviour
{
    [SerializeField]private Slider staminaSlider;
    private float staminaValue;
    [SerializeField]private Slider healthSlider;
    private float healthValue;
    // Start is called before the first frame update
    void Start()
    {
        
        
        PlayerController.StaminaUpdater += UpdateStaminaUIElement;
        PlayerController.HealthUpdater += UpdateHPUIElement;

        staminaSlider.maxValue = GameManager.GetGameManager().m_playerController.getMaxStamina;
        staminaSlider.value = staminaSlider.maxValue;
        staminaValue = staminaSlider.value;

        healthSlider.maxValue = GameManager.GetGameManager().m_playerController.GetMaxHealth();
        healthSlider.value = healthSlider.maxValue;
        healthValue = staminaSlider.value;


    }


    public void UpdateHPUIElement( )
    {
        
        var HP = GameManager.GetGameManager().m_playerController.GetHealth();
        HP = Mathf.Clamp(HP, 0, healthSlider.maxValue);
        healthSlider.value = HP;
        Canvas.ForceUpdateCanvases();
    }

    public void UpdateStaminaUIElement()
    {

        var stamina = GameManager.GetGameManager().m_playerController.getStamina;
        staminaValue = Mathf.Clamp(stamina, 0, staminaSlider.maxValue);
        staminaSlider.value = staminaValue;
        Canvas.ForceUpdateCanvases();
    }
}
