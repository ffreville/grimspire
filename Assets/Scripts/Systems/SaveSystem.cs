using System;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    
    [Header("Save Settings")]
    [SerializeField] private string saveFileName = "grimspire_save.json";
    [SerializeField] private bool useEncryption = false;
    
    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);
    
    public static event Action<GameData> OnGameLoaded;
    public static event Action OnGameSaved;
    public static event Action<string> OnSaveError;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame(GameData gameData)
    {
        try
        {
            string jsonData = JsonUtility.ToJson(gameData, true);
            
            if (useEncryption)
            {
                jsonData = EncryptData(jsonData);
            }
            
            File.WriteAllText(SavePath, jsonData);
            
            Debug.Log($"Game saved successfully to: {SavePath}");
            OnGameSaved?.Invoke();
        }
        catch (Exception e)
        {
            string error = $"Failed to save game: {e.Message}";
            Debug.LogError(error);
            OnSaveError?.Invoke(error);
        }
    }

    public GameData LoadGame()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                Debug.LogWarning("No save file found. Creating new game data.");
                return CreateNewGameData();
            }
            
            string jsonData = File.ReadAllText(SavePath);
            
            if (useEncryption)
            {
                jsonData = DecryptData(jsonData);
            }
            
            GameData gameData = JsonUtility.FromJson<GameData>(jsonData);
            
            Debug.Log("Game loaded successfully");
            OnGameLoaded?.Invoke(gameData);
            
            return gameData;
        }
        catch (Exception e)
        {
            string error = $"Failed to load game: {e.Message}";
            Debug.LogError(error);
            OnSaveError?.Invoke(error);
            
            return CreateNewGameData();
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(SavePath);
    }

    public void DeleteSaveFile()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("Save file deleted successfully");
            }
        }
        catch (Exception e)
        {
            string error = $"Failed to delete save file: {e.Message}";
            Debug.LogError(error);
            OnSaveError?.Invoke(error);
        }
    }

    private GameData CreateNewGameData()
    {
        GameData newGameData = new GameData
        {
            cityName = "Grimspire",
            lastSaveTime = DateTime.Now.ToBinary()
        };
        newGameData.InitializeGameVersion();
        return newGameData;
    }

    private string EncryptData(string data)
    {
        // Simple XOR encryption - replace with proper encryption for production
        string key = "GrimspireKey2024";
        string encrypted = "";
        
        for (int i = 0; i < data.Length; i++)
        {
            encrypted += (char)(data[i] ^ key[i % key.Length]);
        }
        
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(encrypted));
    }

    private string DecryptData(string encryptedData)
    {
        // Simple XOR decryption - replace with proper decryption for production
        string key = "GrimspireKey2024";
        string data = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedData));
        string decrypted = "";
        
        for (int i = 0; i < data.Length; i++)
        {
            decrypted += (char)(data[i] ^ key[i % key.Length]);
        }
        
        return decrypted;
    }

    public void AutoSave(GameData gameData)
    {
        // Auto-save in background
        SaveGame(gameData);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}