using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{
    public static GameBoardManager GameBoardManagerInstance;

    public Transform GameBoardParent, DotScoredZone;

    public List<Transform> ConnectedDots;

    public GameObject DotPrefab;

    public Material Blue, Green, Purple, Red, Yellow;

    private bool shouldIgnoreColor;
    private int colorToIgnore;

    [SerializeField]
    private int rows, columns;

    private void Awake()
    {
        GameBoardManagerInstance = this;
    }

    private void Start()
    {
        StartCoroutine(SpawnDots());
    }

    private IEnumerator SpawnDots()
    {
        int numberOfRows = 0;

        while (numberOfRows < rows)
        {
            for (int i = 0; i < columns; i++)
            {
                Vector3 SpawnPos = new Vector3(i, rows + 1, 0);
                GameObject IndividualDot = Instantiate(DotPrefab, SpawnPos, Quaternion.identity);
                AssignDots(IndividualDot, numberOfRows);
            }

            numberOfRows++;

            yield return null;
        }
    }

    public IEnumerator DotsScored()
    {
        if (ConnectedDots.Count < 2)
        {
            ConnectedDots.Clear();
        }
        else
        {
            int clearedDotCounter = 0;

            while (clearedDotCounter <= ConnectedDots.Count)
            {
                foreach (Transform clearedDots in ConnectedDots)
                {
                    clearedDots.SetParent(DotScoredZone);
                    clearedDots.position = DotScoredZone.position;

                    foreach (Transform remainingDots in GameBoardParent)
                    {
                        if (remainingDots.GetComponent<IndividualDot>().Coordinates.x == clearedDots.GetComponent<IndividualDot>().Coordinates.x &&
                        remainingDots.GetComponent<IndividualDot>().Coordinates.y > clearedDots.GetComponent<IndividualDot>().Coordinates.y)
                        {
                            remainingDots.GetComponent<IndividualDot>().Coordinates.y--;

                            clearedDots.GetComponent<IndividualDot>().Coordinates.y++;
                        }
                    }
                }

                clearedDotCounter++;

                yield return null;
            }

            foreach (Transform remainingDots in GameBoardParent)
            {
                StartCoroutine(remainingDots.GetComponent<IndividualDot>().FallIntoPlace());
            }

            RepopulateDots();
        }
    }

    private void RepopulateDots()
    {
        foreach (Transform clearedDots in ConnectedDots)
        {
            clearedDots.SetParent(GameBoardParent);
            clearedDots.position = new Vector3(clearedDots.GetComponent<IndividualDot>().Coordinates.x, 7, 0);

            AssignDots(clearedDots.gameObject, (int)clearedDots.GetComponent<IndividualDot>().Coordinates.y);
        }

        ConnectedDots.Clear();

        StartCoroutine(ReorganizeHierarchy());
    }

    private IEnumerator ReorganizeHierarchy()
    {
        int childTracker = 0;

        while (childTracker < GameBoardParent.childCount)
        {
            foreach (Transform dot in GameBoardParent)
            {
                dot.SetSiblingIndex((int)dot.position.x + ((int)dot.position.y * 6));
            }

            childTracker++;
            yield return null;
        }

        // Finally we check to see if a certain color of dot is being excluded from making a square. If so, set color exclusion to false.
        if (shouldIgnoreColor)
        {
            shouldIgnoreColor = false;
        }
    }

    private int RandomColor(int minimum, int maximum, int exception)
    {
        int randomColor = Random.Range(0, 5);

        if(randomColor == exception)
        {
            randomColor = (randomColor + 1) % maximum;
        }

        return randomColor;
    }

    public void AssignDots(GameObject dot, int finalYPos)
    {
        int randomColor = 0;

        if (shouldIgnoreColor)
        {
            randomColor = RandomColor(0, 5, colorToIgnore);
        }
        else
        {
            randomColor = Random.Range(0, 5);
        }

        IndividualDot dotScript = dot.GetComponent<IndividualDot>();
        Renderer dotRenderer = dot.GetComponent<Renderer>();

        switch (randomColor)
        {
            case 0:
                dotScript.ThisDotsColor = IndividualDot.Color.Blue;
                dotRenderer.material = Blue;
                break;

            case 1:
                dotScript.ThisDotsColor = IndividualDot.Color.Green;
                dotRenderer.material = Green;
                break;

            case 2:
                dotScript.ThisDotsColor = IndividualDot.Color.Purple;
                dotRenderer.material = Purple;
                break;

            case 3:
                dotScript.ThisDotsColor = IndividualDot.Color.Red;
                dotRenderer.material = Red;
                break;

            case 4:
                dotScript.ThisDotsColor = IndividualDot.Color.Yellow;
                dotRenderer.material = Yellow;
                break;
        }

        dotScript.Coordinates.x = dot.transform.position.x;
        dotScript.Coordinates.y = finalYPos;

        dot.transform.SetParent(GameBoardParent);

        StartCoroutine(dotScript.FallIntoPlace());
    }

    public void DotReassignmentAvoidColor(int color)
    {
        shouldIgnoreColor = true;
        colorToIgnore = color;
    }
}
