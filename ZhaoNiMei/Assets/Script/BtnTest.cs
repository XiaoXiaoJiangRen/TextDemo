using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BtnTest : MonoBehaviour
{

    GameObject gameObj;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
        if (Input.GetMouseButtonUp(0))
        {
            SetBtnUp();
        }
    }
    void OnClick()
    {
        
        //从摄像机发出到点击坐标的射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            //划出射线，只有在scene视图中才能看到
            Debug.DrawLine(ray.origin, hitInfo.point);
            gameObj = hitInfo.collider.gameObject;
            //Debug.Log("click object name is " + gameObj.name);
            //当射线碰撞目标为boot类型的物品，执行拾取操作
            if (gameObj.tag == "Btn")
            {
                SetBtnDown();               
                //Debug.Log("pickup!");
            }
        }
    }
    void SetBtnDown()
    {
        gameObj.GetComponent<Transform>().DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.05f).SetEase(Ease.Flash);
        gameObj.GetComponent<BoxCollider>().enabled = false;      
    }
    void SetBtnUp()
    {
        if (gameObj!=null)
        {
            gameObj.GetComponent<Transform>().DOScale(new Vector3(1f, 1f, 1f), 0.05f).SetEase(Ease.Flash).OnComplete(() => { gameObj.GetComponent<BoxCollider>().enabled = true; });
            //transform.GetComponent<ParseXml>().StartGame(); 
        }
    }

}
