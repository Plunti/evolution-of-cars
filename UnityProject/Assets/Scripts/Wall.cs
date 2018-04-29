using UnityEngine;

public class Wall : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerCarCollider") || collision.gameObject.layer == LayerMask.NameToLayer("AICarCollider"))
            collision.transform.GetComponent<CarController>().Kill();
    }
}
