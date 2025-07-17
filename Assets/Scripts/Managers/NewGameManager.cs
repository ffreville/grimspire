using System.Collections.Generic;
using UnityEngine;

public class NewGameManager : MonoBehaviour
{
    private static NewGameManager instance;
    public static NewGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<NewGameManager>();
                if (instance == null)
                {
                    GameObject newGameManagerObject = new GameObject("NewGameManager");
                    instance = newGameManagerObject.AddComponent<NewGameManager>();
                    DontDestroyOnLoad(newGameManagerObject);
                }
            }
            return instance;
        }
    }

    [Header("New Game Settings")]
    [SerializeField] private string defaultPlayerName = "Seigneur";
    [SerializeField] private string defaultCityName = "Grimspire";
    [SerializeField] private int startingGold = 1000;
    [SerializeField] private int startingPopulation = 50;
    [SerializeField] private int startingAdventurers = 3;

    [Header("Starting Resources")]
    [SerializeField] private int startingIron = 100;
    [SerializeField] private int startingWood = 100;
    [SerializeField] private int startingStone = 50;
    [SerializeField] private int startingMagicCrystal = 20;
    [SerializeField] private int startingReputation = 10;

    [Header("Starting Buildings")]
    [SerializeField] private List<string> startingBuildings = new List<string>
    {
        "TownHall",
        "Barracks",
        "Inn",
        "Marketplace"
    };

    public System.Action OnNewGameStarted;
    public System.Action OnGameInitialized;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void StartNewGame()
    {
        Debug.Log("Starting new game...");
        
        // Show loading screen
        CanvasManager.Instance.ShowLoading();
        
        // Initialize game systems
        InitializeGameSystems();
        
        // Setup starting resources
        SetupStartingResources();
        
        // Setup starting buildings
        SetupStartingBuildings();
        
        // Generate starting adventurers
        GenerateStartingAdventurers();
        
        // Initialize crafting system with starting materials
        InitializeCraftingSystem();
        
        // Initialize progression system
        InitializeProgressionSystem();
        
        // Initialize equipment system
        InitializeEquipmentSystem();
        
        // Show gameplay canvas
        CanvasManager.Instance.HideLoading();
        CanvasManager.Instance.ShowGameplay();
        
        // Fire events
        OnNewGameStarted?.Invoke();
        OnGameInitialized?.Invoke();
        
        Debug.Log("New game started successfully!");
    }

    private void InitializeGameSystems()
    {
        // Initialize GameManager
        if (GameManager.Instance == null)
        {
            GameObject gameManagerObject = new GameObject("GameManager");
            gameManagerObject.AddComponent<GameManager>();
        }
        
        // Initialize ResourceManager
        if (ResourceManager.Instance == null)
        {
            GameObject resourceManagerObject = new GameObject("ResourceManager");
            resourceManagerObject.AddComponent<ResourceManager>();
        }
        
        // Initialize other core systems
        EnsureSystemExists<RecruitmentSystem>("RecruitmentSystem");
        EnsureSystemExists<PartyManagementSystem>("PartyManagementSystem");
        EnsureSystemExists<CraftingSystem>("CraftingSystem");
        EnsureSystemExists<ProgressionSystem>("ProgressionSystem");
        EnsureSystemExists<EquipmentEnhancementSystem>("EquipmentEnhancementSystem");
        EnsureSystemExists<ItemDatabase>("ItemDatabase");
        
        Debug.Log("Game systems initialized");
    }

    private void EnsureSystemExists<T>(string objectName) where T : MonoBehaviour
    {
        if (FindObjectOfType<T>() == null)
        {
            GameObject systemObject = new GameObject(objectName);
            systemObject.AddComponent<T>();
            DontDestroyOnLoad(systemObject);
        }
    }

    private void SetupStartingResources()
    {
        // Setup basic resources
        ResourceManager.Instance.AddResource(Resource.ResourceType.Gold, startingGold);
        ResourceManager.Instance.AddResource(Resource.ResourceType.Population, startingPopulation);
        ResourceManager.Instance.AddResource(Resource.ResourceType.Iron, startingIron);
        ResourceManager.Instance.AddResource(Resource.ResourceType.Wood, startingWood);
        ResourceManager.Instance.AddResource(Resource.ResourceType.Stone, startingStone);
        ResourceManager.Instance.AddResource(Resource.ResourceType.MagicCrystal, startingMagicCrystal);
        ResourceManager.Instance.AddResource(Resource.ResourceType.Reputation, startingReputation);

        Debug.Log($"Starting resources set - Gold: {startingGold}, Population: {startingPopulation}");
    }

    private void SetupStartingBuildings()
    {
        // This would interface with the building system when implemented
        // For now, just log the buildings that should be available
        Debug.Log($"Starting buildings: {string.Join(", ", startingBuildings)}");
        
        // TODO: Interface with ConstructionSystem to create starting buildings
        // foreach (string buildingId in startingBuildings)
        // {
        //     ConstructionSystem.Instance.CreateStartingBuilding(buildingId);
        // }
    }

    private void GenerateStartingAdventurers()
    {
        if (RecruitmentSystem.Instance == null)
        {
            Debug.LogWarning("RecruitmentSystem not found! Cannot generate starting adventurers.");
            return;
        }

        // Generate starting adventurers with different classes
        Adventurer.AdventurerClass[] startingClasses = {
            Adventurer.AdventurerClass.Warrior,
            Adventurer.AdventurerClass.Mage,
            Adventurer.AdventurerClass.Rogue
        };

        string[] startingNames = {
            "Gareth le Brave",
            "Lyanna la Sage",
            "Kael l'Ombre"
        };

        for (int i = 0; i < startingAdventurers && i < startingClasses.Length; i++)
        {
            try
            {
                Adventurer newAdventurer = RecruitmentSystem.Instance.GenerateSpecificAdventurer(startingClasses[i], Adventurer.AdventurerRarity.Common);
                if (newAdventurer != null && i < startingNames.Length)
                {
                    // Note: Adventurer properties are read-only, set during construction
                    // The generated adventurer already has appropriate starting values
                    newAdventurer.SetStatus(Adventurer.AdventurerStatus.Available);
                    
                    // Give starting equipment
                    GiveStartingEquipment(newAdventurer);
                    
                    Debug.Log($"Generated starting adventurer: {newAdventurer.Name} ({newAdventurer.Class})");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error generating starting adventurer: {e.Message}");
            }
        }

        Debug.Log($"Generated {startingAdventurers} starting adventurers");
    }

    private void GiveStartingEquipment(Adventurer adventurer)
    {
        if (ItemDatabase.Instance == null)
        {
            Debug.LogWarning("ItemDatabase not found! Cannot give starting equipment.");
            return;
        }

        try
        {
            // Give class-appropriate starting equipment
            switch (adventurer.Class)
            {
                case Adventurer.AdventurerClass.Warrior:
                    var warriorWeapon = ItemDatabase.Instance.GenerateRandomEquipment(
                        Equipment.EquipmentType.Weapon, 
                        Equipment.EquipmentRarity.Common, 
                        1
                    );
                    var warriorArmor = ItemDatabase.Instance.GenerateRandomEquipment(
                        Equipment.EquipmentType.Armor, 
                        Equipment.EquipmentRarity.Common, 
                        1
                    );
                    if (warriorWeapon != null) adventurer.EquipWeapon(warriorWeapon);
                    if (warriorArmor != null) adventurer.EquipArmor(warriorArmor);
                    break;

                case Adventurer.AdventurerClass.Mage:
                    var mageWeapon = ItemDatabase.Instance.GenerateRandomEquipment(
                        Equipment.EquipmentType.Weapon, 
                        Equipment.EquipmentRarity.Common, 
                        1
                    );
                    var mageArmor = ItemDatabase.Instance.GenerateRandomEquipment(
                        Equipment.EquipmentType.Armor, 
                        Equipment.EquipmentRarity.Common, 
                        1
                    );
                    if (mageWeapon != null) adventurer.EquipWeapon(mageWeapon);
                    if (mageArmor != null) adventurer.EquipArmor(mageArmor);
                    break;

                case Adventurer.AdventurerClass.Rogue:
                    var rogueWeapon = ItemDatabase.Instance.GenerateRandomEquipment(
                        Equipment.EquipmentType.Weapon, 
                        Equipment.EquipmentRarity.Common, 
                        1
                    );
                    var rogueArmor = ItemDatabase.Instance.GenerateRandomEquipment(
                        Equipment.EquipmentType.Armor, 
                        Equipment.EquipmentRarity.Common, 
                        1
                    );
                    if (rogueWeapon != null) adventurer.EquipWeapon(rogueWeapon);
                    if (rogueArmor != null) adventurer.EquipArmor(rogueArmor);
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error giving starting equipment to {adventurer.Name}: {e.Message}");
        }
    }

    private void InitializeCraftingSystem()
    {
        if (CraftingSystem.Instance == null)
        {
            Debug.LogWarning("CraftingSystem not found! Cannot initialize crafting.");
            return;
        }

        try
        {
            // Add starting materials to crafting system
            CraftingSystem.Instance.AddMaterial("Iron", startingIron);
            CraftingSystem.Instance.AddMaterial("Wood", startingWood);
            CraftingSystem.Instance.AddMaterial("Stone", startingStone);
            CraftingSystem.Instance.AddMaterial("MagicCrystal", startingMagicCrystal);
            
            Debug.Log("Crafting system initialized with starting materials");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error initializing crafting system: {e.Message}");
        }
    }

    private void InitializeProgressionSystem()
    {
        if (ProgressionSystem.Instance == null)
        {
            Debug.LogWarning("ProgressionSystem not found! Cannot initialize progression.");
            return;
        }

        try
        {
            // Initialize talent trees for all classes
            // InitializeTalentTrees is private - it's called automatically during ProgressionSystem initialization
            
            Debug.Log("Progression system initialized with talent trees");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error initializing progression system: {e.Message}");
        }
    }

    private void InitializeEquipmentSystem()
    {
        if (EquipmentEnhancementSystem.Instance == null)
        {
            Debug.LogWarning("EquipmentEnhancementSystem not found! Cannot initialize equipment enhancement.");
            return;
        }

        try
        {
            // Initialize gem types and enhancement materials
            // InitializeGemTypes method doesn't exist - enhancement system initializes automatically
            
            Debug.Log("Equipment enhancement system initialized");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error initializing equipment enhancement system: {e.Message}");
        }
    }

    public void CreateQuickStartGame()
    {
        // Quick start for testing purposes
        Debug.Log("Creating quick start game...");
        
        // Use default settings
        StartNewGame();
    }

    public void LoadGame(string saveName)
    {
        Debug.Log($"Loading game: {saveName}");
        
        // Show loading screen
        CanvasManager.Instance.ShowLoading();
        
        // TODO: Interface with SaveSystem to load game data
        // For now, just start a new game
        StartNewGame();
    }

    public bool HasSaveFiles()
    {
        // TODO: Check if save files exist
        return false;
    }

    public List<string> GetSaveFiles()
    {
        // TODO: Get list of save files
        return new List<string>();
    }

    public void DeleteSaveFile(string saveName)
    {
        // TODO: Delete save file
        Debug.Log($"Deleting save file: {saveName}");
    }

    public void SetPlayerName(string playerName)
    {
        defaultPlayerName = playerName;
    }

    public void SetCityName(string cityName)
    {
        defaultCityName = cityName;
    }

    public string GetPlayerName()
    {
        return defaultPlayerName;
    }

    public string GetCityName()
    {
        return defaultCityName;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}