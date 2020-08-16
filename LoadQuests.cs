using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadQuests : MonoBehaviour
{
    List<Quest> quests = new List<Quest>();

    // 注意！此类文件只能从名为“Resources”的文件夹中提取，其他文件夹会造成检索失败
    void Start()
    {
        TextAsset questdata = (TextAsset)Resources.Load("ItemsAndPrice",typeof(TextAsset)) ;   //录入数据,依次为：供货点，产品类型，产品条目，最低店铺等级，商品tag，基础价格（进货）

        string[] data = questdata.text.Split(new char[] { '\n' });

        for(int i = 0;i<data.Length - 1;i++)
        {
            string[] row = data[i].Split(new char[]{ ','});
            Quest q = new Quest();

            q.SupplySites = row[0];
            q.ProductTypes = row[1];
            q.ProductNames = row[2];
            int.TryParse(row[3], out q.StoreLevels); //防止数据空集造成错误
            q.ProductTags = row[4];
            int.TryParse(row[5], out q.ImportPrices);

            quests.Add(q);
        }
        foreach(Quest q in quests)
        {
            Debug.Log(q.ImportPrices);
        }
    }

   
}
