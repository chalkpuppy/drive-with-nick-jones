using UnityEngine;
using TMPro;

public class QualitySettingsManager : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;

    void Start()
    {
        // Populate the dropdown with the quality levels
        qualityDropdown.options.Clear();
        foreach (string quality in QualitySettings.names)
        {
            qualityDropdown.options.Add(new TMP_Dropdown.OptionData(quality));
        }

        // Set the dropdown to the current quality level
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        // Add listener for when the dropdown value changes
        qualityDropdown.onValueChanged.AddListener(delegate { ChangeQualityLevel(); });
    }

    public void ChangeQualityLevel()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value, true);
        PlayerPrefs.SetInt("QualitySetting", qualityDropdown.value);
    }
}
