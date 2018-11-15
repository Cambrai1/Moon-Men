using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ModularWorldGenerator : MonoBehaviour
{
    public bool autoModules = true;
	public RoomModule[] modules;
	public RoomModule startModule;

	public int iterations = 5;
    public enum GenerationMethod { widthFirst, depthFirst };
    public GenerationMethod generationMethod;
    public bool visibleIterations = true;

    private int m_totalRarity;


	void Start()
	{
        if (autoModules) GetAutoModules();
        foreach(RoomModule mod in modules)
        {
            mod.SetRarityMinMax(m_totalRarity);
            m_totalRarity += mod.rarity;
        }
        Debug.Log("Loaded " + modules.Length + " modules, total rarity " + m_totalRarity + "!");

        Regenerate();
	}

    private void GetAutoModules()
    {
        int childCount = transform.childCount;
        modules = new RoomModule[childCount];
        for (int i = 0; i < childCount; i++)
        {
            modules[i] = transform.GetChild(i).GetComponent<RoomModule>();
        }
        startModule = modules[0];
    }
    
    public void Regenerate()
    {
        StartCoroutine(Regenerate(iterations));
    }
    private IEnumerator Regenerate(int _iterations)
    {
        GenerationMethod method = generationMethod;
        RoomModule startingModule = Instantiate(startModule, transform.position, transform.rotation).GetComponent<RoomModule>();
        startingModule.gameObject.SetActive(true);
        List<ModuleConnector> pendingConnections = new List<ModuleConnector>(startModule.GetExits());
        int roomCount = 0;

        float startTime = Time.time;

        switch (method)
        {
            case GenerationMethod.widthFirst:
                {
                    for(int i = 0; i < _iterations; i++)
                    {
                        while(pendingConnections.Count >= 1)
                        {
                            if (pendingConnections[0] != null)
                            {
                                RoomModule newModule = GameObject.Instantiate(GetRandomModuleForConnector(pendingConnections[0], true), transform).GetComponent<RoomModule>();
                                AlignConnectors(pendingConnections[0], newModule.GetEntrance());
                                newModule.gameObject.SetActive(true);
                                pendingConnections[0].linkedConnector = newModule.GetEntrance();
                                newModule.GetEntrance().linkedConnector = pendingConnections[0];
                                newModule.SetId(roomCount);
                                Debug.Log("Spawned Room " + roomCount + ": " + newModule.moduleCode);
                                roomCount++;
                                pendingConnections.RemoveAt(0);

                                foreach(ModuleConnector con in newModule.GetExits())
                                {
                                    pendingConnections.Add(con);
                                }

                                if (visibleIterations) yield return new WaitForEndOfFrame();
                            }
                            else
                            {
                                Debug.LogWarning("PENDING CONNECTIONS [0] SOMEHOW IS NULL");
                            }
                        }
                    }
                }
                break;

            case GenerationMethod.depthFirst:
                {
                    for (int i = 0; i < _iterations; i++)
                    {
                        while (pendingConnections.Count >= 1)
                        {
                            int c = pendingConnections.Count - 1;
                            if (pendingConnections[c] != null)
                            {
                                RoomModule newModule = Instantiate(GetRandomModuleForConnector(pendingConnections[c], true)).GetComponent<RoomModule>();
                                AlignConnectors(pendingConnections[c], newModule.GetEntrance());
                                newModule.gameObject.SetActive(true);
                                pendingConnections[0].linkedConnector = newModule.GetEntrance();
                                newModule.GetEntrance().linkedConnector = pendingConnections[c];
                                newModule.SetId(roomCount);
                                Debug.Log("Spawned Room " + roomCount + ": " + newModule.moduleCode);
                                roomCount++;
                                pendingConnections.RemoveAt(c);

                                foreach (ModuleConnector con in newModule.GetExits())
                                {
                                    pendingConnections.Add(con);
                                }

                                if (visibleIterations) yield return new WaitForSecondsRealtime(0.1f);
                            }
                            else
                            {
                                Debug.LogWarning("PENDING CONNECTIONS [" + c + "] SOMEHOW IS NULL");
                            }
                        }
                    }
                }
                break;
        }

        Debug.Log("GENERATED WORLD IN " + (Time.time - startTime) + " SECONDS");

        yield return null;
    }

	private void AlignConnectors(ModuleConnector oldExit, ModuleConnector newExit)
	{
        Transform newModule = newExit.ParentModule().gameObject.transform;
		var forwardVectorToMatch = -oldExit.transformPoint.forward;
		var correctiveRotation = Azimuth(forwardVectorToMatch) - Azimuth(newExit.transformPoint.forward);
		newModule.RotateAround(newExit.transformPoint.position, Vector3.up, correctiveRotation);
		var correctiveTranslation = oldExit.transformPoint.position - newExit.transformPoint.position;
		newModule.transform.position += correctiveTranslation;
	}

	private GameObject GetRandomModuleForConnector(ModuleConnector _connector, bool _useRarity)
	{
        bool foundModule = false;

        if (_connector == null) return GetSpecificModule("straight");

        int useModId = 0;

        while (!foundModule)
        {
            if(_useRarity)
            {
                int random = Random.Range(0, m_totalRarity);
                for (int i = 0; i < modules.Length; i++)
                {
                    if (!foundModule)
                    {
                        if(modules[i].unique)
                        {

                        }
                        else if (random >= modules[i].GetMinRarity() && random <= modules[i].GetMaxRarity())
                        {
                            useModId = i;
                            foundModule = true;
                        }
                    }
                }
            }
            else
            {
                int random = Random.Range(0, _connector.allowedCodesArray.Length);
                string randomModuleCode = _connector.allowedCodesArray[random];
                for (int i = 0; i < modules.Length; i++)
                {
                    if (modules[i].unique)
                    {

                    }
                    else if (modules[i].moduleCode == randomModuleCode)
                    {
                        useModId = i;
                        foundModule = true;
                    }
                }
            }
        }

        return modules[useModId].gameObject;
    }
    private GameObject GetSpecificModule(string _code)
    {
        for (int i = 0; i < modules.Length; i++)
        {
            if (modules[i].moduleCode == _code)
            {
                return modules[i].gameObject;
            }
        }
        return null;
    }

	private static float Azimuth(Vector3 vector)
	{
		return Vector3.Angle(Vector3.forward, vector) * Mathf.Sign(vector.x);
	}
}
