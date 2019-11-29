using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{
    public Transform GameBoardParent;

    public GameObject DotPrefab;

    public GameObject[,] GameBoard = new GameObject[6, 6];

    public Material Blue, Green, Purple, Red, Yellow;

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

        /*
        Vector3 Pos = new Vector3(IndividualDot.transform.position.x, 0, 0);

            while (Vector3.Distance(IndividualDot.transform.position, Pos) > 0.1f)
            {
                IndividualDot.transform.position = Vector3.Lerp(IndividualDot.transform.position, Pos, 10f * Time.deltaTime);
                yield return null;
            }
            */

            /*
            for (int j = 0; j < 6; j++)
            {
                //Vector3 SpawnPos = new Vector3(i, 7, 0);

                //Vector3 Pos = new Vector3(i, j, 0);
                //GameObject IndividualDot = Instantiate(DotPrefab, SpawnPos, Quaternion.identity);
                //AssignDots(IndividualDot);

                while(Vector3.Distance(IndividualDot.transform.position, Pos) > 0.1f)
                {
                    IndividualDot.transform.position = Vector3.Lerp(IndividualDot.transform.position, Pos, 10f * Time.deltaTime);
                    yield return null;
                }

                //GameBoard[i, j] = IndividualDot;
            }
            */
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

        StartCoroutine(dotScript.FallIntoPlace(finalYPos));
    }
}
