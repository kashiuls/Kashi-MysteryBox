using Rocket.API;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class KashiGizemliKutuPlugin : RocketPlugin<KashiGizemliKutuConfiguration>
{
    private Dictionary<CSteamID, DateTime> lastDailyReward;

    protected override void Load()
    {
        Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerUpdateExperience += OnPlayerUpdateExperience;
        lastDailyReward = new Dictionary<CSteamID, DateTime>();
        Rocket.Core.Logging.Logger.Log("Kashi-GizemliKutuPlugin yüklendi!");
    }

    protected override void Unload()
    {
        Rocket.Unturned.Events.UnturnedPlayerEvents.OnPlayerUpdateExperience -= OnPlayerUpdateExperience;
        Rocket.Core.Logging.Logger.Log("Kashi-GizemliKutuPlugin devre dışı!");
    }

    private void OnPlayerUpdateExperience(UnturnedPlayer player, uint experience)
    {
        // Deneyim puanı güncellenince yapılacak işlemler
    }

    [RocketCommand("gizemlikutu", "Gizemli kutu listesi", "/gizemlikutu", AllowedCaller.Player)]
    [RocketCommandPermission("kashigizemlikutu.gizemlikutu")]
    public void ShowMysteryBoxList(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer player = (UnturnedPlayer)caller;
        foreach (var box in Configuration.Instance.Kutular)
        {
            UnturnedChat.Say(player, $"Kutu: {box.Isim}, Fiyat: {box.Fiyat} deneyim");
        }
    }

    [RocketCommand("gizemlikutual", "Gizemli kutu satın al", "/gizemlikutual <kutu adi>", AllowedCaller.Player)]
    [RocketCommandPermission("kashigizemlikutu.gizemlikutual")]
    public void BuyMysteryBox(IRocketPlayer caller, string[] command)
    {
        if (command.Length != 1)
        {
            UnturnedChat.Say(caller, "Kullanım: /gizemlikutual <kutu adi>");
            return;
        }

        UnturnedPlayer player = (UnturnedPlayer)caller;
        string boxName = command[0];
        var box = Configuration.Instance.Kutular.Find(b => b.Isim.ToLower() == boxName.ToLower());

        if (box == null)
        {
            UnturnedChat.Say(player, "Böyle bir kutu bulunamadı!");
            return;
        }

        if (player.Experience < box.Fiyat)
        {
            UnturnedChat.Say(player, "Yeterli deneyim puanınız yok!");
            return;
        }

        player.Experience -= box.Fiyat;

        ushort itemId = GetRandomItemByChance(box.Esyalar);
        ItemAsset itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, itemId);

        // Oyuncunun konumuna eşyayı düşür
        ItemManager.dropItem(new Item(itemAsset.id, true), player.Position, true, false, true);

        UnturnedChat.Say(player, $"Tebrikler! {itemAsset.itemName} kazandınız!");
    }

    [RocketCommand("gunlukodul", "Günlük ödül al", "/gunlukodul", AllowedCaller.Player)]
    [RocketCommandPermission("kashigizemlikutu.gunlukodul")]
    public void GiveDailyReward(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer player = (UnturnedPlayer)caller;
        CSteamID playerId = player.CSteamID;

        if (lastDailyReward.ContainsKey(playerId) && lastDailyReward[playerId].Date == DateTime.Now.Date)
        {
            UnturnedChat.Say(player, "Günlük ödülünüzü zaten aldınız!");
            return;
        }

        lastDailyReward[playerId] = DateTime.Now;

        ushort rewardItemId = GetRandomItemByChance(Configuration.Instance.GunlukOdul.EsyaIDler);
        byte rewardAmount = Configuration.Instance.GunlukOdul.Miktar;

        // Günlük ödülü oyuncuya ver
        ItemManager.dropItem(new Item(rewardItemId, true), player.Position, true, false, true);

        UnturnedChat.Say(player, "Günlük ödülünüz verildi!");
    }

    private ushort GetRandomItemByChance(List<MysteryBoxItem> items)
    {
        int totalChance = 0;
        foreach (var item in items)
        {
            totalChance += item.Sans;
        }

        int randomChance = UnityEngine.Random.Range(0, totalChance);
        int currentChance = 0;
        foreach (var item in items)
        {
            currentChance += item.Sans;
            if (randomChance < currentChance)
            {
                return item.EsyaID;
            }
        }

        return 0; // Default case, shouldn't happen
    }

    private ushort GetRandomItemByChance(List<DailyRewardItem> items)
    {
        int totalChance = 0;
        foreach (var item in items)
        {
            totalChance += item.Sans;
        }

        int randomChance = UnityEngine.Random.Range(0, totalChance);
        int currentChance = 0;
        foreach (var item in items)
        {
            currentChance += item.Sans;
            if (randomChance < currentChance)
            {
                return item.EsyaID;
            }
        }

        return 0; // Default case, shouldn't happen
    }
}
