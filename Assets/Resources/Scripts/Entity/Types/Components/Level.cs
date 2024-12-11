
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Level : MonoBehaviour 
{
    [SerializeField] private int currentLevel = 1, currentXp, xpToNextLevel, levelUpBase = 200, levelUpFactor = 150, xpGiven;

    public int CurrentLevel { get => currentLevel; }
    public int CurrentXp { get => currentXp; }
    public int XpToNextLevel { get => xpToNextLevel; }
    public int XPGiven { get => xpGiven; set => xpGiven = value; }

    // Stat-specific experience requirements
    private int maxHpXpToNext = 100, powerXpToNext = 100, defenseXpToNext = 100;
    private int maxHpBase = 100, powerBase = 100, defenseBase = 100;
    private int maxHpFactor = 50, powerFactor = 50, defenseFactor = 50;

    private void OnValidate() => xpToNextLevel = ExperienceToNextLevel();

    private int ExperienceToNextLevel() => levelUpBase + currentLevel * levelUpFactor;

    private bool RequiresLevelUp() => currentXp >= xpToNextLevel;

    public void AddExperience(int xp) 
    {
        if (xp == 0 || levelUpBase == 0) return;

        currentXp += xp;

        UIManager.instance.AddMessage($"You gain {xp} experience points.", "#FFFFFF");

        if (RequiresLevelUp()) 
        {
            UIManager.instance.ToggleLevelUpMenu(GetComponent<Actor>());
            UIManager.instance.AddMessage($"You advance to level {currentLevel + 1}!", "#00FF00"); // Green
        }
    }

    private void IncreaseLevel() 
    {
        currentXp -= xpToNextLevel;
        currentLevel++;
        xpToNextLevel = ExperienceToNextLevel();
    }

    // Stat Level-Up Methods with Scaling XP Requirements
    public void IncreaseMaxHp(int amount = 20) 
    {
        if (currentXp < maxHpXpToNext) 
        {
            UIManager.instance.AddMessage($"Not enough XP to increase Max HP!", "#FF0000"); // Red
            return;
        }

        currentXp -= maxHpXpToNext;
        maxHpXpToNext = maxHpBase + (GetComponent<Fighter>().MaxHp / amount) * maxHpFactor;

        GetComponent<Fighter>().MaxHp += amount;
        GetComponent<Fighter>().Hp += amount;

        UIManager.instance.AddMessage($"You feel in better health!", "#00FF00"); // Green
        IncreaseLevel();
    }

    public void IncreasePower(int amount = 1) 
    {
        if (currentXp < powerXpToNext) 
        {
            UIManager.instance.AddMessage($"Not enough XP to increase Power!", "#FF0000"); // Red
            return;
        }

        currentXp -= powerXpToNext;
        powerXpToNext = powerBase + (GetComponent<Actor>().Fighter.BasePower / amount) * powerFactor;

        GetComponent<Actor>().Fighter.BasePower += amount;

        UIManager.instance.AddMessage($"You feel stronger!", "#00FF00"); // Green
        IncreaseLevel();
    }

    public void IncreaseDefense(int amount = 1) 
    {
        if (currentXp < defenseXpToNext) 
        {
            UIManager.instance.AddMessage($"Not enough XP to increase Defense!", "#FF0000"); // Red
            return;
        }

        currentXp -= defenseXpToNext;
        defenseXpToNext = defenseBase + (GetComponent<Actor>().Fighter.BaseDefense / amount) * defenseFactor;

        GetComponent<Actor>().Fighter.BaseDefense += amount;

        UIManager.instance.AddMessage($"You feel tougher!", "#00FF00"); // Green
        IncreaseLevel();
    }

    public LevelState SaveState() => new LevelState
    (
        currentLevel: currentLevel,
        currentXp: currentXp,
        xpToNextLevel: xpToNextLevel
    );

    public void LoadState(LevelState state) 
    {
        currentLevel = state.CurrentLevel;
        currentXp = state.CurrentXp;
        xpToNextLevel = state.XpToNextLevel;
    }
}

public class LevelState 
{
    [SerializeField] private int currentLevel = 1, currentXp, xpToNextLevel;

    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public int CurrentXp { get => currentXp; set => currentXp = value; }
    public int XpToNextLevel { get => xpToNextLevel; set => xpToNextLevel = value; }

    public LevelState(int currentLevel, int currentXp, int xpToNextLevel) 
    {
        this.currentLevel = currentLevel;
        this.currentXp = currentXp;
        this.xpToNextLevel = xpToNextLevel;
    }
}


/*
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Level : MonoBehaviour 

//ADD INTELLIGENCE STAT
{
  [SerializeField] private int currentLevel = 1, currentXp, xpToNextLevel, levelUpBase = 200, levelUpFactor = 150, xpGiven;

  public int CurrentLevel { get => currentLevel; }
  public int CurrentXp { get => currentXp; }
  public int XpToNextLevel { get => xpToNextLevel; }
  public int XPGiven { get => xpGiven; set => xpGiven = value; }

  private void OnValidate() => xpToNextLevel = ExperienceToNextLevel();

  private int ExperienceToNextLevel() => levelUpBase + currentLevel * levelUpFactor;
  private bool RequiresLevelUp() => currentXp >= xpToNextLevel;

  public void AddExperience(int xp) 
  {
    if (xp == 0 || levelUpBase == 0) return;

    currentXp += xp;

    UIManager.instance.AddMessage($"You gain {xp} experience points.", "#FFFFFF");

    if (RequiresLevelUp()) 
    {
      UIManager.instance.ToggleLevelUpMenu(GetComponent<Actor>());
      UIManager.instance.AddMessage($"You advance to level {currentLevel + 1}!", "#00FF00"); //Green
    }
  }

  private void IncreaseLevel() 
  {
    currentXp -= xpToNextLevel;
    currentLevel++;
    xpToNextLevel = ExperienceToNextLevel();
  }

  public void IncreaseMaxHp(int amount = 20) 
  {
    GetComponent<Fighter>().MaxHp += amount;
    GetComponent<Fighter>().Hp += amount;

    UIManager.instance.AddMessage($"You feel as if your in better health!", "#00FF00"); //Green
    IncreaseLevel();
  }

  public void IncreasePower(int amount = 1) 
  {
    GetComponent<Actor>().Fighter.BasePower += amount;

    UIManager.instance.AddMessage($"You feel stronger!", "#00FF00"); //Green
    IncreaseLevel();
  }

  public void IncreaseDefense(int amount = 1) 
  {
    GetComponent<Actor>().Fighter.BaseDefense += amount;

    UIManager.instance.AddMessage($"You feel tougher!", "#00FF00"); //Green
    IncreaseLevel();
  }

  public LevelState SaveState() => new LevelState
  (
    currentLevel: currentLevel,
    currentXp: currentXp,
    xpToNextLevel: xpToNextLevel
  );

  public void LoadState(LevelState state) 
  {
    currentLevel = state.CurrentLevel;
    currentXp = state.CurrentXp;
    xpToNextLevel = state.XpToNextLevel;
  }
}

public class LevelState 
{
  [SerializeField] private int currentLevel = 1, currentXp, xpToNextLevel;

  public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
  public int CurrentXp { get => currentXp; set => currentXp = value; }
  public int XpToNextLevel { get => xpToNextLevel; set => xpToNextLevel = value; }

  public LevelState(int currentLevel, int currentXp, int xpToNextLevel) 
  {
    this.currentLevel = currentLevel;
    this.currentXp = currentXp;
    this.xpToNextLevel = xpToNextLevel;
  }
}
*/