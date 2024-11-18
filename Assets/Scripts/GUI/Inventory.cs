using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Dictionary<string, long> Items = new(1);
    public List<Slot> Slots = new(1);
    [SerializeField] private GameObject SlotPrefab;
    [SerializeField] private RectTransform Parent;
    [SerializeField] private Text _inventoryText;
    [SerializeField] private RectTransform GridBg;
    public static void Load()
    {
        var splitted = Config.Instance.Items.Split("|");
        if (splitted.Length < 2) return;
        for (int i = 0; i < splitted.Length; i+=2)
        {
            Items[splitted[i]] = long.Parse(splitted[i + 1]);
        }
    }
    public static void Save()
    {
        Config.Instance.Items = "";
        if (Items.Count == 0) return;
        foreach (var item in Items)
        {
            Config.Instance.Items += "|" + item.Key + "|" + item.Value;
        }
        Config.Instance.Items = Config.Instance.Items[1..];
    }
    public static void Add(string name)
    {
        if (Items.ContainsKey(name))
        {
            Items[name] += 1;
            return;
        }
        Items[name] = 1;
    }
    private const float STEP_X = 430f;
    private const float STEP_Y = -500f;
    private void AddSlot()
    {
        var cur = Slots.Count;
        var slotObj = Instantiate(SlotPrefab, Parent);
        slotObj.transform.localPosition += new Vector3(
            (cur % 3 - 1) * STEP_X,
            cur / 3 * STEP_Y,
            0
        );
        Slots.Add(slotObj.GetComponent<Slot>());

        var size = Parent.sizeDelta;
        size.y = 600 - STEP_Y * (cur / 3);
        Parent.sizeDelta = size;
    }
    private void Reload()
    {
        _inventoryText.text = Localization.Localize("inventory-blocks");
        var sorted = Items.OrderBy((item) => -item.Value).ToList();


        var list = AssetLoader.ToLoadImages;
        foreach (var item in Items)
        {
            list.Add("Blocks/b_" + item.Key);
        }
        AssetLoader.HandleImages();
        AssetLoader.ToLoadImages = list;

        while (Slots.Count < sorted.Count)
        {
            AddSlot();
        }
        for (int i = 0; i < sorted.Count; i++)
        {
            Slots[i].SetItem(sorted[i].Key, sorted[i].Value);
        }
        Parent.anchoredPosition = Vector2.zero;
        GridBg.sizeDelta *= Vector2.right;
        GridBg.sizeDelta += Vector2.down * (Items.Count * STEP_Y + Screen.height * 3);
        GridBg.anchoredPosition = Vector2.up * Screen.height;
    }

    public static float TargetScale = 0;
    private float _currentScale = 0;
    private ScrollRect _scrollRect;

    private void Start()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>();
        _scrollRect.enabled = false;
        transform.localScale = Vector3.zero;
    }
    private void Update()
    {
        _currentScale = Mathf.Lerp(_currentScale, TargetScale, Time.deltaTime * 20);
        transform.localScale = Vector3.one * _currentScale;
    }
    public void Change()
    {
        TargetScale = 1 - TargetScale;
        if (TargetScale > 0.5f)
        {
            _scrollRect.enabled = true;
            _scrollRect.velocity = Vector2.zero;
            _scrollRect.verticalNormalizedPosition = 0;
            Reload();
        }
        else
        {
            _scrollRect.enabled = false;
            AssetLoader.HandleImages();
        }
    }
}