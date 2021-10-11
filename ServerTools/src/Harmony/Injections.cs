﻿using ServerTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

public static class Injections
{

    public static bool EntityAlive_DamageEntity_Prefix(EntityAlive __instance, DamageSource _damageSource, int _strength)
    {
        try
        {
            if (__instance.entityId != _damageSource.getEntityId())
            {
                return ProcessDamage.Exec(__instance, _damageSource, _strength);
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.EntityAlive_DamageEntity_Prefix: {0}", e.Message));
        }
        return true;
    }

    public static void PlayerLoginRPC_Prefix(string _playerId, out bool __state)
    {
        __state = false;
        try
        {
            if (_playerId != null && _playerId.Length == 17)
            {
                int __maxPlayers = GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount);
                if (ReservedSlots.IsEnabled && ConnectionManager.Instance.ClientCount() > __maxPlayers)
                {
                    if (ReservedSlots.FullServer(_playerId))
                    {
                        GamePrefs.Set(EnumGamePrefs.ServerMaxPlayerCount, __maxPlayers + 1);
                        __state = true;
                        return;
                    }
                }
                if (NewPlayer.IsEnabled && NewPlayer.Block_During_Bloodmoon && PersistentOperations.IsBloodmoon())
                {
                    PersistentPlayerData _ppd = PersistentOperations.GetPersistentPlayerDataFromSteamId(_playerId);
                    if (_ppd == null)
                    {
                        ClientInfo _cInfo = ConnectionManager.Instance.Clients.ForPlayerId(_playerId);
                        if (_cInfo != null)
                        {
                            _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerDenied>().Setup(new GameUtils.KickPlayerData(GameUtils.EKickReason.ManualKick, 0, default, "[ServerTools] - New players are kicked during the bloodmoon. Please return after the bloodmoon is over")));
                            return;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.PlayerLoginRPC_Prefix: {0}", e.Message));
        }
    }

    public static void PlayerLoginRPC_Postfix(bool __state)
    {
        try
        {
            if (__state)
            {
                int _maxPlayers = GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount);
                GamePrefs.Set(EnumGamePrefs.ServerMaxPlayerCount, _maxPlayers - 1);
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.PlayerLoginRPC_Postfix: {0}", e.Message));
        }
    }

    public static bool ChangeBlocks_Prefix(GameManager __instance, string persistentPlayerId, List<BlockChangeInfo> _blocksToChange)
    {
        try
        {
            return BlockChange.ProcessBlockChange(__instance, persistentPlayerId, _blocksToChange);
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.ChangeBlocks_Prefix: {0}", e.Message));
        }
        return true;
    }

    public static void ServerConsoleCommand_Postfix(ClientInfo _cInfo, string _cmd)
    {
        try
        {
            ConsoleCommandLog.Exec(_cInfo, _cmd);
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.ServerConsoleCommand_Postfix: {0}", e.Message));
        }
    }

    public static void AddFallingBlock_Postfix(World __instance, Vector3i _block)
    {
        try
        {
            if (FallingBlocks.IsEnabled)
            {
                FallingBlocks.Single(__instance, _block);
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.AddFallingBlock_Postfix: {0}", e.Message));
        }
    }

    public static void AddFallingBlocks_Postfix(World __instance, IList<Vector3i> _list)
    {
        try
        {
            if (FallingBlocks.IsEnabled)
            {
                FallingBlocks.Multiple(__instance, _list);
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.AddFallingBlocks_Postfix: {0}", e.Message));
        }
    }

    public static void ChatMessageServer_Postfix(ClientInfo _cInfo, EChatType _chatType, string _msg, string _mainName)
    {
        try
        {
            if (DiscordBot.IsEnabled && DiscordBot.Webhook != "" && DiscordBot.Webhook.StartsWith("https://discord.com/api/webhooks") &&
                _chatType == EChatType.Global && !string.IsNullOrWhiteSpace(_mainName) && !_mainName.Contains(DiscordBot.Prefix))
            {
                if (_msg.Contains("[") && _msg.Contains("]"))
                {
                    _msg = Regex.Replace(_msg, @"\[.*?\]", "");
                }
                if (!PersistentOperations.InvalidPrefix.Contains(_msg[0]))
                {
                    if (_mainName.Contains("[") && _mainName.Contains("]"))
                    {
                        _mainName = Regex.Replace(_mainName, @"\[.*?\]", "");
                    }
                    if (_cInfo != null)
                    {
                        if (DiscordBot.LastEntry != _msg)
                        {
                            DiscordBot.LastPlayer = _cInfo.playerId;
                            DiscordBot.LastEntry = _msg;
                            DiscordBot.Queue.Add("[Game] **" + _mainName + "** : " + DiscordBot.LastEntry);
                        }
                        else if (DiscordBot.LastPlayer != _cInfo.playerId)
                        {
                            DiscordBot.LastPlayer = _cInfo.playerId;
                            DiscordBot.LastEntry = _msg;
                            DiscordBot.Queue.Add("[Game] **" + _mainName + "** : " + DiscordBot.LastEntry);
                        }
                    }
                    else if (DiscordBot.LastEntry != _msg)
                    {
                        DiscordBot.LastPlayer = "-1";
                        DiscordBot.LastEntry = _msg;
                        DiscordBot.Queue.Add("[Game] **" + _mainName + "** : " + DiscordBot.LastEntry);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.ChatMessageServer_Postfix: {0}", e.Message));
        }
    }

    public static void GameManager_OnApplicationQuit_Prefix()
    {
        try
        {
            Dictionary<int, EntityPlayer> _entityPlayers = PersistentOperations.GetEntityPlayers();
            if (_entityPlayers != null && _entityPlayers.Count > 0)
            {
                foreach (var _entityPlayer in _entityPlayers)
                {
                    if (_entityPlayer.Value.AttachedToEntity != null && _entityPlayer.Value.IsSpawned())
                    {
                        _entityPlayer.Value.Detach();
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.GameManager_OnApplicationQuit_Prefix: {0}", e.Message));
        }
    }

    public static void GameManager_Cleanup_Finalizer()
    {
        try
        {
            Log.Out("[SERVERTOOLS] SHUTDOWN");
            Process process = Process.GetCurrentProcess();
            if (process != null)
            {
                process.Kill();
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.GameManager_Cleanup_Finalizer: {0}", e.Message));
        }
    }

    public static Exception ObjectiveTreasureChest_CalculateTreasurePoint_Finalizer(Exception __exception, ref Vector3i __result)
    {
        try
        {
            if (__exception != null)
            {
                __result = new Vector3i(0, -99999, 0);
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.ObjectiveTreasureChest_CalculateTreasurePoint_finalizer: {0}", e.Message));
        }
        return null;
    }

    public static bool EntityAlive_ProcessDamageResponse_Prefix(EntityAlive __instance, DamageResponse _dmResponse)
    {
        try
        {
            if (_dmResponse.Source != null && __instance.entityId != _dmResponse.Source.getEntityId())
            {
                if (NewPlayerProtection.IsEnabled && __instance is EntityPlayer)
                {
                    NewPlayerProtection.AddHealing(__instance, _dmResponse);
                }
                return ProcessDamage.Exec(__instance, _dmResponse.Source, _dmResponse.Strength);
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.EntityAlive_ProcessDamageResponse_Prefix: {0}", e.Message));
        }
        return true;
    }

    public static bool GameManager_CollectEntityServer_Prefix(int _entityId)
    {
        try
        {
            if (PersistentOperations.No_Vehicle_Pickup)
            {
                Entity _entity = PersistentOperations.GetEntity(_entityId);
                if (_entity != null && _entity is EntityVehicle)
                {
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.GameManager_CollectEntityServer_Prefix: {0}", e.Message));
        }
        return true;
    }

    public static bool GameManager_OpenTileEntityAllowed_Prefix(ref bool __result, int _entityIdThatOpenedIt, TileEntity _te)
    {
        try
        {
            if (DroppedBagProtection.IsEnabled)
            {
                if (_te is TileEntityLootContainer)
                {
                    TileEntityLootContainer lootContainer = _te as TileEntityLootContainer;
                    if (lootContainer.bPlayerBackpack)
                    {
                        if (!DroppedBagProtection.IsAllowed(_entityIdThatOpenedIt, lootContainer))
                        {
                            ClientInfo cInfo = PersistentOperations.GetClientInfoFromEntityId(_entityIdThatOpenedIt);
                            if (cInfo != null)
                            {
                                Phrases.Dict.TryGetValue("DroppedBagProtection1", out string _phrase);
                                ChatHook.ChatMessage(cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                            }
                            __result = false;
                            return false;
                        }
                    }
                }
            }
            if (Shutdown.UI_Locked)
            {
                if (_te is TileEntityLootContainer)
                {
                    TileEntityLootContainer lootContainer = _te as TileEntityLootContainer;
                    if (lootContainer.bPlayerBackpack)
                    {
                        return true;
                    }
                }
                if (_te is TileEntityWorkstation || _te is TileEntityLootContainer || _te is TileEntitySecureLootContainer
                || _te is TileEntityVendingMachine || _te is TileEntityTrader)
                {
                    ClientInfo cInfo = PersistentOperations.GetClientInfoFromEntityId(_entityIdThatOpenedIt);
                    if (cInfo != null)
                    {
                        Phrases.Dict.TryGetValue("Shutdown3", out string _phrase);
                        ChatHook.ChatMessage(cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                    }
                    __result = false;
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.GameManager_OpenTileEntityAllowed_Prefix: {0}", e.Message));
        }
        return true;
    }

    public static bool EntityAlive_OnEntityDeath_Prefix(EntityAlive __instance)
    {
        try
        {
            if (ReservedSlots.Bonus_Exp && __instance is EntityZombie)
            {
                EntityAlive entityAlive = __instance.GetAttackTarget();
                if (entityAlive != null && entityAlive is EntityPlayer)
                {
                    ClientInfo cInfo = PersistentOperations.GetClientInfoFromEntityId(entityAlive.entityId);
                    if (cInfo != null && ReservedSlots.IsEnabled && ReservedSlots.Dict.ContainsKey(cInfo.playerId))
                    {

                        int experience = EntityClass.list[__instance.entityClass].ExperienceValue;
                        experience = (int)experience / 4;
                        NetPackageEntityAddExpClient package = NetPackageManager.GetPackage<NetPackageEntityAddExpClient>().Setup(entityAlive.entityId, experience, Progression.XPTypes.Kill);
                        ConnectionManager.Instance.SendPackage(package, false, entityAlive.entityId, -1, -1, -1);
                        Log.Out(string.Format("[SERVERTOOLS] Added bonus experience of {0} to reserved player {1} named {2}", experience, cInfo.playerId, cInfo.playerName));
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.EntityAlive_OnEntityDeath_Prefix: {0}", e.Message));
        }
        return true;
    }

    public static void ChunkCluster_AddChunkSync_Postfix(bool __result, Chunk _chunk)
    {
        try
        {
            if (__result && ProtectedSpaces.IsEnabled)
            {
                if (!ProtectedSpaces.ProcessedChunks.Contains(_chunk))
                {
                    ProtectedSpaces.ProcessedChunks.Add(_chunk);
                    ProtectedSpaces.Process();
                }
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.ChunkCluster_AddChunkSync_Postfix: {0}", e.Message));
        }
    }

    public static void World_SpawnEntityInWorld_Postfix(Entity _entity)
    {
        try
        {
            if (DroppedBagProtection.IsEnabled && _entity != null && _entity is EntityBackpack)
            {
                EntityBackpack backpack = _entity as EntityBackpack;
                List<ClientInfo> clients = PersistentOperations.ClientList();
                if (clients != null && clients.Count > 0)
                {
                    for (int i = 0; i < clients.Count; i++)
                    {
                        if (clients[i].latestPlayerData.droppedBackpackPosition == new Vector3i(backpack.position))
                        {
                            if (!DroppedBagProtection.Backpacks.ContainsKey(backpack.entityId))
                            {
                                DroppedBagProtection.Backpacks.Add(backpack.entityId, clients[i].entityId);
                                PersistentContainer.Instance.Backpacks.Add(backpack.entityId, clients[i].entityId);
                            }
                            else
                            {
                                DroppedBagProtection.Backpacks[backpack.entityId] = clients[i].entityId;
                                PersistentContainer.Instance.Backpacks[backpack.entityId] = clients[i].entityId;
                            }
                            PersistentContainer.DataChange = true;
                            return;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[SERVERTOOLS] Error in Injections.World_SpawnEntityInWorld_Postfix: {0}", e.Message));
        }
    }
}