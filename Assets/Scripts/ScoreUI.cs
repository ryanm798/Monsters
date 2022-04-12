using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    TextMeshProUGUI tm;

    [Header("Animation")]
    public AnimationCurve scalePopCurve;
    public AnimationCurve scaleFactorCurve;
    public float animationTime = 0.7f;
    float baseFontSize;
    public float fontScale = 2f;
    public int minPoints = 100;
    public int maxPoints = 3000;
    int prevScore;
    int points;

    [Header("Color")]
    public Color defaultColor = Color.white;
    public Color positiveColor = new Color();
    public Color negativeColor = new Color();
    public int maxColorScore = 10000;
    

    private void Start()
    {
        tm = gameObject.GetComponent<TextMeshProUGUI>();
        tm.text = "0";
        baseFontSize = tm.fontSize;
        prevScore = 0;
        points = 0;
    }

    private void LateUpdate()
    {
        int score = ScoreSystem.Instance.score;
        points += score - prevScore;
        tm.text = score.ToString();

        if (Mathf.Abs(points) > minPoints)
        {
            StopCoroutine("ScalePop");
            StartCoroutine(ScalePop(Mathf.Abs(points)));
        }

        /* if (score > 0)
        {
            if (score > 2000)
            {
                tm.color = new Color(255f / 255f, 243 / 255f, 0 / 255f);
            }
            else if (score > 1500)
            {
                tm.color = new Color(255f / 255f, 246 / 255f, 67 / 255f);
            }
            else if (score > 1000)
            {
                tm.color = new Color(255f / 255f, 249 / 255f, 120 / 255f);
            }
            else if (score > 500)
            {
                tm.color = new Color(255f / 255f, 252 / 255f, 180 / 255f);
            }
            else
            {
                tm.color = new Color(255f/255f, 255/255f, 255/255f);
            }
        } 
        else if (score < 0)
        {
            if (score < -2000)
            {
                tm.color = new Color(88 / 255f, 0 / 255f, 0 / 255f);
            }
            else if (score < -1500)
            {
                tm.color = new Color(131 / 255f, 33 / 255f, 33 / 255f);
            }
            else if (score < -1000)
            {
                tm.color = new Color(173 / 255f, 86 / 255f, 86 / 255f);
            }
            else if (score < -500)
            {
                tm.color = new Color(218 / 255f, 171 / 255f, 171 / 255f);
            }
            else
            {
                tm.color = new Color(255f / 255f, 255 / 255f, 255 / 255f);
            }
        }
        else if (score == 0)
        {
            tm.color = new Color(255f / 255f, 255 / 255f, 255 / 255f);
        } */

        if (score > 0)
            tm.color = Color.Lerp(defaultColor, positiveColor, ((float)score) / maxColorScore);
        else if (score < 0)
            tm.color = Color.Lerp(defaultColor, negativeColor, Mathf.Abs((float)score) / maxColorScore);
        else
            tm.color = defaultColor;

        points = 0;
        prevScore = score;
    }

    IEnumerator ScalePop(int points)
    {
        float t = 0f;
        float pointsNorm = ((float)points - minPoints) / (maxPoints - minPoints);
        float scaleFactor = scaleFactorCurve.Evaluate(pointsNorm);
        float maxFontSize = (baseFontSize * fontScale - baseFontSize) * scaleFactor + baseFontSize;
        while (t < animationTime)
        {
            t += Time.deltaTime;
            tm.fontSize = Mathf.Lerp(baseFontSize, maxFontSize, scalePopCurve.Evaluate(t));
            yield return null;
        }
        tm.fontSize = baseFontSize;
    }

    public void TimedPoints(int timedPoints)
    {
        points -= timedPoints;
    }

}
