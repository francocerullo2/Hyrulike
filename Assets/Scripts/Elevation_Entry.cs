using UnityEngine;

public class Elevation_Entry : MonoBehaviour
{
    public Collider2D[] bridgeColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (Collider2D bridgeCollider in bridgeColliders)
            {
                bridgeCollider.enabled = false;
            }

            // Set the sorting order of the player sprite to be on top of the bridge
            collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 15;
        }
    }
}
