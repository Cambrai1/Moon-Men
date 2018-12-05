using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticMethods
{
    public static Vector2 TopDown(Vector3 _vec)
    {
        Vector2 result = Vector2.zero;

        result.x = _vec.x;
        result.y = _vec.y;

        return result;
    }

    private static List<int> m_usedIds;
    public static int GetUniqueInt()
    {
        if(m_usedIds == null)
        {
            m_usedIds = new List<int>();
        }
        bool foundUnique = false;
        int newId = 0;
        while(foundUnique == false)
        {
            newId = Random.Range((int)0, (int)999999);
            bool foundInList = false;
            foreach(int i in m_usedIds)
            {
                if (newId == i) foundInList = true;
            }
            if (foundInList == false) foundUnique = true;
        }
        return newId;
    }

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}