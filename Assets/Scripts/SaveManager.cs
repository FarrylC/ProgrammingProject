using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    public static void CreateSave(string[] _skillNames, string[] _itemNames, int[] _skillLevels, int[] _skillCosts, int[] _skillAttacks, int[] _itemQuantities, int _playerHp,
        int _playerMaxHp, int _playerMana, int _playerMaxMana, int _playerSkillPoints)
    {
        // Create formatter for binary formatting
        BinaryFormatter formatter = new BinaryFormatter();

        // Create path for where the save will be located
        string path = Application.persistentDataPath + "/save_file";
        FileStream stream = new FileStream(path, FileMode.Create);

        // Create save data
        Save save = new Save(_skillNames, _itemNames, _skillLevels, _skillCosts, _skillAttacks, _itemQuantities, _playerHp, _playerMaxHp, _playerMana, _playerMaxMana,
            _playerSkillPoints);

        // Serialize and close the stream
        formatter.Serialize(stream, save);
        stream.Close();
    }

    public static Save LoadSave()
    {
        string path = Application.persistentDataPath + "/save_file";

        // Check if path exists
        if (File.Exists(path))
        {
            // Create formatter and open path
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            // Deserialize save data
            Save save = formatter.Deserialize(stream) as Save;
            stream.Close();

            // Return save data
            return save;
        }

        // If the path doesn't exist, display error message
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
