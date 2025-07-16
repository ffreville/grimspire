using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class BaseMenu : MonoBehaviour
{
    [Header("Base Menu Settings")]
    [SerializeField] protected Canvas menuCanvas;
    [SerializeField] protected GraphicRaycaster graphicRaycaster;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected Button backButton;
    [SerializeField] protected GameObject loadingPanel;
    
    [Header("Animation Settings")]
    [SerializeField] protected bool useAnimations = true;
    [SerializeField] protected float fadeInDuration = 0.3f;
    [SerializeField] protected float fadeOutDuration = 0.3f;
    [SerializeField] protected AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    protected bool isVisible = false;
    protected bool isAnimating = false;

    public bool IsVisible => isVisible;
    public bool IsAnimating => isAnimating;

    protected virtual void Awake()
    {
        InitializeComponents();
        SetupEventListeners();
    }

    protected virtual void InitializeComponents()
    {
        if (menuCanvas == null)
            menuCanvas = GetComponent<Canvas>();
        
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        if (graphicRaycaster == null)
            graphicRaycaster = GetComponent<GraphicRaycaster>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        if (menuCanvas == null)
        {
            menuCanvas = gameObject.AddComponent<Canvas>();
            menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        
        if (graphicRaycaster == null)
        {
            graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    protected virtual void SetupEventListeners()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonPressed);
        }
    }

    public virtual void Show()
    {
        if (isVisible || isAnimating) return;
        
        gameObject.SetActive(true);
        
        if (useAnimations)
        {
            StartCoroutine(FadeIn());
        }
        else
        {
            ShowImmediate();
        }
        
        OnMenuShown();
    }

    public virtual void Hide()
    {
        if (!isVisible || isAnimating) return;
        
        if (useAnimations)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            HideImmediate();
        }
        
        OnMenuHidden();
    }

    protected virtual void ShowImmediate()
    {
        isVisible = true;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        menuCanvas.enabled = true;
    }

    protected virtual void HideImmediate()
    {
        isVisible = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        menuCanvas.enabled = false;
        gameObject.SetActive(false);
    }

    protected virtual IEnumerator FadeIn()
    {
        isAnimating = true;
        menuCanvas.enabled = true;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeInDuration;
            float curveValue = fadeCurve.Evaluate(progress);
            
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, curveValue);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        isVisible = true;
        isAnimating = false;
    }

    protected virtual IEnumerator FadeOut()
    {
        isAnimating = true;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;
        
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeOutDuration;
            float curveValue = fadeCurve.Evaluate(progress);
            
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, curveValue);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        menuCanvas.enabled = false;
        gameObject.SetActive(false);
        isVisible = false;
        isAnimating = false;
    }

    public virtual void SetLoading(bool loading)
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(loading);
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.interactable = !loading;
        }
    }

    protected virtual void OnBackButtonPressed()
    {
        GoBack();
    }

    protected virtual void GoBack()
    {
        Hide();
    }

    protected virtual void OnMenuShown()
    {
        // Override in derived classes
    }

    protected virtual void OnMenuHidden()
    {
        // Override in derived classes
    }

    protected virtual void OnDestroy()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnBackButtonPressed);
        }
    }
}