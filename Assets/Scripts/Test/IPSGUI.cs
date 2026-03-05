using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class IPSGUI : MonoBehaviour
{
    [Serializable]
    public class ItemIPSView
    {
        public int itemNumber;
        public SlotUpgradeScript slotUpgrade;
        public TextMeshProUGUI outputText;
    }

    [Header("Output")]
    [SerializeField] private TextMeshProUGUI totalIPSText;
    [SerializeField] private TextMeshProUGUI breakdownText;

    [Header("Source")]
    [SerializeField] private SlotUpgradeScript[] slotUpgrades;
    [SerializeField] private bool autoFindSlots = true;
    [SerializeField] private ItemIPSView[] itemViews;

    [Header("Format")]
    [SerializeField] private string totalPrefix = "IPS : ";
    [SerializeField] private string itemPrefix = "Item ";
    [SerializeField] private string itemSuffix = " IPS";
    [SerializeField] private string noDataText = "0";
    [SerializeField] private float refreshInterval = 0.2f;
    [SerializeField] private float clicksPerSecondEstimate = 5f;

    private float timer;

    private void Awake()
    {
        ResolveSlots();
        RefreshIPS();
    }

    private void OnEnable()
    {
        RefreshIPS();
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer < refreshInterval)
        {
            return;
        }

        timer = 0f;
        RefreshIPS();
    }

    private void ResolveSlots()
    {
        if (!autoFindSlots && slotUpgrades != null && slotUpgrades.Length > 0)
        {
            return;
        }

        slotUpgrades = FindObjectsByType<SlotUpgradeScript>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    private void RefreshIPS()
    {
        if ((slotUpgrades == null || slotUpgrades.Length == 0) && autoFindSlots)
        {
            ResolveSlots();
        }

        float totalIPS = 0f;
        StringBuilder builder = breakdownText != null ? new StringBuilder() : null;

        if (slotUpgrades != null)
        {
            for (int i = 0; i < slotUpgrades.Length; i++)
            {
                SlotUpgradeScript slot = slotUpgrades[i];
                float slotIPS = CalculateSlotIPS(slot);
                totalIPS += slotIPS;

                if (builder != null && slotIPS > 0f)
                {
                    builder.Append(GetSlotLabel(slot, i));
                    builder.Append(" : ");
                    builder.Append(FormatNumber(slotIPS));
                    builder.AppendLine();
                }
            }
        }

        if (totalIPSText != null)
        {
            totalIPSText.text = totalPrefix + FormatNumber(totalIPS);
        }

        if (breakdownText != null)
        {
            breakdownText.text = builder != null && builder.Length > 0
                ? builder.ToString().TrimEnd()
                : noDataText;
        }

        UpdateItemViews();
    }

    private void UpdateItemViews()
    {
        if (itemViews == null)
        {
            return;
        }

        for (int i = 0; i < itemViews.Length; i++)
        {
            ItemIPSView itemView = itemViews[i];
            if (itemView == null || itemView.outputText == null)
            {
                continue;
            }

            float itemIPS = CalculateSlotIPS(itemView.slotUpgrade);
            itemView.outputText.text = itemPrefix + itemView.itemNumber + " : " + FormatNumber(itemIPS) + itemSuffix;
        }
    }

    private float CalculateSlotIPS(SlotUpgradeScript slot)
    {
        if (slot == null || slot.level <= 0)
        {
            return 0f;
        }

        if (TryGetEventIPS(slot, out float eventIPS))
        {
            return eventIPS;
        }

        string slotLabel = GetPerSecondLabel(slot);
        if (!string.IsNullOrWhiteSpace(slotLabel))
        {
            float valuePerLevel = ExtractFirstNumber(slotLabel);
            return Mathf.Max(0f, valuePerLevel * slot.level);
        }

        string clickLabel = GetClickLabel(slot);
        if (string.IsNullOrWhiteSpace(clickLabel))
        {
            return 0f;
        }

        float valuePerLevelFromClick = ExtractFirstNumber(clickLabel);
        return Mathf.Max(0f, valuePerLevelFromClick * slot.level * clicksPerSecondEstimate);
    }

    private string GetPerSecondLabel(SlotUpgradeScript slot)
    {
        if (slot == null)
        {
            return string.Empty;
        }

        TextMeshProUGUI[] texts = slot.GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            string text = texts[i].text;
            if (ContainsIgnoreCase(text, "per sec"))
            {
                return text;
            }
        }

        return string.Empty;
    }

    private string GetSlotLabel(SlotUpgradeScript slot, int fallbackIndex)
    {
        if (TryGetEventLabel(slot, out string eventLabel))
        {
            return eventLabel;
        }

        string slotLabel = GetPerSecondLabel(slot);
        if (!string.IsNullOrWhiteSpace(slotLabel))
        {
            return slotLabel;
        }

        string clickLabel = GetClickLabel(slot);
        if (!string.IsNullOrWhiteSpace(clickLabel))
        {
            return clickLabel;
        }

        return itemPrefix + (fallbackIndex + 1);
    }

    private string GetClickLabel(SlotUpgradeScript slot)
    {
        if (slot == null)
        {
            return string.Empty;
        }

        TextMeshProUGUI[] texts = slot.GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            string text = texts[i].text;
            if (ContainsIgnoreCase(text, "per click"))
            {
                return text;
            }
        }

        return string.Empty;
    }

    private static float ExtractFirstNumber(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return 0f;
        }

        StringBuilder numberBuilder = new StringBuilder();
        bool foundDigit = false;

        for (int i = 0; i < text.Length; i++)
        {
            char current = text[i];

            if (char.IsDigit(current))
            {
                numberBuilder.Append(current);
                foundDigit = true;
                continue;
            }

            if ((current == '.' || current == ',') && foundDigit)
            {
                numberBuilder.Append('.');
                continue;
            }

            if (foundDigit)
            {
                break;
            }
        }

        if (numberBuilder.Length == 0)
        {
            return 0f;
        }

        return float.TryParse(numberBuilder.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out float value)
            ? value
            : 0f;
    }

    private static string FormatNumber(float value)
    {
        return Mathf.Approximately(value % 1f, 0f)
            ? value.ToString("0", CultureInfo.InvariantCulture)
            : value.ToString("0.##", CultureInfo.InvariantCulture);
    }

    private static bool ContainsIgnoreCase(string source, string value)
    {
        return !string.IsNullOrEmpty(source)
            && !string.IsNullOrEmpty(value)
            && source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private bool TryGetEventIPS(SlotUpgradeScript slot, out float value)
    {
        value = 0f;

        if (slot == null || slot.OnUpgrade == null)
        {
            return false;
        }

        int eventCount = slot.OnUpgrade.GetPersistentEventCount();
        bool foundValue = false;

        for (int i = 0; i < eventCount; i++)
        {
            string methodName = slot.OnUpgrade.GetPersistentMethodName(i);
            if (string.IsNullOrWhiteSpace(methodName))
            {
                continue;
            }

            if (TryGetPersistentArgument(slot.OnUpgrade, i, out int intArgument, out float floatArgument))
            {
                if (string.Equals(methodName, "addAutoClickAmount", StringComparison.Ordinal))
                {
                    value += intArgument * slot.level;
                    foundValue = true;
                    continue;
                }

                if (string.Equals(methodName, "addAutoClickLikePerSec", StringComparison.Ordinal))
                {
                    value += floatArgument * slot.level;
                    foundValue = true;
                    continue;
                }

                if (string.Equals(methodName, "addClickAmount", StringComparison.Ordinal))
                {
                    value += intArgument * slot.level * clicksPerSecondEstimate;
                    foundValue = true;
                }
            }
        }

        return foundValue;
    }

    private bool TryGetEventLabel(SlotUpgradeScript slot, out string label)
    {
        label = string.Empty;

        if (slot == null || slot.OnUpgrade == null)
        {
            return false;
        }

        int eventCount = slot.OnUpgrade.GetPersistentEventCount();
        for (int i = 0; i < eventCount; i++)
        {
            string methodName = slot.OnUpgrade.GetPersistentMethodName(i);
            if (string.IsNullOrWhiteSpace(methodName))
            {
                continue;
            }

            if (!TryGetPersistentArgument(slot.OnUpgrade, i, out int intArgument, out float floatArgument))
            {
                continue;
            }

            if (string.Equals(methodName, "addAutoClickAmount", StringComparison.Ordinal))
            {
                label = "Add " + FormatNumber(intArgument) + " Like Per Sec";
                return true;
            }

            if (string.Equals(methodName, "addAutoClickLikePerSec", StringComparison.Ordinal))
            {
                label = "Add " + FormatNumber(floatArgument) + " Like Per Sec";
                return true;
            }

            if (string.Equals(methodName, "addClickAmount", StringComparison.Ordinal))
            {
                label = "Add " + FormatNumber(intArgument) + " Like Per Click";
                return true;
            }
        }

        return false;
    }

    private static bool TryGetPersistentArgument(UnityEvent unityEvent, int index, out int intArgument, out float floatArgument)
    {
        intArgument = 0;
        floatArgument = 0f;

        if (unityEvent == null)
        {
            return false;
        }

        object persistentCalls = GetFieldValue(typeof(UnityEventBase), unityEvent, "m_PersistentCalls");
        if (persistentCalls == null)
        {
            return false;
        }

        object calls = GetFieldValue(persistentCalls.GetType(), persistentCalls, "m_Calls");
        if (!(calls is System.Collections.IList callList) || index < 0 || index >= callList.Count)
        {
            return false;
        }

        object call = callList[index];
        if (call == null)
        {
            return false;
        }

        object arguments = GetFieldValue(call.GetType(), call, "m_Arguments");
        if (arguments == null)
        {
            return false;
        }

        object intValue = GetFieldValue(arguments.GetType(), arguments, "m_IntArgument");
        object floatValue = GetFieldValue(arguments.GetType(), arguments, "m_FloatArgument");

        if (intValue is int parsedInt)
        {
            intArgument = parsedInt;
        }

        if (floatValue is float parsedFloat)
        {
            floatArgument = parsedFloat;
        }

        return true;
    }

    private static object GetFieldValue(Type type, object instance, string fieldName)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        return field != null ? field.GetValue(instance) : null;
    }
}
