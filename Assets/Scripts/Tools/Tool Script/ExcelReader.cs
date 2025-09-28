using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

using static ExcelReader;
using static EnumHelper;
using Debug = UnityEngine.Debug;
public class ExcelReader
{
    #region helper function

    class ColumnReader
    {
        private IExcelDataReader reader;
        private int index = 0;

        public ColumnReader(IExcelDataReader reader)
        {
            this.reader = reader;
        }

        public int ReadInt()
        {
            return ExcelReader.ReadInt(reader, index++);
        }

        public string ReadString()
        {
            return ExcelReader.ReadString(reader, index++);
        }

        public float ReadFloat()
        {
            return ExcelReader.ReadFloat(reader, index++);
        }

        public bool ReadBool()
        {
            return ExcelReader.ReadBool(reader, index++);
        }
    }

    static int ReadInt(IExcelDataReader reader, int index)
    {
        try
        {
            if (reader.IsDBNull(index))
                return 0;

            object value = reader.GetValue(index);

            if (value is int i)
                return i;

            if (value is double d) // Excel often stores numbers as double
                return Convert.ToInt32(d);

            if (value is string s)
            {
                if (int.TryParse(s, out int parsed))
                    return parsed;
                else
                    Debug.LogWarning($"[ReadInt] Cell content is a string but cannot be parsed as int: \"{s}\", at column index {index}");
            }

            UnityEngine.Debug.LogWarning($"[ReadInt] Cell content is not an int, but {value.GetType().Name}. Content: {value}, at column index {index}");
            return 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ReadInt] Error while reading int: {ex.Message}, at column index {index}");
            return 0;
        }
    }

    static bool ReadBool(IExcelDataReader r, int index)
    {
        return r.IsDBNull(index) ? false : Convert.ToInt32(r.GetValue(index).ToString()) == 1;
    }


    static string ReadString(IExcelDataReader r, int index)
    {
        return r.IsDBNull(index) ? string.Empty : r.GetValue(index)?.ToString();
    }

    public static List<int> ParseIntListFromCell(string cellContent)
    {
        List<int> result = new List<int>();

        if (string.IsNullOrWhiteSpace(cellContent))
            return result;

        string[] parts = cellContent.Split(',');

        foreach (string part in parts)
        {
            if (int.TryParse(part.Trim(), out int number))
            {
                result.Add(number);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Can't read?{part}");
            }
        }

        return result;
    }

    static float ReadFloat(IExcelDataReader r, int index)
    {
        if (r.IsDBNull(index)) return 0f;

        var value = r.GetValue(index);
        if (value is double d)
            return (float)d;
        if (float.TryParse(value.ToString(), out float result))
            return result;

        Debug.LogWarning($"[Can't read?{index}?value is?{value}");
        return 0f;
    }
    #endregion


    public static List<ExcelStoryData> GetStoryData(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, $"Excel/Story/{fileName}.xlsx");

        List<ExcelStoryData> excelDataList = new List<ExcelStoryData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (!File.Exists(filePath))
            return excelDataList;

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            reader.Read();
            do
            {
                while (reader.Read())
                {
                    var col = new ColumnReader(reader);
                    ExcelStoryData data = new ExcelStoryData
                    {
                        ID = col.ReadInt(),
                        Content = col.ReadString(),
                        Effect = col.ReadString()
                    };
                    excelDataList.Add(data);
                }

            } while (reader.NextResult());
        }
        return excelDataList;
    }


    public static List<ExcelCardData> GetCardData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Excel/Value/CardValue.xlsx");

        List<ExcelCardData> excelDataList = new List<ExcelCardData>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (!File.Exists(filePath))
            return excelDataList;

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            reader.Read();
            do
            {
                while (reader.Read())
                {
                    var col = new ColumnReader(reader);
                    ExcelCardData data = new ExcelCardData
                    {
                        ID = col.ReadInt(),
                        CardName = col.ReadString(),
                        cardType = ParseEnumOrDefault<CardType>(col.ReadString()),
                        rarity = (CardRarity)col.ReadInt(),
                        ability = ParseEnumOrDefault<CardAbility>(col.ReadString()),
                        CardDescribe = col.ReadString(),
                        weaponLevel = col.ReadInt(),
                        maxLevel = col.ReadInt(),
                        damage = col.ReadFloat(),

                    };

                    excelDataList.Add(data);
                }

            } while (reader.NextResult());
        }
        return excelDataList;
    }

}


#region Excel Data


public struct ExcelStoryData
{
    public int ID;
    public string Content;
    public string Effect;
}



public struct ExcelCardData
{
    public int ID;
    public string CardName;
    public CardType cardType;
    public CardRarity rarity;
    public CardAbility ability;
    public string CardDescribe;
    public int weaponLevel;
    public int maxLevel;
    public float damage;

}

#endregion