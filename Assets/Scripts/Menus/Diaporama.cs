using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Diaporama : MonoBehaviour
{
    public Sprite[] images;
    public GameObject[] explicationsPanels;
    public Image imageComponent;
    public Button detailsButton;
    public Button jouerButton;
    public float timeBetweenSlides = 3f;

    private int currentIndex = 0;
    private bool diaporamaEnCours = true;
    private int indexPanneauActuel = -1;

    void Start()
    {
        foreach (GameObject panel in explicationsPanels)
        {
            panel.SetActive(false);
            Button closeButton = panel.GetComponentInChildren<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(FermerPanneauExplication);
            }
        }

        detailsButton.onClick.AddListener(TogglePanneauExplication);
        jouerButton.onClick.AddListener(JouerScene); // Ajout de l'écouteur pour le bouton "Play"

        AfficherImage(0);

        if (diaporamaEnCours)
            RestartDiapo();
    }

    void NextSlide()
    {
        if (!diaporamaEnCours) return;

        currentIndex = (currentIndex + 1) % images.Length;
        AfficherImage(currentIndex);
        RestartDiapo();
    }

    void PrevSlide()
    {
        if (!diaporamaEnCours) return;

        currentIndex = (currentIndex - 1 + images.Length) % images.Length;
        AfficherImage(currentIndex);
        RestartDiapo();
    }

    void RestartDiapo()
    {
        CancelInvoke();
        InvokeRepeating("NextSlide", timeBetweenSlides, timeBetweenSlides);
    }

    void AfficherImage(int index)
    {
        imageComponent.sprite = images[index];
    }

    void TogglePanneauExplication()
    {
        if (indexPanneauActuel == -1)
            AfficherPanneauExplication();
        else
            FermerPanneauExplication();
    }

    void AfficherPanneauExplication()
    {
        diaporamaEnCours = false;

        int index = GetCurrentIndex();
        explicationsPanels[index].SetActive(true);
        indexPanneauActuel = index;
    }

    void FermerPanneauExplication()
    {
        explicationsPanels[indexPanneauActuel].SetActive(false);
        indexPanneauActuel = -1;
        diaporamaEnCours = true;
    }

    void JouerScene()
    {
        SceneManager.LoadScene(currentIndex + 1); // Charger la scène correspondant à l'index de l'image
    }

    public void ImageSuivante()
    {
        NextSlide();
    }

    public void ImagePrecedente()
    {
        PrevSlide();
    }

    private int GetCurrentIndex()
    {
        return currentIndex;
    }
}
