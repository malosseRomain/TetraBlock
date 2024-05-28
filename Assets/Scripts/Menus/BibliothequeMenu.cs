using UnityEngine;
using UnityEngine.SceneManagement;

public class BibliothequeMenu : MonoBehaviour
{
    public GameObject[] panneaux;
    public GameObject[] explicationsPanel;
    public float vitesseDeDefilement = 1.0f;

    private int indexPanneauActuel = 0;
    private bool explicationsOuvertes = false;

    private void Start()
    {
        // Desactivez tous les panneaux d'explication au demarrage.
        foreach (GameObject panel in explicationsPanel)
        {
            panel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!explicationsOuvertes)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                AfficherExplications();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ChangerPanneau(-1);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                ChangerPanneau(1);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                LancerModeDeJeu();
            }
        }
    }

    private void ChangerPanneau(int increment)
    {
        panneaux[indexPanneauActuel].SetActive(false);
        indexPanneauActuel = (indexPanneauActuel + increment + panneaux.Length) % panneaux.Length;
        panneaux[indexPanneauActuel].SetActive(true);
    }

    private void AfficherExplications()
    {
        // Desactivez tous les panneaux d'explication sauf celui correspondant au panneau actuel.
        for (int i = 0; i < explicationsPanel.Length; i++)
        {
            explicationsPanel[i].SetActive(i == indexPanneauActuel);
        }

        explicationsOuvertes = true;
    }

    private void LancerModeDeJeu()
    {
        int sceneIndex = indexPanneauActuel + 2; // Les modes de jeu commencent à partir de la scene 2.

        if (sceneIndex >= 2 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError("Erreur : Index de sc�ne invalide !");
        }
    }

    public void FermerExplications()
    {
        // Desactivez tous les panneaux d'explication.
        foreach (GameObject panel in explicationsPanel)
        {
            panel.SetActive(false);
        }

        explicationsOuvertes = false;
    }

}
