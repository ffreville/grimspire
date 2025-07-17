using UnityEngine;
using UnityEngine.UI;

public class ButtonTester : MonoBehaviour
{
    [Header("Button Testing")]
    [SerializeField] private bool testOnStart = false;
    [SerializeField] private string buttonToTest = "NewGameButton";

    private void Start()
    {
        if (testOnStart)
        {
            Invoke(nameof(TestButton), 1f); // Délai pour s'assurer que tout est initialisé
        }
    }

    [ContextMenu("Test New Game Button")]
    public void TestButton()
    {
        TestSpecificButton(buttonToTest);
    }

    [ContextMenu("Test New Game Button Click")]
    public void TestNewGameButton()
    {
        TestSpecificButton("NewGameButton");
    }

    [ContextMenu("Fix New Game Button")]
    public void FixNewGameButton()
    {
        Button newGameButton = FindButton("NewGameButton");
        if (newGameButton != null)
        {
            // Supprimer tous les listeners existants
            newGameButton.onClick.RemoveAllListeners();
            
            // Ajouter le bon listener
            newGameButton.onClick.AddListener(() => {
                Debug.Log("New Game Button clicked - showing new game panel");
                ShowNewGamePanel();
            });
            
            Debug.Log("✓ New Game Button fixed!");
        }
        else
        {
            Debug.LogError("New Game Button not found!");
        }
    }

    private void TestSpecificButton(string buttonName)
    {
        Debug.Log($"=== Testing Button: {buttonName} ===");
        
        Button button = FindButton(buttonName);
        if (button != null)
        {
            Debug.Log($"✓ Button found: {buttonName}");
            Debug.Log($"  - GameObject: {button.gameObject.name}");
            Debug.Log($"  - Active: {button.gameObject.activeInHierarchy}");
            Debug.Log($"  - Interactable: {button.interactable}");
            Debug.Log($"  - Listener count: {button.onClick.GetPersistentEventCount()}");
            
            // Tester le clic
            Debug.Log("Testing button click...");
            button.onClick.Invoke();
            
            Debug.Log($"✓ Button {buttonName} test complete");
        }
        else
        {
            Debug.LogError($"✗ Button not found: {buttonName}");
            
            // Chercher tous les boutons pour diagnostic
            Debug.Log("Available buttons:");
            Button[] allButtons = FindObjectsOfType<Button>();
            foreach (Button b in allButtons)
            {
                Debug.Log($"  - {b.gameObject.name}");
            }
        }
    }

    private Button FindButton(string buttonName)
    {
        GameObject buttonObj = GameObject.Find(buttonName);
        if (buttonObj != null)
        {
            return buttonObj.GetComponent<Button>();
        }
        return null;
    }

    private void ShowNewGamePanel()
    {
        Debug.Log("ShowNewGamePanel called");
        
        // Masquer tous les panneaux
        HideAllPanels();
        
        // Afficher le panneau de nouveau jeu
        GameObject newGamePanel = GameObject.Find("NewGamePanel");
        if (newGamePanel != null)
        {
            newGamePanel.SetActive(true);
            Debug.Log("✓ New Game Panel shown");
        }
        else
        {
            Debug.LogError("✗ New Game Panel not found");
        }
    }

    private void HideAllPanels()
    {
        string[] panelNames = { "NewGamePanel", "LoadGamePanel", "SettingsPanel" };
        
        foreach (string panelName in panelNames)
        {
            GameObject panel = GameObject.Find(panelName);
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }

    [ContextMenu("Debug All UI Elements")]
    public void DebugAllUIElements()
    {
        Debug.Log("=== UI Elements Debug ===");
        
        // Trouver tous les boutons
        Button[] buttons = FindObjectsOfType<Button>();
        Debug.Log($"Found {buttons.Length} buttons:");
        foreach (Button button in buttons)
        {
            Debug.Log($"  - {button.gameObject.name} (active: {button.gameObject.activeInHierarchy}, interactable: {button.interactable})");
        }
        
        // Trouver tous les panneaux
        Debug.Log("Looking for panels:");
        string[] panelNames = { "NewGamePanel", "LoadGamePanel", "SettingsPanel", "MainMenuPanel" };
        foreach (string panelName in panelNames)
        {
            GameObject panel = GameObject.Find(panelName);
            if (panel != null)
            {
                Debug.Log($"  ✓ {panelName} found (active: {panel.activeInHierarchy})");
            }
            else
            {
                Debug.Log($"  ✗ {panelName} not found");
            }
        }
    }

    [ContextMenu("Force Configure New Game Button")]
    public void ForceConfigureNewGameButton()
    {
        Debug.Log("Force configuring New Game Button...");
        
        // Attendre un peu pour s'assurer que tout est créé
        StartCoroutine(ConfigureAfterDelay());
    }

    private System.Collections.IEnumerator ConfigureAfterDelay()
    {
        yield return new UnityEngine.WaitForSeconds(0.5f);
        
        Button newGameButton = FindButton("NewGameButton");
        if (newGameButton != null)
        {
            // Configuration forcée du bouton
            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(() => {
                Debug.Log("=== NEW GAME BUTTON CLICKED ===");
                ShowNewGamePanel();
            });
            
            // Tester immédiatement
            Debug.Log("Testing configured button...");
            newGameButton.onClick.Invoke();
            
            Debug.Log("✓ New Game Button force configured and tested!");
        }
        else
        {
            Debug.LogError("✗ New Game Button still not found after delay!");
        }
    }
}