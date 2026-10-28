public abstract class CharacterBaseState
{
    protected CharacterStateManager manager;

    public CharacterBaseState(CharacterStateManager stateManager)
    {
        this.manager = stateManager;
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}