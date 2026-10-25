using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ResultsUI : MonoBehaviour
{
    public GameObject finishPanel;
    public GameObject statsPanel;

    public TMP_Text statsText;

    private RunStats runStats;
    private IceCondition iceCondition;

    void Update()
    {
        // Xbox B / PlayStation Circle
        if (statsPanel != null &&
            statsPanel.activeSelf &&
            Gamepad.current != null &&
            Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            ShowSummary();
        }

        // Keyboard Escape also returns from Stats
        if (statsPanel != null &&
            statsPanel.activeSelf &&
            Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ShowSummary();
        }
    }

    public void PrepareStats(
        RunStats stats,
        IceCondition ice)
    {
        runStats = stats;
        iceCondition = ice;

        UpdateStatsText();
    }

    void UpdateStatsText()
    {
        if (runStats == null ||
            iceCondition == null ||
            statsText == null)
            return;

        float totalIceLost =
            iceCondition.maxIce -
            iceCondition.currentIce;

        statsText.text =
            "Run Time: " +
            runStats.runTime.ToString("F1") +
            " sec\n\n" +

            "Melt Damage: " +
            runStats.meltDamage.ToString("F1") +
            "\n" +

            "Hammer Damage: " +
            runStats.hammerDamage.ToString("F1") +
            "\n" +

            "Landing Damage: " +
            runStats.landingDamage.ToString("F1") +
            "\n\n" +

            "Hammer Hits: " +
            runStats.hammerHits +
            "\n" +

            "Hardest Landing: " +
            runStats.hardestLanding.ToString("F2") +
            "\n\n" +

            "Total Ice Lost: " +
            totalIceLost.ToString("F1") +
            "\n" +

            "Ice Delivered: " +
            Mathf.RoundToInt(
                iceCondition.GetIcePercentage()
            ) +
            "%";
    }

    public void ShowStats()
    {
        finishPanel.SetActive(false);
        statsPanel.SetActive(true);
    }

    public void ShowSummary()
    {
        statsPanel.SetActive(false);
        finishPanel.SetActive(true);
    }
}