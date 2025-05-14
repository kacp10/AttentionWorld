using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions; // Required for validation

public class ProfileManagerParents : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text userNameText;   // Username Text
    public TMP_InputField inputEmail; // Email Input
    public TMP_InputField inputPhone; // Phone Input
    public Button saveButton;      // Save Button
    public Button backButton;      // Back Button
    public TMP_Text feedbackText;  // Feedback Message

    private string loggedInUser;

    void Start()
    {
        // Get logged-in user data (only the parent's name from the database)
        loggedInUser = UserSession.Instance?.GetLoggedInUser() ?? "Unknown";

        // Display the name of the logged-in user
        userNameText.text = loggedInUser;

        // Add listeners to buttons
        saveButton.onClick.AddListener(OnSaveButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    // Validate email format
    private bool IsValidEmail(string email)
    {
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, emailPattern);
    }

    // Validate phone format (+57 for Colombian phone numbers)
    private bool IsValidPhoneNumber(string phone)
    {
        var phonePattern = @"^\+57\d{10}$";  // Matches +57 and 10 digits
        return Regex.IsMatch(phone, phonePattern);
    }

    // Save button click handler
    private void OnSaveButtonClicked()
    {
        string email = inputEmail.text.Trim();
        string phone = inputPhone.text.Trim();

        // Validate email
        if (!IsValidEmail(email))
        {
            feedbackText.text = "⚠️ Correo electrónico no válido.";
            feedbackText.color = Color.red;
            return;
        }

        // Validate phone number
        if (!IsValidPhoneNumber(phone))
        {
            feedbackText.text = "⚠️ Número de teléfono no válido (Debe comenzar con +57 y tener 10 dígitos).";
            feedbackText.color = Color.red;
            return;
        }

        // Update the parent info in your database if needed (not required for now)
        // You can make API calls here to update the data

        // Show feedback message after saving
        feedbackText.text = "✔️ Datos actualizados correctamente.";
        feedbackText.color = Color.green;

        // Hide feedback message after 2 seconds
        Invoke("HideFeedback", 2f);
    }

    // Hide feedback after a short delay
    private void HideFeedback()
    {
        feedbackText.text = "";
    }

    // Back button click handler (returns to the HomeParentsScene)
    private void OnBackButtonClicked()
    {
        SceneManager.LoadScene("HomeParentsScene");
    }
}
