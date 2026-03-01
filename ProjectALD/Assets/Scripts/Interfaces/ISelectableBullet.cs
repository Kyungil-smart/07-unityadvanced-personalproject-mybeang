using System.Collections.Generic;
using UnityEngine;

public interface ISelectableBullet
{
    public void ClearStorage();
    public void SetStorage(List<ItemType> itemTypes);
    public void SelectBullet(ItemType itemType);
}
