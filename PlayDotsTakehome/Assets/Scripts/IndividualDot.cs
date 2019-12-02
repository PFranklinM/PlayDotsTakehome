using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualDot : MonoBehaviour
{
    public enum Color { None, Blue, Green, Purple, Red, Yellow };
    public Color ThisDotsColor;

    public Vector2 Coordinates;

    [SerializeField]
    private float dotFallSpeed;

    public IEnumerator FallIntoPlace()
    {
        Vector3 DropDotPos = new Vector3(Coordinates.x, Coordinates.y - 0.5f, 0);

        while(Vector3.Distance(transform.position, DropDotPos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, DropDotPos, dotFallSpeed * Time.deltaTime);

            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        transform.position = Coordinates;
    }

    private void OnMouseDown()
    {
        DotConnectionManager.DotConnectionManagerInstance.StartLine(transform);

        DotConnectionManager.DotConnectionManagerInstance.CurrentlySelectedDotColor = (DotConnectionManager.SelectedDotColor)(int)ThisDotsColor;
    }

    private void OnMouseEnter()
    {
        DotConnectionManager.DotConnectionManagerInstance.AnotherDotEntered(transform);
    }
}
