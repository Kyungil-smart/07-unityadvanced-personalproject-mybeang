using System.Collections;

public interface IBeltBehavior
{
    public IEnumerator DeliverItemCoroutine();
    public void GetItem();
    public void PutItem();
}
