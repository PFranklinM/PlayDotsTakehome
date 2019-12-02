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
                Vector3 SpawnPos = new Vector3(i, rows * 2, 0);
                GameObject IndividualDot = Instantiate(DotPrefab, SpawnPos, Quaternion.identity);
                AssignDots(IndividualDot.transform, numberOfRows);
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
                foreach (Transform ClearedDots in ConnectedDots)
                {
                    ClearedDots.SetParent(DotScoredZone);
                    ClearedDots.position = DotScoredZone.position;

                    foreach (Transform RemainingDots in GameBoardParent)
                    {
                        if (RemainingDots.GetComponent<IndividualDot>().Coordinates.x == ClearedDots.GetComponent<IndividualDot>().Coordinates.x &&
                        RemainingDots.GetComponent<IndividualDot>().Coordinates.y > ClearedDots.GetComponent<IndividualDot>().Coordinates.y)
                        {
                            RemainingDots.GetComponent<IndividualDot>().Coordinates.y--;

                            ClearedDots.GetComponent<IndividualDot>().Coordinates.y++;
                        }
                    }
                }

                clearedDotCounter++;

                yield return null;
            }

            foreach (Transform RemainingDots in GameBoardParent)
            {
                if (RemainingDots.transform.position != new Vector3(RemainingDots.GetComponent<IndividualDot>().Coordinates.x,
                    RemainingDots.GetComponent<IndividualDot>().Coordinates.y,
                    0))
                {
                    StartCoroutine(RemainingDots.GetComponent<IndividualDot>().FallIntoPlace());
                }
            }

            RepopulateDots();
        }
    }

    private void RepopulateDots()
    {
        foreach (Transform ClearedDots in ConnectedDots)
        {
            ClearedDots.SetParent(GameBoardParent);
            ClearedDots.position = new Vector3(ClearedDots.GetComponent<IndividualDot>().Coordinates.x, 7, 0);

            AssignDots(ClearedDots, (int)ClearedDots.GetComponent<IndividualDot>().Coordinates.y);
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

    public void AssignDots(Transform Dot, int finalYPos)
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

        IndividualDot DotScript = Dot.GetComponent<IndividualDot>();
        Renderer DotRenderer = Dot.GetComponent<Renderer>();

        switch (randomColor)
        {
            case 0:
                DotScript.ThisDotsColor = IndividualDot.Color.Blue;
                DotRenderer.material = Blue;
                break;

            case 1:
                DotScript.ThisDotsColor = IndividualDot.Color.Green;
                DotRenderer.material = Green;
                break;

            case 2:
                DotScript.ThisDotsColor = IndividualDot.Color.Purple;
                DotRenderer.material = Purple;
                break;

            case 3:
                DotScript.ThisDotsColor = IndividualDot.Color.Red;
                DotRenderer.material = Red;
                break;

            case 4:
                DotScript.ThisDotsColor = IndividualDot.Color.Yellow;
                DotRenderer.material = Yellow;
                break;
        }

        DotScript.Coordinates.x = Dot.transform.position.x;
        DotScript.Coordinates.y = finalYPos;

        Dot.transform.SetParent(GameBoardParent);

        StartCoroutine(DotScript.FallIntoPlace());
    }

    public void DotReassignmentAvoidColor(int color)
    {
        shouldIgnoreColor = true;
        colorToIgnore = color;
    }
}
