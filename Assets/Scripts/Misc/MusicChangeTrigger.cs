using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeTrigger : MonoBehaviour
{
    [Header("Area Type")]
    [SerializeField] private StageAudioAreaType areaType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerCat")) return;
        StartCoroutine(_RandomDelayStart());

    }

    private IEnumerator _RandomDelayStart() 
    {
        yield return new WaitForSeconds(Random.Range(0, 5));
        AudioManager.Instance.SetMusicByArea(areaType);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerCat")) return;
        AudioManager.Instance.StopMusic();
    }
}
