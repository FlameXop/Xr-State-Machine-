using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using TMPro;

public class ARStateMachineManager : MonoBehaviour
{
    [Header("AR Components")]
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public GameObject characterPrefab;

    [Header("Character Settings")]
    [Tooltip("Reduce this value (e.g., 0.1 to 0.3) if the character spawns too huge")]
    public float characterScale = 0.2f;

    [Header("UI Elements")]
    public TextMeshProUGUI topLeftStateText;
    public GameObject sitPanel;   // Contains "Stand" button
    public GameObject standPanel; // Contains "Breakdance", "Walk", "Fist Fight", "Sit" buttons

    private GameObject spawnedCharacter;
    private Animator anim;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        sitPanel.SetActive(false);
        standPanel.SetActive(false);
        topLeftStateText.text = "State: Scanning Floor... (Move phone slowly)";
    }

    void Update()
    {
        // 1. BEFORE SPAWN
        if (spawnedCharacter == null)
        {
            // Feedback as soon as ANY plane is detected so you know you can tap
            if (planeManager.trackables.count > 0)
            {
                topLeftStateText.text = "Floor Detected! Tap floor to spawn.";
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // Ignore UI touches
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    return;

                // Use TrackableType.Planes for faster detection on early plane estimates
                if (raycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.Planes))
                {
                    SpawnCharacter(hits[0].pose);
                }
            }
        }
        // 2. AFTER SPAWN
        else
        {
            SyncUIWithAnimatorState();
        }
    }

    private void SpawnCharacter(Pose hitPose)
    {
        spawnedCharacter = Instantiate(characterPrefab, hitPose.position, hitPose.rotation);

        // Apply custom scale so the character isn't giant
        spawnedCharacter.transform.localScale = Vector3.one * characterScale;

        // Rotate character to face camera horizontally
        Vector3 lookDir = Camera.main.transform.position - spawnedCharacter.transform.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            spawnedCharacter.transform.rotation = Quaternion.LookRotation(lookDir);

        // Use GetComponentInChildren in case Animator is on a child object!
        anim = spawnedCharacter.GetComponentInChildren<Animator>();

        // Immediately show Sitting UI as fallback when spawned
        sitPanel.SetActive(true);
        standPanel.SetActive(false);
        topLeftStateText.text = "Current State: Sitting Idle";

        // Hide AR Planes after spawn
        planeManager.enabled = false;
        foreach (ARPlane plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
    }

    private void SyncUIWithAnimatorState()
    {
        if (anim == null)
        {
            topLeftStateText.text = "Error: No Animator found on Character Prefab!";
            return;
        }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("sitting idle"))
        {
            topLeftStateText.text = "Current State: Sitting Idle";
            sitPanel.SetActive(true);
            standPanel.SetActive(false);
        }
        else if (stateInfo.IsName("sit to stand"))
        {
            topLeftStateText.text = "Current State: Sit to Stand";
            sitPanel.SetActive(false);
            standPanel.SetActive(false);
        }
        else if (stateInfo.IsName("stand to sit"))
        {
            topLeftStateText.text = "Current State: Stand to Sit";
            sitPanel.SetActive(false);
            standPanel.SetActive(false);
        }
        else if (stateInfo.IsName("stand idle"))
        {
            topLeftStateText.text = "Current State: Stand Idle";
            sitPanel.SetActive(false);
            standPanel.SetActive(true);
        }
        else if (stateInfo.IsName("breakdance to idle"))
        {
            topLeftStateText.text = "Current State: Breakdance";
            sitPanel.SetActive(false);
            standPanel.SetActive(true);
        }
        else if (stateInfo.IsName("walk"))
        {
            topLeftStateText.text = "Current State: Walk";
            sitPanel.SetActive(false);
            standPanel.SetActive(true);
        }
        else if (stateInfo.IsName("fist fight"))
        {
            topLeftStateText.text = "Current State: Fist Fight";
            sitPanel.SetActive(false);
            standPanel.SetActive(true);
        }
    }

    // --- BUTTON ON-CLICK FUNCTIONS ---
    public void PressStandButton()
    {
        if (anim != null) anim.SetBool("isStanding", true);
    }

    public void PressSitButton()
    {
        if (anim != null) anim.SetBool("isStanding", false);
    }

    public void PressBreakdanceButton()
    {
        if (anim != null)
        {
            ResetAllActionTriggers();
            anim.SetTrigger("trBreakdance");
        }
    }

    public void PressWalkButton()
    {
        if (anim != null)
        {
            ResetAllActionTriggers();
            anim.SetTrigger("trWalk");
        }
    }

    public void PressFistFightButton()
    {
        if (anim != null)
        {
            ResetAllActionTriggers();
            anim.SetTrigger("trFistFight");
        }
    }

    private void ResetAllActionTriggers()
    {
        anim.ResetTrigger("trBreakdance");
        anim.ResetTrigger("trWalk");
        anim.ResetTrigger("trFistFight");
    }
}