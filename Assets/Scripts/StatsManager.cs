using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    
    [Header("Stat Values (0-100)")]
    public int heart = 50;
    public int gold = 50;
    public int military = 50;
    public int faith = 50;
    
    [Header("UI References - Assign your stat bars here")]
    public Slider heartBar;
    public Slider goldBar;
    public Slider militaryBar;
    public Slider faithBar;
    
    [Header("UI Text (Optional)")]
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI militaryText;
    public TextMeshProUGUI faithText;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start()
    {
        UpdateAllUI();
    }
    
    public void ModifyStat(StatType type, int amount)
    {
        switch (type)
        {
            case StatType.Heart:
                heart = Mathf.Clamp(heart + amount, 0, 100);
                break;
            case StatType.Gold:
                gold = Mathf.Clamp(gold + amount, 0, 100);
                break;
            case StatType.Military:
                military = Mathf.Clamp(military + amount, 0, 100);
                break;
            case StatType.Faith:
                faith = Mathf.Clamp(faith + amount, 0, 100);
                break;
        }
        
        UpdateAllUI();
        CheckGameOver();
    }
    
    private void UpdateAllUI()
    {
        if (heartBar != null) heartBar.value = heart;
        if (goldBar != null) goldBar.value = gold;
        if (militaryBar != null) militaryBar.value = military;
        if (faithBar != null) faithBar.value = faith;
        
        if (heartText != null) heartText.text = heart.ToString();
        if (goldText != null) goldText.text = gold.ToString();
        if (militaryText != null) militaryText.text = military.ToString();
        if (faithText != null) faithText.text = faith.ToString();
    }
    
    private void CheckGameOver()
    {
        if (heart <= 0 || gold <= 0 || military <= 0 || faith <= 0 ||
            heart >= 100 || gold >= 100 || military >= 100 || faith >= 100)
        {
            Debug.Log("Game Over! A stat reached 0 or 100");
            // Add your game over logic here
        }
    }
}