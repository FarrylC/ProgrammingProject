using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status", menuName = "Status")]
public class Status : ScriptableObject
{
    public enum StatusType { Null, Charge, Burn, Frost, Soak };
    public StatusType statusType;

    public enum PlayerStat { Null, HP, MaxHP, Mana, MaxMana, PhysicalAttack, MagicalAttack, PhysicalDefense, MagicalDefense, Speed};
    public Sprite sprite;

    [Header("Effect #1: Character gains/loses n/n% of some stat for each stack of some status A whenever the character receives some status B")]
    public bool isGain;
    public int gainLoseValue;
    public bool isPercentage;
    public PlayerStat gainLoseStat;
    public StatusType gainLoseForStatus;
    public StatusType whenReceiving;

    [Header("Effect #2: Skills cost n/n% more/less mana for each stack of some status")]
    public int moreLessValue;
    public bool moreLessIsPercentage;
    public bool doesCostMore;
    public StatusType moreLessForStatus;

    [Header("Effect #3: Stack increases/decreases by n at the start/end of each turn, plus/minus m for each stack of some status")]
    public bool doesIncrease;
    public int increaseDecreaseValue;
    public bool doesAtStartOfTurn;
    public bool doesAddToValue;
    public int bonusValue;
    public StatusType increaseDecreaseForStatus;

    [Header("Current and max value of status")]
    public int stackValue;
    public int maxStackValue;

    public string Effect()
    {
        string effectDescription = statusType + ": ";

        // Add effect #1 to description
        if(gainLoseValue != 0)
        {
            effectDescription += "Character ";

            if (isGain)
                effectDescription += "gains ";
            else
                effectDescription += "loses ";

            effectDescription += gainLoseValue;

            if (isPercentage)
                effectDescription += "% of ";

            if (gainLoseStat == PlayerStat.HP)
                effectDescription += "HP";
            else if (gainLoseStat == PlayerStat.MaxHP)
                effectDescription += "max HP";
            else if (gainLoseStat == PlayerStat.Mana)
                effectDescription += "mana";
            else if (gainLoseStat == PlayerStat.MaxMana)
                effectDescription += "max mana";
            else if (gainLoseStat == PlayerStat.PhysicalAttack)
                effectDescription += "physical attack";
            else if (gainLoseStat == PlayerStat.MagicalAttack)
                effectDescription += "magical attack";
            else if (gainLoseStat == PlayerStat.PhysicalDefense)
                effectDescription += "physical defense";
            else if (gainLoseStat == PlayerStat.MagicalDefense)
                effectDescription += "magical defense";
            else if (gainLoseStat == PlayerStat.Speed)
                effectDescription += "speed";

            if(gainLoseForStatus != StatusType.Null)
                effectDescription += " for each stack of " + gainLoseForStatus;

            if (whenReceiving != StatusType.Null)
                effectDescription += " whenever the character receives " + whenReceiving;

            effectDescription += ". ";
        }

        // Add effect #2 to description
        if (moreLessValue != 0)
        {
            effectDescription += "Skills cost " + moreLessValue;

            if (moreLessIsPercentage)
                effectDescription += "%";

            if (doesCostMore)
                effectDescription += " more";
            else
                effectDescription += " less";

            if (moreLessForStatus != StatusType.Null)
                effectDescription += " mana for each stack of " + moreLessForStatus;

            effectDescription += ". ";
        }

        // Add effect #3 to description
        if (increaseDecreaseValue != 0)
        {
            effectDescription += "Stack ";

            if (doesIncrease)
                effectDescription += "increases ";
            else
                effectDescription += "decreases ";

            effectDescription += "by " + increaseDecreaseValue;

            if (doesAtStartOfTurn)
                effectDescription += " at the start of each turn";
            else
                effectDescription += " at the end of each turn";

            if(bonusValue != 0)
            {
                if (doesAddToValue)
                    effectDescription += ", plus ";
                else
                    effectDescription += ", minus ";

                effectDescription += bonusValue + " for each stack of " + increaseDecreaseForStatus;
            }

            effectDescription += ". ";
        }

        return effectDescription;
    }
}
