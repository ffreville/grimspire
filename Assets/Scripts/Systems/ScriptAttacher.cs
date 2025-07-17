using UnityEngine;

public class ScriptAttacher : MonoBehaviour
{
    [Header("Auto-Attachment Settings")]
    [SerializeField] private bool attachOnStart = true;
    [SerializeField] private bool debugMode = true;

    private void Start()
    {
        if (attachOnStart)
        {
            AttachAllScripts();
        }
    }

    [ContextMenu("Attach All Scripts")]
    public void AttachAllScripts()
    {
        if (debugMode)
        {
            Debug.Log("=== Attaching Scripts to GameObjects ===");
        }

        // Attacher les scripts aux managers
        AttachManagerScripts();

        // Attacher les scripts aux systèmes
        AttachSystemScripts();

        if (debugMode)
        {
            Debug.Log("=== Script Attachment Complete ===");
        }
    }

    private void AttachManagerScripts()
    {
        // CanvasManager
        AttachScriptToGameObject("CanvasManager", typeof(CanvasManager));

        // NewGameManager
        AttachScriptToGameObject("NewGameManager", typeof(NewGameManager));

        // MainMenuManager
        AttachScriptToGameObject("MainMenuManager", typeof(MainMenuManager));

        // GameIntegrationManager
        AttachScriptToGameObject("GameIntegrationManager", typeof(GameIntegrationManager));

        // ResourceManager
        AttachScriptToGameObject("ResourceManager", typeof(ResourceManager));

        // GameManager
        AttachScriptToGameObject("GameManager", typeof(GameManager));
    }

    private void AttachSystemScripts()
    {
        // RecruitmentSystem
        AttachScriptToGameObject("RecruitmentSystem", typeof(RecruitmentSystem));

        // PartyManagementSystem
        AttachScriptToGameObject("PartyManagementSystem", typeof(PartyManagementSystem));

        // CraftingSystem
        AttachScriptToGameObject("CraftingSystem", typeof(CraftingSystem));

        // ProgressionSystem
        AttachScriptToGameObject("ProgressionSystem", typeof(ProgressionSystem));

        // EquipmentEnhancementSystem
        AttachScriptToGameObject("EquipmentEnhancementSystem", typeof(EquipmentEnhancementSystem));

        // ItemDatabase
        AttachScriptToGameObject("ItemDatabase", typeof(ItemDatabase));

        // UIEventConfigurator
        AttachScriptToGameObject("UIEventConfigurator", typeof(UIEventConfigurator));

        // UIAutoInitializer
        AttachScriptToGameObject("UIAutoInitializer", typeof(UIAutoInitializer));
    }

    private void AttachScriptToGameObject(string gameObjectName, System.Type scriptType)
    {
        GameObject targetObject = GameObject.Find(gameObjectName);
        if (targetObject != null)
        {
            // Vérifier si le script existe déjà
            Component existingComponent = targetObject.GetComponent(scriptType);
            if (existingComponent == null)
            {
                // Attacher le script
                targetObject.AddComponent(scriptType);
                
                if (debugMode)
                {
                    Debug.Log($"✓ Attached {scriptType.Name} to {gameObjectName}");
                }
            }
            else
            {
                if (debugMode)
                {
                    Debug.Log($"○ {scriptType.Name} already attached to {gameObjectName}");
                }
            }
        }
        else
        {
            if (debugMode)
            {
                Debug.LogWarning($"✗ GameObject '{gameObjectName}' not found");
            }
        }
    }

    [ContextMenu("Check All Scripts")]
    public void CheckAllScripts()
    {
        Debug.Log("=== Checking Script Attachments ===");

        string[] managerNames = {
            "CanvasManager", "NewGameManager", "MainMenuManager", "GameIntegrationManager",
            "ResourceManager", "GameManager", "RecruitmentSystem", "PartyManagementSystem",
            "CraftingSystem", "ProgressionSystem", "EquipmentEnhancementSystem", "ItemDatabase",
            "UIEventConfigurator", "UIAutoInitializer"
        };

        foreach (string managerName in managerNames)
        {
            GameObject obj = GameObject.Find(managerName);
            if (obj != null)
            {
                Component[] components = obj.GetComponents<Component>();
                Debug.Log($"{managerName}: {components.Length} components");
                foreach (Component comp in components)
                {
                    if (comp != null)
                    {
                        Debug.Log($"  - {comp.GetType().Name}");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"✗ {managerName} not found");
            }
        }
    }

    [ContextMenu("Fix MainMenuManager")]
    public void FixMainMenuManager()
    {
        GameObject mainMenuManager = GameObject.Find("MainMenuManager");
        if (mainMenuManager != null)
        {
            // Supprimer tous les composants sauf Transform
            Component[] components = mainMenuManager.GetComponents<Component>();
            foreach (Component comp in components)
            {
                if (comp != null && comp.GetType() != typeof(Transform))
                {
                    DestroyImmediate(comp);
                }
            }

            // Attacher le bon script
            mainMenuManager.AddComponent<MainMenuManager>();
            
            Debug.Log("✓ MainMenuManager fixed!");
        }
        else
        {
            Debug.LogError("MainMenuManager GameObject not found!");
        }
    }

    [ContextMenu("Create Missing Managers")]
    public void CreateMissingManagers()
    {
        Debug.Log("=== Creating Missing Managers ===");

        // Liste des managers essentiels
        string[] essentialManagers = {
            "CanvasManager", "NewGameManager", "MainMenuManager", "GameIntegrationManager"
        };

        System.Type[] managerTypes = {
            typeof(CanvasManager), typeof(NewGameManager), typeof(MainMenuManager), typeof(GameIntegrationManager)
        };

        for (int i = 0; i < essentialManagers.Length; i++)
        {
            GameObject manager = GameObject.Find(essentialManagers[i]);
            if (manager == null)
            {
                // Créer le GameObject
                manager = new GameObject(essentialManagers[i]);
                
                // Attacher le script
                manager.AddComponent(managerTypes[i]);
                
                // Marquer comme persistent
                DontDestroyOnLoad(manager);
                
                Debug.Log($"✓ Created {essentialManagers[i]} with {managerTypes[i].Name}");
            }
            else
            {
                // Vérifier si le script est attaché
                Component comp = manager.GetComponent(managerTypes[i]);
                if (comp == null)
                {
                    manager.AddComponent(managerTypes[i]);
                    Debug.Log($"✓ Added {managerTypes[i].Name} to existing {essentialManagers[i]}");
                }
                else
                {
                    Debug.Log($"○ {essentialManagers[i]} already has {managerTypes[i].Name}");
                }
            }
        }

        Debug.Log("=== Manager Creation Complete ===");
    }

    [ContextMenu("Test New Game Button")]
    public void TestNewGameButton()
    {
        Debug.Log("=== Testing New Game Button ===");
        
        // Vérifier que MainMenuManager existe et a le script
        GameObject mainMenuManager = GameObject.Find("MainMenuManager");
        if (mainMenuManager != null)
        {
            MainMenuManager script = mainMenuManager.GetComponent<MainMenuManager>();
            if (script != null)
            {
                Debug.Log("✓ MainMenuManager script found");
                
                // Tester le bouton
                Button newGameButton = GameObject.Find("NewGameButton")?.GetComponent<Button>();
                if (newGameButton != null)
                {
                    Debug.Log("✓ NewGameButton found, testing click...");
                    
                    // Configurer l'événement si nécessaire
                    newGameButton.onClick.RemoveAllListeners();
                    newGameButton.onClick.AddListener(() => {
                        Debug.Log("New Game Button clicked via ScriptAttacher!");
                        script.ShowNewGamePanel();
                    });
                    
                    // Tester le clic
                    newGameButton.onClick.Invoke();
                    
                    Debug.Log("✓ New Game Button test complete!");
                }
                else
                {
                    Debug.LogError("✗ NewGameButton not found");
                }
            }
            else
            {
                Debug.LogError("✗ MainMenuManager script not found");
            }
        }
        else
        {
            Debug.LogError("✗ MainMenuManager GameObject not found");
        }
    }
}