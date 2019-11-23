using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{
    public GameObject DotPrefab;

    public GameObject[,] GameBoard = new GameObject[6, 6];

    public Material Blue, Green, Purple, Red, Yellow;

    private void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Vector3 Pos = new Vector3(i, j, 0);
                GameObject IndividualDot = Instantiate(DotPrefab, Pos, Quaternion.identity);

                GameBoard[i, j] = IndividualDot;
            }
        }

        AssignDots();
    }

    private void AssignDots()
    {
        foreach(GameObject dot in GameBoard)
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
            dotScript.Coordinates.y = dot.transform.position.y;
        }
    }
}
