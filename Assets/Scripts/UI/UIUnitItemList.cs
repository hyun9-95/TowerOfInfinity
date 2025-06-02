using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUnitItemList : MonoBehaviour
{
    [SerializeField]
    GameObject templateItem;

    [SerializeField]
    Transform contentsTransform;

    [SerializeField]
    VerticalLayoutGroup verticalLayoutGroup;

    private int useCount = 0;
    private readonly List<GameObject> unitList = new();
    Vector2 newSize = Vector2.zero;

    private void Awake()
    {
        if (verticalLayoutGroup != null)
            newSize = new Vector2(0, verticalLayoutGroup.padding.top + verticalLayoutGroup.padding.bottom);
    }

    public void ResetUseCount()
    {
        useCount = 0;
    }

    public void AddUnitItemWithModels<T>(T[] models) where T : IBaseUnitModel
    {
        for (int i = 0; i < models.Length; i++)
        {
            useCount++;

            GameObject go;

            if (GetUnitListCount() < useCount)
            {
                go = Instantiate(templateItem, contentsTransform);
                go.SafeSetActive(false);
                unitList.Add(go);
            }
            else
            {
                go = GetUnit(i);
            }

            if (go == null)
                continue;

            BaseUnit<T> unit = go.GetComponent<BaseUnit<T>>();

            SetUnitModel(unit, models[i]);
        }
    }

    public T AddUnitItem<T>(int index) where T : Component
    {
        useCount++;

        GameObject go;

        if (GetUnitListCount() < useCount)
        {
            go = Instantiate(templateItem, contentsTransform);
            go.SafeSetActive(false);
            unitList.Add(go);
        }
        else
        {
            go = GetUnit(index);
        }

        return go.GetComponent<T>();
    }

    private void SetUnitModel<T>(BaseUnit<T> unit, T model) where T : IBaseUnitModel
    {
        if (unit != null)
        {
            unit.SetModel(model);
            unit.Refresh();
            unit.Show();
        }
        else
        {
            Logger.Null("unit");
        }
    }

    public GameObject GetUnit(int index)
    {
        if (index < 0 || index >= unitList.Count)
            return null;

        return unitList[index];
    }

    public int GetUnitListCount()
    {
        return unitList.Count;
    }

    public void ActiveOffNotUse()
    {
        for (int i = useCount; i < GetUnitListCount(); ++i)
            unitList[i].SetActive(false);
    }

    public void ResizeContents(Vector2 newSize)
    {
        RectTransform rectTransform = contentsTransform.GetComponent<RectTransform>();

        if (rectTransform != null)
            rectTransform.sizeDelta = newSize;
    }
}
