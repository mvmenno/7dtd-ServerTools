﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ServerTools
{
    public class Jail
    {
        public static bool IsEnabled = false, Jail_Shock = false;
        public static int Jail_Size = 8;
        public static string Command_set_jail = "set jail", Command_jail = "jail", Command_unjail = "unjail", Command_forgive = "forgive";
        public static string Jail_Position = "0,0,0";
        public static SortedDictionary<string, Vector3> JailReleasePosition = new SortedDictionary<string, Vector3>();
        public static List<string> Jailed = new List<string>();

        public static void SetJail(ClientInfo _cInfo)
        {
            string[] _command1 = { Command_set_jail };
            if (!GameManager.Instance.adminTools.CommandAllowedFor(_command1, _cInfo))
            {
                Phrases.Dict.TryGetValue("Jail10", out string _phrase);
                ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
            }
            else
            {
                EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
                if (_player != null)
                {
                    int _x = (int)_player.position.x;
                    int _y = (int)_player.position.y;
                    int _z = (int)_player.position.z;
                    string _sposition = _x + "," + _y + "," + _z;
                    Jail_Position = _sposition;
                    Config.WriteXml();
                    Phrases.Dict.TryGetValue("Jail3", out string _phrase);
                    _phrase = _phrase.Replace("{JailPosition}", Jail_Position);
                    ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                }
            }
        }

        public static void PutInJail(ClientInfo _cInfo, string _playerName)
        {
            string[] _command2 = { Command_jail };
            if (!GameManager.Instance.adminTools.CommandAllowedFor(_command2, _cInfo))
            {
                Phrases.Dict.TryGetValue("Jail10", out string _phrase);
                ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
            }
            else
            {
                if (Jail_Position == "0,0,0" || Jail_Position == "0 0 0" || Jail_Position == "")
                {
                    Phrases.Dict.TryGetValue("Jail4", out string _phrase);
                    ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                }
                else
                {
                    _playerName = _playerName.Replace(Command_jail + " ", "");
                    ClientInfo _PlayertoJail = ConsoleHelper.ParseParamIdOrName(_playerName);
                    if (_PlayertoJail == null)
                    {
                        Phrases.Dict.TryGetValue("Jail11", out string _phrase);
                        _phrase = _phrase.Replace("{PlayerName}", _playerName);
                        ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                    }
                    else
                    {
                        if (!Jailed.Contains(_PlayertoJail.playerId))
                        {
                            PutPlayerInJail(_cInfo, _PlayertoJail);
                        }
                        else
                        {
                            Phrases.Dict.TryGetValue("Jail5", out string _phrase);
                            _phrase = _phrase.Replace("{PlayerName}", _playerName);
                            ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                        }
                    }
                }
            }
        }

        private static void PutPlayerInJail(ClientInfo _cInfo, ClientInfo _PlayertoJail)
        {
            string[] _cords = Jail_Position.Split(',');
            int.TryParse(_cords[0], out int _x);
            int.TryParse(_cords[1], out int _y);
            int.TryParse(_cords[2], out int _z);
            _PlayertoJail.SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(new Vector3(_x, _y, _z), null, false));
            Jailed.Add(_PlayertoJail.playerId);
            PersistentContainer.Instance.Players[_PlayertoJail.playerId].JailTime = 60;
            PersistentContainer.Instance.Players[_PlayertoJail.playerId].JailName= _PlayertoJail.playerName;
            PersistentContainer.Instance.Players[_PlayertoJail.playerId].JailDate = DateTime.Now;
            PersistentContainer.DataChange = true;
            Phrases.Dict.TryGetValue("Jail1", out string _phrase);
            ChatHook.ChatMessage(_PlayertoJail, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
            if (Jail_Shock)
            {
                Phrases.Dict.TryGetValue("Jail8", out _phrase);
                ChatHook.ChatMessage(_PlayertoJail, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
            }
            Phrases.Dict.TryGetValue("Jail6", out _phrase);
            _phrase = _phrase.Replace("{PlayerName}", _PlayertoJail.playerName);
            ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
        }

        public static void RemoveFromJail(ClientInfo _cInfo, string _playerName)
        {
            string[] _command3 = { Command_unjail };
            if (!GameManager.Instance.adminTools.CommandAllowedFor(_command3, _cInfo))
            {
                Phrases.Dict.TryGetValue("Jail10", out string _phrase);
                ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
            }
            else
            {
                _playerName = _playerName.Replace("unjail ", "");
                ClientInfo _PlayertoUnJail = ConsoleHelper.ParseParamIdOrName(_playerName);
                if (_PlayertoUnJail == null)
                {
                    Phrases.Dict.TryGetValue("Jail11", out string _phrase);
                    _phrase = _phrase.Replace("{PlayerName}", _playerName);
                    ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                }
                else
                {
                    int _jailTime = PersistentContainer.Instance.Players[_PlayertoUnJail.playerId].JailTime;
                    if (_jailTime == 0)
                    {
                        Phrases.Dict.TryGetValue("Jail7", out string _phrase);
                        _phrase = _phrase.Replace("{PlayerName}", _playerName);
                        ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                    }
                    else
                    {
                        if (!Jailed.Contains(_PlayertoUnJail.playerId))
                        {
                            Phrases.Dict.TryGetValue("Jail7", out string _phrase);
                            _phrase = _phrase.Replace("{PlayerName}", _playerName);
                            ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                        }
                        else
                        {
                            Jailed.Remove(_PlayertoUnJail.playerId);
                            PersistentContainer.Instance.Players[_PlayertoUnJail.playerId].JailTime = 0;
                            PersistentContainer.DataChange = true;
                            EntityPlayer _player = GameManager.Instance.World.Players.dict[_PlayertoUnJail.entityId];
                            EntityBedrollPositionList _position = _player.SpawnPoints;
                            if (_position.Count > 0 && (PersistentOperations.ClaimedByAllyOrSelf(_PlayertoUnJail.playerId, _position.GetPos()) || PersistentOperations.ClaimedByNone(_PlayertoUnJail.playerId, _position.GetPos())))
                            {

                                _PlayertoUnJail.SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(new Vector3(_position[0].x, _position[0].y + 1, _position[0].z), null, false));
                            }
                            else
                            {
                                Vector3[] _pos = GameManager.Instance.World.GetRandomSpawnPointPositions(1);
                                if (PersistentOperations.ClaimedByAllyOrSelf(_PlayertoUnJail.playerId, new Vector3i(_pos[0].x, _pos[0].y, _pos[0].z)) || PersistentOperations.ClaimedByNone(_PlayertoUnJail.playerId, new Vector3i(_pos[0].x, _pos[0].y, _pos[0].z)))
                                {
                                    _PlayertoUnJail.SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(new Vector3(_pos[0].x, _pos[0].y + 1, _pos[0].z), null, false));
                                }
                                else
                                {
                                    _pos = GameManager.Instance.World.GetRandomSpawnPointPositions(1);
                                    if (PersistentOperations.ClaimedByAllyOrSelf(_PlayertoUnJail.playerId, new Vector3i(_pos[0].x, _pos[0].y, _pos[0].z)) || PersistentOperations.ClaimedByNone(_PlayertoUnJail.playerId, new Vector3i(_pos[0].x, _pos[0].y, _pos[0].z)))
                                    {
                                        _PlayertoUnJail.SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(new Vector3(_pos[0].x, _pos[0].y + 1, _pos[0].z), null, false));
                                    }
                                    else
                                    {
                                        _pos = GameManager.Instance.World.GetRandomSpawnPointPositions(1);
                                        if (PersistentOperations.ClaimedByAllyOrSelf(_PlayertoUnJail.playerId, new Vector3i(_pos[0].x, _pos[0].y, _pos[0].z)) || PersistentOperations.ClaimedByNone(_PlayertoUnJail.playerId, new Vector3i(_pos[0].x, _pos[0].y, _pos[0].z)))
                                        {
                                            _PlayertoUnJail.SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(new Vector3(_pos[0].x, _pos[0].y + 1, _pos[0].z), null, false));
                                        }
                                    }
                                }
                            }
                            Phrases.Dict.TryGetValue("Jail2", out string _phrase);
                            ChatHook.ChatMessage(_PlayertoUnJail, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                        }
                    }
                }
            }
        }

        public static void StatusCheck()
        {
            if (Jailed.Count > 0)
            {
                for (int i = 0; i < Jailed.Count; i++)
                {
                    ClientInfo _cInfo = ConnectionManager.Instance.Clients.ForPlayerId(Jailed[i]);
                    if (_cInfo != null)
                    {
                        EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
                        if (_player.Spawned && _player.IsAlive())
                        {
                            string[] _cords = Jail_Position.Split(',');
                            int.TryParse(_cords[0], out int _x);
                            int.TryParse(_cords[1], out int _y);
                            int.TryParse(_cords[2], out int _z);
                            Vector3 _vector3 = _player.position;
                            if ((_x - _vector3.x) * (_x - _vector3.x) + (_z - _vector3.z) * (_z - _vector3.z) >= Jail_Size * Jail_Size) 
                            {
                                _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(new Vector3(_x, _y, _z), null, false));
                                if (Jail_Shock)
                                {
                                    _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup("buff buffShocked", true));
                                    Phrases.Dict.TryGetValue("Jail9", out string _phrase);
                                    ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void Clear()
        {
            for (int i = 0; i < Jailed.Count; i++)
            {
                string _id = Jailed[i];
                if (Jailed.Contains(_id))
                {
                    int _jailTime = PersistentContainer.Instance.Players[_id].JailTime;
                    DateTime _jailDate = PersistentContainer.Instance.Players[_id].JailDate;
                    if (_jailTime > 0)
                    {
                        TimeSpan varTime = DateTime.Now - _jailDate;
                        double fractionalMinutes = varTime.TotalMinutes;
                        int _timepassed = (int)fractionalMinutes;
                        if (_timepassed >= _jailTime)
                        {
                            ClientInfo _cInfo = ConnectionManager.Instance.Clients.ForPlayerId(_id);
                            if (_cInfo != null)
                            {
                                EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
                                if (_player.IsSpawned())
                                {
                                    Jailed.Remove(_id);
                                    PersistentContainer.Instance.Players[_cInfo.playerId].JailTime = 0;
                                    PersistentContainer.DataChange = true;
                                    EntityBedrollPositionList _position = _player.SpawnPoints;
                                    if (_position.Count > 0)
                                    {
                                        _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(new Vector3(_position[0].x, _position[0].y + 1, _position[0].z), null, false));
                                    }
                                    else
                                    {
                                        Vector3[] _pos = GameManager.Instance.World.GetRandomSpawnPointPositions(1);
                                        _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(new Vector3(_pos[0].x, _pos[0].y + 1, _pos[0].z), null, false));
                                    }
                                    Phrases.Dict.TryGetValue("Jail2", out string _phrase);
                                    ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                                }
                            }
                            else
                            {
                                Jailed.Remove(_id);
                                PersistentContainer.Instance.Players[_cInfo.playerId].JailTime = 0;
                                PersistentContainer.DataChange = true;
                            }
                        }
                    }
                }
            }
        }

        public static void JailList()
        {
            for (int i = 0; i < PersistentContainer.Instance.Players.SteamIDs.Count; i++)
            {
                string _id = PersistentContainer.Instance.Players.SteamIDs[i];
                PersistentPlayer p = PersistentContainer.Instance.Players[_id];
                {
                    int _jailTime = p.JailTime;
                    if (_jailTime > 0 || _jailTime == -1)
                    {
                        if (_jailTime == -1)
                        {
                            Jailed.Add(_id);
                        }
                        else
                        {
                            DateTime _jailDate = p.JailDate;
                            TimeSpan varTime = DateTime.Now - _jailDate;
                            double fractionalMinutes = varTime.TotalMinutes;
                            int _timepassed = (int)fractionalMinutes;
                            if (_timepassed < _jailTime)
                            {
                                Jailed.Add(_id);
                            }
                            else
                            {
                                PersistentContainer.Instance.Players[_id].JailTime = 0;
                                PersistentContainer.DataChange = true;
                            }
                        }
                    }
                }
            }
        }
    }
}