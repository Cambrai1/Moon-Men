using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioBehaviour : MonoBehaviour
{

    public AudioSource[] sourcesInScene;
    List<AudioSource> sourcesList;

    // Use this for initialization
    void Start()
    {
        StartCoroutine("SoundsTick");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private AudioSource j;

    IEnumerator SoundsTick()
    {
        while (true)
        {
            sourcesList = new List<AudioSource>();
            for (int x = 0; x < sourcesInScene.Length; x++)
            {
                sourcesList.Add(sourcesInScene[x]);
            }
            for (int z = 0; z < sourcesList.Count; z++)
            {
                yield return new WaitForSeconds(10);
                int i = Random.Range(0, sourcesList.Count);
                j = sourcesList[i];
                sourcesList[i].Play();
                sourcesList.Remove(j);
            }
        }
    }
}
