using System.Collections;

public interface ICoroutinePerformer
{
    public void StartRoutine(IEnumerator enumerator);
}
