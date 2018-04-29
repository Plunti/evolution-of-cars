using System.IO;
using UnityEngine;
using SimpleFileBrowser;
using NeuralNet;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance = null;

    // Prefabs
    public GameObject UIPrefab;
    public GameObject EvolutionManagerPrefab;
    public GameObject PlayerCarPrefab;
    public GameObject AICarPrefab;

    // Sprites
    public Sprite RedCarSprite;
    public Sprite GreenCarSprite;
    public Sprite BlueCarSprite;
    public Sprite YellowCarSprite;

    public int CurrentRoad { get; private set; }

    private PlayerCar PlayerCar;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CurrentRoad = SceneManager.GetActiveScene().buildIndex;
        Instantiate(UIPrefab);
    }

    public void OnPlayerCarChange(bool playerCar)
    {
        if (playerCar)
        {
            PlayerCar = Instantiate(PlayerCarPrefab, transform.position, transform.rotation).GetComponent<PlayerCar>();
            Camera.main.GetComponent<CameraController>().Target = PlayerCar.transform;
        }
        else
        {
            Destroy(PlayerCar.gameObject);
            Camera.main.GetComponent<CameraController>().Target = null;
        }
    }

    public void OnEvolutionChange(bool evolution)
    {
        if (evolution)
            EvolutionManager.StartNewEvolution(transform.position, transform.rotation);
        else
            EvolutionManager.Instance.StopEvolution();
    }

    public void ChangeRoad(int road)
    {
        if (CurrentRoad != road)
        {
            SceneManager.LoadSceneAsync(road);
            CurrentRoad = road;
            UI.Instance.PlayerCarToggle.isOn = false;
            UI.Instance.EvolutionToggle.isOn = false;
            UI.Instance.EvolutionUI.SetActive(false);
        }
    }

    public void AddAICar()
    {
        if (FileBrowser.IsOpen)
            FileBrowser.HideDialog();
        FileBrowser.ShowLoadDialog((path) => { AddAICar(path); }, null, title: "Add AICar", loadButtonText: "Add");
    }

    private void AddAICar(string networkPath)
    {
        AICar car = null;
        using (StreamReader file = File.OpenText(networkPath))
        {
            JsonSerializer serializer = new JsonSerializer();
            NeuralNetwork network = (NeuralNetwork)serializer.Deserialize(file, typeof(NeuralNetwork));
            car = Instantiate(AICarPrefab, transform.position, transform.rotation).GetComponent<AICar>();
            car.Network = network;
            Camera.main.GetComponent<CameraController>().Target = car.transform;
        }
    }

    public void StoreBestNetwork()
    {
        if (EvolutionManager.Instance != null)
        {
            if (FileBrowser.IsOpen)
                FileBrowser.HideDialog();
            FileBrowser.ShowSaveDialog((path) => { StoreBestNetwork(path); }, null, title: "Store Best Network", saveButtonText: "Store");
        }
    }

    private void StoreBestNetwork(string networkPath)
    {
        using (StreamWriter file = File.CreateText(networkPath))
        {
            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            serializer.Serialize(file, EvolutionManager.Instance.BestNetwork);
        }
    }
}
