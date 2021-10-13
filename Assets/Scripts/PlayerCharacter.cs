using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Character", menuName = "Player Character")]
public class PlayerCharacter : ScriptableObject, Character
{
    public new string name;
    private int  hp, mana;
    [SerializeField] private int level, maxHp, maxMana, physicalAttack, magicalAttack, physicalDefense, magicalDefense, speed;
    public Sprite sprite;
    public List<Skill> startingSkills, skills;
    [HideInInspector] public int skillPoints;
    public AnimationCurve maxHpCurve, maxManaCurve, physAtkCurve, magiAtkCurve, physDefCurve, magiDefCurve, speedCurve;
    private List<Status> statuses;

    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
        }
    }

    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
        }
    }

    public int MaxHP
    {
        get
        {
            return maxHp;
        }
        set
        {
            maxHp = value;
        }
    }

    public int Mana
    {
        get
        {
            return mana;
        }
        set
        {
            mana = value;
        }
    }

    public int MaxMana
    {
        get
        {
            return maxMana;
        }
        set
        {
            maxMana = value;
        }
    }

    public int PhysicalAttack
    {
        get
        {
            return physicalAttack;
        }
        set
        {
            physicalAttack = value;
        }
    }

    public int MagicalAttack
    {
        get
        {
            return magicalAttack;
        }
        set
        {
            magicalAttack = value;
        }
    }

    public int PhysicalDefense
    {
        get
        {
            return physicalDefense;
        }
        set
        {
            physicalDefense = value;
        }
    }

    public int MagicalDefense
    {
        get
        {
            return magicalDefense;
        }
        set
        {
            magicalDefense = value;
        }
    }

    public int Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }

    public List<Status> Statuses
    {
        get
        {
            return statuses;
        }
        set
        {
            statuses = value;
        }
    }

    public void AddStatus(Status status, int stackValue)
    {
        // If the character does not have any statuses, add the status
        if(statuses.Count == 0)
        {
            statuses.Insert(0, status);
            statuses[0].stackValue += stackValue;
            return;
        }

        // Otherwise, go through the character's existing statuses
        foreach (Status currentstatus in statuses)
        {
            // If the character already has the applied status, add to its stack value
            if (currentstatus.statusType == status.statusType)
            {
                currentstatus.stackValue += stackValue;
            }

            // Otherwise, give the character that status
            else
            {
                statuses.Insert(0, status);
                statuses[0].stackValue += stackValue;
            }
        }
    }

    public string GetCharacterName()
    {
        return name;
    }

    public void RestoreDefault()
    {
        hp = maxHp;
        mana = maxMana;
        statuses = new List<Status>();
    }
}
