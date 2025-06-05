using System.Collections;
using TMPro;
using UnityEngine;

public class LetterFadeInEffect : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    [TextArea(3, 10)] public string[] sentences;

    [Header("Настройки")]
    public float typingSpeed = 0.05f;         // задержка между буквами
    public float pauseAfterSentence = 1.5f;   // пауза после предложения
    public float fadeDuration = 0.5f;         // время на плавное появление одной буквы

    private int index = 0;

    void Start()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        while (index < sentences.Length)
        {
            yield return StartCoroutine(TypeSentenceWithFade(sentences[index]));
            yield return new WaitForSeconds(pauseAfterSentence);
            index++;
        }
    }

    IEnumerator TypeSentenceWithFade(string sentence)
    {
        textDisplay.text = sentence;
        textDisplay.ForceMeshUpdate();
        TMP_TextInfo textInfo = textDisplay.textInfo;

        // Установим альфу всех символов в 0
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertIndex = textInfo.characterInfo[i].vertexIndex;
            Color32[] newColors = textInfo.meshInfo[matIndex].colors32;

            for (int j = 0; j < 4; j++)
            {
                newColors[vertIndex + j].a = 0;
            }
        }
        textDisplay.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        // Постепенно показываем каждую букву
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            float t = 0;
            while (t < fadeDuration)
            {
                float alpha = Mathf.Lerp(0, 255, t / fadeDuration);
                int matIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertIndex = textInfo.characterInfo[i].vertexIndex;
                Color32[] newColors = textInfo.meshInfo[matIndex].colors32;

                for (int j = 0; j < 4; j++)
                {
                    newColors[vertIndex + j].a = (byte)alpha;
                }

                textDisplay.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                t += Time.deltaTime;
                yield return null;
            }

            // Установим финальную альфу 255
            int matIdx = textInfo.characterInfo[i].materialReferenceIndex;
            int vIdx = textInfo.characterInfo[i].vertexIndex;
            Color32[] finalColors = textInfo.meshInfo[matIdx].colors32;

            for (int j = 0; j < 4; j++)
            {
                finalColors[vIdx + j].a = 255;
            }

            textDisplay.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
