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
}

public enum GenerationMethod
{
    widthFirst,
    depthFirst
}

[System.Serializable]
public class ModuleRule
{
    public string moduleCode;
    public int minimum = 0;
    public int maximum = 999;
}