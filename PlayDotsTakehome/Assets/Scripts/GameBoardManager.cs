using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{
    public static GameBoardManager GameBoardManagerInstance;

    public Transform GameBoardParent;

    public GameObject DotPrefab;

    //public GameObject[,] GameBoard = new GameObject[6, 6];

    public Material Blue, Green, Purple, Red, Yellow;

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

        while (numberOfRows < 6)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector3 SpawnPos = new Vector3(i, 7, 0);
                GameObject IndividualDot = Instantiate(DotPrefab, SpawnPos, Quaternion.identity);
                AssignDots(IndividualDot, numberOfRows);
            }

            yield return new WaitForSeconds(0.1f);

            numberOfRows++;

            yield return null;
        }
    }

    private void AssignDots(GameObject dot, int finalYPos)
    {
        int randomColor = Random.Range(0, 5);

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
}
