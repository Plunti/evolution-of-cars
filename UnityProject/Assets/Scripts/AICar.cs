using System;
using System.Collections;
using UnityEngine;
using NeuralNet;

public class AICar : CarController
{
    private static readonly Vector2[] LineDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.right, Vector2.left, (Vector2.up + Vector2.right).normalized, (Vector2.up + Vector2.left).normalized };

    [SerializeField] private float AllowedIdleTime = 5;
    [SerializeField] private float RayLength = 2;

    public NeuralNetwork Network { get; set; }
    public int CheckpointCount { get; private set; }
    public float LastCheckpointTime { get; private set; }
    public float Fitness { get; private set; }

    private double[] NeuralInput = new double[6];

    private LineRenderer RayRenderer;

    protected override void Awake()
    {
        base.Awake();

        RayRenderer = GetComponent<LineRenderer>();
        StartCoroutine(IsNotImproving());
    }

    private void Update()
    {
        Vector2[] rayDirections = new Vector2[] { transform.up, -transform.up, transform.right, -transform.right, (transform.up + transform.right).normalized, (transform.up - transform.right).normalized };
        for (int i = 0; i < NeuralInput.Length; i++)
            NeuralInput[i] = CastRay(rayDirections[i], LineDirections[i], i) / RayLength;
    }

    protected override void GetInput(out float vertical, out float horizontal)
    {
        double[] neuralOutput = Network.FeedForward(NeuralInput);

        if (neuralOutput[0] >= 0.5f)
            vertical = 1;
        else
            vertical = 0;

        if (neuralOutput[1] < 0.25f)
            horizontal = -1;
        else if (neuralOutput[1] > 0.75f)
            horizontal = 1;
        else
            horizontal = 0;

        if (vertical == 0 && horizontal == 0)
            vertical = 1;
    }

    public override void Kill()
    {
        if (EvolutionManager.Instance != null)
            EvolutionManager.Instance.CarDead(this);
        else
            Destroy(gameObject);
    }

    public void CheckpointHit()
    {
        CheckpointCount++;
        LastCheckpointTime = Time.time;
        Fitness = (EvolutionManager.Instance == null) ? CheckpointCount : CheckpointCount + (-1f / EvolutionManager.Instance.MaxTime * (LastCheckpointTime - EvolutionManager.Instance.GenerationStartTime) + 1);
    }

    private double CastRay(Vector2 rayDirection, Vector2 lineDirection, int lineIndex)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, RayLength, LayerMask.GetMask("WallCollider"));
        float distance = (hit.collider != null) ? hit.distance : RayLength;
        RayRenderer.SetPosition(lineIndex * 2, lineDirection * distance);
        return distance;
    }

    private IEnumerator IsNotImproving()
    {
        while (true)
        {
            int oldCheckpointCount = CheckpointCount;
            yield return new WaitForSeconds(AllowedIdleTime);
            if (oldCheckpointCount == CheckpointCount)
                Kill();
        }
    }
}
