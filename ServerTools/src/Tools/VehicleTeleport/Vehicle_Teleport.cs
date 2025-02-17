﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ServerTools
{
    class VehicleTeleport
    {
        public static bool IsEnabled = false, Bike = false, Mini_Bike = false, Motor_Bike = false, Jeep = false, Gyro = false, Inside_Claim = false;
        public static int Delay_Between_Uses = 120, Distance = 50, Command_Cost = 0;
        public static string Command_bike = "bike", Command_minibike = "minibike", Command_motorbike = "motorbike", Command_jeep = "jeep", Command_gyro = "gyro";

        public static void Exec(ClientInfo _cInfo, int _vehicle)
        {
            Entity _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
            Entity _attachedEntity = _player.AttachedToEntity;
            if (_attachedEntity == null)
            {
                if (Delay_Between_Uses < 1)
                {
                    if (Wallet.IsEnabled && Command_Cost >= 1)
                    {
                        CommandCost(_cInfo, _player, _vehicle);
                    }
                    else
                    {
                        TeleVehicle(_cInfo, _player, _vehicle);
                    }
                }
                else
                {
                    DateTime _lastVehicle = new DateTime();
                    if (_vehicle == 1)
                    {
                        _lastVehicle = PersistentContainer.Instance.Players[_cInfo.playerId].LastBike;
                    }
                    else if (_vehicle == 2)
                    {
                        _lastVehicle = PersistentContainer.Instance.Players[_cInfo.playerId].LastMiniBike;
                    }
                    else if (_vehicle == 3)
                    {
                        _lastVehicle = PersistentContainer.Instance.Players[_cInfo.playerId].LastMotorBike;
                    }
                    else if (_vehicle == 4)
                    {
                        _lastVehicle = PersistentContainer.Instance.Players[_cInfo.playerId].LastJeep;
                    }
                    else if (_vehicle == 5)
                    {
                        _lastVehicle = PersistentContainer.Instance.Players[_cInfo.playerId].LastGyro;
                    }
                    TimeSpan varTime = DateTime.Now - _lastVehicle;
                    double fractionalMinutes = varTime.TotalMinutes;
                    int _timepassed = (int)fractionalMinutes;
                    if (ReservedSlots.IsEnabled && ReservedSlots.Reduced_Delay)
                    {
                        if (ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                        {
                            ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out DateTime _dt);
                            if (DateTime.Now < _dt)
                            {
                                int _delay = Delay_Between_Uses / 2;
                                Time(_cInfo, _player, _vehicle, _timepassed, _delay);
                                return;
                            }
                        }
                    }
                    Time(_cInfo, _player, _vehicle, _timepassed, Delay_Between_Uses);
                }
            }
            else
            {
                SaveVehicle(_cInfo, _player, _vehicle, _attachedEntity.entityId);
            }
        }

        public static void Time(ClientInfo _cInfo, Entity _player, int _vehicle, int _timepassed, int _delay)
        {
            if (_timepassed >= _delay)
            {
                if (Wallet.IsEnabled && Command_Cost >= 1)
                {
                    CommandCost(_cInfo, _player, _vehicle);
                }
                else
                {
                    TeleVehicle(_cInfo, _player, _vehicle);
                }
            }
            else
            {
                int _timeleft = _delay - _timepassed;
                Phrases.Dict.TryGetValue("VehicleTeleport7", out string _phrase);
                _phrase = _phrase.Replace("{DelayBetweenUses}", _delay.ToString());
                _phrase = _phrase.Replace("{TimeRemaining}", _timeleft.ToString());
                ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
            }
        }


        public static void CommandCost(ClientInfo _cInfo, Entity _player, int _vehicle)
        {
            if (Wallet.GetCurrentCoins(_cInfo.playerId) >= Command_Cost)
            {
                TeleVehicle(_cInfo, _player, _vehicle);
            }
            else
            {
                Phrases.Dict.TryGetValue("VehicleTeleport9", out string _phrase);
                _phrase = _phrase.Replace("{CoinName}", Wallet.Coin_Name);
                ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
            }
        }

        public static void SaveVehicle(ClientInfo _cInfo, Entity _player, int _vehicle, int _vehicleId)
        {
            if (Inside_Claim)
            {
                World world = GameManager.Instance.World;
                Vector3 _position = _player.GetPosition();
                int x = (int)_position.x;
                int y = (int)_position.y;
                int z = (int)_position.z;
                Vector3i _vec3i = new Vector3i(x, y, z);
                PersistentPlayerList _persistentPlayerList = GameManager.Instance.GetPersistentPlayerList();
                PersistentPlayerData _persistentPlayerData = _persistentPlayerList.GetPlayerDataFromEntityID(_player.entityId);
                EnumLandClaimOwner _owner = world.GetLandClaimOwner(_vec3i, _persistentPlayerData);
                if (!PersistentOperations.ClaimedByAllyOrSelf(_cInfo.playerId, _vec3i))
                {
                    Phrases.Dict.TryGetValue("VehicleTeleport1", out string _phrase);
                    ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                    return;
                }
            }
            string _entityName = "", _vehicleName = "";
            if (_vehicle == 1)
            {
                _entityName = "vehicleBicycle";
                _vehicleName = "bike";
            }
            else if (_vehicle == 2)
            {
                _entityName = "vehicleMinibike";
                _vehicleName = "minibike";
            }
            else if (_vehicle == 3)
            {
                _entityName = "vehicleMotorcycle";
                _vehicleName = "motorbike";
            }
            else if (_vehicle == 4)
            {
                _entityName = "vehicle4x4Truck";
                _vehicleName = "jeep";
            }
            else if (_vehicle == 5)
            {
                _entityName = "vehicleGyrocopter";
                _vehicleName = "gyro";
            }
            if (_player.AttachedToEntity.EntityClass.entityClassName.ToString() == _entityName)
            {
                if (_vehicle == 1)
                {
                    PersistentContainer.Instance.Players[_cInfo.playerId].BikeId = _vehicleId;
                }
                else if (_vehicle == 2)
                {
                    PersistentContainer.Instance.Players[_cInfo.playerId].MiniBikeId = _vehicleId;
                }
                else if (_vehicle == 3)
                {
                    PersistentContainer.Instance.Players[_cInfo.playerId].MotorBikeId = _vehicleId;
                }
                else if (_vehicle == 4)
                {
                    PersistentContainer.Instance.Players[_cInfo.playerId].JeepId = _vehicleId;
                }
                else if (_vehicle == 5)
                {
                    PersistentContainer.Instance.Players[_cInfo.playerId].GyroId = _vehicleId;
                }
                PersistentContainer.DataChange = true;
                Phrases.Dict.TryGetValue("VehicleTeleport2", out string _phrase);
                _phrase = _phrase.Replace("{Vehicle}", _vehicleName);
                ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
            }
            else
            {
                Phrases.Dict.TryGetValue("VehicleTeleport8", out string _phrase);
                _phrase = _phrase.Replace("{Vehicle}", _vehicleName);
                ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
            }
        }

        public static void TeleVehicle(ClientInfo _cInfo, Entity _player, int _vehicle)
        {
            int _vehicleId = 0;
            if (_vehicle == 1)
            {
                _vehicleId = PersistentContainer.Instance.Players[_cInfo.playerId].BikeId;
            }
            else if (_vehicle == 2)
            {
                _vehicleId = PersistentContainer.Instance.Players[_cInfo.playerId].MiniBikeId;
            }
            else if (_vehicle == 3)
            {
                _vehicleId = PersistentContainer.Instance.Players[_cInfo.playerId].MotorBikeId;
            }
            else if (_vehicle == 4)
            {
                _vehicleId = PersistentContainer.Instance.Players[_cInfo.playerId].JeepId;
            }
            else if (_vehicle == 5)
            {
                _vehicleId = PersistentContainer.Instance.Players[_cInfo.playerId].GyroId;
            }
            if (_vehicleId != 0)
            {
                List<Entity> Entities = GameManager.Instance.World.Entities.list;
                for (int i = 0; i < Entities.Count; i++)
                {
                    Entity _entity = Entities[i];
                    if (!_entity.IsClientControlled())
                    {
                        if (_entity.entityId == _vehicleId)
                        {
                            if ((_player.position.x - _entity.position.x) * (_player.position.x - _entity.position.x) + (_player.position.z - _entity.position.z) * (_player.position.z - _entity.position.z) <= Distance * Distance)
                            {
                                if (_entity.AttachedToEntity == false)
                                {
                                    _entity.SetPosition(_player.position);
                                    if (Wallet.IsEnabled && Command_Cost >= 1)
                                    {
                                        Wallet.SubtractCoinsFromWallet(_cInfo.playerId, Command_Cost);
                                    }
                                    if (_vehicle == 1)
                                    {
                                        PersistentContainer.Instance.Players[_cInfo.playerId].LastBike = DateTime.Now;
                                    }
                                    if (_vehicle == 2)
                                    {
                                        PersistentContainer.Instance.Players[_cInfo.playerId].LastMiniBike = DateTime.Now;
                                    }
                                    if (_vehicle == 3)
                                    {
                                        PersistentContainer.Instance.Players[_cInfo.playerId].LastMotorBike = DateTime.Now;
                                    }
                                    if (_vehicle == 4)
                                    {
                                        PersistentContainer.Instance.Players[_cInfo.playerId].LastJeep = DateTime.Now;
                                    }
                                    if (_vehicle == 5)
                                    {
                                        PersistentContainer.Instance.Players[_cInfo.playerId].LastGyro = DateTime.Now;
                                    }
                                    PersistentContainer.DataChange = true;
                                    Phrases.Dict.TryGetValue("VehicleTeleport3", out string _phrase);
                                    ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                                    return;
                                }
                                else
                                {
                                    Phrases.Dict.TryGetValue("VehicleTeleport6", out string _phrase);
                                    ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                                    return;
                                }
                            }
                        }
                    }
                }
                Phrases.Dict.TryGetValue("VehicleTeleport5", out string _phrase1);
                ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase1 + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
            }
            else
            {
                Phrases.Dict.TryGetValue("VehicleTeleport4", out string _phrase);
                ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
            }
        }
    }
}
