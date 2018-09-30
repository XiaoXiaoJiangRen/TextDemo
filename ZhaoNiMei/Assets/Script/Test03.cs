/*
 * 在Unity前端实现实现找你妹图片随机分布
 * 
 * 
 * */
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Test03 : MonoBehaviour
{

    private List<Vector3> unitSizeList = new List<Vector3>();//所有网格的坐标点集合
    private List<GameObject> itemList = new List<GameObject>();//所有实例化出来的物品集合   
    private Dictionary<string, Vector2> imgSizeDic = new Dictionary<string, Vector2>();//保存所有素材图片的大小
    public Transform goodsFather;//生成物品的父物体
    private bool isFull_01 = false;//是否铺满，没有位置了    

    void Start()
    {
        GetAllPos();
        StorageAllImgSize();
        CreateGoods();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 储存所有图片的大小
    /// </summary>
    void StorageAllImgSize()
    {
        imgSizeDic.Add("10001", new Vector2(250, 195));
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
        int Y = 754;
        for (int i = 0; i < X; i += 20)
        {
            for (int j = 0; j < Y; j += 20)
            {
                unitSizeList.Add(new Vector3(300 + i, Y - j, 0));
            }
        }
    }
    /// <summary>
    /// 第一遍随机生成图片
    /// </summary>
    void CreateGoods()
    {
        do
        {
            int goodsNum = Random.Range(1, 21);
            string typeName;
            if (goodsNum < 10)
                typeName = "1000" + goodsNum;
            else
                typeName = "100" + goodsNum;

            Vector3 pos = GetEmpty(GetImageSize(typeName));
            if (pos != Vector3.one)
            {
                GetSizeTypeToList(typeName, pos);
                SetUnitSizeListData(pos, GetImageSize(typeName));
                //Debug.Log("第一次" + typeName);
            }
            else
            {
                //用小的图片在填一遍
                AgainCreatGoods(GetImageSize(typeName));
            }
        } while (!isFull_01);


    }
    /// <summary>
    /// 再次生成图片填充空白区域
    /// </summary>
    /// <param name="vector2"></param>
    void AgainCreatGoods(Vector2 vector2)
    {


        float width = vector2.x;
        float height = vector2.y;
        List<string> smallerList = new List<string>();//储存比目标值小的图片
        //Debug.Log(width + "---" + height);
        foreach (var item in imgSizeDic)
        {
            if ((item.Value.x < width && item.Value.y < height) || (item.Value.x == width && item.Value.y < height) || (item.Value.x < width && item.Value.y == height))
            {
                smallerList.Add(item.Key);
                //Debug.Log(item.Value.x+"---"+ item.Value.y);
            }
        }

        if (smallerList.Count != 0)
        {


            for (int i = 0; i < smallerList.Count; i++)
            {
                Vector3 pos = GetEmpty(GetImageSize(smallerList[i]));
                if (pos != Vector3.one)
                {
                    GetSizeTypeToList(smallerList[i], pos);
                    SetUnitSizeListData(pos, GetImageSize(smallerList[i]));
                    //Debug.Log("第二次" + j + smallerList[i]);
                }
            }


        }

    }

    /// <summary>
    /// 在字典中获取将要生成的图片的大小
    /// </summary>
    /// <param name="typeNmae"></param>
    /// <returns></returns>
    Vector2 GetImageSize(string typeNmae)
    {
        return imgSizeDic[typeNmae];
    }
    /// <summary>
    /// 实例化相应的形状
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    void GetSizeTypeToList(string typeName, Vector3 vect)
    {

        GameObject goods = Resources.Load<GameObject>("1");
        GameObject go = Instantiate(goods);
        go.transform.parent = goodsFather.transform;
        go.transform.localPosition = Vector3.zero;
        go.GetComponent<Image>().overrideSprite = Resources.Load(typeName, typeof(Sprite)) as Sprite;
        go.GetComponent<Image>().SetNativeSize();
        
        //Vector2 vector2= go.GetComponent<Image>().rectTransform.sizeDelta;
        //PlaceSizeTypeToPos(go, vector2);

        //PlaceSizeTypeToPos(go, vect);
        Debug.Log(vect);
        go.transform.localPosition = new Vector3(vect.x, vect.y - 924, vect.z);
        go.name = typeName;
        itemList.Add(go);
    }

    /// <summary>
    /// 获取空的部分是否放得下目标形状
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    Vector3 GetEmpty(Vector2 vector2)
    {
        //List<Vector3> listX = new List<Vector3>();
        //List<Vector3> listY = new List<Vector3>();
        //List<Vector3> listW = new List<Vector3>();
        List<Vector3> listThisImage = new List<Vector3>();
        List<Vector3> v3 = new List<Vector3>();
        for (int i = 0; i < unitSizeList.Count; i += 5)
        {

            //listX = unitSizeList.FindAll((item) => { return item.x < unitSizeList[i].x + vector2.x && item.x >= unitSizeList[i].x && item.y == unitSizeList[i].y; });
            //listY = unitSizeList.FindAll((item) => { return item.x == unitSizeList[i].x && item.y > unitSizeList[i].y - vector2.y && item.y <= unitSizeList[i].y; });
            //listW = unitSizeList.FindAll((item) => { return item.x < unitSizeList[i].x + vector2.x && item.x >= unitSizeList[i].x && item.y > unitSizeList[i].y - vector2.y && item.y <= unitSizeList[i].y; });

            //List<Vector3> v3X = listX.FindAll((item) => { return item.z == 1; });
            //List<Vector3> v3Y = listY.FindAll((item) => { return item.z == 1; });
            //List<Vector3> v3W = listW.FindAll((item) => { return item.z == 1; });
            //if (v3X.Count == 0 && v3Y.Count == 0 && v3W.Count == 0 && unitSizeList[i].x + vector2.x <= 1350 && unitSizeList[i].y - vector2.y >= 50)
            //{
            //    return unitSizeList[i];
            //}
            //查找将要放置的图片大小面积区域的所有坐标点
            listThisImage = unitSizeList.FindAll((item) =>
             {
                 return ((item.x >= unitSizeList[i].x) && (item.x < unitSizeList[i].x + vector2.x)) && ((item.y <= unitSizeList[i].y) && (item.y > unitSizeList[i].y - vector2.y));
             });
            v3 = listThisImage.FindAll((item) => { return item.z == 1; });
            if (v3.Count == 0 && unitSizeList[i].x + vector2.x <= 1350 && unitSizeList[i].y - vector2.y >= 50)
            {
                return unitSizeList[i];
            }           
        }
        ////}
        //if (isFull_01)
        //{
        //    isFull_02 = true;
        //}
        isFull_01 = true;
        return Vector3.one;
    }

    /// <summary>
    /// 将相应的形状放置到相应的坐标
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="vect"></param>
    void PlaceSizeTypeToPos(GameObject go, Vector3 vect)
    {
        
        go.transform.localPosition = new Vector3(vect.x, vect.y - 924, vect.z);
        //SetUnitSizeListData(vect, go.GetComponent<RectTransform>().rect.size);
    }

    /// <summary>
    /// 设置形状占用的坐标点的z值为1：被占用
    /// </summary>
    void SetUnitSizeListData(Vector3 pos, Vector2 size)
    {
        for (int i = 0; i < unitSizeList.Count; i++)
        {
            if (((unitSizeList[i].x >= pos.x) && (unitSizeList[i].x < pos.x + size.x)) && ((unitSizeList[i].y <= pos.y) && (unitSizeList[i].y > pos.y - size.y)))
            {
                unitSizeList[i] = new Vector3(unitSizeList[i].x, unitSizeList[i].y, 1);
            }
        }
    }
}
