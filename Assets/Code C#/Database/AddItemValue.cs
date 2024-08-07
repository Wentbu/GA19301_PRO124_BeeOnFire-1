using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class AddItemValue : MonoBehaviour
{
    private static readonly string GetItemValueURL = "https://phamduchuan.name.vn/GetValueItems.php";
    private string assetFolderPath = "Assets/ItemsData/";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetItemsData());
    }

    IEnumerator GetItemsData()
    {
        UnityWebRequest www = UnityWebRequest.Get(GetItemValueURL);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            var jsonResult = www.downloadHandler.text;
            Debug.Log("Received JSON: " + jsonResult); // Log JSON nhận được để kiểm tra

            List<ItemData> itemsList = ParseJsonData(jsonResult);
            foreach (var item in itemsList)
            {
                if (item != null)
                {
                    Debug.Log($"Item Id: {item.Items_Id}, Name: {item.Items_Name}");
                }
                else
                {
                    Debug.LogError("Null item found in list.");
                }
                // Tạo ScriptableObject cho mỗi hàng trong CSDL
                ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.Items_Id = item.Items_Id;
                itemData.Items_Name = item.Items_Name.ToString();
                itemData.Feature = item.Feature.ToString();
                itemData.Duration = item.Duration;

                // Lưu ScriptableObject vào tệp
                SaveItemDataAsAsset(itemData, item.Items_Name);
            }

            // Lưu các thay đổi trong Editor
            AssetDatabase.SaveAssets();
        }
    }

    List<ItemData> ParseJsonData(string json)
    {
        // Nếu JSON không đúng định dạng, log lỗi để kiểm tra
        try
        {
            ItemsList itemList = JsonUtility.FromJson<ItemsList>(json);
            return itemList.items;
        }
        catch (System.Exception e)
        {
            Debug.LogError("JSON Parse Error: " + e.Message);
            return new List<ItemData>();
        }
    }

    void SaveItemDataAsAsset(ItemData itemData, string itemName)
    {
        // Đảm bảo thư mục tồn tại
        if (!AssetDatabase.IsValidFolder(assetFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", "ItemsData");
        }

        string assetPath = assetFolderPath + itemName + ".asset";
        AssetDatabase.CreateAsset(itemData, assetPath);
    }

    [System.Serializable]
    public class ItemsList
    {
        public List<ItemData> items;
    }
}
