using UnityEngine;

[System.Serializable]
public class CardData
{
    public string cardTitle;
    [TextArea(3, 6)]
    public string cardDescription;
    
    // Visual - You'll assign your card image here in the Inspector
    public Sprite cardImage;
    
    // Swipe Left Option
    public string leftOptionText;
    public StatChange[] leftStatChanges;
    
    // Swipe Right Option
    public string rightOptionText;
    public StatChange[] rightStatChanges;
}

[System.Serializable]
public class StatChange
{
    public StatType statType;
    public int changeAmount;
}

public enum StatType
{
    Heart,      // Changed one stat to Heart as requested
    Gold,
    Military,
    Faith
}