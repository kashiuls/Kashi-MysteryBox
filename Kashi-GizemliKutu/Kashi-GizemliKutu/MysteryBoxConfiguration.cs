using Rocket.API;
using System.Collections.Generic;

public class KashiGizemliKutuConfiguration : IRocketPluginConfiguration
{
    public List<MysteryBox> Kutular { get; set; }
    public DailyReward GunlukOdul { get; set; }

    public void LoadDefaults()
    {
        Kutular = new List<MysteryBox>
        {
            new MysteryBox
            {
                Isim = "Kutu1",
                Fiyat = 100,
                Esyalar = new List<MysteryBoxItem>
                {
                    new MysteryBoxItem { EsyaID = 101, Sans = 50 },
                    new MysteryBoxItem { EsyaID = 102, Sans = 30 },
                    new MysteryBoxItem { EsyaID = 103, Sans = 20 }
                }
            },
            new MysteryBox
            {
                Isim = "Kutu2",
                Fiyat = 200,
                Esyalar = new List<MysteryBoxItem>
                {
                    new MysteryBoxItem { EsyaID = 201, Sans = 50 },
                    new MysteryBoxItem { EsyaID = 202, Sans = 30 },
                    new MysteryBoxItem { EsyaID = 203, Sans = 20 }
                }
            }
        };

        GunlukOdul = new DailyReward
        {
            EsyaIDler = new List<DailyRewardItem>
            {
                new DailyRewardItem { EsyaID = 363, Sans = 50 },
                new DailyRewardItem { EsyaID = 364, Sans = 30 },
                new DailyRewardItem { EsyaID = 365, Sans = 20 }
            },
            Miktar = 1
        };
    }
}

public class MysteryBox
{
    public string Isim { get; set; }
    public uint Fiyat { get; set; }
    public List<MysteryBoxItem> Esyalar { get; set; }
}

public class MysteryBoxItem
{
    public ushort EsyaID { get; set; }
    public int Sans { get; set; }
}

public class DailyReward
{
    public List<DailyRewardItem> EsyaIDler { get; set; }
    public byte Miktar { get; set; }
}

public class DailyRewardItem
{
    public ushort EsyaID { get; set; }
    public int Sans { get; set; }
}
