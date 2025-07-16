using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    
    [Header("Menu References")]
    [SerializeField] private BaseMenu[] allMenus;
    
    private Dictionary<System.Type, BaseMenu> menuDictionary;
    private Stack<BaseMenu> menuStack;
    private BaseMenu currentMenu;

    public BaseMenu CurrentMenu => currentMenu;
    public int MenuStackCount => menuStack.Count;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMenus();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeMenus()
    {
        menuDictionary = new Dictionary<System.Type, BaseMenu>();
        menuStack = new Stack<BaseMenu>();
        
        if (allMenus == null)
        {
            allMenus = FindObjectsOfType<BaseMenu>(true);
        }
        
        foreach (BaseMenu menu in allMenus)
        {
            if (menu != null)
            {
                menuDictionary[menu.GetType()] = menu;
                menu.gameObject.SetActive(false);
            }
        }
    }

    public T OpenMenu<T>() where T : BaseMenu
    {
        return OpenMenu<T>(true);
    }

    public T OpenMenu<T>(bool addToStack) where T : BaseMenu
    {
        if (menuDictionary.TryGetValue(typeof(T), out BaseMenu menu))
        {
            return OpenMenu(menu as T, addToStack);
        }
        
        Debug.LogError($"Menu of type {typeof(T)} not found!");
        return null;
    }

    public T OpenMenu<T>(T menuInstance, bool addToStack = true) where T : BaseMenu
    {
        if (menuInstance == null)
        {
            Debug.LogError("Menu instance is null!");
            return null;
        }
        
        if (currentMenu != null)
        {
            if (addToStack)
            {
                menuStack.Push(currentMenu);
            }
            currentMenu.Hide();
        }
        
        currentMenu = menuInstance;
        currentMenu.Show();
        
        return menuInstance;
    }

    public void CloseCurrentMenu()
    {
        if (currentMenu != null)
        {
            currentMenu.Hide();
            currentMenu = null;
        }
        
        if (menuStack.Count > 0)
        {
            BaseMenu previousMenu = menuStack.Pop();
            if (previousMenu != null)
            {
                currentMenu = previousMenu;
                currentMenu.Show();
            }
        }
    }

    public void CloseAllMenus()
    {
        if (currentMenu != null)
        {
            currentMenu.Hide();
            currentMenu = null;
        }
        
        while (menuStack.Count > 0)
        {
            BaseMenu menu = menuStack.Pop();
            if (menu != null)
            {
                menu.Hide();
            }
        }
    }

    public T GetMenu<T>() where T : BaseMenu
    {
        if (menuDictionary.TryGetValue(typeof(T), out BaseMenu menu))
        {
            return menu as T;
        }
        
        return null;
    }

    public bool IsMenuOpen<T>() where T : BaseMenu
    {
        T menu = GetMenu<T>();
        return menu != null && menu.IsVisible;
    }

    public bool IsMenuInStack<T>() where T : BaseMenu
    {
        T menu = GetMenu<T>();
        return menu != null && menuStack.Contains(menu);
    }

    public void GoBack()
    {
        CloseCurrentMenu();
    }

    public void RegisterMenu<T>(T menu) where T : BaseMenu
    {
        if (menu != null)
        {
            menuDictionary[typeof(T)] = menu;
        }
    }

    public void UnregisterMenu<T>() where T : BaseMenu
    {
        if (menuDictionary.ContainsKey(typeof(T)))
        {
            menuDictionary.Remove(typeof(T));
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentMenu != null)
            {
                GoBack();
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}