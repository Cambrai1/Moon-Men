using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloMap : MonoBehaviour
{
    public List<GameObject> mapPieces;
    public Material hologramMaterial;
    public Transform hologramCenter;
    public Renderer projectorBeam;
    public Light hologramLight;
    public ParticleSystemRenderer particles;
    public float hologramScale = 0.01f;
    private ModularWorldGenerator m_gen;
    public bool trueRotation = true;
    public bool active = false;

    private List<Renderer> m_renderers;

    private bool m_initialSpawned = false;
    private Vector3 m_originalCenter;

    private void Start()
    {
        if (!m_gen) m_gen = GameObject.Find("GENERATED").GetComponent<ModularWorldGenerator>();
        m_originalCenter = hologramCenter.localPosition;
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

        if(active && m_initialSpawned)
        {
            foreach (Renderer r in m_renderers)
            {
                Vector3 diff = transform.position;
                r.material.SetVector("_WaveOrigin", diff);
            }
        }
    }

    public void RespawnPeices()
    {
        if(mapPieces.Count >= 1)
        {
            foreach (GameObject module in mapPieces)
            {
                GameObject.Destroy(module);
            }
        }
        mapPieces = new List<GameObject>();
        m_renderers = new List<Renderer>();


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
                    r.material.SetVector("_LocalOffset", r.transform.position);
                    r.gameObject.layer = LayerMask.NameToLayer("World");
                    m_renderers.Add(r);
                }
                mapPieces.Add(newMapObject);
            }
        }

        float minX = 0, maxX = 0, minZ = 0, maxZ = 0, cX = 0, cZ = 0;
        foreach (GameObject r in mapPieces)
        {
            if (r.transform.localPosition.x < minX) minX = r.transform.localPosition.x;
            if (r.transform.localPosition.x > maxX) maxX = r.transform.localPosition.x;
            if (r.transform.localPosition.z < minZ) minZ = r.transform.localPosition.z;
            if (r.transform.localPosition.z > maxZ) maxZ = r.transform.localPosition.z;
        }
        cX = Mathf.Lerp(minX, maxX, 0.5f);
        cZ = Mathf.Lerp(minZ, maxZ, 0.5f);
        Debug.Log("CX : " + cX);
        Debug.Log("CZ : " + cZ);

        cX = minX + 0.5f * (maxX - minX);
        cZ = minZ + 0.5f * (maxZ - minZ);

        foreach (GameObject obj in mapPieces)
        {
            Vector3 newPos = obj.transform.localPosition;
            newPos.x -= cX;
            newPos.z -= cZ;
            obj.transform.localPosition = newPos;
        }

        m_initialSpawned = true;
        if (active) TurnOn();
        else TurnOff();
    }

    public void TurnOn()
    {
        foreach(GameObject module in mapPieces)
        {
            module.SetActive(true);
        }
        if (hologramLight) hologramLight.enabled = true;
        if (projectorBeam) projectorBeam.enabled = true;
        if (particles) particles.enabled = true;
        active = true;
    }
    public void TurnOff()
    {
        foreach (GameObject module in mapPieces)
        {
            module.SetActive(false);
        }
        if (hologramLight) hologramLight.enabled = false;
        if (projectorBeam) projectorBeam.enabled = false;
        if (particles)
            if (particles) particles.enabled = false;
        active = false;
    }
    public void Toggle()
    {
        if (active) TurnOff();
        else TurnOn();
    }
}