using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager s_soundManager;

    public static SoundManager GetSoundManager()
    {
        return s_soundManager;
    }

    [SerializeField] AudioSource m_backgroundMusic;
    [SerializeField] AudioSource m_mainPlayer;
    readonly Dictionary<int, AudioClip> m_soundClips = new Dictionary<int, AudioClip>();

    private void Awake()
    {
        s_soundManager = this;
    }
    //play the associated clip.
    public void PlaySound(int id)
    {
        m_mainPlayer.clip = m_soundClips[id];
        m_mainPlayer.Play();
        m_mainPlayer.time = 0.03f;
    }
    //Add the Audio Clip to the register
    public int RegisterSoundToAction(AudioClip clip)
    {
        //See if the audio Clip is already registered
        foreach (int key in m_soundClips.Keys)
        {
            if (m_soundClips[key] == clip)
            {
                return key;
            }
        }
        //otherwise create a new key
        int id;
        do
        {
            id = UnityEngine.Random.Range(0, int.MaxValue);
        }
        while (m_soundClips.ContainsKey(id));

        m_soundClips.Add(id, clip);
        return id;
    }

}
