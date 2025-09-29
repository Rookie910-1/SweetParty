using System.Collections;
using UnityEngine;
[AddComponentMenu("PLAYER TWO/Scripts/Misc/Glider")]
public class Glider :MonoBehaviour
{

    public Player Player;

    public AudioSource m_audio;

    public TrailRenderer[] trails;

    public float scaleDuration = 0.7f;

    public AudioClip openAudio;
    
    public AudioClip closeAudio;
    protected virtual void Start()
    {
        InitializePlayer();
        InitializeAudio();
        InitializeCallbacks();
        InitializeGlider();
    }

    public virtual void InitializePlayer()
    {
        if (!Player)
            Player = GetComponentInParent<Player>();
    }
    
    public virtual void InitializeAudio()
    {
        if(!TryGetComponent<AudioSource>(out m_audio))
            m_audio = gameObject.AddComponent<AudioSource>();
    }
    
    public virtual void InitializeCallbacks()
    {
        Player.playerEvents.OnGlidingStart.AddListener(ShowGlider);
        Player.playerEvents.OnGlidingStop.AddListener(HideGlider);
    }
    
    public virtual void InitializeGlider()
    {
        SetTrailsEmitting(false);
        transform.localScale=Vector3.zero;
    }

    protected virtual void ShowGlider()
    {
        Debug.Log("ShowGlider");
        StopAllCoroutines();
        StartCoroutine(ScaleGliderRoutine(Vector3.zero, Vector3.one));
        SetTrailsEmitting(true);
        m_audio.PlayOneShot(openAudio);
    }
    protected virtual void HideGlider()
    {
        Debug.Log("HideGlider");
        StopAllCoroutines();
        StartCoroutine(ScaleGliderRoutine(Vector3.one, Vector3.zero));
        SetTrailsEmitting(false);
        m_audio.PlayOneShot(openAudio);
    }

    protected virtual void SetTrailsEmitting(bool value)
    {
        if (trails == null) return;

        foreach (var trail in trails)
        {
            trail.emitting = value;
        }
    }

    protected IEnumerator ScaleGliderRoutine(Vector3 from, Vector3 to)
    {
        var time = 0f;
        transform.localScale = from;

        while (time < scaleDuration)
        {
            var scale = Vector3.Lerp(from, to, time / scaleDuration);
            transform.localScale = scale;
            time += Time.deltaTime;
            yield return null;
        }
      
        transform.localScale = to;
    }
}
