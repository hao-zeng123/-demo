using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PalInfo
{
    public string name;
    public Sprite icon;
    public GameObject prefab;
}

public class GameMode : MonoBehaviour
{
    public static GameMode Instance { get; private set; }

    public GameObject gameOverPanel;

    public RectTransform bagPanel;
    public RectTransform contentParent;
    public PalItem prefabUiItem;
    public Text textPalBallNum;

    Player player;

    public int numPalBall = 0;

    public List<PalInfo> listPalInfo;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(false);
        }
        if (textPalBallNum)
        {
            textPalBallNum.text = numPalBall.ToString();
        }
        if (bagPanel)
        {
            bagPanel.gameObject.SetActive(false);
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        if (player && player.isDie && !gameOverPanel.activeInHierarchy)
        {
            GameOver();
        }
    }

    public PalInfo GetPalInfo(string name)
    {
        PalInfo info = listPalInfo.Find((pal) => pal.name == name);
        if (info == null)
        {
            Debug.LogWarning("没找到帕鲁的信息：" + name);
            return null;
        }

        return info;
    }

    public void GameOver()
    {
        gameOverPanel?.SetActive(true);
    }

    public void Restart()
    {
        if (player)
        {
            player.Revive();
        }
        gameOverPanel.SetActive(false);
    }

    public void OpenBagPanel()
    {
        bool active = bagPanel.gameObject.activeInHierarchy;
        if (active)
        {
            bagPanel.gameObject.SetActive(false);
        }
        else
        {
            bagPanel.gameObject.SetActive(true);
        }
    }

    // 捕捉到帕鲁，添加帕鲁到界面列表里
    public void AddPal(Enemy enemy)
    {
        if (!prefabUiItem) { return; }
        // 创建UI对象，一格包含一个帕鲁的信息。
        PalItem item = Instantiate(prefabUiItem, contentParent);
        // 设置帕鲁名字等信息到UI元素中，后续使用
        PalHealth palHealth = enemy.GetComponent<PalHealth>();
        item.name = enemy.name;
        item.hp = palHealth.hp;
        item.maxHp = palHealth.maxHp;
    }

    // 增加一个帕鲁球
    public void AddPalBall()
    {
        numPalBall++;
        if (textPalBallNum)
        {
            textPalBallNum.text = numPalBall.ToString();
        }

    }

    // 消耗一个帕鲁球
    public void RemovePalBall()
    {
        numPalBall--;
        if (textPalBallNum)
        {
            textPalBallNum.text = numPalBall.ToString();
        }
    }

    // 释放帕鲁到场景中
    public GameObject ReleasePal(PalItem palItem)
    {
        var info = GetPalInfo(palItem.name);
        if (info == null || !info.prefab)
        {
            return null;
        }

        GameObject obj = Instantiate(info.prefab);
        obj.transform.position = player.transform.position + (Vector3)Random.insideUnitCircle * 2;
        return obj;
    }
}
