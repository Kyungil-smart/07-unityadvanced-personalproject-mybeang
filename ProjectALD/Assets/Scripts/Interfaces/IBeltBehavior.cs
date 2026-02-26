using System.Collections;

public interface IBeltBehavior
{
    public IEnumerator GetItem();
    public IEnumerator PutItem();
}
