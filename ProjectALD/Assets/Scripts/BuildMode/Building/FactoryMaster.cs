using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryMaster : ObjectOnTile, IMovableBuilding, IInteractableBeltGet, IInteractableBeltPut, ISelectableBullet, ISellable
{
    // Inspector
    [SerializeField] private ItemType _curBulletType;

    public ItemType curBulletType
    {
        get { return _curBulletType; }
        set
        {
            _curBulletType = value;
            SelectBullet(value);
        }
    }
    [SerializeField] private GameObject _bulletBoxPrefab;
    [SerializeField] private List<GameObject> _bulletPrefabs;  // pairing to _cacheBullets
    [SerializeField] private GameObject _panel;
    [SerializeField] private SpriteRenderer _bulletSROnPanel;
    [SerializeField] private List<FactorySlave> _factorySlaves;
    
    // Storage
    private Dictionary<ItemType, Queue<Item>> InputStorage = new();
    private Queue<Item> _outputStorage = new();
    
    // Others
    private readonly int _maxInput = 10;
    private readonly int _maxOutput = 10;
    private BulletBox _bulletBox;
    private List<Item> _cacheBullets = new();  // pairing to bulletPrefabs
    private Coroutine _createCoroutine;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        foreach (var prefab in _bulletPrefabs)
        {
            _cacheBullets.Add(prefab.GetComponent<Item>()); 
        }
    }
    
    private void OnEnable()
    {
        InitNumberOfConnectPoint();
    }

    private (Item item, GameObject prefab) GetBulletData(ItemType itemType)
    {
        int index = -1; 
        for (int i = 0; i < _cacheBullets.Count; i++)
        {
            if (_cacheBullets[i].itemType == itemType)
            {
                index = i;
                break;
            }
        }
        if (index == -1) return (null, null);
        return (_cacheBullets[index], _bulletPrefabs[index]);
    }
    
    protected override void InitNumberOfConnectPoint()
    {
        // ToDo. factory slave 가 있는 경우 고려 필요.
        bothes.Add(new ConnectPoint(ConnectPointType.Both, Direction.East, null, gameObject));
        bothes.Add(new ConnectPoint(ConnectPointType.Both, Direction.West, null, gameObject));
        bothes.Add(new ConnectPoint(ConnectPointType.Both, Direction.South, null, gameObject));
        bothes.Add(new ConnectPoint(ConnectPointType.Both, Direction.North, null, gameObject));  
    }
    
    public override void PutOnTileHandler()
    {
        _spriteRenderer.sortingLayerName = "FactoryMine";
        // ToDo. factory slave 가 있는 경우 고려 필요.
        foreach (var both in bothes)
        {
            ConnectToNeighbor(both);
        }
        // connectable 확인 된 후에 진행.
        _createCoroutine = StartCoroutine(CreateCroutine());
    }
    
    private void ReadyToCreate()
    {
        ClearStorage();
        SetStorage();
        
        // 공장에 간판 걸기
        (var item, var prefab) = GetBulletData(_curBulletType);
        if (item.itemType == ItemType.Cannon)
        {
            _bulletSROnPanel.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
        else
        {
            _bulletSROnPanel.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        _bulletSROnPanel.sprite = item.spriteRenderer.sprite;
        _panel.SetActive(true);

        if (_createCoroutine != null)
        {   // bullet Type 변경 후 이미 생산중인 것이 있으면 모두 버리고 새로 시작
            StopCoroutine(_createCoroutine);
            _bulletBox = null;
            _createCoroutine = StartCoroutine(CreateCroutine());
        }
    }

    public override void TakeOffTileHandler()
    {
        if (_createCoroutine != null) StopCoroutine(_createCoroutine);
        _createCoroutine = null;
    }

    public Item InteractBeltGet()
    {
        if (_outputStorage.Count > 0)
            return _outputStorage.Dequeue();
        return null;
    }

    private IEnumerator CreateCroutine()
    {
        while (true)
        {
            CreateBulletBox();
            yield return new WaitForSeconds(2f);
        }
    }

    private void ReadyBulletBox()
    {
        if (_curBulletType != ItemType.None)
        {
            GameObject bulletBoxObj = ObjectPoolManager.Instance.PopGameObject(_bulletBoxPrefab.name);
            bulletBoxObj.SetActive(false);
            _bulletBox = bulletBoxObj.GetComponent<BulletBox>();
            (Item bullet, GameObject bulletObj) = GetBulletData(_curBulletType);
            _bulletBox.SetLabel(_curBulletType, bullet.spriteRenderer);    
        }
    }
    
    private bool IsEnoughResources()
    {
        PrintLog("재고를 확인한다.");
        List<bool> results =  new();
        if (_bulletBox == null) return false;
        PrintLog("탄약 박스는 확인되었다.");
        if (_bulletBox.data.copperCnt > 0)
            results.Add(InputStorage[ItemType.Copper].Count >= _bulletBox.data.copperCnt);
        if (_bulletBox.data.iconCnt > 0)
            results.Add(InputStorage[ItemType.Iron].Count >= _bulletBox.data.iconCnt);
        if (_bulletBox.data.fireEleCnt > 0)
            results.Add(InputStorage[ItemType.Fire].Count >= _bulletBox.data.fireEleCnt);
        if (_bulletBox.data.iceEleCnt > 0)
            results.Add(InputStorage[ItemType.Ice].Count >= _bulletBox.data.iceEleCnt);
        foreach (var res in results)
        {
            if (!res)
            {
                PrintLog($"재고가 충분하지 않다.");
                return false;
            } 
        }
        PrintLog($"재고가 충분하다.");
        return true;
    }

    private void WastResource(ItemType itemType, int count)
    {
        if (count > 0)
            for (int i = 0; i < count; i++)
                InputStorage[itemType].Dequeue();
    }
    
    private void CreateBulletBox()
    {
        if (_curBulletType == ItemType.None) return;
        if (_outputStorage.Count > _maxOutput) return;

        if (InputStorage.Keys.Count == 0)
        {
            PrintLog("인입 창고가 비었다. 창고 준비하자.");
            ReadyToCreate();
        }
        
        PrintLog("탄환 박스 생성 시작.");
        // bulletbox 가 없으면 생성
        if (_bulletBox == null)
            ReadyBulletBox();
        PrintLog("탄환 박스가 준비되었다.");
        // 창고에 충분한 리소스들이 있는지 확인
        if (!IsEnoughResources()) return;
        
        // 소비 
        PrintLog("자원을 소비한다.");
        WastResource(ItemType.Copper, _bulletBox.data.copperCnt);
        WastResource(ItemType.Iron, _bulletBox.data.iconCnt);
        WastResource(ItemType.Fire, _bulletBox.data.fireEleCnt);
        WastResource(ItemType.Ice, _bulletBox.data.iceEleCnt);
        
        // 생산
        PrintLog("탄환을 생산해 탄환 박스에 넣는다.");
        (Item bullet, GameObject bulletPrefab) = GetBulletData(_curBulletType);
        while (!_bulletBox.IsFull())
        {
            GameObject bulletObj = ObjectPoolManager.Instance.PopGameObject(bulletPrefab.name);
            bulletObj.SetActive(false);
            _bulletBox.PutBullet(bulletObj);
        }
        // 생산 완료 후
        PrintLog("탄환 박스를 출하 창고로 보낸다.");
        _outputStorage.Enqueue(_bulletBox);
        _bulletBox = null;
    }

    public void ClearStorage()
    {
        InputStorage = null;
        InputStorage = new ();
        _outputStorage.Clear();
    }

    public void SetStorage()
    {
        // 레시피에 맞춰 인입 창고 재구성
        BulletData bulletData = DataManager.Instance.bulletData[$"{_curBulletType}SO"];
        PrintLog($"{_curBulletType} 의 창고 재구성.");
        if (bulletData.copperCnt > 0) InputStorage.Add(ItemType.Copper, new Queue<Item>());
        if (bulletData.iconCnt > 0) InputStorage.Add(ItemType.Iron, new Queue<Item>());
        if (bulletData.fireEleCnt > 0) InputStorage.Add(ItemType.Fire, new Queue<Item>());
        if (bulletData.iceEleCnt > 0) InputStorage.Add(ItemType.Ice, new Queue<Item>());
    }

    public void SelectBullet(ItemType itemType)  
    {   // Interface 의 명분을 주기 위해...
        ReadyToCreate();
    }

    public void InteractBeltPut(Item acquiredItem)
    {
        if (InputStorage == null || acquiredItem == null) return;
        PrintLog($"{acquiredItem.itemType} 이 들어온다. ");
        if (InputStorage.ContainsKey(acquiredItem.itemType) && InputStorage[acquiredItem.itemType].Count < _maxInput)
        {
            PrintLog($"{acquiredItem.itemType}를 창고에 쌓자. ");
            InputStorage[acquiredItem.itemType].Enqueue(acquiredItem);
        }
    }

    public void SellSelf()
    {
        PlayerStatusManager.Instance.EarnGold(DataManager.Instance.buildCostData["Factory"]);
        ClearAllConnectPoints();
        _bulletSROnPanel.sprite = null;
        _panel.SetActive(false);
        ObjectPoolManager.Instance.PushGameObject(gameObject);
    }
}
