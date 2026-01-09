using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    
    [Header("Card Prefab - Assign your card GameObject")]
    public GameObject cardPrefab;
    
    [Header("Card Data - Create ScriptableObjects or add here")]
    public List<CardData> cardDeck = new List<CardData>();
    
    [Header("Spawn Settings")]
    public Transform cardSpawnPoint;
    public float cardSpacing = 10f;
    
    private Queue<CardData> remainingCards = new Queue<CardData>();
    private CardSwipe currentCard;
    private int cardIndex = 0;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start()
    {
        ShuffleDeck();
        SpawnNextCard();
    }
    
    private void ShuffleDeck()
    {
        // Create a shuffled queue of cards
        List<CardData> shuffled = new List<CardData>(cardDeck);
        
        for (int i = 0; i < shuffled.Count; i++)
        {
            CardData temp = shuffled[i];
            int randomIndex = Random.Range(i, shuffled.Count);
            shuffled[i] = shuffled[randomIndex];
            shuffled[randomIndex] = temp;
        }
        
        remainingCards.Clear();
        foreach (CardData card in shuffled)
        {
            remainingCards.Enqueue(card);
        }
    }
    
    public void OnCardSwiped()
    {
        // Wait a moment then spawn next card
        Invoke(nameof(SpawnNextCard), 0.5f);
    }
    
    private void SpawnNextCard()
    {
        if (remainingCards.Count == 0)
        {
            Debug.Log("Deck is empty! Reshuffling...");
            ShuffleDeck();
        }
        
        CardData nextCardData = remainingCards.Dequeue();
        
        // If using prefab instantiation
        if (cardPrefab != null)
        {
            Vector3 spawnPos = cardSpawnPoint != null ? cardSpawnPoint.position : Vector3.zero;
            GameObject newCardObj = Instantiate(cardPrefab, spawnPos, Quaternion.identity);
            currentCard = newCardObj.GetComponent<CardSwipe>();
        }
        else if (currentCard == null)
        {
            currentCard = FindObjectOfType<CardSwipe>();
        }
        
        if (currentCard != null)
        {
            currentCard.SetCardData(nextCardData);
            currentCard.ResetCard();
        }
        
        cardIndex++;
    }
}