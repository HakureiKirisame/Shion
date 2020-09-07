using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ImportSimulation : MonoBehaviour
{
    List<Quest> quests = new List<Quest>();
    public float InitialMoney;
    public int StoreLevels;
    public int ImportTypeAmounts;

    public string[] ProductNames;
    public int[] ImportAmounts;
    public float TotalCosts;
    public float ImportDiscounts; //之后需细化
    public float FinalImportCosts;

    public GameObject layoutObject;
    public InputField inputFieldPrefab;


    public GameObject AmountLayoutObject;
    public InputField AmountInputFieldPrefab;


    public Text CostBeforeDis;

    private int k;
    private int j = 0;
    private int l = 0;
    void Start()
    {
        ImportData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //以下为进口代码

    public void ImportData()
    {
        
        TextAsset questdata = (TextAsset)Resources.Load("ItemsAndPrice", typeof(TextAsset));   //录入数据,依次为：供货点，产品类型，产品条目，最低店铺等级，商品tag，基础价格（进货）

        string[] data = questdata.text.Split(new char[] { '\n' });
        k = data.Length - 1;
        for (int i = 1; i < data.Length - 1; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });

            Quest q = new Quest();

            q.SupplySites = row[0];
            q.ProductTypes = row[1];
            q.ProductNames = row[2];
            int.TryParse(row[3], out q.StoreLevels); //防止数据空集造成错误+格式转换
            q.ProductTags = row[4];
            int.TryParse(row[5], out q.ImportPrices);

            quests.Add(q);
        }
    }

    public int[] StoreLevelJudge(int i) //用于商店等级判断
    {
        int[] a=new int[k];
        int j = 0;
        foreach (Quest q in quests)
        {

            if (i < q.StoreLevels)
            {
                a[j] = 0;
            }
            else
                a[j] = 1;

            j = j++;
        }
        return a; //若a[i]=0，则不可调用，=1则可调用
    }

    public void ProductNamesAmount() //进货的种类数量，若商店等级不符，则驳回
    {
        int location = 0;
        for (int i = 0; i < ImportTypeAmounts ; i++)
        {
            foreach (Quest q in quests)
            {
                if (ProductNames[i] != q.ProductNames)
                {
                    location = location++;
                }
                else
                {
                    if (StoreLevelJudge(StoreLevels)[location] == 0)
                    {
                        Debug.Log("未达到要求的商铺等级");
                    }
                   
                }
            }
            location = 0;
        }
    }

    public float TotalCost() //未折扣之前的总价格
    {

        int location = 0;
        for (int i = 0; i < ImportTypeAmounts; i++)
        {
            foreach (Quest q in quests)
            {
                if (ProductNames[i] != q.ProductNames)
                {
                    location = location++;
                }
                else
                {
                    if (StoreLevelJudge(StoreLevels)[location] == 0)
                    {
                        Debug.Log("未达到要求的商铺等级");
                    }
                    TotalCosts += q.ImportPrices * ImportAmounts[i];
                }
            }
            location = 0;
        }
        return TotalCosts;
    }

    public float FinalImportCost() //通过店员折扣后总进货价
    {
        FinalImportCosts = (1 - ImportDiscounts) * TotalCost();

        if (FinalImportCosts > InitialMoney)
            Debug.Log("钱不够");

        return FinalImportCosts;

    }

  //以下为UI部分代码
    public void Ini(string NewText)
    {
        float temp = float.Parse(NewText);
        InitialMoney = temp;
    }
    public void Amount(string NewText)
    {
        int temp = int.Parse(NewText);
        ImportTypeAmounts = temp;

        ProductNames = new string[ImportTypeAmounts];
        ImportAmounts = new int[ImportTypeAmounts];

        var inputFieldList = new string[ImportTypeAmounts];
        var AmountinputFieldList = new int[ImportTypeAmounts];

        for (int i = 0; i < ImportTypeAmounts; i++)
        {
            // InputField go = ;
            Instantiate(inputFieldPrefab,layoutObject.transform).tag = "Prefab";
            
            Instantiate(AmountInputFieldPrefab, AmountLayoutObject.transform).tag = "Prefab";
            
            
            //var inputField = go;
            ProductNames[i] = "null";
            ImportAmounts[i] = 0;
        }
    }

    public void Level(string NewText)
    {
        int temp = int.Parse(NewText);
        StoreLevels = temp;
        StoreLevelJudge(StoreLevels);

    }
    public void Name(string NewText)
    {
        ProductNames[j] = NewText;
        j++;
        ProductNamesAmount();
    }


    public void Amounts(string NewText)
    {
        ImportAmounts[l] = int.Parse(NewText);
        l++;
    }

    public void CostBeforeDiscount()
    {
        TotalCost();
        CostBeforeDis.text = " " + TotalCosts;
    }

    public void Discount(string NewText)
    {
        float temp = float.Parse(NewText);
        ImportDiscounts = temp;
    }

    public void CostAterDiscount(string NewText)
    {
        float temp = float.Parse(NewText);
        FinalImportCosts = temp;
    }

    public void Clear()
    {
        ImportAmounts = new int[ImportTypeAmounts];
        ProductNames = new string[ImportTypeAmounts];
        TotalCosts = 0;
        GameObject[] all = GameObject.FindGameObjectsWithTag("Prefab");
        for (int i=0;i<all.Length;i++)
        {
            Destroy(all[i]);
        }
        j = 0;
        l = 0;

    }
}
