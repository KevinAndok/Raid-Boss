using System.Collections;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public enum TextColor { red, green, blue, white };

    public GameObject textPrefab;
    public int poolSize;

    public float defaultLifeTime = .5f;
    public float floatSpeed = 1;

    public Color greenColor, redColor, blueColor;

    public static FloatingText instance;

    public static int nextInUse = 0;

    public static TextMeshPro[] texts;
    public static Coroutine[] textLifetime;

    private void Awake()
    {
        if (!instance) instance = this;
        texts = new TextMeshPro[poolSize];
        textLifetime = new Coroutine[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            var textObject = Instantiate(textPrefab, transform);
            texts[i] = textObject.GetComponent<TextMeshPro>();
            texts[i].text = "";
        }
    }

    public static void CreateFloatingText(Vector2 position, string message, TextColor textColor)
    {
        texts[nextInUse].gameObject.transform.position = position + new Vector2(Random.Range(-.2f, .2f), Random.Range(-.2f, .2f));
        texts[nextInUse].text = message;

        Color color;
        if (textColor == TextColor.blue) color = instance.blueColor;
        else if (textColor == TextColor.green) color = instance.greenColor;
        else if (textColor == TextColor.red) color = instance.redColor;
        else color = Color.white;

        texts[nextInUse].color = color;

        if (textLifetime[nextInUse] != null)
            instance.StopCoroutine(textLifetime[nextInUse]);
        textLifetime[nextInUse] = instance.StartCoroutine(HideTextAfterTime(nextInUse, instance.defaultLifeTime));

        nextInUse = (nextInUse + 1) % instance.poolSize;
    }

    static IEnumerator HideTextAfterTime(int index, float time)
    {
        var transform = texts[index].transform;

        do
        {
            transform.position += Vector3.up * instance.floatSpeed * Time.fixedDeltaTime;
            time -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        } while (time > 0);

        texts[index].text = "";
        textLifetime[index] = null;
    }
}
