using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public string[] characterNames, itemNames;
    public string[][] skillNames;
    public int[] itemQuantities;
    public int[][] skillLevels;
    public int[] characterLevels, characterHp, characterMana, characterSkillPoints;

    public Save(string[] _characterNames, string[] _itemNames, string[][] _skillNames, int[] _itemQuantities, int[][] _skillLevels, int[] _characterLevels, int[] _characterHp,
        int[] _characterMana, int[] _characterSkillPoints)
    {
        characterNames = _characterNames;
        itemNames = _itemNames;
        skillNames = _skillNames;
        itemQuantities = _itemQuantities;
        skillLevels = _skillLevels;
        characterLevels = _characterLevels;
        characterHp = _characterHp;
        characterMana = _characterMana;
        characterSkillPoints = _characterSkillPoints;
    }
}
