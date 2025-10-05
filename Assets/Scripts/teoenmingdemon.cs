using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class teoenmingdemon : MonoBehaviour
{
    public float fadeDuration = 5f;
    public float enMingSpeakingTime = 4f;
    public Image myImg;

    void OnEnable()
    {
        GameManager.OnPlayerDie += TriggerFadeIn;
    }

    void OnDisable()
    {
        GameManager.OnPlayerDie -= TriggerFadeIn;
    }

    void Start()
    {

        if (myImg == null)
        {
            Debug.LogError("No Image component found on this GameObject!");
            return;
        }

        // Start fully transparent
        Color tempColor = myImg.color;
        tempColor.a = 0f;
        myImg.color = tempColor;
        Debug.Log("Start alpha: " + myImg.color.a);
    }

    private void TriggerFadeIn()
    {
        Debug.Log("TriggerFadeIn called");
        Color tempColor = myImg.color;
        tempColor.a = 1;
        myImg.color = tempColor;
        AudioManager.I.PlayEnMingTalk();
        StartCoroutine(MyWaitCoroutine());
        StartCoroutine(FadeOut());

    }

    private IEnumerator FadeOut()
    {

        float elapsedTime = 0f;
        Color tempColor = myImg.color;
        while (elapsedTime < fadeDuration)
        {
            Debug.Log($"{elapsedTime}");
            elapsedTime += Time.unscaledDeltaTime;
            tempColor.a = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            myImg.color = tempColor;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private IEnumerator FadeIn()
    {

        float elapsedTime = 0f;
        Color tempColor = myImg.color;
        while (elapsedTime < fadeDuration)
        {
            Debug.Log($"{elapsedTime}");
            elapsedTime += Time.unscaledDeltaTime;
            tempColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            myImg.color = tempColor;
            yield return null;
        }
    }


    IEnumerator MyWaitCoroutine()
    {



        yield return new WaitForSeconds(enMingSpeakingTime);


    }
}