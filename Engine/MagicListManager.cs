﻿using System;
using System.Collections.Generic;
using IniParser;

namespace Engine.Gui
{
    public static class MagicListManager
    {
        private const int MaxMagic = 49;
        private const int ListStart = 1;
        private const int ListEnd = 36;
        private static readonly MagicItemInfo[] MagicList = new MagicItemInfo[MaxMagic + 1];

        public const int XiuLianIndex = 49;
        public static void LoadList(string filePath)
        {
            RenewList();
            GuiManager.UpdateMagicView();// clear
            try
            {
                var parser = new FileIniDataParser();
                var data = parser.ReadFile(filePath, Globals.SimpleChinaeseEncoding);
                foreach (var sectionData in data.Sections)
                {
                    int head;
                    if (int.TryParse(sectionData.SectionName, out head))
                    {
                        var section = data[sectionData.SectionName];
                        MagicList[head] = new MagicItemInfo(
                            section["IniFile"],
                            int.Parse(section["Level"]),
                            int.Parse(section["Exp"])
                            );
                    }
                }
            }
            catch (Exception exception)
            {
                RenewList();
                Log.LogFileLoadError("Magic list", filePath, exception);
            }
            GuiManager.UpdateMagicView();
        }

        public static bool IndexInRange(int index)
        {
            return (index > 0 && index <= MaxMagic);
        }

        public static void RenewList()
        {
            for (var i = 1; i <= MaxMagic; i++)
            {
                MagicList[i] = null;
            }
        }

        public static void ExchangeListItem(int index1, int index2)
        {
            if (index1 != index2 &&
                IndexInRange(index1) &&
                IndexInRange(index2))
            {
                var temp = MagicList[index1];
                MagicList[index1] = MagicList[index2];
                MagicList[index2] = temp;
            }
        }

        public static Magic Get(int index)
        {
            var itemInfo = GetItemInfo(index);
            return (itemInfo != null) ?
                itemInfo.TheMagic :
                null;
        }

        public static Texture GetTexture(int index)
        {
            var magic = Get(index);
            if (magic != null)
            {
                if(index >= 40 && index <= 44)
                    return new Texture(magic.Icon);
                else
                    return new Texture(magic.Image);
            }
            return null;
        }

        public static Asf GetImage(int index)
        {
            var magic = Get(index);
            if (magic != null)
                return magic.Image;
            return null;
        }

        public static Asf GetIcon(int index)
        {
            var magic = Get(index);
            if (magic != null)
                return magic.Icon;
            return null;
        }

        public static MagicItemInfo GetItemInfo(int index)
        {
            return IndexInRange(index) ? MagicList[index] : null;
        }

        public static bool AddMagicToList(string fileName, out int index, out Magic outMagic)
        {
            index = -1;
            outMagic = null;
            for (var i = 1; i <= MaxMagic; i++)
            {
                if (MagicList[i] != null)
                {
                    var magic = MagicList[i].TheMagic;
                    if (magic != null)
                    {
                        if (magic.FileName == fileName)
                        {
                            index = i;
                            outMagic = magic;
                            return false;
                        }
                    }
                }
            }

            for (var i = ListStart; i <= ListEnd; i++)
            {
                if (MagicList[i] == null)
                {
                    MagicList[i] = new MagicItemInfo(fileName, 1, 0);
                    index = i;
                    outMagic = MagicList[i].TheMagic;
                    return true;
                }
            }
            return false;
        }

        public class MagicItemInfo
        {
            public Magic TheMagic { private set; get; }
            public int Level { private set; get; }
            public int Exp { private set; get; }

            public MagicItemInfo(string iniFile, int level, int exp)
            {
                var magic = Utils.GetMagic(iniFile, false);
                if (magic != null)
                {
                    TheMagic = magic.GetLevel(level);
                    TheMagic.ItemInfo = this;
                }
                Level = level;
                Exp = exp;
            }
        }
    }
}