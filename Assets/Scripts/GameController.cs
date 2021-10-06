using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Player player;
    public int playerMaxHp, playerMaxMana;
    public List<InventorySlot> inventory;
    public List<Skill> playerSkills, startingSkills, existingSkills;
    public List<Item> existingItems;
    public List<Enemy> potentialEnemies;
    public Battle currentBattle;
    public SpriteRenderer enemySpriteRender;
    public GameObject playerStatus, enemyStatus, textbox, battleMenu, skillsMenuInBattle, battleEnd;
    public List<GameObject> skillSlotsInBattle, itemSlots, skillSlots, levelUpButtons;

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        // Check for conflicting instances
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Save singleton instance
        Instance = this;

        // Don't destroy when loading between scenes
        DontDestroyOnLoad(gameObject);

    }

    // Start is called before the first frame update
    void Start()
    {
        // Create default player object
        player = new Player(playerMaxHp, playerMaxMana, 0);

        // Create player skills
        foreach(Skill skill in startingSkills)
        {
            playerSkills.Add(Instantiate(skill));
        }

        // Create inventory
        inventory = new List<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMainMenu()
    {
        skillSlotsInBattle.Clear();
        itemSlots.Clear();
        skillSlots.Clear();
        levelUpButtons.Clear();
        currentBattle = null;

        SceneManager.LoadScene("Menu");
        StartCoroutine(LoadingMainMenu());
    }

    public void LoadBattle()
    {
        SceneManager.LoadScene("BattleScreen");
        currentBattle = new Battle(Instantiate(potentialEnemies[Random.Range(0, potentialEnemies.Count)]));
        StartCoroutine(LoadingBattle());
    }

    public void LoadItems()
    {
        SceneManager.LoadScene("ItemMenu");
        StartCoroutine(LoadingItems());
    }

    public void LoadSkills()
    {
        SceneManager.LoadScene("SkillMenu");
        StartCoroutine(LoadingSkills());
    }

    public void ShowSkillsInBattle()
    {
        battleMenu.SetActive(false);
        skillsMenuInBattle.SetActive(true);

        if (skillSlotsInBattle.Count > 0)
            return;

        // Get skills slots for showing skills
        foreach(GameObject skillSlot in GameObject.FindGameObjectsWithTag("Skill Slot"))
        {
            skillSlotsInBattle.Add(skillSlot);
        }

        // Show available skills
        for(int i = 0; i < skillSlotsInBattle.Count; i++)
        {
            if (i < playerSkills.Count)
            {
                skillSlotsInBattle[i].SetActive(true);
                skillSlotsInBattle[i].GetComponentInChildren<UnityEngine.UI.Text>().text = playerSkills[i].name;
                skillSlotsInBattle[i].GetComponent<Buttons>().correspondingSkill = playerSkills[i];
            }

            else
                skillSlotsInBattle[i].SetActive(false);
        }
    }

    public void BackToBattleMenu()
    {
        battleMenu.SetActive(true);
        skillsMenuInBattle.SetActive(false);
    }

    public void SkillEffect(Skill skill)
    {
        // Check mana cost
        if (player.mana >= skill.manaCost)
        {
            player.mana -= skill.manaCost;
        }
        else
        {
            DisplayText("Not enough mana.");
            return;
        }

        // Deal damage
        currentBattle.enemy.hp = Mathf.Clamp(currentBattle.enemy.hp - skill.attack, 0, currentBattle.enemy.maxHp);
        DisplayText("Dealt " + skill.attack + " to the enemy " + currentBattle.enemy.name + ".");
        UpdateBattleStatus();

        // End turn
        skillsMenuInBattle.SetActive(false);

        // Check enemy defeated
        if (currentBattle.enemy.hp <= 0)
        {
            StartCoroutine(DefeatedEnemy());
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    public void LevelUpSkill(Skill skill)
    {
        /*for(int i = 0; i < playerSkills.Count; i++)
        {
            if(playerSkills)
        }*/
        skill.level++;
        skill.attack++;
        player.skillPoints--;
        UpdateSkills();
        DisplayText("[LV " + skill.level + "] " + skill.name + ": " + skill.description + " Costs " + skill.manaCost + " mana and deals " + skill.attack + " damage.");
    }

    public void UpdateBattleStatus()
    {
        playerStatus.GetComponentInChildren<UnityEngine.UI.Text>().text = "You\nHP: " + player.hp + "/" + player.maxHp + "\nMana: " + player.mana + "/" + playerMaxMana;

        if (currentBattle == null)
            return;

        Enemy currentEnemy = currentBattle.enemy;
        enemyStatus.GetComponentInChildren<UnityEngine.UI.Text>().text = currentEnemy.name + "\nHP: " + currentEnemy.hp + "/" + currentEnemy.maxHp;
    }

    public void UpdateSkills()
    {
        playerStatus.GetComponentInChildren<UnityEngine.UI.Text>().text = "Skill Points: " + player.skillPoints;

        // Show skills
        for (int i = 0; i < skillSlots.Count; i++)
        {
            if (i < playerSkills.Count)
            {
                skillSlots[i].SetActive(true);
                skillSlots[i].GetComponentInChildren<UnityEngine.UI.Text>().text = "[LV " + playerSkills[i].level + "] " + playerSkills[i].name;
                skillSlots[i].GetComponent<Buttons>().correspondingSkill = playerSkills[i];
            }

            else
                skillSlots[i].SetActive(false);
        }

        // Show level up buttons
        foreach(GameObject levelUpButton in levelUpButtons)
        {
            Buttons button = levelUpButton.GetComponent<Buttons>().correspondingButton;
            if (button != null && button.correspondingSkill != null && (player.skillPoints == 0 || button.correspondingSkill.level == button.correspondingSkill.maxLevel))
            {
                levelUpButton.SetActive(false);
            }

            else if (button != null && button.correspondingSkill != null && (player.skillPoints > 0 && button.correspondingSkill.level < button.correspondingSkill.maxLevel))
            {
                levelUpButton.SetActive(true);
            }
        }
    }

    public void DisplayText(string text)
    {
        textbox.GetComponentInChildren<UnityEngine.UI.Text>().text = text;
    }

    public void AddToInventory(Item item, int quantity)
    {
        foreach(InventorySlot inventorySlot in inventory)
        {
            if(inventorySlot.item.name == item.name)
            {
                inventorySlot.quantity += quantity;
                return;
            }
        }

        inventory.Add(new InventorySlot(item, quantity));
    }

    public void SaveData()
    {
        string[] _skillNames, _itemNames;
        int[] _skillLevels, _skillCosts, _skillAttacks, _itemQuantities;
        int _playerHp, _playerMaxHp, _playerMana, _playerMaxMana, _playerSkillPoints;

        // Save skills
        _skillNames = new string[playerSkills.Count];
        _skillLevels = new int[playerSkills.Count];
        _skillCosts = new int[playerSkills.Count];
        _skillAttacks = new int[playerSkills.Count];

        for(int i = 0; i < playerSkills.Count; i++)
        {
            _skillNames[i] = playerSkills[i].name;
            _skillLevels[i] = playerSkills[i].level;
            _skillCosts[i] = playerSkills[i].manaCost;
            _skillAttacks[i] = playerSkills[i].attack;
        }

        // Save items
        _itemNames = new string[inventory.Count];
        _itemQuantities = new int[inventory.Count];

        for (int i = 0; i < inventory.Count; i++)
        {
            _itemNames[i] = inventory[i].item.name;
            _itemQuantities[i] = inventory[i].quantity;
        }

        // Save player data
        _playerHp = player.hp;
        _playerMaxHp = player.maxHp;
        _playerMana = player.mana;
        _playerMaxMana = player.maxMana;
        _playerSkillPoints = player.skillPoints;

        // Save
        SaveManager.CreateSave(_skillNames, _itemNames, _skillLevels, _skillCosts, _skillAttacks, _itemQuantities, _playerHp, _playerMaxHp, _playerMana, _playerMaxMana,
            _playerSkillPoints);
    }

    public void LoadData()
    {
        Save loadedSave = SaveManager.LoadSave();

        // Load skills
        playerSkills.Clear();
        for(int i = 0; i < loadedSave.skillNames.Length; i++)
        {
            // Check existing skills
            foreach(Skill existingSkill in existingSkills)
            {
                // If saved skill matches existing skill, load with saved data
                if(loadedSave.skillNames[i] == existingSkill.name)
                {
                    playerSkills.Add(Instantiate(existingSkill));
                    playerSkills[i].level = loadedSave.skillLevels[i];
                    playerSkills[i].manaCost = loadedSave.skillCosts[i];
                    playerSkills[i].attack = loadedSave.skillAttacks[i];
                }
            }
        }

        // Load items
        inventory.Clear();
        for(int i = 0; i < loadedSave.itemNames.Length; i++)
        {
            // Check existing items
            foreach(Item existingItem in existingItems)
            {
                // If loaded item matches existing item, load with saved data
                if(loadedSave.itemNames[i] == existingItem.name)
                {
                    inventory.Add(new InventorySlot(Instantiate(existingItem), loadedSave.itemQuantities[i]));
                }
            }
        }

        // Load player data
        player.hp = loadedSave.playerHp;
        player.maxHp = loadedSave.playerMaxHp;
        player.mana = loadedSave.playerMana;
        player.maxMana = loadedSave.playerMaxMana;
        player.skillPoints = loadedSave.playerSkillPoints;

        LoadMainMenu();
    }

    IEnumerator LoadingMainMenu()
    {
        yield return new WaitForSeconds(0.00001f);

        playerStatus = GameObject.FindGameObjectWithTag("Player Status");
        UpdateBattleStatus();
    }

    IEnumerator LoadingItems()
    {
        yield return new WaitForSeconds(0.00001f);

        textbox = GameObject.FindGameObjectWithTag("Textbox");

        // Get items slots for showing items
        foreach (GameObject itemSlot in GameObject.FindGameObjectsWithTag("Item Slot"))
        {
            itemSlots.Add(itemSlot);
        }

        // Show items
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < inventory.Count)
            {
                itemSlots[i].SetActive(true);
                itemSlots[i].GetComponentInChildren<UnityEngine.UI.Text>().text = "[X" + inventory[i].quantity + "] " + inventory[i].item.name;
                itemSlots[i].GetComponent<Buttons>().correspondingItem = inventory[i].item;
            }

            else
                itemSlots[i].SetActive(false);
        }

        // If no items, show no items message
        if(inventory.Count == 0)
        {
            itemSlots[0].SetActive(true);
            itemSlots[0].GetComponentInChildren<UnityEngine.UI.Text>().text = "You have no items.";
        }
    }

    IEnumerator LoadingSkills()
    {
        yield return new WaitForSeconds(0.00001f);

        playerStatus = GameObject.FindGameObjectWithTag("Player Status");
        textbox = GameObject.FindGameObjectWithTag("Textbox");

        // Get skill slots for showing skills
        foreach (GameObject skillSlot in GameObject.FindGameObjectsWithTag("Skill Slot"))
        {
            skillSlots.Add(skillSlot);
        }

        // Get level up buttons
        foreach (GameObject levelUpButton in GameObject.FindGameObjectsWithTag("Level Up Skill"))
        {
            levelUpButtons.Add(levelUpButton);
        }

        UpdateSkills();

        // If no items, show no items message
        if (playerSkills.Count == 0)
        {
            skillSlots[0].SetActive(true);
            skillSlots[0].GetComponentInChildren<UnityEngine.UI.Text>().text = "You have no skills.";
        }
    }

    IEnumerator LoadingBattle()
    {
        yield return new WaitForSeconds(0.00001f);

        playerStatus = GameObject.FindGameObjectWithTag("Player Status");
        enemyStatus = GameObject.FindGameObjectWithTag("Enemy Status");
        UpdateBattleStatus();

        textbox = GameObject.FindGameObjectWithTag("Textbox");
        DisplayText("A " + currentBattle.enemy.name + " appeared!");

        battleMenu = GameObject.FindGameObjectWithTag("Battle Menu");
        skillsMenuInBattle = GameObject.FindGameObjectWithTag("Skills Menu (Battle)");
        skillsMenuInBattle.SetActive(false);
        battleEnd = GameObject.FindGameObjectWithTag("Battle End");
        battleEnd.SetActive(false);

        enemySpriteRender = GameObject.FindGameObjectWithTag("Enemy Sprite Renderer").GetComponent<SpriteRenderer>();
        enemySpriteRender.sprite = currentBattle.enemy.sprite;
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        Skill enemySkill = currentBattle.enemy.skills[Random.Range(0, currentBattle.enemy.skills.Length)];
        player.hp -= enemySkill.attack;
        DisplayText("The enemy " + currentBattle.enemy.name + " used " + enemySkill.name + " and dealt " + enemySkill.attack + " damage.");
        UpdateBattleStatus();

        if (player.hp > 0)
            battleMenu.SetActive(true);
        else
            StartCoroutine(GameOver());
    }

    IEnumerator DefeatedEnemy()
    {
        yield return new WaitForSeconds(1f);

        player.skillPoints++;
        AddToInventory(Instantiate(currentBattle.enemy.drop), 1);
        DisplayText("Defeated the enemy " + currentBattle.enemy.name + ". Obtained 1 " + currentBattle.enemy.drop.name + ". Gained 1 skill point.");

        enemySpriteRender.sprite = null;
        battleEnd.SetActive(true);
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("GameOver");
    }

    public class Player
    {
        public int hp, maxHp, mana, maxMana, skillPoints;

        // Player constructor
        public Player(int _maxHp, int _maxMana, int _skillPoints)
        {
            hp = _maxHp;
            maxHp = _maxHp;
            mana = _maxMana;
            maxMana = _maxMana;
            skillPoints = _skillPoints;
        }
    }

    public class InventorySlot
    {
        public Item item;
        public int quantity;

        public InventorySlot(Item _item, int _quantity)
        {
            item = _item;
            quantity = _quantity;
        }
    }

    public class Battle
    {
        public Enemy enemy;

        public Battle(Enemy _enemy)
        {
            enemy = _enemy;
        }
    }
}
