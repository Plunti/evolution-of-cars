using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    // Singleton
    public static UI Instance = null;

    public GameObject DefaultUI { get; private set; }
    public GameObject EvolutionUI { get; private set; }

    // DefaultUI Content
    public Toggle Road1Toggle { get; private set; }
    public Toggle Road2Toggle { get; private set; }
    public Toggle PlayerCarToggle { get; private set; }
    public Toggle EvolutionToggle { get; private set; }
    public Button AddAICarButton { get; private set; }

    // EvolutionUI Content
    public Text TimeText { get; private set; }
    public Text GenerationText { get; private set; }
    public Text GenerationTimeText { get; private set; }
    public Text PopulationSizeText { get; private set; }
    public Text LivingBestFitnessText { get; private set; }
    public Text LivingBestCheckpointsText { get; private set; }
    public Text BestFitnessText { get; private set; }
    public Text BestCheckpointsText { get; private set; }
    public Text BestTimeText { get; private set; }
    public Text LastImprovementText { get; private set; }
    public Button StoreBestNetworkButton { get; private set; }

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
        DefaultUI = gameObject.transform.Find("DefaultUI").gameObject;
        EvolutionUI = gameObject.transform.Find("EvolutionUI").gameObject;

        // DefaultUI Initalization
        Road1Toggle = DefaultUI.transform.Find("Road1Toggle").GetComponent<Toggle>();
        Road2Toggle = DefaultUI.transform.Find("Road2Toggle").GetComponent<Toggle>();
        PlayerCarToggle = DefaultUI.transform.Find("PlayerCarToggle").GetComponent<Toggle>();
        EvolutionToggle = DefaultUI.transform.Find("EvolutionToggle").GetComponent<Toggle>();
        AddAICarButton = DefaultUI.transform.Find("AddAICarButton").GetComponent<Button>();

        switch (GameManager.Instance.CurrentRoad) {
            case 0:
                Road1Toggle.isOn = true;
                break;
            case 1:
                Road2Toggle.isOn = true;
                break;
        }

        // DefaultUI Listener
        PlayerCarToggle.onValueChanged.AddListener(GameManager.Instance.OnPlayerCarChange);
        EvolutionToggle.onValueChanged.AddListener(GameManager.Instance.OnEvolutionChange);
        AddAICarButton.onClick.AddListener(GameManager.Instance.AddAICar);
        Road1Toggle.onValueChanged.AddListener(delegate { GameManager.Instance.ChangeRoad(0); });
        Road2Toggle.onValueChanged.AddListener(delegate { GameManager.Instance.ChangeRoad(1); });

        // EvolutionUI Initalization
        TimeText = EvolutionUI.transform.Find("TimeText").GetComponent<Text>();
        GenerationText = EvolutionUI.transform.Find("GenerationText").GetComponent<Text>();
        GenerationTimeText = EvolutionUI.transform.Find("GenerationTimeText").GetComponent<Text>();
        PopulationSizeText = EvolutionUI.transform.Find("PopulationSizeText").GetComponent<Text>();
        LivingBestFitnessText = EvolutionUI.transform.Find("LivingBestFitnessText").GetComponent<Text>();
        LivingBestCheckpointsText = EvolutionUI.transform.Find("LivingBestCheckpointsText").GetComponent<Text>();
        BestFitnessText = EvolutionUI.transform.Find("BestFitnessText").GetComponent<Text>();
        BestCheckpointsText = EvolutionUI.transform.Find("BestCheckpointsText").GetComponent<Text>();
        BestTimeText = EvolutionUI.transform.Find("BestTimeText").GetComponent<Text>();
        LastImprovementText = EvolutionUI.transform.Find("LastImprovementText").GetComponent<Text>();
        StoreBestNetworkButton = EvolutionUI.transform.Find("StoreBestNetworkButton").GetComponent<Button>();

        // EvolutionUI Listener
        StoreBestNetworkButton.onClick.AddListener(GameManager.Instance.StoreBestNetwork);
    }

    private void OnGUI()
    {
        if (Event.current.Equals(Event.KeyboardEvent("escape"))) {
            DefaultUI.SetActive(!DefaultUI.gameObject.activeSelf);
            EvolutionUI.SetActive(!EvolutionUI.gameObject.activeSelf && EvolutionManager.Instance != null);
        }
    }
}
