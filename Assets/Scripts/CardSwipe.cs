using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class CardSwipe : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Card UI References - Assign these")]
    public Image cardImage;
    public TextMeshProUGUI cardTitle;
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI leftOptionText;
    public TextMeshProUGUI rightOptionText;
    
    [Header("Swipe Settings")]
    public float swipeThreshold = 150f;
    public float rotationStrength = 0.05f;
    public float returnSpeed = 10f;
    
    private RectTransform rectTransform;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isDragging = false;
    private bool isReturning = false;
    
    private CardData currentCardData;
    private Canvas canvas;
    private Mouse mouse;
    private Touchscreen touchscreen;
    
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPosition = rectTransform.anchoredPosition;
        startRotation = rectTransform.rotation;
        
        // Get input devices
        mouse = Mouse.current;
        touchscreen = Touchscreen.current;
    }
    
    public void SetCardData(CardData data)
    {
        currentCardData = data;
        
        // Assign your visuals here
        if (cardImage != null && data.cardImage != null)
            cardImage.sprite = data.cardImage;
            
        if (cardTitle != null)
            cardTitle.text = data.cardTitle;
            
        if (cardDescription != null)
            cardDescription.text = data.cardDescription;
            
        if (leftOptionText != null)
            leftOptionText.text = data.leftOptionText;
            
        if (rightOptionText != null)
            rightOptionText.text = data.rightOptionText;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            OnDragEnd();
        }
    }
    
    private void Update()
    {
        if (isReturning)
        {
            ReturnToCenter();
            return;
        }
        
        if (!isDragging) return;
        
        // Get current input position
        Vector2 inputPosition = Vector2.zero;
        
        if (mouse != null && mouse.leftButton.isPressed)
        {
            inputPosition = mouse.position.ReadValue();
        }
        else if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
        {
            inputPosition = touchscreen.primaryTouch.position.ReadValue();
        }
        
        if (inputPosition != Vector2.zero)
        {
            OnDrag(inputPosition);
        }
    }
    
    private void OnDrag(Vector2 screenPosition)
    {
        // Convert screen position to canvas position
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.worldCamera,
            out localPoint
        );
        
        // Move card horizontally only
        Vector3 newPos = new Vector3(localPoint.x, startPosition.y, 0);
        rectTransform.anchoredPosition = newPos;
        
        // Rotate card based on position
        float offset = rectTransform.anchoredPosition.x - startPosition.x;
        float rotation = offset * rotationStrength;
        rectTransform.rotation = Quaternion.Euler(0, 0, rotation);
        
        // Show left/right text hints
        if (leftOptionText != null)
            leftOptionText.gameObject.SetActive(offset < -50f);
        if (rightOptionText != null)
            rightOptionText.gameObject.SetActive(offset > 50f);
    }
    
    private void OnDragEnd()
    {
        isDragging = false;
        float offset = rectTransform.anchoredPosition.x - startPosition.x;
        
        // Check if swiped far enough
        if (Mathf.Abs(offset) > swipeThreshold)
        {
            if (offset > 0)
                SwipeRight();
            else
                SwipeLeft();
        }
        else
        {
            // Return to center
            isReturning = true;
            if (leftOptionText != null)
                leftOptionText.gameObject.SetActive(false);
            if (rightOptionText != null)
                rightOptionText.gameObject.SetActive(false);
        }
    }
    
    private void SwipeLeft()
    {
        StartCoroutine(AnimateSwipe(-1));
        ApplyStatChanges(currentCardData.leftStatChanges);
    }
    
    private void SwipeRight()
    {
        StartCoroutine(AnimateSwipe(1));
        ApplyStatChanges(currentCardData.rightStatChanges);
    }
    
    private IEnumerator AnimateSwipe(int direction)
    {
        float elapsed = 0f;
        float duration = 0.3f;
        Vector3 targetPos = startPosition + new Vector3(direction * 2000f, 0, 0);
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, targetPos, elapsed / duration);
            yield return null;
        }
        
        // Tell CardManager to load next card
        CardManager.Instance.OnCardSwiped();
        
        // Reset this card
        ResetCard();
    }
    
    private void ReturnToCenter()
    {
        rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, startPosition, Time.deltaTime * returnSpeed);
        rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, startRotation, Time.deltaTime * returnSpeed);
        
        if (Vector3.Distance(rectTransform.anchoredPosition, startPosition) < 0.01f)
        {
            rectTransform.anchoredPosition = startPosition;
            rectTransform.rotation = startRotation;
            isReturning = false;
        }
    }
    
    private void ApplyStatChanges(StatChange[] changes)
    {
        if (changes == null || changes.Length == 0) return;
        
        foreach (StatChange change in changes)
        {
            StatsManager.Instance.ModifyStat(change.statType, change.changeAmount);
        }
    }
    
    public void ResetCard()
    {
        rectTransform.anchoredPosition = startPosition;
        rectTransform.rotation = startRotation;
        isDragging = false;
        isReturning = false;
        
        if (leftOptionText != null)
            leftOptionText.gameObject.SetActive(false);
        if (rightOptionText != null)
            rightOptionText.gameObject.SetActive(false);
    }
}