using System.Collections;
using UnityEngine;
using TMPro;

public class FinishTrigger : MonoBehaviour
{
    public GameObject finishPanel;
    public TMP_Text finishResultText;

    public ResultsUI resultsUI;

    public float stopDelay = 1.5f;

    private bool finished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (finished)
            return;

        Rigidbody rb =
            other.attachedRigidbody;

        if (rb == null)
            return;

        if (!rb.CompareTag("Player"))
            return;

        IceCondition ice =
            rb.GetComponent<IceCondition>();

        if (ice != null &&
            ice.HasFailed())
            return;

        finished = true;

        RunStats stats =
            rb.GetComponent<RunStats>();

        if (stats != null)
            stats.FinishRun();

        if (ice != null)
        {
            ice.StopMelting();

            float percentage =
                ice.GetIcePercentage();

            if (finishResultText != null)
            {
                finishResultText.text =
                    "Ice Remaining: " +
                    Mathf.RoundToInt(
                        percentage
                    ) +
                    "%";
            }

            if (resultsUI != null)
            {
                resultsUI.PrepareStats(
                    stats,
                    ice
                );
            }
        }

        finishPanel.SetActive(true);

        StartCoroutine(
            StopCarAfterDelay(rb)
        );
    }

    private IEnumerator StopCarAfterDelay(
        Rigidbody rb)
    {
        yield return new WaitForSeconds(
            stopDelay
        );

        rb.linearVelocity =
            Vector3.zero;

        rb.angularVelocity =
            Vector3.zero;

        rb.isKinematic = true;
    }
}