using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private List<EventInstance> events;
    private List<StudioEventEmitter> emitters;

    private EventInstance musicEventInstance;
    private void Awake()
    {
        if (Instance != this)
            Debug.LogError("Singleton Error for audio manager");

        Instance = this;

        events = new List<EventInstance>();
        emitters = new List<StudioEventEmitter>();
    }

    private void Start()
    {
        InitializeMusic(FMODEvents.instance.music);
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventRef, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventRef;
        emitters.Add(emitter);
        return emitter;
    }

    public EventInstance CreateEventInstance(EventReference eventRef)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        events.Add(eventInstance);
        return eventInstance;

    }

    public void SetMusicVersion(float version)
    {
        musicEventInstance.setParameterByName("Triggre", version);
    }

    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateEventInstance(musicEventReference);
        musicEventInstance.start();
    }
    private void CleanUp()
    {
        foreach(EventInstance eventInstance in events)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        // stop all of the event emitters, because if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in emitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}