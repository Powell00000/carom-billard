using UnityEngine;

public class Ball : MonoBehaviour
{
    private float radius;

    public virtual void Initialize()
    {
        CalculateRadius();
    }

    protected virtual void FixedUpdate()
    {
        StickToTable();
    }

    private void CalculateRadius()
    {
        radius = transform.localScale.y / 2;
    }

    private void StickToTable()
    {

        if (Physics.Raycast(transform.position, Vector3.down, out var raycastHit, LayerMask.GetMask("Table")))
        {
            transform.position = transform.position.WithY(raycastHit.point.y + radius);
        }
    }
}
