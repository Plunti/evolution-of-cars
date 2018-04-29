using System;
using System.Collections.Generic;
using UnityEngine;
using NeuralNet;

public class EvolutionManager : MonoBehaviour
{
    // Singleton
    public static EvolutionManager Instance = null;

    [SerializeField] private uint[] NetworkTopology;
    [SerializeField] private int ElitismCount = 3;
    [SerializeField] private int RandomCount = 15;
    [SerializeField] private int MutationCount = 40;
    [SerializeField] private int CrossoverCount = 40;
    [SerializeField] private int TournamentSize = 30;
    [SerializeField] private double UniformRate = 0.8;
    [SerializeField] private double MutationProbability = 0.3;
    [SerializeField] private double MutationAmount = 2;

    private List<AICar> Population = new List<AICar>();

    public int MaxTime { get; private set; }
    private int CheckpointCount;
    private int PopulationCount;

    private float StartTime;
    private int GenerationCount = 1;
    public float GenerationStartTime { get; private set; }
    private int CurrentPopulationCount;

    public NeuralNetwork BestNetwork { get; private set; }
    private float BestFitness = 0;
    private float BestCheckpoints = 0;
    private float BestTime = 0;

    private AICar fittestLivingCar;
    public AICar FittestLivingCar {
        get {
            return fittestLivingCar;
        }
        private set
        {
            fittestLivingCar = value;
            Camera.main.GetComponent<CameraController>().Target = FittestLivingCar.transform;
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        MaxTime = 30;
        CheckpointCount = GameObject.Find("Checkpoints").transform.childCount;
        PopulationCount = ElitismCount + RandomCount + MutationCount + CrossoverCount;
        StartTime = Time.time;
        GenerationStartTime = Time.time;
        CurrentPopulationCount = PopulationCount;
        UI.Instance.PopulationSizeText.text = "Population Size: " + CurrentPopulationCount;

        for (int i = Population.Count; i < PopulationCount; i++)
            AddCar(new NeuralNetwork(NetworkTopology), GameManager.Instance.RedCarSprite);
    }

    private void Update()
    {
        if (Time.time - GenerationStartTime >= MaxTime)
            NextGeneration();
        UI.Instance.TimeText.text = "Time: " + FormatSeconds(Time.time - StartTime);
        UI.Instance.GenerationTimeText.text = "Generation Time: " + FormatSeconds(Time.time - GenerationStartTime, 3);

        CalculateFittestLivingCar();

        UI.Instance.LivingBestFitnessText.text = "Living Best Fitness: " + FittestLivingCar.Fitness.ToString("N2");
        double currentBestCheckpointsPercentage = (double)FittestLivingCar.CheckpointCount / CheckpointCount;
        UI.Instance.LivingBestCheckpointsText.text = "Living Best Checkpoints: " + FittestLivingCar.CheckpointCount + " / " + CheckpointCount + " (" + currentBestCheckpointsPercentage.ToString("P2") + ")";

        if (FittestLivingCar.Fitness > BestFitness)
        {
            BestNetwork = FittestLivingCar.Network;
            BestFitness = FittestLivingCar.Fitness;
            BestCheckpoints = FittestLivingCar.CheckpointCount;
            BestTime = FittestLivingCar.LastCheckpointTime - GenerationStartTime;

            UI.Instance.BestFitnessText.text = "Best Fitness: " + BestFitness.ToString("N2");
            double bestCheckpointsPercentage = (double)BestCheckpoints / CheckpointCount;
            UI.Instance.BestCheckpointsText.text = "Best Checkpoints: " + BestCheckpoints + " / " + CheckpointCount + " (" + bestCheckpointsPercentage.ToString("P2") + ")";
            UI.Instance.BestTimeText.text = "Best Time: " + FormatSeconds(BestTime, 3);
            UI.Instance.LastImprovementText.text = "Last Improvement: " + GenerationCount;
        }
    }

    public static void StartNewEvolution(Vector2 position, Quaternion rotation)
    {
        Instance = Instantiate(GameManager.Instance.EvolutionManagerPrefab, position, rotation).GetComponent<EvolutionManager>();
        UI.Instance.EvolutionUI.SetActive(true);
    }

    public void StopEvolution()
    {
        Destroy(gameObject);
        Instance = null;
        UI.Instance.EvolutionUI.SetActive(false);
    }

    public void AddCar(NeuralNetwork network, Sprite sprite = null)
    {
        AICar car = Instantiate(GameManager.Instance.AICarPrefab, transform.position, transform.rotation, transform).GetComponent<AICar>();
        car.Network = network;
        if (sprite != null)
            car.GetComponentInParent<SpriteRenderer>().sprite = sprite;
        Population.Add(car);
    }

    public void CarDead(AICar DeadCar)
    {
        DeadCar.gameObject.SetActive(false);
        CurrentPopulationCount--;
        UI.Instance.PopulationSizeText.text = "Population Size: " + CurrentPopulationCount;
        if (CurrentPopulationCount == 0)
            NextGeneration();
    }

    private void CalculateFittestLivingCar()
    {
        fittestLivingCar = null;
        for (int i = 0; i < Population.Count; i++)
            if (Population[i].gameObject.activeSelf && (fittestLivingCar == null || Population[i].Fitness > fittestLivingCar.Fitness))
                FittestLivingCar = Population[i];
    }

    private void NextGeneration()
    {
        GenerationStartTime = Time.time;
        GenerationCount++;
        UI.Instance.GenerationText.text = "Generation: " + GenerationCount;
        CurrentPopulationCount = PopulationCount;
        UI.Instance.PopulationSizeText.text = "Population Size: " + CurrentPopulationCount;

        // Copy and order previous population
        AICar[] previousPopulation = new AICar[PopulationCount];
        Population.CopyTo(previousPopulation);
        Array.Sort(previousPopulation, ((x, y) => y.Fitness.CompareTo(x.Fitness)));

        // Destroy and clear population
        foreach (AICar car in Population)
            Destroy(car.gameObject);
        Population.Clear();

        // Elitism
        for (int i = 0; i < ElitismCount; i++)
            AddCar(previousPopulation[i].Network, GameManager.Instance.GreenCarSprite);

        // Random
        for (int i = 0; i < RandomCount; i++)
            AddCar(new NeuralNetwork(NetworkTopology), GameManager.Instance.RedCarSprite);

        // Mutation of fittest car
        for (int i = 0; i < MutationCount; i++)
            AddCar(new NeuralNetwork(previousPopulation[0].Network).Mutate(MutationProbability, MutationAmount), GameManager.Instance.YellowCarSprite);

        // Crossover
        for (int i = 0; i < CrossoverCount; i++)
        {
            // Selection
            AICar tournamentWinner1 = TournamentSelection(previousPopulation);
            AICar tournamentWinner2 = TournamentSelection(previousPopulation);

            // Crossover
            NeuralNetwork child = NeuralNetwork.Crossover(tournamentWinner1.Network, tournamentWinner2.Network, UniformRate);

            // Mutation
            AddCar(child.Mutate(MutationProbability, MutationAmount), GameManager.Instance.BlueCarSprite);
        }
    }

    private AICar TournamentSelection(AICar[] population)
    {
        AICar[] tournament = new AICar[TournamentSize];
        for (int i = 0; i < TournamentSize; i++)
            tournament[i] = population[(int)(NeuralMath.RandomDouble() * PopulationCount)];
        AICar fittestCar = tournament[0];
        for (int i = 1; i < TournamentSize; i++)
            if (tournament[i].Fitness > fittestCar.Fitness)
                fittestCar = tournament[i];
        return fittestCar;
    }

    private String FormatSeconds(float seconds, int length = 4)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        switch (length)
        {
            case 2:
                return string.Format("{0:D2}:{1:D3}", time.Seconds, time.Milliseconds);
            case 3:
                return string.Format("{0:D2}:{1:D2}:{2:D3}", time.Minutes, time.Seconds, time.Milliseconds);
            default:
                return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
        }
    }
}
