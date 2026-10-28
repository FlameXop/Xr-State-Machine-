using UnityEngine;

public class StandIdleState : CharacterBaseState
{
    public StandIdleState(CharacterStateManager stateManager) : base(stateManager) { }

    public override void OnEnter()
    {
        // 1. Guaranteed UI Update & Show Stand Action Buttons
        manager.SetStatusText("Current State: Stand Idle");
        manager.sitPanel.SetActive(false);
        manager.standPanel.SetActive(true);

        // 2. Drive Animator (Triggers sit-to-stand if coming from sit)
        if (manager.Anim != null)
        {
            manager.Anim.SetBool("isStanding", true);
        }
    }

    public override void OnUpdate()
    {
        // Optional: Play crossfade to "stand idle" if Animator got stuck in "sit to stand"
    }

    public override void OnExit()
    {
        // Clean up if transitioning back to sit
    }
}