using UnityEngine;
using UnityEngine.UI;

public class RadialProgressBar : MonoBehaviour
{
    public float fillSpeed = 1.0f; // Vitesse de remplissage de la barre
    public float emptySpeed = 1.0f; // Vitesse de vidange de la barre lorsque la r�serve est utilis�e
    public float reserveDelay = 2.0f; // D�lai de la r�serve en secondes
    public Color fillColor = Color.green;
    public Color emptyColor = Color.red;

    public Image radialFill;
    private float currentTime = 0.0f;
    private bool reserveUsed = false;

    private void Start()
    {
        radialFill = GetComponent<Image>();
        radialFill.fillAmount = 1.0f; // La barre est pleine au d�part
        radialFill.color = fillColor; // Couleur de la barre pleine
    }

    private void Update()
    {
        if (!reserveUsed)
            return;

        currentTime -= Time.deltaTime * emptySpeed;
        float fillAmount = currentTime / reserveDelay;
        radialFill.fillAmount = fillAmount;
        radialFill.color = Color.Lerp(emptyColor, fillColor, fillAmount);

        if (currentTime <= 0.0f)
        {
            currentTime = 0.0f;
            reserveUsed = false;
            radialFill.color = fillColor;
        }
    }

    public void StartFilling()
    {
        currentTime = reserveDelay;
        reserveUsed = true;
        radialFill.fillAmount = 0.0f; // R�initialise imm�diatement le fillAmount � 0 lorsque vous utilisez la r�serve
        radialFill.color = emptyColor; // Change la couleur pour correspondre � la barre vide
    }

    public void StopFilling()
    {
        reserveUsed = false;
        radialFill.fillAmount = 1.0f; // R�tablit le fillAmount � 1 lorsque vous arr�tez de remplir
        radialFill.color = fillColor; // R�tablit la couleur � la barre pleine
    }
}
