using System.IO;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public enum Mode
{
    ModeClassique,
    ModeRetro,
    ModeZen,
    ModeBonus
}

[System.Serializable]
public class SaveData : MonoBehaviour
{
    /*
    Cette classe est un Singleton (une seule instance). Le patron a été raccourci grâce au système de propriétés avec C#
    */
    private static SaveData _instance;
    public static SaveData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SaveData>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("SaveData");
                    _instance = singletonObject.AddComponent<SaveData>();
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    private bool _HideGhost;

    public bool HideGhost
    {
        get
        {
            Debug.Log("Getting HideGhost: " + _HideGhost);
            return _HideGhost;
        }
        set
        {
            Debug.Log("Setting HideGhost: " + value);
            _HideGhost = value;
        }
    }
    private string PathFile = Application.dataPath + "/data.txt";
    public Mode mode;

    [SerializeField]
    public static KeyCode MovGauche { get; private set; } = KeyCode.Q;
    public static KeyCode MovDroite { get; private set; } = KeyCode.D;

    public static KeyCode MovBas { get; private set; } = KeyCode.S;
    public static KeyCode RotGauche { get; private set; } = KeyCode.A;
    public static KeyCode RotDroite { get; private set; } = KeyCode.E;
    public static KeyCode DescInstante { get; private set; } = KeyCode.Space;
    public static KeyCode PlacReserve { get; private set; } = KeyCode.C;
    public static KeyCode MovHaut { get; private set; } = KeyCode.Z;



    public Dictionary<string, KeyCode> DicKeyCode = new Dictionary<string, KeyCode>()
    {
        {"MovGauche",      MovGauche},
        {"MovDroite",      MovDroite},
        {"RotGauche",      RotGauche},
        {"RotDroite",      RotDroite},
        {"MovBas",         MovBas},
        {"DescInstante",   DescInstante},
        {"PlacReserve",    PlacReserve},
        {"MovHaut",        MovHaut}
    };

    public Text scoreListText;
    public List<int> scoresListClassique = new List<int>();
    public List<int> scoresListRetro = new List<int>();
    public List<int> scoresListBonus = new List<int>();

    private GameObject[] objectsWithTag;
    private GameData1 gameData;

    void Start()
    {
        objectsWithTag = GameObject.FindGameObjectsWithTag("Ghost");

        if (File.Exists(PathFile))
        {
            Load();
        }
        else
        {
            Save();
        }

        ToggleGhostObjects();
    }


    public static KeyCode ToKeyCode(string KeyStr)
    {
        try
        {
            KeyCode result = (KeyCode)Enum.Parse(typeof(KeyCode), KeyStr, true);
            return result;
        }
        catch (ArgumentException)
        {
            return KeyCode.Space;
        }
    }

    public static bool ToBool(string KeyStr)
    {
        if (KeyStr == "True")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void ToggleGhostObjects()
    {
        foreach (GameObject obj in objectsWithTag)
        {
            if (obj != null)
            {
                obj.SetActive(HideGhost);
            }
        }
    }

    public void AddScore(int newScore)
    {
        switch (mode)
        {
            case Mode.ModeClassique:
                scoresListClassique.Add(newScore);
                scoresListClassique = scoresListClassique.OrderByDescending(score => score).ToList();
                break;
            case Mode.ModeRetro:
                scoresListRetro.Add(newScore);
                scoresListRetro = scoresListRetro.OrderByDescending(score => score).ToList();
                break;
            case Mode.ModeBonus:
                scoresListBonus.Add(newScore);
                scoresListBonus = scoresListBonus.OrderByDescending(score => score).ToList();
                break;
            default:
                break;
        }

        UpdateScoreListText();
        scoreListText.gameObject.SetActive(true);
    }

    private void UpdateScoreListText()
    {
        string scoreText = "High Scores:\n\n";

        switch (mode)
        {
            case Mode.ModeClassique:
                scoresListClassique = EnsureMinimumLength(scoresListClassique);
                for (int i = 0; i < 10; i++)
                {
                    scoreText += (i + 1) + ". " + scoresListClassique[i] + "\n";
                }
                break;
            case Mode.ModeRetro:
                scoresListRetro = EnsureMinimumLength(scoresListRetro);
                for (int i = 0; i < 10; i++)
                {
                    scoreText += (i + 1) + ". " + scoresListRetro[i] + "\n";
                }
                break;
            case Mode.ModeBonus:
                scoresListBonus = EnsureMinimumLength(scoresListBonus);
                for (int i = 0; i < 10; i++)
                {
                    scoreText += (i + 1) + ". " + scoresListBonus[i] + "\n";
                }
                break;
            default:
                break;
        }

        scoreListText.text = scoreText;
    }

    private List<int> EnsureMinimumLength(List<int> scoresList)
    {
        while (scoresList.Count < 10)
        {
            scoresList.Add(0);
        }
        return scoresList;
    }

    public void HideScoreTable()
    {
        scoreListText.gameObject.SetActive(false);
    }

    public void Save()
    {
        gameData = new GameData1(mode, HideGhost, DicKeyCode, scoresListClassique, scoresListRetro, scoresListBonus);
        File.WriteAllText(PathFile, gameData.ToSaveString());
    }

    public void Load()
    {
        string saveString = File.ReadAllText(PathFile);
        gameData = GameData1.FromSaveString(saveString);

        HideGhost = gameData.HideGhost;
        DicKeyCode = gameData.DicKeyCode;
        scoresListClassique = gameData.scoresListClassique;
        scoresListRetro = gameData.scoresListRetro;
        scoresListBonus = gameData.scoresListBonus;
    }
}

[System.Serializable]
public class GameData1
{
    public Mode mode;
    public bool HideGhost;
    public Dictionary<string, KeyCode> DicKeyCode;
    public List<int> scoresListClassique;
    public List<int> scoresListRetro;
    public List<int> scoresListBonus;

    public GameData1(Mode mode, bool hideGhost, Dictionary<string, KeyCode> dicKeyCode, List<int> scoresListClassique, List<int> scoresListRetro, List<int> scoresListBonus)
    {
        this.mode = mode;
        this.HideGhost = hideGhost;
        this.DicKeyCode = dicKeyCode;
        this.scoresListClassique = scoresListClassique ?? new List<int>();
        this.scoresListRetro = scoresListRetro ?? new List<int>();
        this.scoresListBonus = scoresListBonus ?? new List<int>();
    }

    public string ToSaveString()
    {
        string saveString = "";

        foreach (var kvp in DicKeyCode)
        {
            saveString += kvp.Key + "=" + kvp.Value.ToString() + "\n";
        }

        saveString += "HideGhost=" + HideGhost.ToString() + "\n";
        saveString += "RetroScores=" + string.Join(",", scoresListRetro) + "\n";
        saveString += "ClassiqueScores=" + string.Join(",", scoresListClassique) + "\n";
        saveString += "BonusScores=" + string.Join(",", scoresListBonus) + "\n";

        return saveString;
    }

    public static GameData1 FromSaveString(string saveString)
    {
        GameData1 gameData = new GameData1(Mode.ModeClassique, false, new Dictionary<string, KeyCode>(), null, null, null);

        string[] lines = saveString.Split('\n');

        foreach (string line in lines)
        {
            string[] parts = line.Split('=');

            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                if (key.StartsWith("Mov") || key.StartsWith("Rot") || key.StartsWith("Desc") || key.StartsWith("Plac") || key.StartsWith("MovHaut"))
                {
                    gameData.DicKeyCode[key] = SaveData.ToKeyCode(value);
                }
                else if (key == "HideGhost")
                {
                    gameData.HideGhost = SaveData.ToBool(value);
                }
                else if (key == "RetroScores")
                {
                    gameData.scoresListRetro = ParseScores(value);
                }
                else if (key == "ClassiqueScores")
                {
                    gameData.scoresListClassique = ParseScores(value);
                }
                else if (key == "BonusScores")
                {
                    gameData.scoresListBonus = ParseScores(value);
                }
            }
        }

        return gameData;
    }

    private static List<int> ParseScores(string scoresString)
    {
        return scoresString.Split(',').Select(s => int.TryParse(s, out int score) ? score : 0).ToList();
    }
}