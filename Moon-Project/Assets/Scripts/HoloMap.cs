using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloMap : MonoBehaviour
{
    public List<GameObject> mapPeicees;
    public Material hologramMaterial;
    public Transform hologramCenter;
    public Renderer projectorBeam;
    public Light hologramLight;
    public float hologramScale = 0.01f;
    private ModularWorldGenerator m_gen;
    public bool trueRotation = true;
    public bool active = false;

    private bool m_initialSpawned = false;

    private void Start()
    {
        if (!m_gen) m_gen = GameObject.Find("GENERATED").GetComponent<ModularWorldGenerator>();
    }

    private void Update()
    {
        if(!m_initialSpawned)
        {
            if(m_gen.generationComplete)
            {
                RespawnPeices();
            }
        }

        if(trueRotation)
        {
            hologramCenter.rotation = Quaternion.identity;
        }
    }

    public void RespawnPeices()
    {
        if(mapPeicees.Count >= 1)
        {
            foreach (GameObject module in mapPeicees)
            {
                GameObject.Destroy(module);
            }
        }
        mapPeicees = new List<GameObject>();

        foreach(RoomModule module in m_gen.GetSpawnedModules())
        {
           if(module.moduleCode != "null")
            {
                GameObject newMapObject = Instantiate(module.GetMapObject(), hologramCenter);
                newMapObject.transform.localPosition = module.GetMapObject().transform.position * hologramScale;
                newMapObject.transform.localRotation = module.GetMapObject().transform.transform.rotation;
                newMapObject.transform.localScale = module.GetMapObject().transform.localScale * hologramScale;
                foreach (Renderer r in newMapObject.GetComponentsInChildren<Renderer>())
                {
                    r.material = hologramMaterial;
                    r.gameObject.layer = LayerMask.NameToLayer("World");
                }
                mapPeicees.Add(newMapObject);
            }
        }

        m_initialSpawned = true;
        TurnOn();
    }

    public void TurnOn()
    {
        foreach(GameObject module in mapPeicees)
        {
            module.SetActive(true);
        }
        if (hologramLight) hologramLight.enabled = true;
        if (projectorBeam) projectorBeam.enabled = true;
        active = true;
    }
    public void TurnOff()
    {
        foreach (GameObject module in mapPeicees)
        {
            module.SetActive(false);
        }
        if (hologramLight) hologramLight.enabled = false;
        if (projectorBeam) projectorBeam.enabled = false;
        active = false;
    }
    public void Toggle()
    {
        if (active) TurnOff();
        else TurnOn();
    }
}