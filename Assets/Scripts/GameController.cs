using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Player player;
    public int playerMaxHp, playerMaxMana;
    public List<InventorySlot> inventory;
    public List<Skill> existingSkills;
    public List<PlayerCharacter> existingPlayerCharacters;
    public List<Item> existingItems;
    public List<Enemy> potentialEnemies;
    public List<Status> existingStatuses;
    public Battle currentBattle;
    public SpriteRenderer playerSpriteRenderer, enemySpriteRender;
    public GameObject playerStatus, enemyStatus, textbox, battleMenu, skillsMenuInBattle, battleEnd;
    public List<GameObject> playerStatusSlots, enemyStatusSlots, skillSlotsInBattle, itemSlots, skillSlots, levelUpButtons;

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
        player = new Player();
        player.party = new List<PlayerCharacter>();
        player.party.Add(Instantiate(existingPlayerCharacters[0]));
        player.party[0].RestoreDefault();

        // Give default skills to starting character
        foreach(Skill skill in player.party[0].startingSkills)
        {
            foreach(Skill existingSkill in existingSkills)
            {
                if (skill.name == existingSkill.name)
                    player.party[0].skills.Add(Instantiate(existingSkill));
            }
        }

        // Create inventory
        inventory = new List<InventorySlot>();

        // Show player sprite on title screen
        playerSpriteRenderer = GameObject.FindGameObjectWithTag("Player Sprite Renderer").GetComponent<SpriteRenderer>();
        playerSpriteRenderer.sprite = player.party[0].sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMainMenu()
    {
        playerStatusSlots.Clear();
        enemyStatusSlots.Clear();
        skillSlotsInBattle.Clear();
        itemSlots.Clear();
        skillSlots.Clear();
        levelUpButtons.Clear();
        currentBattle = null;
        player.party[0].Statuses.Clear();

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
            if (i < player.party[0].skills.Count)
            {
                skillSlotsInBattle[i].SetActive(true);
                skillSlotsInBattle[i].GetComponentInChildren<Text>().text = player.party[0].skills[i].name;
                skillSlotsInBattle[i].GetComponent<Buttons>().correspondingSkill = player.party[0].skills[i];
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

    public void SkillEffectHelper(Skill skill)
    {
        // Check mana cost
        if (player.party[0].Mana >= skill.manaCost[skill.level - 1])
        {
            player.party[0].Mana -= skill.manaCost[skill.level - 1];
        }
        else
        {
            DisplayText("Not enough mana.");
            return;
        }

        // Deal damage
        SkillEffect(skill, player.party[0], currentBattle.enemy);
        UpdateBattleStatus();

        // End turn
        skillsMenuInBattle.SetActive(false);

        // Check enemy defeated
        if (currentBattle.enemy.HP <= 0)
        {
            StartCoroutine(DefeatedEnemy());
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    public void SkillEffect(Skill skill, Character self, Character enemy)
    {
        string battleText = "";

        // Perform skill effect #1
        if (skill.applyValue.Length != 0)
        {
            List<Character> targets = new List<Character>();
            int appliedAmount = skill.applyValue[skill.level - 1];
            Status applyingStatus = null;

            // Check through existing statuses to find the one to apply
            foreach (Status existingStatus in existingStatuses)
            {
                if (existingStatus.statusType == skill.applyStatus)
                    applyingStatus = Instantiate(existingStatus);
            }

            // Get corresponding targets
            if (skill.applyTarget == Skill.Target.Self)
            {
                targets.Add(self);
                battleText += self.GetCharacterName();
            }
            else if (skill.applyTarget == Skill.Target.Enemy)
            {
                targets.Add(enemy);
                battleText += enemy.GetCharacterName();
            }
            else if (skill.applyTarget == Skill.Target.Both)
            {
                targets.Add(self);
                targets.Add(enemy);
                battleText += self.GetCharacterName() + " and " + enemy.GetCharacterName();
            }

            // Apply effect for each target
            foreach (Character target in targets)
            {
                target.AddStatus(applyingStatus, appliedAmount);
            }

            battleText += " received " + appliedAmount + " " + applyingStatus.statusType + "! ";
        }

        // Perform skill effect #2
        if (skill.convertToStatus != Status.StatusType.Null)
        {
            
        }

        // Perform skill effect #3
        if (skill.baseDamageValue.Length != 0)
        {
            List<Character> targets = new List<Character>();
            int damage = skill.baseDamageValue[skill.level - 1];

            // Get corresponding targets
            if (skill.damageTarget == Skill.Target.Self)
            {
                targets.Add(self);
                battleText += self.GetCharacterName();
            }
            else if (skill.damageTarget == Skill.Target.Enemy)
            {
                targets.Add(enemy);
                battleText += enemy.GetCharacterName();
            }
            else if (skill.damageTarget == Skill.Target.Both)
            {
                targets.Add(self);
                targets.Add(enemy);
                battleText += self.GetCharacterName() + " and " + enemy.GetCharacterName();
            }

            // Additional effect: deal damage for each stack of some status on self/enemy
            if(skill.damageForStatus != Status.StatusType.Null)
            {
                List<Character> secondaryTargets = new List<Character>();
                int multiplier = 0;

                // Get corresponding targets
                if (skill.damageForStatusOnTarget == Skill.Target.Self)
                {
                    secondaryTargets.Add(self);
                }
                else if (skill.damageForStatusOnTarget == Skill.Target.Enemy)
                {
                    secondaryTargets.Add(enemy);
                }
                else if (skill.damageForStatusOnTarget == Skill.Target.Both)
                {
                    secondaryTargets.Add(self);
                    secondaryTargets.Add(enemy);
                }

                // Check for statuses in each target
                foreach(Character secondaryTarget in secondaryTargets)
                {
                    // If there's a matching status, add its stack value to the multiplier
                    foreach(Status checkStatus in secondaryTarget.Statuses)
                    {
                        if (checkStatus.statusType == skill.damageForStatus)
                            multiplier += checkStatus.stackValue;
                    }
                }

                damage *= multiplier;
            }

            // Deal damage for each target
            foreach (Character target in targets)
            {
                target.HP = Mathf.Clamp(target.HP - damage, 0, target.MaxHP);
            }

            battleText += " received " + damage + " damage! ";
        }

        // Perform skill effect #4
        if (skill.bonusDamageValue.Length != 0)
        {
            
        }

        // Perform skill effect #5
        if (skill.applyStatusAfterDamageValue.Length != 0)
        {
            List<Character> targets = new List<Character>();
            int appliedAmount = skill.applyStatusAfterDamageValue[skill.level - 1];
            Status applyingStatus = null;

            // Check through existing statuses to find the one to apply
            foreach (Status existingStatus in existingStatuses)
            {
                if (existingStatus.statusType == skill.applyStatusAfterDamage)
                    applyingStatus = Instantiate(existingStatus);
            }

            // Get corresponding targets
            if (skill.applyStatusAfterDamageTarget == Skill.Target.Self)
            {
                targets.Add(self);
                battleText += self.GetCharacterName();
            }
            else if (skill.applyStatusAfterDamageTarget == Skill.Target.Enemy)
            {
                targets.Add(enemy);
                battleText += enemy.GetCharacterName();
            }
            else if (skill.applyStatusAfterDamageTarget == Skill.Target.Both)
            {
                targets.Add(self);
                targets.Add(enemy);
                battleText += self.GetCharacterName() + " and " + enemy.GetCharacterName();
            }

            // Apply effect for each target
            foreach (Character target in targets)
            {
                target.AddStatus(applyingStatus, appliedAmount);
            }

            battleText += " received " + appliedAmount + " " + applyingStatus.statusType + "! ";
        }

        // Perform skill effect #6
        if (skill.healValue.Length != 0)
        {
            
        }

        DisplayText(battleText);
    }

    /*public void CheckSkillTarget(Skill.Target skillTarget, List<Character> targets, string battleText, Character self, Character enemy)
    {
        if (skillTarget == Skill.Target.Self)
        {
            targets.Add(self);
            battleText += self.GetCharacterName();
        }
        else if (skillTarget == Skill.Target.Enemy)
        {
            targets.Add(enemy);
            battleText += enemy.GetCharacterName();
        }
        else if (skillTarget == Skill.Target.Both)
        {
            targets.Add(self);
            targets.Add(enemy);
            battleText += self.GetCharacterName() + " and " + enemy.GetCharacterName();
        }
    }*/

    public void LevelUpSkill(Skill skill)
    {
        for(int i = 0; i < player.party[0].skills.Count; i++)
        {
            if (player.party[0].skills[i].name == skill.name && player.party[0].skills[i].level < player.party[0].skills[i].maxLevel)
            {
                player.party[0].skills[i].level++;
                DisplayText(skill.ToString());
            }
        }
        player.party[0].skillPoints--;
        UpdateSkills();
    }

    public void UpdateBattleStatus()
    {
        // Update player status
        PlayerCharacter character = player.party[0];
        string playerStatusText = player.party[0].name + "\nHP: " + character.HP + "/" + character.MaxHP + "\nMana: " + character.Mana + "/" + character.MaxMana;
        playerStatus.GetComponentInChildren<Text>().text = playerStatusText;

        // If the player is not in battle, stop here
        if (currentBattle == null)
            return;

        // Show player special statuses
        for (int i = 0; i < playerStatusSlots.Count; i++)
        {
            if (i < player.party[0].Statuses.Count)
            {
                playerStatusSlots[i].SetActive(true);
                playerStatusSlots[i].GetComponentInChildren<Image>().sprite = player.party[0].Statuses[i].sprite;
                playerStatusSlots[i].GetComponentInChildren<Text>().text = player.party[0].Statuses[i].stackValue.ToString();
            }

            else
                playerStatusSlots[i].SetActive(false);
        }

        // Update enemy status
        Enemy currentEnemy = currentBattle.enemy;
        enemyStatus.GetComponentInChildren<Text>().text = currentEnemy.name + "\nHP: " + currentEnemy.HP + "/" + currentEnemy.MaxHP;

        // Show enemy special statuses
        for (int i = 0; i < enemyStatusSlots.Count; i++)
        {
            if (i < currentEnemy.Statuses.Count)
            {
                enemyStatusSlots[i].SetActive(true);
                enemyStatusSlots[i].GetComponentInChildren<Image>().sprite = currentEnemy.Statuses[i].sprite;
                enemyStatusSlots[i].GetComponentInChildren<Text>().text = currentEnemy.Statuses[i].stackValue.ToString();
            }

            else
                enemyStatusSlots[i].SetActive(false);
        }
    }

    public void UpdateSkills()
    {
        playerStatus.GetComponentInChildren<Text>().text = "Skill Points: " + player.party[0].skillPoints;

        // Show skills
        for (int i = 0; i < skillSlots.Count; i++)
        {
            if (i < player.party[0].skills.Count)
            {
                Skill currentSkill = player.party[0].skills[i];
                skillSlots[i].SetActive(true);
                skillSlots[i].GetComponentInChildren<Text>().text = "[" + currentSkill.manaCost[currentSkill.level - 1] + " Mana] LV " +
                    currentSkill.level + " " + currentSkill.name;
                skillSlots[i].GetComponent<Buttons>().correspondingSkill = currentSkill;
            }

            else
                skillSlots[i].SetActive(false);
        }

        // Show level up buttons
        foreach(GameObject levelUpButton in levelUpButtons)
        {
            Buttons button = levelUpButton.GetComponent<Buttons>().correspondingButton;
            PlayerCharacter character = player.party[0];
            if (button != null && button.correspondingSkill != null && (character.skillPoints == 0 || button.correspondingSkill.level == button.correspondingSkill.maxLevel))
            {
                levelUpButton.SetActive(false);
            }

            else if (button != null && button.correspondingSkill != null && (character.skillPoints > 0 && button.correspondingSkill.level < button.correspondingSkill.maxLevel))
            {
                levelUpButton.SetActive(true);
            }
        }
    }

    public void DisplayText(string text)
    {
        textbox.GetComponentInChildren<Text>().text = text;
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
        string[] _characterNames, _itemNames;
        string[][] _skillNames;
        int[] _itemQuantities;
        int[][] _skillLevels;
        int[] _characterLevels, _characterHp, _characterMana, _characterSkillPoints;

        // Save character data
        _characterNames = new string[player.party.Count];
        _characterLevels = new int[player.party.Count];
        _characterHp = new int[player.party.Count];
        _characterMana = new int[player.party.Count];
        _characterSkillPoints = new int[player.party.Count];

        for (int i = 0; i < player.party.Count; i++)
        {
            _characterNames[i] = player.party[i].name;
            _characterHp[i] = player.party[i].HP;
            _characterMana[i] = player.party[i].Mana;
            _characterSkillPoints[i] = player.party[i].skillPoints;
        }

        // Save skills
        _skillNames = new string[player.party.Count][];
        _skillLevels = new int[player.party.Count][];

        // Store skills in 2D array
        for (int i = 0; i < player.party.Count; i++)
        {
            _skillNames[i] = new string[player.party[i].skills.Count];
            _skillLevels[i] = new int[player.party[i].skills.Count];

            for(int j = 0; j < player.party[i].skills.Count; j++)
            {
                _skillNames[i][j] = player.party[i].skills[j].name;
                _skillLevels[i][j] = player.party[i].skills[j].level;
            }
        }

        // Save items
        _itemNames = new string[inventory.Count];
        _itemQuantities = new int[inventory.Count];

        for (int i = 0; i < inventory.Count; i++)
        {
            _itemNames[i] = inventory[i].item.name;
            _itemQuantities[i] = inventory[i].quantity;
        }

        // Save
        SaveManager.CreateSave(_characterNames, _itemNames, _skillNames, _itemQuantities, _skillLevels, _characterLevels, _characterHp, _characterMana, _characterSkillPoints);
    }

    public void LoadData()
    {
        Save loadedSave = SaveManager.LoadSave();

        // Load character data
        player.party.Clear();
        for(int i = 0; i < loadedSave.characterNames.Length; i++)
        {
            // Check existing player characters
            foreach(PlayerCharacter existingCharacter in existingPlayerCharacters)
            {
                // if saved character matches existing character, load with saved data
                if(loadedSave.characterNames[i] == existingCharacter.name)
                {
                    player.party.Add(Instantiate(existingCharacter));
                    player.party[i].Level = loadedSave.characterLevels[i];
                    player.party[i].HP = loadedSave.characterHp[i];
                    player.party[i].Mana = loadedSave.characterMana[i];
                    player.party[i].skillPoints = loadedSave.characterSkillPoints[i];
                    player.party[i].Statuses = new List<Status>();
                }
            }
        }

        // Load skills for each character
        for(int i = 0; i < loadedSave.characterNames.Length; i++)
        {
            player.party[i].skills.Clear();

            // Load each skill
            for(int j = 0; j < loadedSave.skillNames[i].Length; j++)
            {
                // Check existing skills
                foreach (Skill existingSkill in existingSkills)
                {
                    if(loadedSave.skillNames[i][j] == existingSkill.name)
                    {
                        player.party[i].skills.Add(Instantiate(existingSkill));
                        player.party[i].skills[j].level = loadedSave.skillLevels[i][j];
                    }
                }
            }
        }

        // Load items
        inventory.Clear();
        for (int i = 0; i < loadedSave.itemNames.Length; i++)
        {
            // Check existing items
            foreach (Item existingItem in existingItems)
            {
                // If loaded item matches existing item, load with saved data
                if (loadedSave.itemNames[i] == existingItem.name)
                {
                    inventory.Add(new InventorySlot(Instantiate(existingItem), loadedSave.itemQuantities[i]));
                }
            }
        }

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
                itemSlots[i].GetComponentInChildren<Text>().text = "[X" + inventory[i].quantity + "] " + inventory[i].item.name;
                itemSlots[i].GetComponent<Buttons>().correspondingItem = inventory[i].item;
            }

            else
                itemSlots[i].SetActive(false);
        }

        // If no items, show no items message
        if(inventory.Count == 0)
        {
            itemSlots[0].SetActive(true);
            itemSlots[0].GetComponentInChildren<Text>().text = "You have no items.";
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

        // If no skills, show no items message
        if (player.party[0].skills.Count == 0)
        {
            skillSlots[0].SetActive(true);
            skillSlots[0].GetComponentInChildren<Text>().text = "You have no skills.";
        }
    }

    IEnumerator LoadingBattle()
    {
        yield return new WaitForSeconds(0.00001f);

        // Get textboxes for showing status
        playerStatus = GameObject.FindGameObjectWithTag("Player Status");
        enemyStatus = GameObject.FindGameObjectWithTag("Enemy Status");

        // Get status slots for showing player special statuses
        foreach (GameObject statusSlot in GameObject.FindGameObjectsWithTag("Player Special Status"))
        {
            playerStatusSlots.Add(statusSlot);
        }

        // Get status slots for showing enemy special statuses
        foreach (GameObject statusSlot in GameObject.FindGameObjectsWithTag("Enemy Special Status"))
        {
            enemyStatusSlots.Add(statusSlot);
        }

        UpdateBattleStatus();

        // Get textbox for displaying battle text
        textbox = GameObject.FindGameObjectWithTag("Textbox");
        DisplayText("A " + currentBattle.enemy.name + " appeared!");

        // Get battle menu
        battleMenu = GameObject.FindGameObjectWithTag("Battle Menu");
        skillsMenuInBattle = GameObject.FindGameObjectWithTag("Skills Menu (Battle)");
        skillsMenuInBattle.SetActive(false);
        battleEnd = GameObject.FindGameObjectWithTag("Battle End");
        battleEnd.SetActive(false);

        // Get battle sprites
        playerSpriteRenderer = GameObject.FindGameObjectWithTag("Player Sprite Renderer").GetComponent<SpriteRenderer>();
        playerSpriteRenderer.sprite = player.party[0].sprite;
        enemySpriteRender = GameObject.FindGameObjectWithTag("Enemy Sprite Renderer").GetComponent<SpriteRenderer>();
        enemySpriteRender.sprite = currentBattle.enemy.sprite;
    }

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        Skill enemySkill = currentBattle.enemy.skills[Random.Range(0, currentBattle.enemy.skills.Count)];
        int damage = enemySkill.baseDamageValue[enemySkill.level - 1];

        player.party[0].HP -= damage;
        DisplayText("The enemy " + currentBattle.enemy.name + " used " + enemySkill.name + " and dealt " + damage + " damage.");
        UpdateBattleStatus();

        if (player.party[0].HP > 0)
            battleMenu.SetActive(true);
        else
            StartCoroutine(GameOver());
    }

    IEnumerator DefeatedEnemy()
    {
        yield return new WaitForSeconds(1f);

        player.party[0].skillPoints++;
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

    /*public class Player
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
    }*/

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
            enemy.RestoreDefault();
        }
    }
}
