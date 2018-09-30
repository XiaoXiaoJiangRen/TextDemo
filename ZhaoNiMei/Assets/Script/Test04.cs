using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Test04 : MonoBehaviour
{
    private Canvas canvas;

    private List<GameObject> allAnimalList = new List<GameObject>();//保存所有实例化出来的动物图片
    private GameObject allAnimalFather;//所有动物图片的父物体


    private List<LinkGameInfo> linkGameList = new List<LinkGameInfo>();
    private Dictionary<int, GameInfo> gameInfoDict = new Dictionary<int, GameInfo>();
    private ParseXml parseXml;

    private List<string> animalName = new List<string>();//所有要生成的图片的名字   
    private List<GameObject> selectImg = new List<GameObject>();//存放两次选中的图片
    private List<GameObject> tipsImglist = new List<GameObject>();//存放提示的两个图片

    private int gameNumber = 1;//游戏轮数
    private string selectImgName = "0";//点击选中图片的名字
    private bool isClick = true;//在点击到两个相同图片，但图片没有消除时是否可以在点击其他图片

    public GameObject lineSpecialEffect;//线的特效 
    private GameObject animalCard;//消除的动物的卡片
    private GameObject boom01;//爆炸特效
    private GameObject boom02;
    public GameObject boomPrefab;//爆炸预制体
    private Animator anim;
    private bool isBoom = false;

    private Button tipsBtn;//提示按钮
    private Tweener tweener01;//第一个提示图片的tweener01
    private Tweener tweener02;//第二个提示图片的tweener02
    private int index01 = 0;//第一个提示图片在集合中的索引
    private int index02 = 0;//第二个提示图片在集合中的索引

    private Slider baiFenBiSlider;//总共完成百分比的slider
    private Text baiFenText;//总共完成的百分比
    private Image lunShuImg;//第几轮图片
    private Image diuShuImg;//本轮还剩几对动物图片
    private Text xianSuoTimeText;//线索倒计时
    private Image alarmClockImg;//闹钟图片
    private Slider countTimeSlider;//倒计时slider
    private Text countTimeText;//倒计时

    private int countTime = 60;//本轮剩余时间
    private int tipsCountTime = 10;//提示冷却时间
    private int animalImgDuiShu;//剩余图片的对数
    private bool isOver = false;//本轮是否结束
    private float totalNeedSelectImg;//总共需要选出来的动物图片
    private float curSelectImgNum;//当前完成选出来的图片个数

    private Tweener countTimeTweener;//倒计时文本的Tweener
    private Tweener alarmClockTweener;//闹钟图片的Tweener

    // Use this for initialization
    void Start()
    {
        canvas = transform.parent.GetComponent<Canvas>();
        allAnimalFather = transform.Find("AnimalFather").gameObject;
        parseXml = GetComponent<ParseXml>();
        linkGameList = parseXml.linkGameList;
        gameInfoDict = parseXml.gameInfoDict;
        animalImgDuiShu = int.Parse(linkGameList[gameNumber].totalCount) / 2;

        tipsBtn = transform.Find("Menu_bar/CueButton").GetComponent<Button>();
        tipsBtn.onClick.AddListener(TipsClue);
        animalCard = transform.Find("Card").gameObject;

        baiFenBiSlider = transform.Find("Photo_box/Photo_box01/BaiFenBiSlider").GetComponent<Slider>();
        baiFenText = transform.Find("Photo_box/Photo_box01/BaiFenBi").GetComponent<Text>();
        lunShuImg = transform.Find("Menu_bar/1").GetComponent<Image>();
        diuShuImg = transform.Find("Menu_bar/Pairs").GetComponent<Image>();
        xianSuoTimeText = transform.Find("Menu_bar/CueButton/Time").GetComponent<Text>();
        alarmClockImg = transform.Find("Menu_bar/CountDown/AlarmClock").GetComponent<Image>();
        countTimeSlider = transform.Find("Menu_bar/CountDown/CountTimeSlider").GetComponent<Slider>();
        countTimeText = transform.Find("Menu_bar/CountDown/CountTimeText").GetComponent<Text>();

        diuShuImg.sprite = Resources.Load<Sprite>("LianLianKan/" + animalImgDuiShu);
        totalNeedSelectImg = (int.Parse(linkGameList[1].totalCount) + int.Parse(linkGameList[2].totalCount) + int.Parse(linkGameList[3].totalCount)) / 2;
        baiFenBiSlider.maxValue = totalNeedSelectImg;

        GetAnimalNum();
        StartCoroutine(CountTime());
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }

        if (isBoom)
        {
            AnimatorStateInfo animatorInfo = anim.GetCurrentAnimatorStateInfo(0);
            if ((animatorInfo.normalizedTime > 1.0f) && (animatorInfo.IsName("boom")))//normalizedTime: 范围0 -- 1,  0是动作开始，1是动作结束
            {
                boom01.SetActive(false);
                boom02.SetActive(false);
                isBoom = false;
                if (animalImgDuiShu == 0 && gameNumber <= 3)
                {
                    NextRoundGame();
                }
            }
        }

    }
    //获取每一轮总的图片个数
    void GetAnimalNum()
    {
        int animaTotalNum = int.Parse(linkGameList[gameNumber].totalCount);

        SetGroupHeight(animaTotalNum);
        RandomAnimaIndex(animaTotalNum);
        CreateCheckerboard(animaTotalNum);
    }
    //设置盛放所有动物图片allAnimalFather的高
    void SetGroupHeight(int totalAnimalNum)
    {
        float width = 900f;
        if (totalAnimalNum >= 16 && totalAnimalNum <= 36)
        {
            allAnimalFather.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 600f);
        }
        else
        {
            switch (totalAnimalNum)
            {
                case 2:
                    allAnimalFather.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 150f);
                    break;
                case 4:
                    allAnimalFather.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 300f);
                    break;
                case 6:
                    allAnimalFather.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 300f);
                    break;
                case 8:
                    allAnimalFather.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 300f);
                    break;
                case 10:
                    allAnimalFather.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 300f);
                    break;
                case 12:
                    allAnimalFather.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 450f);
                    break;
                case 14:
                    allAnimalFather.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 450f);
                    break;
                default:
                    break;
            }
        }
    }
    //随机动物图片的位置
    void RandomAnimaIndex(int totalAnimalNum)
    {
        int randomIndex = 0;//随机的位置
        string[] animaType = linkGameList[gameNumber].type.Split('|');
        string[] animaCount = linkGameList[gameNumber].count.Split('|');

        int[] nums = new int[totalAnimalNum];
        for (int i = 0; i < totalAnimalNum; i++)
        {
            animalName.Add("0");
            nums[i] = i;
        }

        for (int i = 0; i < animaType.Length; i++)
        {
            for (int j = 0; j < int.Parse(animaCount[i]); j++)
            {
                //animalName.Add(animaType[i]);
                do
                {
                    randomIndex = Random.Range(0, totalAnimalNum);
                } while (nums[randomIndex] == 100);
                //animalIndex.Add(randomIndex);

                animalName[randomIndex] = animaType[i];
                nums[randomIndex] = 100;
            }
        }
    }
    //生成棋盘
    void CreateCheckerboard(int totalAnimalNum)
    {
        for (int i = 0; i < totalAnimalNum; i++)
        {
            GameObject go = Instantiate(Resources.Load("Prefab/AnimalItem")) as GameObject;
            go.transform.SetParent(allAnimalFather.transform, false);
            go.transform.Find("Father/AnimalImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("SuCai/" + animalName[i]);
            go.transform.Find("Father/SelectingAnimalImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("SuCai/" + animalName[i] + "Selecting");
            go.name = animalName[i];
            allAnimalList.Add(go);
        }

    }

    void OnClick()
    {
        //从摄像机发出到点击坐标的射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            GameObject gameObj = hitInfo.collider.gameObject;
            //当射线碰撞目标为boot类型的物品，执行拾取操作 
            if (gameObj.tag == "Btn" && isClick)
            {
                gameObj.transform.Find("Father/SelectingAnimalImage").gameObject.SetActive(true);
                gameObj.GetComponent<BoxCollider>().enabled = false;
                selectImg.Add(gameObj);
                JudgeImgIsSame(gameObj.name);
            }
        }
    }

    //判断连续点击的连个图片是否相同
    void JudgeImgIsSame(string name)
    {
        if (name == selectImgName)
        {
            isClick = false;
            lineSpecialEffect.SetActive(true);

            SetSpecialEffect(selectImg[0], selectImg[1]);

            Invoke("EiminateSameImg", 0.2f);
        }
        else
        {
            if (selectImgName == "0")
                selectImgName = name;
            else
            {
                selectImg[0].transform.Find("Father/SelectingAnimalImage").gameObject.SetActive(false);
                selectImg[0].GetComponent<BoxCollider>().enabled = true;
                selectImg.RemoveAt(0);
                selectImgName = name;
            }

        }
    }

    //设置闪电图片的方向和比例
    void SetSpecialEffect(GameObject obj1, GameObject obj2)
    {
        Vector3 startPos = UIPosToWordPos(obj1);
        Vector3 endPos = UIPosToWordPos(obj2);
        Vector3 pos = (startPos + endPos) / 2;
        lineSpecialEffect.transform.position = pos;

        lineSpecialEffect.transform.rotation = Quaternion.Euler(Vector3.zero);
        Vector3 direction = endPos - startPos;//终点减去起点
        float angle = Vector3.Angle(direction, Vector3.down);//计算旋转角度

        if (startPos.x < endPos.x)
            lineSpecialEffect.transform.Rotate(0, 0, angle);
        else
            lineSpecialEffect.transform.Rotate(0, 0, -angle);

        float h = lineSpecialEffect.GetComponent<SpriteRenderer>().sprite.bounds.size.y;//计算图片的高           
        float distance = (startPos - endPos).magnitude;//计算两个相同图片之间的距离
        float y = distance / h;//计算连个相同图片之间的距离与图片高的比例
        lineSpecialEffect.transform.localScale = new Vector3(1, y, 1);
    }
    //UI坐标转成世界坐标
    Vector3 UIPosToWordPos(GameObject obj)
    {
        Vector3 scr01 = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, obj.transform.position);
        //scr01.z = 0;
        //scr01.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        Vector3 pos = Camera.main.ScreenToWorldPoint(scr01);
        pos.z = 0;
        return pos;
    }
    //消除相同的图片
    void EiminateSameImg()
    {
        for (int i = 0; i < selectImg.Count; i++)
        {
            selectImg[i].transform.Find("Father/SelectingAnimalImage").gameObject.SetActive(false);
            selectImg[i].transform.GetChild(0).gameObject.SetActive(false);
            if (tipsImglist.Count != 0)
            {
                if (tipsImglist[i].name == selectImg[i].name)
                {
                    tweener01.Kill();
                    tweener02.Kill();
                    tipsImglist[index01].transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    tipsImglist[index02].transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
            }
        }
        SetBaiFenBi();
        //ShowAnimalCard(selectImg[0].name);
        CreateBoom(selectImg[0], selectImg[1]);
        selectImg.Clear();
        selectImgName = "0";
        isClick = true;
        lineSpecialEffect.SetActive(false);

        SetDuiShuImg();
    }
    //给与消除的提示信息
    void TipsClue()
    {
        SetTipsBtnState();

        selectImgName = "0";
        string name = "00";
        List<string> nameList = new List<string>();
        if (selectImg.Count != 0)
        {
            selectImg[0].GetComponent<BoxCollider>().enabled = true;
            selectImg.Clear();
        }
        if (tipsImglist.Count >= 2)
        {
            tweener01.Kill();
            tweener02.Kill();
            tipsImglist[index01].transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            tipsImglist[index02].transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        for (int i = 0; i < allAnimalList.Count; i++)
        {
            if (allAnimalList[i].transform.GetChild(0).gameObject.activeSelf)
            {
                allAnimalList[i].transform.Find("Father/SelectingAnimalImage").gameObject.SetActive(false);
                nameList.Add(allAnimalList[i].name);
            }
        }

        if (nameList.Count != 0)
        {
            int nameIndex = Random.Range(0, nameList.Count);
            name = nameList[nameIndex];
            if (name != "00")
            {
                tipsImglist = allAnimalList.FindAll(item => item.name == name && item.transform.GetChild(0).gameObject.activeSelf != false);

                if (tipsImglist.Count >= 2)
                {

                    index01 = Random.Range(0, tipsImglist.Count);
                    if (index01 + 1 < tipsImglist.Count)
                        index02 = index01 + 1;
                    else
                        index02 = index01 - 1;
                    tweener01.Kill();
                    tweener02.Kill();
                    tipsImglist[index01].transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -30f));
                    tipsImglist[index02].transform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -30f));
                    tweener01 = tipsImglist[index01].transform.GetChild(0).transform.DOLocalRotate(new Vector3(0, 0, 30f), 0.5f).SetEase(Ease.Flash).SetLoops(-1, LoopType.Yoyo);
                    tweener02 = tipsImglist[index02].transform.GetChild(0).transform.DOLocalRotate(new Vector3(0, 0, 30f), 0.5f).SetEase(Ease.Flash).SetLoops(-1, LoopType.Yoyo);
                }
            }
        }

    }
    //显示消除的动物的卡片
    void ShowAnimalCard(string animalName)
    {
        animalCard.SetActive(true);
        if (animalName == "lion1" || animalName == "lion2")
        {
            animalCard.transform.Find("AnimalImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("SuCai/Lion");
            animalCard.transform.Find("AnimalName").GetComponent<Text>().text = "Lion";
        }
        else if (animalName == "monkey1" || animalName == "monkey2")
        {
            animalCard.transform.Find("AnimalImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("SuCai/Monkey");
            animalCard.transform.Find("AnimalName").GetComponent<Text>().text = "Monkey";
        }
        else if (animalName == "tiger1" || animalName == "tiger2")
        {
            animalCard.transform.Find("AnimalImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("SuCai/Tiger");
            animalCard.transform.Find("AnimalName").GetComponent<Text>().text = "Tiger";
        }
        else
        {
            animalCard.transform.Find("AnimalImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("SuCai/Zebra");
            animalCard.transform.Find("AnimalName").GetComponent<Text>().text = "Zebra";
        }
        animalCard.transform.DOScale(1, 0.5f);
        animalCard.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f).OnComplete(HideAnimalCard);

    }
    //隐藏动物卡片
    void HideAnimalCard()
    {
        animalCard.transform.DOLocalRotate(new Vector3(0, 0, 180), 0.5f).SetDelay(1f);
        animalCard.transform.DOScale(0, 0.5f).SetDelay(1f);
    }
    //创建
    void CreateBoom(GameObject obj1, GameObject obj2)
    {
        if (boom01 == null || boom02 == null)
        {
            boom01 = Instantiate(boomPrefab) as GameObject;
            boom02 = Instantiate(boomPrefab) as GameObject;
        }
        boom01.SetActive(true);
        boom02.SetActive(true);
        boom01.transform.position = UIPosToWordPos(obj1);
        boom02.transform.position = UIPosToWordPos(obj2);

        anim = boom01.GetComponent<Animator>();
        isBoom = true;

    }
    //本轮剩余时间
    IEnumerator CountTime()
    {
        yield return new WaitForSeconds(1);
        countTime--;
        countTimeText.text = countTime + "s";
        countTimeSlider.value = countTime;
        SetCountTimeTips(countTime);
    }
    //设置倒计时提示（小于10S时的效果）
    void SetCountTimeTips(int countTime)
    {
        if (countTime > 0)
        {
            if (countTime == 9)
            {
                countTimeText.color = Color.white;
                alarmClockTweener = alarmClockImg.transform.DOLocalMove(new Vector3(255f, 382, 0), 0.05f).SetLoops(-1, LoopType.Restart);
                countTimeTweener = countTimeText.DOColor(Color.red, 0.08f).OnComplete(() =>
                { countTimeText.DOColor(Color.white, 0.1f); }).SetLoops(-1, LoopType.Yoyo);
            }
            StartCoroutine(CountTime());
        }
        else
        {
            gameNumber += 1;
            if (gameNumber <= 3)
            {
                InitPanelData();
                //NextRoundGame();
            }
            else
            {
                StopAllCoroutines();
                //游戏结束
            }
        }
    }
    //设置闹钟状态
    void SetCountTimeState()
    {
        countTime = 60;
        countTimeText.text = countTime + "s";
        countTimeSlider.value = countTime;
    }
    //提示倒计时
    IEnumerator TipsCountTime()
    {
        yield return new WaitForSeconds(1);
        tipsCountTime--;
        xianSuoTimeText.text = tipsCountTime + "s";
        if (tipsCountTime > 0)
            StartCoroutine(TipsCountTime());
        else
            tipsBtn.interactable = true;
    }
    //设置提示按钮状态
    void SetTipsBtnState()
    {
        tipsCountTime = 10;
        xianSuoTimeText.text = tipsCountTime + "s";
        StartCoroutine(TipsCountTime());
        tipsBtn.interactable = false;
    }
    //设置本轮还有几对动物图片
    void SetDuiShuImg()
    {
        animalImgDuiShu -= 1;
        if (animalImgDuiShu > 0)
        {
            diuShuImg.sprite = Resources.Load<Sprite>("LianLianKan/" + animalImgDuiShu);
        }
        else
        {
            diuShuImg.sprite = Resources.Load<Sprite>("LianLianKan/" + animalImgDuiShu);
            gameNumber += 1;
            if (gameNumber <= 3)
            {
                InitPanelData();
                //NextRoundGame();
            }
            else
            {
                StopAllCoroutines();
                //游戏结束
            }

        }
    }
    //设置百分比Slider
    void SetBaiFenBi()
    {
        curSelectImgNum += 1;
        int baiFenBi = (int)((curSelectImgNum / totalNeedSelectImg) * 100);
        baiFenText.text = baiFenBi.ToString() + "%";
        baiFenBiSlider.value = curSelectImgNum;
    }
    //初始化面板信息
    void InitPanelData()
    {
        SetCountTimeState();
        lunShuImg.sprite = Resources.Load<Sprite>("LunShu/" + gameNumber);
        if (countTimeTweener != null && alarmClockTweener != null)
        {
            countTimeTweener.Kill();
            alarmClockTweener.Kill();
            countTimeText.color = Color.black;
            alarmClockImg.transform.localPosition = new Vector3(252f, 379f, 0);
        }
    }
    //下一轮游戏开始
    void NextRoundGame()
    {
        animalImgDuiShu = int.Parse(linkGameList[gameNumber].totalCount) / 2;
        diuShuImg.sprite = Resources.Load<Sprite>("LianLianKan/" + animalImgDuiShu);

        for (int i = 0; i < allAnimalList.Count; i++)
            allAnimalList[i].SetActive(false);
        allAnimalList.Clear();

        animalName.Clear();
        selectImg.Clear();
        tipsImglist.Clear();
        selectImgName = "0";

        GetAnimalNum();
        //StartCoroutine(CountTime());
    }
}
