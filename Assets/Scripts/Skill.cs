using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
public class Skill : ScriptableObject
{
    public enum Target { None, Self, Enemy, Both };

    public new string name;
    public string description;
    public int level, maxLevel;
    
    public int[] manaCost;

    [Header("Effect #1: Apply n stacks of some status to self/enemy")]
    public int[] applyValue;
    public Status.StatusType applyStatus;
    public Target applyTarget;

    [Header("Effect #2: Convert all/some stacks of some status A on self/enemy into some status B")]
    public bool doesConvertAll;
    public int[] convertSomeValue;
    public Status.StatusType convertFromStatus;
    public Target convertTarget;
    public Status.StatusType convertToStatus;

    [Header("Effect #3: Deal n base magical/physical damage to self or/and enemy for each stack of some status on self/enemy")]
    public int[] baseDamageValue;
    public bool isMagicalDamage;
    public Target damageTarget;
    public Status.StatusType damageForStatus;
    public Target damageForStatusOnTarget;

    [Header("Effect #4: If the target has some status, base damage increases by n%")]
    public Target bonusForStatusOnTarget;
    public Status.StatusType bonusForStatus;
    public int[] bonusDamageValue;

    [Header("Effect #5: Apply n stacks of some status to self/enemy after damage")]
    public int[] applyStatusAfterDamageValue;
    public Status.StatusType applyStatusAfterDamage;
    public Target applyStatusAfterDamageTarget;

    [Header("Effect #6: Heal n/n% of max mana/HP for each stack of some status on self/enemy")]
    public int[] healValue;
    public bool isPercentage;
    public bool doesHealMana;
    public Status.StatusType healForStatus;
    public Target healForStatusOnTarget;

    public string TargetToString(Target someTarget)
    {
        if (someTarget == Target.Self)
            return "self";
        else if (someTarget == Target.Enemy)
            return "the enemy";
        else if (someTarget == Target.Both)
            return "self and the enemy";
        else
            return "no target";
    }

    public string Effect()
    {
        string effectDescription = "";
        bool hasMultipleEffects = false;

        // Add effect #1 to description
        if(applyValue.Length != 0)
        {
            effectDescription += "Apply " + applyValue[level - 1] + " stacks of " + applyStatus + " to " + TargetToString(applyTarget) + ".";
            hasMultipleEffects = true;
        }

        // Add effect #2 to description
        if(convertToStatus != Status.StatusType.Null)
        {
            if (hasMultipleEffects)
                effectDescription += " ";

            effectDescription += "Convert ";

            if (doesConvertAll)
                effectDescription += "all stacks";
            else
            {
                effectDescription += convertSomeValue[level - 1];
                if (convertSomeValue[level - 1] > 1)
                    effectDescription += " stacks";
                else
                    effectDescription += " stack";
            }

            effectDescription += " of " + convertFromStatus + " into " + convertToStatus + ".";
            hasMultipleEffects = true;
        }

        // Add effect #3 to description
        if(baseDamageValue.Length != 0)
        {
            if (hasMultipleEffects)
                effectDescription += " ";

            effectDescription += "Deal a base of " + baseDamageValue[level - 1] + " ";

            if (isMagicalDamage)
                effectDescription += "magical";
            else
                effectDescription += "physical";

            effectDescription += " damage to " + TargetToString(damageTarget);

            if(damageForStatus != Status.StatusType.Null)
            {
                effectDescription += " for each stack of " + damageForStatus + " on " + TargetToString(damageForStatusOnTarget);
            }

            effectDescription += ".";
            hasMultipleEffects = true;
        }

        // Add effect #4 to description
        if(bonusDamageValue.Length != 0)
        {
            if (hasMultipleEffects)
                effectDescription += " ";

            effectDescription += "If there is " + bonusForStatus + " on " + TargetToString(bonusForStatusOnTarget) + ", base damage increases by " + bonusDamageValue[level - 1] +
                ".";
            hasMultipleEffects = true;
        }

        // Add effect #5 to description
        if(applyStatusAfterDamageValue.Length != 0)
        {
            if (hasMultipleEffects)
                effectDescription += " ";

            effectDescription += "Then, apply " + applyStatusAfterDamageValue[level - 1] + " " + applyStatusAfterDamage + " to " + TargetToString(applyStatusAfterDamageTarget) +
                ".";
            hasMultipleEffects = true;
        }

        // Add effect #6 to description
        if(healValue.Length != 0)
        {
            if (hasMultipleEffects)
                effectDescription += " ";

            effectDescription += "Heal " + healValue[level - 1];

            if (isPercentage)
                effectDescription += "%";

            effectDescription += "of max ";

            if (doesHealMana)
                effectDescription += "mana";
            else
                effectDescription += "HP";

            if(healForStatus != Status.StatusType.Null)
            {
                effectDescription += " for each stack of " + healForStatus + " on " + TargetToString(healForStatusOnTarget);
            }

            effectDescription += ".";
            hasMultipleEffects = true;
        }

        return effectDescription;
    }

    public override string ToString()
    {
        string toString = "[" + manaCost[level - 1] + " Mana] LV" + level + " " + name + ": " + description + " (" + Effect() + ")";
        return toString;
    }
}
