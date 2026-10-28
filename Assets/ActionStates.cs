using UnityEngine;

public class BreakdanceState : CharacterBaseState
{
    public BreakdanceState(CharacterStateManager stateManager) : base(stateManager) { }

    public override void OnEnter()
    {
        manager.SetStatusText("Current State: Breakdance");
        manager.standPanel.SetActive(true);

        if (manager.Anim != null)
        {
            manager.ResetActionTriggers();
            manager.Anim.SetTrigger("trBreakdance");
        }
    }

    public override void OnUpdate() { }
    public override void OnExit() { }
}

public class WalkState : CharacterBaseState
{
    public WalkState(CharacterStateManager stateManager) : base(stateManager) { }

    public override void OnEnter()
    {
        manager.SetStatusText("Current State: Walk");
        manager.standPanel.SetActive(true);

        if (manager.Anim != null)
        {
            manager.ResetActionTriggers();
            manager.Anim.SetTrigger("trWalk");
        }
    }

    public override void OnUpdate() { }
    public override void OnExit() { }
}

public class FistFightState : CharacterBaseState
{
    public FistFightState(CharacterStateManager stateManager) : base(stateManager) { }

    public override void OnEnter()
    {
        manager.SetStatusText("Current State: Fist Fight");
        manager.standPanel.SetActive(true);

        if (manager.Anim != null)
        {
            manager.ResetActionTriggers();
            manager.Anim.SetTrigger("trFistFight");
        }
    }

    public override void OnUpdate() { }
    public override void OnExit() { }
}