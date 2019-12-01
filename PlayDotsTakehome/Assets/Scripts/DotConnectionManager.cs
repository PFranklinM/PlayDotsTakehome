using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DotConnectionManager : MonoBehaviour
{
    public static DotConnectionManager DotConnectionManagerInstance;

    public LineRenderer DotConnector, DotScoreGraphic;

    private float dotScoreGraphicXPos;

    public enum SelectedDotColor { None, Blue, Green, Purple, Red, Yellow };
    public SelectedDotColor CurrentlySelectedDotColor;

    public Transform DotGameBoard;
    public Transform DotScoredZone;

    private bool lineActive = false;
    private IndividualDot PreviousDot;
    private IndividualDot TouchedDot;

    public List<Transform> ConnectedDots;

    private void Awake()
    {
        DotConnectionManagerInstance = this;
    }

    public void AnotherDotEntered(IndividualDot dot)
    {
        if (lineActive)
        {
            if (Mathf.Abs(PreviousDot.Coordinates.x - dot.Coordinates.x) == 1 && Mathf.Abs(PreviousDot.Coordinates.y - dot.Coordinates.y) == 0 ||
            Mathf.Abs(PreviousDot.Coordinates.x - dot.Coordinates.x) == 0 && Mathf.Abs(PreviousDot.Coordinates.y - dot.Coordinates.y) == 1)
            {
                if ((int)dot.ThisDotsColor == (int)PreviousDot.ThisDotsColor)
                {
                    DotConnected(dot.transform);
                }
            }
        }
    }

    public void StartLine(Transform ClickedDot)
    {
        ConnectedDots.Add(ClickedDot);

        PreviousDot = ClickedDot.GetComponent<IndividualDot>();

        Vector3 Pos = new Vector3(ClickedDot.position.x,
            ClickedDot.position.y,
            -1);

        DotConnector.SetPosition(DotConnector.positionCount - 1, Pos);

        DotConnector.material = ClickedDot.GetComponent<Renderer>().material;

        DotConnector.positionCount++;

        dotScoreGraphicXPos = 0.1f;

        DotScoreGraphic.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphic.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));

        DotScoreGraphic.material = ClickedDot.GetComponent<Renderer>().material;

        StartCoroutine(MouseDragLineRenderer());
    }

    private void DotConnected(Transform NewDot)
    {
        ConnectedDots.Add(NewDot);

        PreviousDot = NewDot.GetComponent<IndividualDot>();

        Vector3 Pos = new Vector3(NewDot.position.x,
            NewDot.position.y,
            -1);

        DotConnector.SetPosition(DotConnector.positionCount - 1, Pos);

        DotConnector.positionCount++;

        dotScoreGraphicXPos *= 2f;

        DotScoreGraphic.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphic.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));
    }

    private IEnumerator MouseDragLineRenderer()
    {
        lineActive = true;

        while (Input.GetMouseButton(0))
        {
            Vector3 screenPoint = Input.mousePosition;
            screenPoint.z = 9;
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);

            DotConnector.SetPosition(DotConnector.positionCount - 1, worldPoint);

            yield return null;
        }

        dotScoreGraphicXPos = 0.0f;

        DotScoreGraphic.SetPosition(0, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphic.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));

        DotConnector.positionCount = 1;

        StartCoroutine(DotRefill());

        lineActive = false;
    }

    private IEnumerator DotRefill()
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

                    foreach (Transform remainingDots in DotGameBoard)
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

            foreach (Transform remainingDots in DotGameBoard)
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
            clearedDots.SetParent(DotGameBoard);
            clearedDots.position = new Vector3(clearedDots.GetComponent<IndividualDot>().Coordinates.x, 7, 0);

            GameBoardManager.GameBoardManagerInstance.AssignDots(clearedDots.gameObject, (int)clearedDots.GetComponent<IndividualDot>().Coordinates.y);
        }

        ConnectedDots.Clear();

        StartCoroutine(ReorganizeHierarchy());
    }

    private IEnumerator ReorganizeHierarchy()
    {
        int childTracker = 0;

        while (childTracker < DotGameBoard.childCount)
        {
            foreach (Transform dot in DotGameBoard)
            {
                dot.SetSiblingIndex((int)dot.position.x + ((int)dot.position.y * 6));
            }

            childTracker++;
            yield return null;
        }
    }
}
