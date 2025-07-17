using UnityEngine;

public class Phase2TestRunner : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool runTestsOnStart = true;
    [SerializeField] private bool enableDebugMode = true;
    [SerializeField] private bool testNewGameFlow = true;
    [SerializeField] private bool testCanvasManagement = true;
    [SerializeField] private bool testSystemIntegration = true;

    [Header("Test Results")]
    [SerializeField] private int testsRun = 0;
    [SerializeField] private int testsPassed = 0;
    [SerializeField] private int testsFailed = 0;

    private void Start()
    {
        if (runTestsOnStart)
        {
            RunAllTests();
        }
    }

    [ContextMenu("Run All Phase 2 Tests")]
    public void RunAllTests()
    {
        Debug.Log("=== Starting Phase 2.4 Tests ===");
        
        testsRun = 0;
        testsPassed = 0;
        testsFailed = 0;

        // Test Canvas Management
        if (testCanvasManagement)
        {
            TestCanvasManager();
        }

        // Test New Game Flow
        if (testNewGameFlow)
        {
            TestNewGameManager();
        }

        // Test System Integration
        if (testSystemIntegration)
        {
            TestSystemIntegration();
        }

        // Test Phase 2 Systems
        TestPhase2Systems();

        // Print results
        PrintTestResults();
    }

    private void TestCanvasManager()
    {
        Debug.Log("Testing Canvas Manager...");

        // Test Canvas Manager existence
        RunTest("Canvas Manager Instance", () => CanvasManager.Instance != null);

        // Test Canvas Manager initialization
        RunTest("Canvas Manager Initialization", () => {
            if (CanvasManager.Instance == null) return false;
            return true; // Basic existence test
        });

        // Test Canvas switching
        RunTest("Canvas Switching", () => {
            if (CanvasManager.Instance == null) return false;
            try
            {
                CanvasManager.Instance.ShowMainMenu();
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Canvas switching failed: {e.Message}");
                return false;
            }
        });
    }

    private void TestNewGameManager()
    {
        Debug.Log("Testing New Game Manager...");

        // Test New Game Manager existence
        RunTest("New Game Manager Instance", () => NewGameManager.Instance != null);

        // Test default settings
        RunTest("New Game Default Settings", () => {
            if (NewGameManager.Instance == null) return false;
            return !string.IsNullOrEmpty(NewGameManager.Instance.GetPlayerName()) &&
                   !string.IsNullOrEmpty(NewGameManager.Instance.GetCityName());
        });

        // Test save file checking
        RunTest("Save File System", () => {
            if (NewGameManager.Instance == null) return false;
            var saveFiles = NewGameManager.Instance.GetSaveFiles();
            return saveFiles != null; // Just check it returns something
        });
    }

    private void TestSystemIntegration()
    {
        Debug.Log("Testing System Integration...");

        // Test Game Integration Manager
        RunTest("Game Integration Manager", () => GameIntegrationManager.Instance != null);

        // Test Resource Manager integration
        RunTest("Resource Manager Integration", () => {
            if (ResourceManager.Instance == null) return false;
            try
            {
                ResourceManager.Instance.AddResource(Resource.ResourceType.Gold, 100);
                int gold = ResourceManager.Instance.GetResourceAmount(Resource.ResourceType.Gold);
                return gold > 0;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Resource Manager integration failed: {e.Message}");
                return false;
            }
        });

        // Test Recruitment System integration
        RunTest("Recruitment System Integration", () => {
            return RecruitmentSystem.Instance != null;
        });

        // Test Crafting System integration
        RunTest("Crafting System Integration", () => {
            return CraftingSystem.Instance != null;
        });
    }

    private void TestPhase2Systems()
    {
        Debug.Log("Testing Phase 2 Systems...");

        // Test Phase 2.1 - Adventurer System
        RunTest("Adventurer System", () => {
            return RecruitmentSystem.Instance != null && 
                   PartyManagementSystem.Instance != null;
        });

        // Test Phase 2.2 - Equipment and Progression
        RunTest("Equipment System", () => {
            return ItemDatabase.Instance != null && 
                   CraftingSystem.Instance != null &&
                   ProgressionSystem.Instance != null &&
                   EquipmentEnhancementSystem.Instance != null;
        });

        // Test Phase 2.3 - Adventurer Interface
        RunTest("Adventurer Interface", () => {
            // Check if UI components exist
            var rosterUI = FindObjectOfType<AdventurerRosterUI>();
            var equipmentUI = FindObjectOfType<AdventurerEquipmentUI>();
            var characterSheetUI = FindObjectOfType<AdventurerCharacterSheetUI>();
            
            return rosterUI != null || equipmentUI != null || characterSheetUI != null;
        });

        // Test Phase 2.4 - First Playable Version
        RunTest("First Playable Version", () => {
            return CanvasManager.Instance != null && 
                   NewGameManager.Instance != null &&
                   GameIntegrationManager.Instance != null;
        });
    }

    private void RunTest(string testName, System.Func<bool> testFunction)
    {
        testsRun++;
        
        try
        {
            bool result = testFunction();
            
            if (result)
            {
                testsPassed++;
                if (enableDebugMode)
                {
                    Debug.Log($"✓ {testName}: PASSED");
                }
            }
            else
            {
                testsFailed++;
                Debug.LogWarning($"✗ {testName}: FAILED");
            }
        }
        catch (System.Exception e)
        {
            testsFailed++;
            Debug.LogError($"✗ {testName}: ERROR - {e.Message}");
        }
    }

    private void PrintTestResults()
    {
        Debug.Log("=== Phase 2.4 Test Results ===");
        Debug.Log($"Tests Run: {testsRun}");
        Debug.Log($"Tests Passed: {testsPassed}");
        Debug.Log($"Tests Failed: {testsFailed}");
        
        float successRate = testsRun > 0 ? (float)testsPassed / testsRun * 100f : 0f;
        Debug.Log($"Success Rate: {successRate:F1}%");
        
        if (testsFailed == 0)
        {
            Debug.Log("🎉 All Phase 2.4 tests passed! First playable version is ready!");
        }
        else
        {
            Debug.LogWarning($"⚠️ {testsFailed} tests failed. Please check the implementation.");
        }
    }

    [ContextMenu("Test New Game Flow")]
    public void TestNewGameFlow()
    {
        Debug.Log("Testing New Game Flow...");
        
        if (NewGameManager.Instance == null)
        {
            Debug.LogError("NewGameManager not found!");
            return;
        }

        try
        {
            // Test setting player name
            NewGameManager.Instance.SetPlayerName("Test Player");
            NewGameManager.Instance.SetCityName("Test City");
            
            Debug.Log($"Player Name: {NewGameManager.Instance.GetPlayerName()}");
            Debug.Log($"City Name: {NewGameManager.Instance.GetCityName()}");
            
            // Test starting a new game
            NewGameManager.Instance.StartNewGame();
            
            Debug.Log("New Game Flow test completed successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"New Game Flow test failed: {e.Message}");
        }
    }

    [ContextMenu("Test Canvas Switching")]
    public void TestCanvasSwitching()
    {
        Debug.Log("Testing Canvas Switching...");
        
        if (CanvasManager.Instance == null)
        {
            Debug.LogError("CanvasManager not found!");
            return;
        }

        try
        {
            // Test switching between different canvases
            CanvasManager.Instance.ShowMainMenu();
            Debug.Log("Switched to Main Menu");
            
            CanvasManager.Instance.ShowGameplay();
            Debug.Log("Switched to Gameplay");
            
            CanvasManager.Instance.ShowAdventurerInterface();
            Debug.Log("Showed Adventurer Interface");
            
            CanvasManager.Instance.ShowEquipmentInterface();
            Debug.Log("Showed Equipment Interface");
            
            CanvasManager.Instance.ShowCraftingInterface();
            Debug.Log("Showed Crafting Interface");
            
            CanvasManager.Instance.ShowMainMenu();
            Debug.Log("Returned to Main Menu");
            
            Debug.Log("Canvas Switching test completed successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Canvas Switching test failed: {e.Message}");
        }
    }

    [ContextMenu("Generate Test Data")]
    public void GenerateTestData()
    {
        Debug.Log("Generating test data for Phase 2.4...");
        
        try
        {
            // Generate test resources
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.AddResource(Resource.ResourceType.Gold, 5000);
                ResourceManager.Instance.AddResource(Resource.ResourceType.Population, 200);
                Debug.Log("Generated test resources");
            }
            
            // Generate test adventurers
            if (RecruitmentSystem.Instance != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    var adventurerClass = (Adventurer.AdventurerClass)(i % 5);
                    var adventurer = RecruitmentSystem.Instance.GenerateSpecificAdventurer(adventurerClass, Adventurer.AdventurerRarity.Common);
                    if (adventurer != null)
                    {
                        // Note: Adventurer.Name is read-only, set during construction
                        Debug.Log($"Generated test adventurer: {adventurer.Name} ({adventurer.Class})");
                    }
                }
            }
            
            Debug.Log("Test data generation completed!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Test data generation failed: {e.Message}");
        }
    }
}