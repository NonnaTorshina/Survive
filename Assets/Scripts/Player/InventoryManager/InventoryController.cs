using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �����, ���������� �� ������ ���������
/// </summary>
public class InventoryController : MonoBehaviour
{
    /// <summary>
    /// Список пар предметов и их префабов
    /// </summary>
    public ItemToPrefabDatabase itemToPrefabDatabase;

    /// <summary>
    /// Enum, содержащий подбираемые предметы
    /// </summary>
    public enum PickableItems
    {
        None,
        Knife,
        Pistol,
        Shotgun,
        Medkit,
        Grenade
    }

    /// <summary>
    /// Массив предметов, представляющих инвентарь игрока
    /// </summary>
    public Item[] currentInventory { get; set; }

    /// <summary>
    /// Вместимость инвентаря
    /// </summary>
    private int inventoryCapacity;

    /// <summary>
    /// Индекс выбранного предмета в инвентаре
    /// </summary>
    private int currentPickableItemIndex;

    /// <summary>
    /// Массив объектов, содержащих рамки выделения предмета
    /// </summary>
    public GameObject[] selectedItemImages;

    /// <summary>
    /// Массив, содержащий изображения подобранных предметов инвентаря
    /// </summary>
    public Image[] inventoryItemImages;

    /// <summary>
    /// Спрайт пустого слота
    /// </summary>
    public Sprite emptySlotImage;

    /// <summary>
    /// Метод, вызывающийся при старте объекта
    /// </summary>
    void Start()
    {
        inventoryCapacity = 2;
        currentPickableItemIndex = 0;
        currentInventory = new Item[inventoryCapacity];
        for (int i = 0; i < inventoryCapacity; i++)
        {
            currentInventory[i] = null;
            inventoryItemImages[i].sprite = emptySlotImage;
        }

        if (itemToPrefabDatabase == null)
        {
            Debug.LogWarning("ItemToPrefabDatabase not loaded!");
        }

        if (selectedItemImages == null)
        {
            Debug.LogWarning("SelectedItemImages not loaded!");
        }
    }

    /// <summary>
    /// Метод, вызывающийся каждый игровой кадр
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectNextItem();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectPreviousItem();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse4))
        {
            Drop();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Use();
        }
    }
    
    /// <summary>
    /// Метод, выделяющий предыдущий предмет в инвентаре
    /// </summary>
    private void SelectNextItem()
    {
        if (selectedItemImages != null)
        {
            selectedItemImages[currentPickableItemIndex].SetActive(false);
        }

        currentPickableItemIndex = (currentPickableItemIndex + 1) % inventoryCapacity;

        if (selectedItemImages != null)
        {
            selectedItemImages[currentPickableItemIndex].SetActive(true);
        }

        SoundController.Instance.PlaySound(SoundType.Select, SoundController.Instance.inventoryAudioSource);
    }

    /// <summary>
    /// Метод, выделяющий следующий предмет в инвентаре
    /// </summary>
    private void SelectPreviousItem()
    {
        if (selectedItemImages != null)
        {
            selectedItemImages[currentPickableItemIndex].SetActive(false);
        }

        currentPickableItemIndex = (currentPickableItemIndex - 1 + inventoryCapacity) % inventoryCapacity;
        
        if (selectedItemImages != null)
        {
            selectedItemImages[currentPickableItemIndex].SetActive(true);
        }

        SoundController.Instance.PlaySound(SoundType.Select, SoundController.Instance.inventoryAudioSource);
    }

    /// <summary>
    /// Метод, возвращающий текущий выбранный предмет в инвенторе
    /// </summary>
    /// <returns>Текущий выбранный предмет в инвенторе</returns>
    public Item GetCurrentPickableItem()
    {
        if (currentInventory != null)
        {
            return currentInventory[currentPickableItemIndex];
        }
        return null;
    }

    /// <summary>
    /// Функция использования предмета
    /// </summary>
    private void Use()
    {
        if (GetCurrentPickableItem() != null)
        {
            if (GetCurrentPickableItem() is DamagableItem damagableItem)
            {
                damagableItem.AttackScript.Attack();
            }
            else if (GetCurrentPickableItem() is StackableItem stackableItem)
            {
                if (stackableItem.UseScript.Use())
                {
                    RemoveElementFromInventory();
                }
            }
        }
    }

    /// <summary>
    /// Метод, выбрасывающий предмет из инвентаря
    /// </summary>
    private void Drop()
    {
        if (currentInventory != null && GetCurrentPickableItem() != null)
        {
            GameObject droppedItem = GetPrefabFromDatabase(currentInventory[currentPickableItemIndex].UniqueName);
            Instantiate(droppedItem, transform.position, Quaternion.identity);
            RemoveElementFromInventory();
            SoundController.Instance.PlaySound(SoundType.Drop, SoundController.Instance.inventoryAudioSource);
        }
    }

    public void RemoveElementFromInventory()
    {
        currentInventory[currentPickableItemIndex] = null;
        inventoryItemImages[currentPickableItemIndex].sprite = emptySlotImage;
    }
    
    /// <summary>
    /// Метод, возвращающий префаб по имени предмета
    /// </summary>
    /// <param name="name">Имя предмета</param>
    /// <returns>Префаб по имени предмета</returns>
    private GameObject GetPrefabFromDatabase(PickableItems name)
    {
        foreach (ItemToPrefabDatabase.ItemToPrefabEntry pair in itemToPrefabDatabase.ItemToPrefabPairs)
        {
            if (pair.uniqueName == name)
            {
                return pair.prefab;
            }
        }
        return null;
    }

    /// <summary>
    /// Метод, добавляющий подобранный предмет в инвентарь игрока
    /// </summary>
    /// <param name="pickable">Подобранный предмет</param>
    /// <returns>True- если предмет был добавлен в инвентар, иначе - false</returns>
    public bool AddItemToInventory(IPickable pickable)
    {
        if (currentInventory != null)
        {
            int freeItemIndex = FindFreeSlot();
            if (freeItemIndex != -1)
            {
                if (pickable is IWeapon weapon)
                {
                    currentInventory[freeItemIndex] = new DamagableItem(pickable.type, weapon.Damage, weapon.AttackSpeed, weapon.script);
                }
                else if(pickable is IUsable usable)
                {
                    currentInventory[freeItemIndex] = new StackableItem(pickable.type, 1, usable.script);
                }
                SetInventoryImage(pickable, freeItemIndex);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Метод, отображающий изображение подобранного предмета в инвенторе игрока
    /// </summary>
    /// <param name="pickable">Подобранный предмет</param>
    /// <param name="freeItemIndex">Индекс свободного лсота в инвенторе</param>
    private void SetInventoryImage(IPickable pickable, int freeItemIndex)
    {
        if (inventoryItemImages != null && inventoryItemImages[freeItemIndex] != null)
        {
            inventoryItemImages[freeItemIndex].sprite = pickable.inventoryImage;
        }
    }

    /// <summary>
    /// Метод, находящий индекс свободного слота в инвенторе игрока
    /// </summary>
    /// <returns>Индекс свободного слота в инвентаре игрока</returns>
    private int FindFreeSlot()
    {
        for (int i = 0; i < inventoryCapacity; i++)
        {
            if (currentInventory[i] == null)
            {
                return i;
            }
        }
        return -1;
    }
}
