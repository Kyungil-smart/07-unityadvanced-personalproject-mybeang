using System.Collections;

public interface IBeltBehavior
{
    public IEnumerator DeliverItem();
    public void GetItem();
    public void PutItem();
}
