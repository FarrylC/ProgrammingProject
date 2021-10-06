using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public string[] skillNames, itemNames;
    public int[] skillLevels, skillCosts, skillAttacks, itemQuantities;
    public int playerHp, playerMaxHp, playerMana, playerMaxMana, playerSkillPoints;

    public Save(string[] _skillNames, string[] _itemNames, int[] _skillLevels, int[] _skillCosts, int[] _skillAttacks, int[] _itemQuantities, int _playerHp, int _playerMaxHp,
        int _playerMana, int _playerMaxMana, int _playerSkillPoints)
    {
        skillNames = _skillNames;
        itemNames = _itemNames;
        skillLevels = _skillLevels;
        skillCosts = _skillCosts;
        skillAttacks = _skillAttacks;
        itemQuantities = _itemQuantities;
        playerHp = _playerHp;
        playerMaxHp = _playerMaxHp;
        playerMana = _playerMana;
        playerMaxMana = _playerMaxMana;
        playerSkillPoints = _playerSkillPoints;
    }
}
