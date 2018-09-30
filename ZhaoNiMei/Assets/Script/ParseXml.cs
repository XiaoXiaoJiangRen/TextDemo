using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class ParseXml : MonoBehaviour
{
    public Dictionary<int, GameInfo> gameInfoDict = new Dictionary<int, GameInfo>();
    private List<CategoryInfo> categoryList = new List<CategoryInfo>();
    private List<GoodsInfo> goodsList = new List<GoodsInfo>();
    System.Random random = new System.Random();

    private List<Vector3> unitSizeList = new List<Vector3>();//所有网格的坐标点集合      
    private Dictionary<string, Vector2> imgSizeDic = new Dictionary<string, Vector2>();//保存所有素材图片的大小    
    private bool isFull_01 = false;//是否铺满，没有位置了

    //保存轮次和所有要生成的图片的名字和位置
    public Dictionary<int, Dictionary<string, Vector3>> numImgPosDic = new Dictionary<int, Dictionary<string, Vector3>>();
    //所有要生成的图片的名字和位置   
    Dictionary<string, Vector3> imgPosDic01 = new Dictionary<string, Vector3>();
    Dictionary<string, Vector3> imgPosDic02 = new Dictionary<string, Vector3>();
    Dictionary<string, Vector3> imgPosDic03 = new Dictionary<string, Vector3>();
    Dictionary<string, Vector3> imgPosDic04 = new Dictionary<string, Vector3>();
    private int num = 0;//记录随机出来的第几张图片

    public List<LinkGameInfo> linkGameList = new List<LinkGameInfo>();

    public Slider slider;

    private void Awake()
    {
        LoadSourceConfig();
    }
    void Start()
    {
        //LoadSourceConfig();
        GetAllPos();
        StartCalculation();

    }

    public void LoadSourceConfig()
    {
        XmlDocument xdoc = new XmlDocument();
        //xdoc.Load(@"C:\Users\ACS\Desktop\OnlineGame4.0\SourceConfig.xml");//从表格的存放路径来获取
        xdoc.Load(@"E:\XiangMuYuanMa\ZhaoNiMei\SourceConfig.xml");//从表格的存放路径来获取
        InitCategory(xdoc);
        InitGoodInfo(xdoc);
        InitGameInfo(xdoc);
        InitLinkGameInfo(xdoc);
    }

    private void InitGameInfo(XmlDocument doc)
    {
        XmlNodeList gameNode = doc.SelectNodes("GoodsList/GameInfo");
        //StringBuilder strBuilder = new StringBuilder();
        foreach (XmlNode node in gameNode)
        {
            GameInfo gameInfo = new GameInfo();
            gameInfo.id = node.Attributes["Id"].Value;
            gameInfo.type = node.Attributes["Type"].Value;
            gameInfo.group = node.Attributes["Detailed"].Value;
            gameInfoDict.Add(int.Parse(gameInfo.id), gameInfo);
        }
    }
    private void InitCategory(XmlDocument doc)
    {
        XmlNodeList categoryNode = doc.SelectNodes("GoodsList/Category");
        foreach (XmlNode node in categoryNode)
        {
            CategoryInfo categoryInfo = new CategoryInfo();
            categoryInfo.id = node.Attributes["Id"].Value;
            categoryInfo.type = node.Attributes["Type"].Value;
            categoryInfo.goodgroup = node.Attributes["ResId"].Value;
            categoryList.Add(categoryInfo);
        }
    }
    private void InitGoodInfo(XmlDocument doc)
    {
        XmlNodeList goodsNode = doc.SelectNodes("GoodsList/GoodsInfo");
        foreach (XmlNode node in goodsNode)
        {
            GoodsInfo goodsInfo = new GoodsInfo();
            goodsInfo.id = node.Attributes["Id"].Value;
            goodsInfo.goodName = node.Attributes["Name"].Value;
            goodsInfo.width = node.Attributes["Width"].Value;
            goodsInfo.hight = node.Attributes["Hight"].Value;
            goodsList.Add(goodsInfo);
            if (goodsInfo.width != " " && goodsInfo.hight != " " && goodsInfo.goodName != " ")
            {
                //把所有图片的名字和大小储存在字典中
                imgSizeDic.Add(goodsInfo.goodName, new Vector2(float.Parse(goodsInfo.width), float.Parse(goodsInfo.hight)));
            }
        }
    }

    private void InitLinkGameInfo(XmlDocument doc)
    {
        XmlNodeList LinkGameNode = doc.SelectNodes("GoodsList/LinkGameInfo");
        foreach (XmlNode node in LinkGameNode)
        {
            LinkGameInfo linkGameInfo = new LinkGameInfo();
            linkGameInfo.id = node.Attributes["Id"].Value;
            linkGameInfo.type = node.Attributes["Type"].Value;
            linkGameInfo.count = node.Attributes["Count"].Value;
            linkGameInfo.totalCount = node.Attributes["TotalCount"].Value;
            linkGameList.Add(linkGameInfo);
        }

    }
    #region 

    /// <summary>
    /// 开始计算图片坐标
    /// </summary>
    private void StartCalculation()
    {
        string[] goodgroup = gameInfoDict[1].group.Split('|');
        string[] strs;
        for (int i = 0; i < goodgroup.Length; i++)
        {
            strs = goodgroup[i].Split(',');
            Group(strs);
            //numImgPosDic.Add(int.Parse(strs[0]), imgPosDic);
            if (strs[0] == "1")
            {
                numImgPosDic.Add(1, imgPosDic01);
            }
            else if (strs[0] == "2")
            {
                numImgPosDic.Add(2, imgPosDic02);
            }
            else if (strs[0] == "3")
            {
                numImgPosDic.Add(3, imgPosDic03);
            }
            else if (strs[0] == "4")
            {
                numImgPosDic.Add(4, imgPosDic04);
            }
            //imgPosDic.Clear();
            SetUnitSizeListZ();
            num = 0;
            isFull_01 = false;
        }
        jkjk();
    }
    void jkjk()
    {
        foreach (var item in numImgPosDic)
        {
            foreach (var sasa in item.Value)
            {
                Debug.Log(sasa.Key + "--" + sasa.Value);
            }
        }
    }

    private void Group(string[] strs)
    {
        string goods = categoryList[int.Parse(strs[0])].goodgroup;
        string roundtime = strs[1];
        //string times = strs[2];
        string[] goodNum = goods.Split('|');
        string[] totalNum = goods.Split('|');
        int count = goodsList.Count;

        int[] nums = new int[count];
        for (int i = 0; i < count; i++)
        {
            nums[i] = i;
        }
        nums[int.Parse(totalNum[0])] = 0;
        nums[int.Parse(totalNum[1])] = 0;
        nums[int.Parse(totalNum[2])] = 0;

        GetImgPos(goodNum, nums, count, strs);
    }
    /// <summary>
    /// 找到合适的位置放置图片
    /// </summary>
    void GetImgPos(string[] goodNum, int[] nums, int count, string[] strs)
    {
        int randomNum1 = 0;
        int randomNum2 = 0;

        List<string> selectImg = new List<string>();//选中的图片（再填的时候需要过滤掉的图片）
        List<int> selectImgWeiZhi = new List<int>();//选中的图片需要在第几次实例出来
        for (int i = 0; i < 2; i++)
        {
            do
            {
                randomNum1 = random.Next(0, 3);
            } while (goodNum[randomNum1] == "0");
            selectImg.Add(goodsList[int.Parse(goodNum[randomNum1])].goodName);
            goodNum[randomNum1] = "0";

            //给需要选中的图片随机在第几次生成
            int weiZhi = UnityEngine.Random.Range(1, 12);
            if (i == 0)
            {
                selectImgWeiZhi.Add(weiZhi);
            }
            else
            {
                if (weiZhi == selectImgWeiZhi[i - 1])
                {
                    do
                    {
                        weiZhi = UnityEngine.Random.Range(1, 12);
                    } while (weiZhi == selectImgWeiZhi[i - 1]);
                }
                selectImgWeiZhi.Add(weiZhi);
            }
        }

        do
        {
            do
            {
                randomNum2 = random.Next(0, count);
            } while (nums[randomNum2] == 0);

            num += 1;
            string goodsName = "";

            //在这随机插入需要选出来的图片
            if (selectImgWeiZhi[0] == num)
            {
                goodsName = selectImg[0];
            }
            else if (selectImgWeiZhi[1] == num)
            {
                goodsName = selectImg[1];
            }
            else
            {
                goodsName = goodsList[nums[randomNum2]].goodName;
                nums[randomNum2] = 0;
            }

            Vector3 pos = GetEmpty(GetImageSize(goodsName));
            if (pos != Vector3.one)
            {
                SetUnitSizeListData(pos, GetImageSize(goodsName));
                //imgPosDic.Add(goodsName, pos);

                if (strs[0] == "1")
                {
                    imgPosDic01.Add(goodsName, pos);
                }
                else if (strs[0] == "2")
                {
                    imgPosDic02.Add(goodsName, pos);
                }
                else if (strs[0] == "3")
                {
                    imgPosDic03.Add(goodsName, pos);
                }
                else if (strs[0] == "4")
                {
                    imgPosDic04.Add(goodsName, pos);
                }
            }
            else
            {
                //用小的图片在填一遍
                AgainCreatGoods(GetImageSize(goodsName), selectImg, strs);
            }
        } while (!isFull_01);

    }
    #endregion


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
    /// 再次生成图片填充空白区域
    /// </summary>
    /// <param name="vector2"></param>
    void AgainCreatGoods(Vector2 vector2, List<string> list, string[] strs)
    {
        float width = vector2.x;
        float height = vector2.y;
        List<string> smallerList = new List<string>();//储存比目标值小的图片

        foreach (var item in imgSizeDic)
        {
            if ((item.Value.x < width && item.Value.y < height) || (item.Value.x == width && item.Value.y < height) || (item.Value.x < width && item.Value.y == height))
            {
                if (item.Key != list[0] && item.Key != list[1])
                {
                    smallerList.Add(item.Key);
                }
            }
        }

        if (smallerList.Count != 0)
        {
            //for (int j = 0; j < 3; j++)
            //{
            for (int i = 0; i < smallerList.Count; i++)
            {
                //Vector3 v3;
                //if (!imgPosDic.TryGetValue(smallerList[i], out v3))
                //{
                //    Vector3 pos = GetEmpty(GetImageSize(smallerList[i]));
                //    if (pos != Vector3.one)
                //    {
                //        SetUnitSizeListData(pos, GetImageSize(smallerList[i]));
                //        imgPosDic.Add(smallerList[i], pos);
                //        //if (strs[0] == "1")
                //        //{
                //        //    imgPosDic01.Add(smallerList[i], pos);
                //        //}
                //        //else if (strs[0] == "2")
                //        //{
                //        //    imgPosDic02.Add(smallerList[i], pos);
                //        //}
                //        //else if (strs[0] == "3")
                //        //{
                //        //    imgPosDic03.Add(smallerList[i], pos);
                //        //}
                //        //else if (strs[0] == "4")
                //        //{
                //        //    imgPosDic04.Add(smallerList[i], pos);
                //        //}
                //    }
                //}

                switch (strs[0])
                {
                    case "1":
                        SetDic(imgPosDic01, smallerList[i]);
                        break;
                    case "2":
                        SetDic(imgPosDic02, smallerList[i]);
                        break;
                    case "3":
                        SetDic(imgPosDic03, smallerList[i]);
                        break;
                    case "4":
                        SetDic(imgPosDic04, smallerList[i]);
                        break;
                    default:
                        break;
                }
            }
            //}

        }

    }
    void SetDic(Dictionary<string, Vector3> dic, string typeName)
    {
        Vector3 v3;
        if (!dic.TryGetValue(typeName, out v3))
        {
            Vector3 pos = GetEmpty(GetImageSize(typeName));
            if (pos != Vector3.one)
            {
                SetUnitSizeListData(pos, GetImageSize(typeName));
                dic.Add(typeName, pos);
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
    /// 获取空的部分是否放得下目标形状
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    Vector3 GetEmpty(Vector2 vector2)
    {
        List<Vector3> listThisImage = new List<Vector3>();

        for (int i = 0; i < unitSizeList.Count; i += 10)
        {
            //查找将要放置的图片大小面积区域的所有坐标点
            listThisImage = unitSizeList.FindAll((item) =>
            {
                return ((item.x >= unitSizeList[i].x) && (item.x < unitSizeList[i].x + vector2.x)) && ((item.y <= unitSizeList[i].y) && (item.y > unitSizeList[i].y - vector2.y));
            });

            //for (int j = 0; j < unitSizeList.Count; j++)
            //{
            //    if ((unitSizeList[j].x >= unitSizeList[i].x && unitSizeList[j].x < unitSizeList[i].x + vector2.x) && (unitSizeList[j].y <= unitSizeList[i].y && unitSizeList[j].y > unitSizeList[i].y - vector2.y))
            //    {
            //        listThisImage.Add(unitSizeList[j]);
            //    }
            //}

            List<Vector3> v3 = listThisImage.FindAll((item) => { return item.z == 1; });
            if (v3.Count == 0 && unitSizeList[i].x + vector2.x <= 1350 && unitSizeList[i].y - vector2.y >= 50)
            {
                return unitSizeList[i];
            }

        }
        num = 0;
        isFull_01 = true;
        return Vector3.one;
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
    /// <summary>
    /// 设置所有网格点的Z值为0
    /// </summary>
    void SetUnitSizeListZ()
    {
        for (int i = 0; i < unitSizeList.Count; i++)
        {
            if (unitSizeList[i].z == 1)
                unitSizeList[i] = new Vector3(unitSizeList[i].x, unitSizeList[i].y, 0);
        }
    }
}
