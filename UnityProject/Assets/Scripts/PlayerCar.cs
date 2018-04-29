using UnityEngine;

public class PlayerCar : CarController
{
    protected override void GetInput(out float vertical, out float horizontal)
    {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
    }

    public override void Kill()
    {
        transform.position = GameManager.Instance.transform.position;
        transform.rotation = GameManager.Instance.transform.rotation;
    }
}
