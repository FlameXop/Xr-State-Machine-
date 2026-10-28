using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using TMPro;

public class CharacterStateManager : MonoBehaviour
{
    [Header("AR References")]
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public GameObject characterPrefab;
    public float characterScale = 0.2f;

    [Header("UI References")]
    public TextMeshProUGUI topLeftStateText;
    public GameObject sitPanel;
    public GameObject standPanel;

    // Runtime References
    public Animator Anim { get; private set; }
    private GameObject spawnedCharacter;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // State Machine Instances
    private CharacterBaseState currentState;
    public SittingIdleState SittingState { get; private set; }
    public StandIdleState StandingState { get; private set; }
    public BreakdanceState BreakdanceState { get; private set; }
    public WalkState WalkState { get; private set; }
    public FistFightState FistFightState { get; private set; }
    public ARAutoFocusBlur autoFocusBlur;

    void Awake()
    {
        // Initialize all concrete state instances
        SittingState = new SittingIdleState(this);
        StandingState = new StandIdleState(this);
        BreakdanceState = new BreakdanceState(this);
        WalkState = new WalkState(this);
        FistFightState = new FistFightState(this);
    }

    void Start()
    {
        sitPanel.SetActive(false);
        standPanel.SetActive(false);
        SetStatusText("State: Scanning Floor...");
    }

    void Update()
    {
        // 1. Handle AR Plane Spawning before character exists
        if (spawnedCharacter == null)
        {
            if (planeManager.trackables.count > 0)
            {
                SetStatusText("Floor Detected! Tap to Spawn.");
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    return;

                if (raycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.Planes))
                {
                    SpawnCharacter(hits[0].pose);
                }
            }
            return;
        }

        // 2. Run OnUpdate() of the active State
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }

    private void SpawnCharacter(Pose hitPose)
    {
        spawnedCharacter = Instantiate(characterPrefab, hitPose.position, hitPose.rotation);
        if (autoFocusBlur != null)
            autoFocusBlur.EnableFocusOnCharacter(spawnedCharacter.transform);
        spawnedCharacter.transform.localScale = Vector3.one * characterScale;

        Vector3 lookDir = Camera.main.transform.position - spawnedCharacter.transform.position;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            spawnedCharacter.transform.rotation = Quaternion.LookRotation(lookDir);

        Anim = spawnedCharacter.GetComponentInChildren<Animator>();

        // Disable AR planes once spawned
        planeManager.enabled = false;
        foreach (ARPlane plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }

        // Enter initial state: Sitting Idle
        ChangeState(SittingState);
    }

    // Core State Machine Transition Method
    public void ChangeState(CharacterBaseState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.OnEnter();
        }
    }

    public void SetStatusText(string text)
    {
        if (topLeftStateText != null)
            topLeftStateText.text = text;
    }

    public void ResetActionTriggers()
    {
        if (Anim == null) return;
        Anim.ResetTrigger("trBreakdance");
        Anim.ResetTrigger("trWalk");
        Anim.ResetTrigger("trFistFight");
    }

    // --- UI BUTTON HOOKS (Link your Buttons to these) ---
    public void OnStandButtonClicked() => ChangeState(StandingState);
    public void OnSitButtonClicked() => ChangeState(SittingState);
    public void OnBreakdanceButtonClicked() => ChangeState(BreakdanceState);
    public void OnWalkButtonClicked() => ChangeState(WalkState);
    public void OnFistFightButtonClicked() => ChangeState(FistFightState);
}