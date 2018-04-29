using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private List<string> HittedCars = new List<string>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("AICarCollider"))
        {
            AICar car = other.transform.GetComponentInParent<AICar>();
            if (car != null && !HittedCars.Contains(car.ID))
            {
                HittedCars.Add(car.ID);
                car.CheckpointHit();
            }
        }
    }
}
