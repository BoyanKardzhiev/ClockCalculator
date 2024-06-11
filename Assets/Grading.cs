using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Grading : MonoBehaviour
{
    bool confirm;
    List<int> elements = new List<int>();
    public List<Sprite> chosenSprites = new List<Sprite>();
    public GameObject listElements, TryAgainButton;
    public List<GameObject> Stars = new List<GameObject>();
    public List<GameObject> HalfStars = new List<GameObject>();
    public List<GameObject> Faces = new List<GameObject>();
    public TextMeshProUGUI FinalGradeText;
    public GameObject FinalGradeBackground;
    public class DataRow
    {
        public string TestedElement { get; set; }
        public float? ClearnessOfVisual { get; set; }
        public float? ReadabilityOfTime { get; set; }
        public float? TimeCommunication { get; set; }
        public float? ActivityCommunication { get; set; }
        public float? Intuitiveness { get; set; }
        public float? Aesthetics { get; set; }
    }

    public static List<DataRow> data = new List<DataRow>
    {
        new DataRow { TestedElement = "Black on white (text and numbers)", ClearnessOfVisual = 5, ReadabilityOfTime = 5, TimeCommunication = 4, ActivityCommunication = null, Intuitiveness = null, Aesthetics = 4 },
        new DataRow { TestedElement = "Voice", ClearnessOfVisual = null, ReadabilityOfTime = 3, TimeCommunication = 3, ActivityCommunication = 4.5f, Intuitiveness = 5, Aesthetics = null },
        new DataRow { TestedElement = "Symbols-B&W person doing activity", ClearnessOfVisual = 4, ReadabilityOfTime = null, TimeCommunication = null, ActivityCommunication = 4, Intuitiveness = null, Aesthetics = null },
        new DataRow { TestedElement = "Symbols-Little colors", ClearnessOfVisual = 4.5f, ReadabilityOfTime = null, TimeCommunication = null, ActivityCommunication = 4, Intuitiveness = null, Aesthetics = null },
        new DataRow { TestedElement = "Symbols-Object related to the activity", ClearnessOfVisual = 4.5f, ReadabilityOfTime = null, TimeCommunication = null, ActivityCommunication = 2.5f, Intuitiveness = null, Aesthetics = null },
        new DataRow { TestedElement = "Symbols-Really detailed with many elements", ClearnessOfVisual = 2, ReadabilityOfTime = null, TimeCommunication = null, ActivityCommunication = 1, Intuitiveness = null, Aesthetics = null },
        new DataRow { TestedElement = "Images- Unknown people", ClearnessOfVisual = 5, ReadabilityOfTime = null, TimeCommunication = null, ActivityCommunication = 2.5f, Intuitiveness = null, Aesthetics = null },
        new DataRow { TestedElement = "Images- Focus on the activity", ClearnessOfVisual = 5, ReadabilityOfTime = null, TimeCommunication = null, ActivityCommunication = 4, Intuitiveness = null, Aesthetics = null },
        new DataRow { TestedElement = "Dark/Light Sun/Moon mode", ClearnessOfVisual = 4, ReadabilityOfTime = 5, TimeCommunication = 4, ActivityCommunication = 4, Intuitiveness = null, Aesthetics = 4 },
        new DataRow { TestedElement = "Analog", ClearnessOfVisual = 4, ReadabilityOfTime = 3.5f, TimeCommunication = 2, ActivityCommunication = null, Intuitiveness = null, Aesthetics = 5 },
        new DataRow { TestedElement = "Digital", ClearnessOfVisual = 4, ReadabilityOfTime = 4, TimeCommunication = 3.5f, ActivityCommunication = null, Intuitiveness = null, Aesthetics = 5 },
        new DataRow { TestedElement = "Vibration", ClearnessOfVisual = null, ReadabilityOfTime = 1, TimeCommunication = 1, ActivityCommunication = 3, Intuitiveness = 1, Aesthetics = null },
        new DataRow { TestedElement = "Notification Sound", ClearnessOfVisual = null, ReadabilityOfTime = null, TimeCommunication = 3, ActivityCommunication = 2.5f, Intuitiveness = 1, Aesthetics = null }
    };

    public void CalculateGrade(List<int> elements)
    {
        var selectedElements = elements.Select(index => data[index]).ToList();

        var categories = new List<string> { "ClearnessOfVisual", "ReadabilityOfTime", "TimeCommunication", "ActivityCommunication", "Intuitiveness", "Aesthetics" };

        var meanScores = new Dictionary<string, float>();
        foreach (var category in categories)
        {
            var values = selectedElements.Select(row => GetCategoryValue(row, category)).Where(value => value.HasValue).Select(value => value.Value).ToList();
            if (values.Count > 0)
            {
                meanScores[category] = values.Average();
            }
            else
            {
                meanScores[category] = 0;
            }
        }

        var overallGrade = meanScores.Values.Average();

        int extraElementsCount = Math.Max(0, elements.Count - 3);
        float penalty = extraElementsCount * 0.1f;
        float finalGrade = overallGrade - penalty;

        var includedElements = selectedElements.Select(e => e.TestedElement).ToList();

        // Output results
        Debug.Log("Included elements: " + string.Join(", ", includedElements));
        Debug.Log("Mean scores by category:");
        foreach (var score in meanScores)
        {
            Debug.Log($"{score.Key}: {score.Value}");
        }
        Debug.Log("Penalty: " + penalty);
        Debug.Log("Final grade: " + finalGrade);
        FinalGradeText.text = " Final Grade: " + Mathf.Round(finalGrade*100.0f)*0.01f + " ouf of 5.";
        FinalGradeBackground.GetComponent<Image>().color = new Color (1, 1, 1, 0);
        TryAgainButton.SetActive(true);
        for(int i = 0; i<Mathf.FloorToInt(finalGrade); i++)
        {
            Stars[i].SetActive(true);
        }
        for (int i = 0; i < Mathf.Round(finalGrade); i++)
        {
            HalfStars[i].SetActive(true);
        }
        Faces[Mathf.FloorToInt(finalGrade)].SetActive(true);
    }

    private static float? GetCategoryValue(DataRow row, string category)
    {
        switch (category)
        {
            case "ClearnessOfVisual": return row.ClearnessOfVisual;
            case "ReadabilityOfTime": return row.ReadabilityOfTime;
            case "TimeCommunication": return row.TimeCommunication;
            case "ActivityCommunication": return row.ActivityCommunication;
            case "Intuitiveness": return row.Intuitiveness;
            case "Aesthetics": return row.Aesthetics;
            default: return null;
        }
    }


    void Start()
    {
        confirm = false;
        TryAgainButton.SetActive(false);
        for(int i = 0; i<listElements.transform.childCount; i++)
        {
            listElements.transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < Stars.Count; i++)
        {
            Stars[i].SetActive(false);
        }
        for (int i = 0; i < HalfStars.Count; i++)
        {
            HalfStars[i].SetActive(false);
        }
        for (int i = 0; i < Faces.Count; i++)
        {
            Faces[i].SetActive(false);
        }
        FinalGradeText.text = "";
        //var elements = new List<int> { 0, 1, 2, 8 };  // Indices of selected elements
        //CalculateGrade(elements);
    }

    void Update()
    {
        if (!confirm)
        {
            for(int i = 0; i<chosenSprites.Count; i++)
            {
                listElements.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = chosenSprites[i];
                listElements.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            CalculateGrade(elements);
        }
    }

    public void ConfirmSelection()
    {
        confirm = true;
    }

    public void AddElement(int index)
    {
        elements.Add(index);
    }

    public void TakeSprite(Sprite sprite)
    {
        chosenSprites.Add(sprite);
    }

    public void ResetProgram()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
