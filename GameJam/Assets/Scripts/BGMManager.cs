using System.Collections;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bgm;

    [SerializeField] private float fadeInTime = 2f;
    [SerializeField] private float fadeOutTime = 2f;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        audioSource.clip = bgm;

        audioSource.volume = 0f;

        audioSource.Play();

        float timer = 0f;

        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;

            audioSource.volume =
                Mathf.Lerp(0f, 1f, timer / fadeInTime);

            yield return null;
        }

        audioSource.volume = 1f;
    }

    public IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;

        float timer = 0f;

        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;

            audioSource.volume =
                Mathf.Lerp(startVolume, 0f, timer / fadeOutTime);

            yield return null;
        }

        audioSource.volume = 0f;

        audioSource.Stop();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(FadeOut());
        }
    }
}