using StageSelection;
using System.Collections.Generic;

using CustomInput.Information;

using UnityEngine;
using UnityEngine.EventSystems;

public class AudioManager : SubManager<AudioManager>
{
    public List<AudioClip> musicList;
    public AudioClip clickSound;
    public AudioClip dragSound;

    private AudioSource m_MusicSource;
    private AudioSource m_MenuSource;
    private bool m_MuteMusic;
    private bool m_MuteSoundEffects;

    public void ChangeMusic(int sceneIndex)
    {
        if (m_MusicSource == null)
        {
            m_MusicSource = gameObject.AddComponent<AudioSource>();
            m_MusicSource.loop = true;
        }
        m_MusicSource.clip = musicList[sceneIndex];
        m_MusicSource.Play();
    }

    protected override void OnPress(TouchInformation touchInfo)
    {
        if (m_MuteSoundEffects)
            return;

        m_MenuSource.clip = clickSound;
        if (EventSystem.current != null)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
                m_MenuSource.Play();
        }

        // See if Hit certain GameObject
        var ray = Camera.main.ScreenPointToRay(touchInfo.position);
        var hit = new RaycastHit();                                     // Make a Hit

        //See if the ray hit anything
        if (!(Physics.Raycast(ray.origin, ray.direction, out hit)))       // If not, stop execution
            return;

        var obj = hit.transform.gameObject;
        if(obj.GetComponent<MonoNode>() != null)
            m_MenuSource.Play();
    }

    public void PlayDragSound()
    {
        if (m_MuteSoundEffects)
            return;
        m_MenuSource.clip = dragSound;
        m_MenuSource.Play();
    }

    public void MuteMusicToggle()
    {
        // Mute
        if (m_MuteMusic == false)
        {
            m_MusicSource.Pause();
        }
        // Unmute
        else
        {
            m_MusicSource.Play();
        }

        m_MuteMusic = !m_MuteMusic;
    }

    public void MuteSoundsToggle()
    {
        m_MuteSoundEffects = !m_MuteSoundEffects;
    }

    protected override void Init()
    {
        DontDestroyOnLoad(gameObject);

        m_MenuSource = gameObject.AddComponent<AudioSource>();
        m_MenuSource.clip = clickSound;

        ChangeMusic(0);
    }
}