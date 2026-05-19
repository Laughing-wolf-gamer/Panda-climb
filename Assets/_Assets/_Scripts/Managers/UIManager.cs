using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class UIManager : MonoBehaviour {
    
    [Header("Game View")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image fuelAmountBar;
    [SerializeField] private TextMeshProUGUI fuelAmountText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI[] currentDistacnceTextsArray,highScoreDistanceTexts;
    [SerializeField] private TextMeshProUGUI[] collectedCoinsCount,TotalCoins;
    [SerializeField] private GameDataSO gameDataSO;
    [Header("Settings ")]
    [SerializeField] private GameObject settingView;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private TextMeshProUGUI soundViewText,musicViewText;
    [SerializeField] private Toggle soundToggle,musicToggle;
    private bool isSettingsOn;
    public static UIManager current;
    private void Awake(){

        current = this;
    }
    private void Start(){
        // CheckSettings();
        // CheckAudioStatus();
        // soundToggle.onValueChanged.AddListener((value) =>{
        //     gameDataSO.ToggelSound(value);
        //     CheckAudioStatus();
        // });
        // musicToggle.onValueChanged.AddListener((value) =>{
        //     gameDataSO.ToggelMusic(value);
        //     CheckAudioStatus();
        // });
    }
    public void SetcurrentDistance(float currentDistance){
        foreach(TextMeshProUGUI texts in currentDistacnceTextsArray){
            texts.SetText(string.Concat(currentDistance.ToString("0"),"m"));
        }
    }
    public void SetCoinsAmount(int amount){
        foreach(TextMeshProUGUI texts in collectedCoinsCount){
            texts.SetText(string.Concat(amount,"$"));
        }
    }
    public void SetLongestDistance(){
        foreach(TextMeshProUGUI texts in highScoreDistanceTexts){
            texts.SetText(string.Concat(gameDataSO.GetLongestDistance().ToString("0"),"m"));
        }
    }
    public void SetSpeed(float currentSpeed){
        if(speedText != null){
            speedText.SetText(string.Concat(currentSpeed.ToString("F2"),"Km/s"));
        }
    }
    // public void ToggleMusic(){
    //     CheckAudioStatus();
    // }
    // public void ToggleSound(){
    //     CheckAudioStatus();
    // }
    private void CheckAudioStatus(){
        if(musicToggle != null){
            musicToggle.isOn = gameDataSO.GetMusicState();
        }
        if(soundToggle != null){
            soundToggle.isOn = gameDataSO.GetSoundState();
        }
    }
    public void SetFuelAmount(float currentAmount){
        float percentage = currentAmount * 100f;
        fuelAmountText.SetText(string.Concat(percentage.ToString("0"),"%"));
        fuelAmountBar.fillAmount = currentAmount;
    }
    public void SettingsViewToggle(){
        isSettingsOn = !isSettingsOn;
        CheckSettings();
    }
    private void CheckSettings(){
        settingView.SetActive(isSettingsOn);
    }
   
}
