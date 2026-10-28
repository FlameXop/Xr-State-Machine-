using UnityEngine;

public class SittingIdleState : CharacterBaseState
{
    public SittingIdleState(CharacterStateManager stateManager) : base(stateManager) { }

    public override void OnEnter()
    {
        // 1. Update Top-Left UI & Show Only Stand Button
        manager.SetStatusText("Current State: Sitting Idle");
        manager.sitPanel.SetActive(true);
        manager.standPanel.SetActive(false);

        // 2. Drive Animator
        if (manager.Anim != null)
        {
            manager.Anim.SetBool("isStanding", false);
        }
    }

    public override void OnUpdate()
    {
        // Runs every frame while sitting
    }

    public override void OnExit()
    {
        // Hide Sitting UI when leaving this state
        manager.sitPanel.SetActive(false);
    }
}