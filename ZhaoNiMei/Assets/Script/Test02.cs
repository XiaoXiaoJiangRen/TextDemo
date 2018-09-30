using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test02 : MonoBehaviour
{

    private List<Vector3> unitSizeList = new List<Vector3>();//所有网格的坐标点集合
    private List<GameObject> itemList = new List<GameObject>();//所有实例化出来的物品集合
    private Dictionary<string, int> itemPrantDicDataList = new Dictionary<string, int>();
    private Dictionary<string, Vector2> imgSizeDic = new Dictionary<string, Vector2>();//保存所有素材图片的大小
    public Transform goodsFather;//生成物品的父物体
    // Use this for initialization
    void Start()
    {
        GetAllPos();
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void StartGame()
    {
        Vector3 vector3 = Vector3.zero;
       
        for (int i = 0; i < 4; i++)
        {
            int a = Random.Range(1, 101);
            if (a <= 20)
            {
                PlaceSizeTypeItem("1245");
            }
            else if (a > 20 && a <= 40)
            {
                PlaceSizeTypeItem("123");
            }
            else if (a > 40 && a <= 60)
            {
                PlaceSizeTypeItem("147");
            }
            else if (a > 60 && a <= 80)
            {
                PlaceSizeTypeItem("12");
            }
            else if (a > 80)
            {
                PlaceSizeTypeItem("14");
            }
        }         

    }
    //储存所有图片的大小
    void StorageAllImgSize()
    {
        imgSizeDic.Add("10001",new Vector2(250,195));
        imgSizeDic.Add("10002", new Vector2(179, 121));
        imgSizeDic.Add("10003", new Vector2(171, 200));
        imgSizeDic.Add("10004", new Vector2(300, 310));
        imgSizeDic.Add("10005", new Vector2(120, 104));
        imgSizeDic.Add("10006", new Vector2(100, 92));
        imgSizeDic.Add("10007", new Vector2(123, 142));
        imgSizeDic.Add("10008", new Vector2(150, 178));
        imgSizeDic.Add("10009", new Vector2(200, 124));
        imgSizeDic.Add("10010", new Vector2(200, 193));
        imgSizeDic.Add("10011", new Vector2(170, 136));
        imgSizeDic.Add("10012", new Vector2(174, 282));
        imgSizeDic.Add("10013", new Vector2(140, 150));
        imgSizeDic.Add("10014", new Vector2(150, 145));
        imgSizeDic.Add("10015", new Vector2(300, 155));
        imgSizeDic.Add("10016", new Vector2(100, 87));
        imgSizeDic.Add("10017", new Vector2(180, 135));
        imgSizeDic.Add("10018", new Vector2(180, 187));
        imgSizeDic.Add("10019", new Vector2(122, 200));
        imgSizeDic.Add("10020", new Vector2(150, 127));
    }
    /// <summary>
    /// 获取网格上所有点的坐标
    /// </summary>
    void GetAllPos()
    {
        int X = 1350;
        int Y = 924;

        for (int i = 0; i < X; i++)
        {

            for (int j = 0; j < Y; j++)
            {
                unitSizeList.Add(new Vector3(i, Y-j, 0));
            }
        }       
    }
    /// <summary>
    /// 获取空的部分是否放得下目标形状
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    Vector3 GetEmpty(string typeName)
    {
        List<Vector3> items = new List<Vector3>();
        //获取所有空的坐标点
        items = unitSizeList.FindAll((item) =>
        {
            return item.z == 0;
        });
       
        if (items.Count == 0)            
            return Vector3.zero;

        for (int i = 0; i < items.Count; i++)
        {            
            Vector3 one = Vector3.zero;
            Vector3 two = Vector3.zero;
            Vector3 three = Vector3.zero;
            switch (typeName)
            {
                case "1245":
                    //one = items.Find((item) => (item.x == items[i].x + 1 && item.y == items[i].y && item.x < 1350 - 1));
                    //two = items.Find((item) => (item.x == items[i].x && item.y == items[i].y - 1));
                    //three = items.Find((item) => (item.x == items[i].x + 1 && item.y == items[i].y - 1));
                    one = items.Find((item) => (item.x == items[i].x + 1 && item.y == items[i].y && item.x < 1350 - 1));
                    two = items.Find((item) => (item.x == items[i].x && item.y == items[i].y - 1));
                    three = items.Find((item) => (item.x == items[i].x + 1 && item.y == items[i].y - 1));
                    if (one != Vector3.zero && two != Vector3.zero && three != Vector3.zero)
                        return items[i];
                    break;
                case "123":
                    one = items.Find((item) => (item.x == items[i].x + 1 && item.y == items[i].y && item.x < 1350 - 1));
                    two = items.Find((item) => (item.x == items[i].x + 2 && item.y == items[i].y));
                    if (one != Vector3.zero && two != Vector3.zero)
                        return items[i];
                    break;
                case "147":
                    one = items.Find((item) => (item.x == items[i].x && item.y == items[i].y - 1 && item.x < 1350 - 1));
                    two = items.Find((item) => (item.x == items[i].x && item.y == items[i].y - 2));
                    if (one != Vector3.zero && two != Vector3.zero)
                        return items[i];
                    break;
                case "12":
                    one = items.Find((item) => (item.x == items[i].x + 1 && item.y == items[i].y && item.x < 1350 - 1));
                    if (one != Vector3.zero)
                        return items[i];
                    break;
                case "14":
                    one = items.Find((item) => (item.x == items[i].x && item.y == items[i].y - 1 && item.x < 1350 - 1));
                    if (one != Vector3.zero)
                        return items[i];
                    break;
                case "1":
                    return items[0];
            }
        }
        return Vector3.zero;
    }

    /// <summary>
    /// 放置指定形状
    /// </summary>
    bool PlaceSizeTypeItem(string typeName)
    {
        bool isSucceed = false;
        Vector3 vect = GetSizeTypeToList(typeName);
       
        if (vect != Vector3.zero)
        {
            isSucceed = true;
            PlaceSizeTypeToPos(typeName, vect);
        }
        return isSucceed;
    }

    /// <summary>
    /// 实例化相应的形状，获取该形状能放得下区域的坐标
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    Vector3 GetSizeTypeToList(string typeName)
    {
        //TODO
        GameObject goods = Resources.Load<GameObject>(typeName);
        GameObject go = Instantiate(goods);
        go.transform.parent = goodsFather.transform;
        go.transform.localPosition = Vector3.zero;
        //itemPrantDicDataList.Add(typeName, go);
        itemList.Add(go);
        return GetEmpty(typeName);
    }

    /// <summary>
    /// 将相应的形状放置到相应的坐标
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="vect"></param>
    void PlaceSizeTypeToPos(string typeName, Vector3 vect)
    {        
        GameObject wuPin = itemList.Find((item) => (item.name == typeName+ "(Clone)"));
        wuPin.transform.localPosition = new Vector3(vect.x,vect.y-924,vect.z);
        SetUnitSizeListData(vect, wuPin.GetComponent<RectTransform>().rect.size);        
    }

    /// <summary>
    /// 获取最少个数的形状
    /// </summary>
    string GetDicDataMinCountItem(string name1, string name2)
    {
        return itemPrantDicDataList[name1] <= itemPrantDicDataList[name2] ? name1 : name2;
    }
    /// <summary>
    /// 设置形状占用的坐标点的z值为1：被占用
    /// </summary>
    void SetUnitSizeListData(Vector2 pos, Vector2 size)
    {
        for (int i = 0; i < unitSizeList.Count; i++)
        {
            if (((unitSizeList[i].x >= pos.x) && (unitSizeList[i].x < pos.x + size.x)) && ((unitSizeList[i].y <= pos.y) && (unitSizeList[i].y > pos.y - size.y)))
                unitSizeList[i] = new Vector3(unitSizeList[i].x, unitSizeList[i].y, 1);
        }
    }

    //作者：傲视三国
    //链接：https://www.jianshu.com/p/ffab04c4053d
    //來源：简书
    //简书著作权归作者所有，任何形式的转载都请联系作者获得授权并注明出处。
}
