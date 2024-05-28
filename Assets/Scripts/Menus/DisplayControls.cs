using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayControls : MonoBehaviour
{
    public SaveData GameData;

    public Text Gauche, Droite, RotGauche, RotDroite, DescRapide, DescInstante, PlacReserve, Haut;
    public Button changeGauche, changeDroite, changeRotGauche, changeRotDroite, changeDescRapide, changeDescInstante, changePlacReserve, changeHaut;

    private bool waitingForKey = false;
    private string keyToChange = "";


    private Dictionary<string, KeyCode> keyValuePairs;

    private void Start()
    {
        keyValuePairs = GameData.DicKeyCode;

        UpdateControlText();
    }

    private void Update()
    {
        if (!waitingForKey)
            return;

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                if (!IsKeyAlreadyAssigned(keyCode))
                {
                    Debug.Log(keyCode);
                    ChangeKey(keyToChange, keyCode);
                    waitingForKey = false;
                    keyToChange = "";
                }
                else
                {
                    // Gerez le conflit de touches
                    GetTextObjectByKey(keyToChange).color = Color.red;
                    Debug.Log("La touche " + keyCode + " est deja assignee a une autre action.");
                    waitingForKey = false;
                    keyToChange = "";
                }
            }
        }
    }

    private void UpdateControlText()
    {
        Gauche.text = "Decalage gauche : " + keyValuePairs["MovGauche"];
        Droite.text = "Decalage droite : " + keyValuePairs["MovDroite"];
        RotGauche.text = "Rotation gauche : " + keyValuePairs["RotGauche"];
        RotDroite.text = "Rotation droite : " + keyValuePairs["RotDroite"];
        DescRapide.text = "Descente rapide : " + keyValuePairs["MovBas"];
        DescInstante.text = "Descente instantanee/Verrouiller : " + keyValuePairs["DescInstante"];
        PlacReserve.text = "Placement en reserve : " + keyValuePairs["PlacReserve"];
        Haut.text = "Remonter (mode classique) : " + keyValuePairs["MovHaut"];
    }

    public void StartKeyChangeProcess(string key)
    {
        waitingForKey = true;
        keyToChange = key;
        GetTextObjectByKey(key).color = Color.grey;
    }

    private void ChangeKey(string key, KeyCode newKey)
    {
        if (keyValuePairs.ContainsKey(key))
        {
            keyValuePairs[key] = newKey;
            GameData.DicKeyCode = keyValuePairs;

            GetTextObjectByKey(key).color = Color.green;
            UpdateControlText();
        }
    }

    private bool IsKeyAlreadyAssigned(KeyCode newKey)
    {
        foreach (KeyCode keyCode in keyValuePairs.Values)
        {
            if (keyCode == newKey)
            {
                return true;
            }
        }
        return false;
    }

    private Text GetTextObjectByKey(string key)
    {
        switch (key)
        {
            case "MovGauche":
                return Gauche;
            case "MovDroite":
                return Droite;
            case "RotGauche":
                return RotGauche;
            case "RotDroite":
                return RotDroite;
            case "DescRapide":
                return DescRapide;
            case "DescInstante":
                return DescInstante;
            case "PlacReserve":
                return PlacReserve;
            case "MovHaut":
                return Haut;
            default:
                Debug.Log("invalid :" + key);
                return null;
        }
    }
}
