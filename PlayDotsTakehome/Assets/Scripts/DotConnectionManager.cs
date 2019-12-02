using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DotConnectionManager : MonoBehaviour
{
    public static DotConnectionManager DotConnectionManagerInstance;

    public LineRenderer DotConnector, DotScoreGraphicTop, DotScoreGraphicBot, DotScoreGraphicLeft, DotScoreGraphicRight;

    [SerializeField]
    private float dotScoreGraphicGrowAmount;
    private float dotScoreGraphicXPos;

    public enum SelectedDotColor { None, Blue, Green, Purple, Red, Yellow };
    public SelectedDotColor CurrentlySelectedDotColor;

    private bool lineActive = false;

    private Transform StartingDot, PreviousDot, TouchedDot;

    private void Awake()
    {
        DotConnectionManagerInstance = this;
    }

    public void StartLine(Transform ClickedDot)
    {
        StartingDot = ClickedDot;
        PreviousDot = StartingDot;

        GameBoardManager.GameBoardManagerInstance.ConnectedDots.Add(ClickedDot);

        Vector3 Pos = new Vector3(ClickedDot.position.x,
            ClickedDot.position.y,
            -1);

        DotConnector.SetPosition(DotConnector.positionCount - 1, Pos);

        DotConnector.material = ClickedDot.GetComponent<Renderer>().material;

        DotConnector.positionCount++;

        dotScoreGraphicXPos = 0.1f;

        DotScoreGraphicTop.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicTop.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));

        DotScoreGraphicTop.material = ClickedDot.GetComponent<Renderer>().material;
        DotScoreGraphicBot.material = ClickedDot.GetComponent<Renderer>().material;

        StartCoroutine(MouseDragLineRenderer());
    }

    public void AnotherDotEntered(Transform EnteredDot)
    {
        if (lineActive)
        {
            IndividualDot PreviousDotScript = PreviousDot.GetComponent<IndividualDot>();
            IndividualDot EnteredDotScript = EnteredDot.GetComponent<IndividualDot>();

            if (Mathf.Abs(PreviousDotScript.Coordinates.x - EnteredDotScript.Coordinates.x) == 1 && Mathf.Abs(PreviousDotScript.Coordinates.y - EnteredDotScript.Coordinates.y) == 0 ||
                       Mathf.Abs(PreviousDotScript.Coordinates.x - EnteredDotScript.Coordinates.x) == 0 && Mathf.Abs(PreviousDotScript.Coordinates.y - EnteredDotScript.Coordinates.y) == 1)
            {
                if ((int)EnteredDotScript.ThisDotsColor == (int)PreviousDotScript.ThisDotsColor)
                {
                    NewDotActions(EnteredDot);
                }
            }
        }
    }

    private void NewDotActions(Transform Dot)
    {
        if(Dot == StartingDot && GameBoardManager.GameBoardManagerInstance.ConnectedDots.Count > 2)
        {
            MadeASquare();
        }
        else if (GameBoardManager.GameBoardManagerInstance.ConnectedDots.Contains(Dot))
        {
            DotDisconnected(Dot);
        }
        else
        {
            DotConnected(Dot);
        }
    }

    private void MadeASquare()
    {
        foreach(Transform Dot in GameBoardManager.GameBoardManagerInstance.GameBoardParent)
        {
            if ((int)Dot.GetComponent<IndividualDot>().ThisDotsColor == (int)CurrentlySelectedDotColor)
            {
                GameBoardManager.GameBoardManagerInstance.ConnectedDots.Add(Dot);
            }
        }

        // Since there is a none color option, we want to subtract 1 from the enum int to account for only colors.
        GameBoardManager.GameBoardManagerInstance.DotReassignmentAvoidColor((int)CurrentlySelectedDotColor - 1);


        DotScoreGraphicTop.SetPosition(0, new Vector3(-4.5f, 0, 0));
        DotScoreGraphicTop.SetPosition(1, new Vector3(4.5f, 0, 0));

        DotScoreGraphicBot.SetPosition(0, new Vector3(-4.5f, 0, 0));
        DotScoreGraphicBot.SetPosition(1, new Vector3(4.5f, 0, 0));

        DotScoreGraphicLeft.gameObject.SetActive(true);
        DotScoreGraphicLeft.material = StartingDot.GetComponent<Renderer>().material;

        DotScoreGraphicRight.gameObject.SetActive(true);
        DotScoreGraphicRight.material = StartingDot.GetComponent<Renderer>().material;
    }

    private void DotConnected(Transform Dot)
    {
        PreviousDot = Dot;

        GameBoardManager.GameBoardManagerInstance.ConnectedDots.Add(Dot);

        Vector3 Pos = new Vector3(Dot.position.x,
            Dot.position.y,
            -1);

        DotConnector.SetPosition(DotConnector.positionCount - 1, Pos);

        DotConnector.positionCount++;

        dotScoreGraphicXPos *= dotScoreGraphicGrowAmount;

        DotScoreGraphicTop.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicTop.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));
    }

    private void DotDisconnected(Transform Dot)
    {
        PreviousDot = Dot;

        Vector3 Pos = new Vector3(Dot.position.x,
            Dot.position.y,
            -1);

        DotConnector.SetPosition(DotConnector.positionCount - 1, Pos);

        DotConnector.positionCount--;

        dotScoreGraphicXPos /= dotScoreGraphicGrowAmount;

        DotScoreGraphicTop.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicTop.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));

        GameBoardManager.GameBoardManagerInstance.ConnectedDots.RemoveAt(GameBoardManager.GameBoardManagerInstance.ConnectedDots.Count - 1);
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

        DotScoreGraphicTop.SetPosition(0, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicTop.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(0, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));

        DotConnector.positionCount = 1;

        DotScoreGraphicLeft.gameObject.SetActive(false);
        DotScoreGraphicRight.gameObject.SetActive(false);

        StartCoroutine(GameBoardManager.GameBoardManagerInstance.DotsScored());

        lineActive = false;
    }
}
