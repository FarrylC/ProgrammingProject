using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Character
{
    int Level { get; set; }
    int HP { get; set; }
    int MaxHP { get; set; }
    int Mana { get; set; }
    int MaxMana { get; set; }
    int PhysicalAttack { get; set; }
    int MagicalAttack { get; set; }
    int PhysicalDefense { get; set; }
    int MagicalDefense { get; set; }
    int Speed { get; set; }

    List<Status> Statuses { get; set; }
    void AddStatus(Status status, int stackValue);

    string GetCharacterName();
}
