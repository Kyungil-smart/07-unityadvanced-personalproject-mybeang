using System.Collections;
using UnityEngine;

public class BridgeBelt : ObjectOnTile, IBeltBehavior, IInteractableBeltPut
{
    public IEnumerator GetItem()
    {
        yield return null;
    }

    public IEnumerator PutItem()
    {
        yield return null;
    }

    public void InteractBeltPut(BaseBelt baseBelt)
    {
        
    }
}
