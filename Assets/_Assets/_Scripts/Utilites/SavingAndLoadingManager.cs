using UnityEngine;
namespace GamerWolf.Utils{

    public class SavingAndLoadingManager : MonoBehaviour{
        
        [SerializeField] private SaveData saveData;
        public static SavingAndLoadingManager instance {get;private set;}
        
        
        private void Awake(){
            if(instance == null){
                instance = this;
            }else{
                Destroy(instance.gameObject);
            }
            DontDestroyOnLoad(gameObject);
            LoadGame();
        }
        [ContextMenu("SAVE GAME")]
        public void SaveGame(){
            saveData.gameData.Save();
        }
        [ContextMenu("LOAD GAME")]
        public void LoadGame(){
            saveData.gameData.Load();
        }
        private void OnApplicationPause(bool pause){
            if(pause){
                SaveGame();
            }
        }
        
        private void OnApplicationQuit(){
            SaveGame();
            
        }

    }
    [System.Serializable]
    public struct SaveData{
        public  GameDataSO gameData;
        // public RewardsSO[] rewardsSos;
        // public LevelDataSO[] levelDatas;
        // public SettingsSO settingsSO;
        
    }

}