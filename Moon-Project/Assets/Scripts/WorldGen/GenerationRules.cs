using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationRules", menuName ="Generation Rules", order = 1)]
public class GenerationRules : ScriptableObject
{
    public int maximumRooms = 30;
    public int minimumRooms = 20;
    public GenerationMethod generationMethod;
    public List<ModuleRule> moduleRules;

    public void SetUp()
    {
        foreach(ModuleRule rule in moduleRules)
        {
            if(rule.bannedCodes.Length != 0)
            {
                rule.bannedCodesArray = rule.bannedCodes.Split(',');
            }
            else
            {
                rule.bannedCodesArray = new string[0];
            }
        }
    }
}

public enum GenerationMethod
{
    original,
    predictive
}

[System.Serializable]
public class ModuleRule
{
    public string moduleCode;
    public int minimum = 0;
    public int maximum = 999;
    public string bannedCodes;
    public string[] bannedCodesArray;
}