using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GenerationRules", menuName ="Generation Rules", order = 1)]
public class GenerationRules : ScriptableObject
{
    public int maximumRooms = 30;
    public int minimumRooms = 20;
    public int iterations = 2;
    public GenerationMethod generationMethod;
}

public enum GenerationMethod
{
    widthFirst,
    depthFirst
}