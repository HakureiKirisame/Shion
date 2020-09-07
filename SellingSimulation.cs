using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingSimulation : MonoBehaviour
{
    List<Quest> quests = new List<Quest>();
    private float[] BasicSellingPrices; //一般价格
    private float[] WholeSalePrices; //批发价格
    private float[] BookPrices; //订货价格

    public string SellingType; //选择售卖模式，可输入基础，批发，订货，（导购，自动交易和combo还没做）
    //缺导购，连击combo,自动交易
    public float[] DecidedSellingPrice;
    public float Revenue;
    public float FinalMoney;
    public string DelayOrNot; //判断订后交付成功或失败，输入“成功”或“失败”


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ImportSimulation>().ImportData();
        DecidedSellingPrice = new float[GetComponent<ImportSimulation>().ImportTypeAmounts];
    }

    // Update is called once per frame
    void Update()
    {   
    }


    //以下为卖货代码


    public void SelectSellingType()
    {
        if (SellingType == "基础")
            BasicSell();
        else if (SellingType == "批发")
            WholeSale();
        else if (SellingType == "订货")
            Book();

    }

    public float BasicSell() //一般交易
    {
        
        int a = GetComponent<ImportSimulation>().ImportTypeAmounts;
        int location = 0;
        
        
        for (int i = 0; i < a; i++)
        {
            foreach (Quest q in quests)
            {
                if (GetComponent<ImportSimulation>().ProductNames[i] != q.ProductNames)
                {
                    location = location++;
                }
                else
                    BasicSellingPrices[i] = (float)(1.2 * q.ImportPrices);
            }
            Revenue += DecidedSellingPrice[i] * GetComponent<ImportSimulation>().ImportAmounts[i];  //自行决定价格的利润，未计算顾客心情等
        }
        return Revenue;
    }

    public float WholeSale() //批发
    {
        int a = GetComponent<ImportSimulation>().ImportTypeAmounts;
        int location = 0;

        for (int i = 0; i < a; i++)
        {
            foreach (Quest q in quests)
            {
                if (GetComponent<ImportSimulation>().ProductNames[i] != q.ProductNames)
                {
                    location = location++;
                }
                else
                { 
                    WholeSalePrices[i] = (float)(1.02 * q.ImportPrices);
                    Revenue += (float)1.02 * WholeSalePrices[i] * GetComponent<ImportSimulation>().ImportAmounts[i];  //未计算顾客心情等,售价为进货的102%

                }
            }
           
        }
        return Revenue;
    }

    public float Book() //订货代码
    {
        int a = GetComponent<ImportSimulation>().ImportTypeAmounts;
        int location = 0;
        float Prepaid = 0;

        for (int i = 0; i < a; i++)
        {
            foreach (Quest q in quests)
            {
                if (GetComponent<ImportSimulation>().ProductNames[i] != q.ProductNames)
                {
                    location = location++;
                }
                else
                { 
                    BookPrices[i] = (float)(1.2 * q.ImportPrices);
                    Prepaid +=(float)0.123* q.ImportPrices  * GetComponent<ImportSimulation>().ImportAmounts[i];  //未计算顾客心情等,售价为进货的102%
                }
            }
           
        }

        if (Delay() == true)
            Revenue += Prepaid;
        else
            Revenue -= 2 * Prepaid;
        
        return Revenue;
    }

    public bool Delay() //是否按时交货
    {
        bool a=true;
        if (DelayOrNot == "成功")
            a = true;
        else if (DelayOrNot == "失败")
            a = false;
        else
            Debug.Log("未正确判明");

        return a;
    }

    public float Final() //最终余额
    {
        FinalMoney = GetComponent<ImportSimulation>().InitialMoney - GetComponent<ImportSimulation>().FinalImportCosts + Revenue;
        return FinalMoney;
    }
}
