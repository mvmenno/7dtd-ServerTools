﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ServerTools
{
    class Bounties
    {
        public static bool IsEnabled = false;
        public static int Minimum_Bounty = 5, Kill_Streak = 0, Bonus = 25;
        public static string Command83 = "bounty";
        public static Dictionary<int, int> KillStreak = new Dictionary<int, int>();
        private static string file = string.Format("Bounty_{0}.txt", DateTime.Today.ToString("M-d-yyyy"));
        private static string filepath = string.Format("{0}/Logs/BountyLogs/{1}", API.ConfigPath, file);

        public static void BountyList(ClientInfo _cInfo)
        {
            List<ClientInfo> ClientInfoList = ConnectionManager.Instance.Clients.List.ToList();
            for (int i = 0; i < ClientInfoList.Count; i++)
            {
                ClientInfo _cInfo2 = ClientInfoList[i];
                if (_cInfo2 != null)
                {
                    int _currentbounty = PersistentContainer.Instance.Players[_cInfo2.playerId].Bounty;
                    if (_currentbounty > 0)
                    {
                        Phrases.Dict.TryGetValue(538, out string _phrase538);
                        _phrase538 = _phrase538.Replace("{PlayerName}", _cInfo2.playerName);
                        _phrase538 = _phrase538.Replace("{EntityId}", _cInfo2.entityId.ToString());
                        _phrase538 = _phrase538.Replace("{CurrentBounty}", _currentbounty.ToString());
                        _phrase538 = _phrase538.Replace("{Minimum}", Minimum_Bounty.ToString());
                        _phrase538 = _phrase538.Replace("{CoinName}", Wallet.Coin_Name);
                        ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase538 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                    }
                }
            }
            Phrases.Dict.TryGetValue(537, out string _phrase537);
            _phrase537 = _phrase537.Replace("{CommandPrivate}", ChatHook.Command_Private);
            _phrase537 = _phrase537.Replace("{Command83}", Command83);
            ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase537 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
        }

        public static void NewBounty(ClientInfo _cInfo, string _message)
        {
            if (_message.Contains(" "))
            {
                string[] _idAndBounty = _message.Split(' ').ToArray();
                if (int.TryParse(_idAndBounty[0], out int _id))
                {
                    if (int.TryParse(_idAndBounty[1], out int _bounty))
                    {
                        ClientInfo _cInfo2 = ConnectionManager.Instance.Clients.ForEntityId(_id);
                        if (_cInfo2 != null)
                        {
                            if (_bounty < Minimum_Bounty)
                            {
                                _bounty = Minimum_Bounty;
                            }
                            int _currentCoins = Wallet.GetCurrentCoins(_cInfo.playerId);
                            if (_currentCoins >= _bounty)
                            {
                                Wallet.SubtractCoinsFromWallet(_cInfo.playerId, _bounty);
                                int _currentbounty = PersistentContainer.Instance.Players[_cInfo2.playerId].Bounty;
                                PersistentContainer.Instance.Players[_cInfo2.playerId].Bounty = _currentbounty + _bounty;
                                PersistentContainer.Instance.Save();
                                using (StreamWriter sw = new StreamWriter(filepath, true))
                                {
                                    sw.WriteLine(string.Format("{0}: {1} {2} added {3} bounty to {4} {5}.", DateTime.Now, _cInfo.playerId, _cInfo.playerName, _bounty, _cInfo2.playerId, _cInfo2.playerName));
                                    sw.WriteLine();
                                    sw.Flush();
                                    sw.Close();
                                }
                                Phrases.Dict.TryGetValue(535, out string _phrase535);
                                _phrase535 = _phrase535.Replace("{Value}", Minimum_Bounty.ToString());
                                _phrase535 = _phrase535.Replace("{PlayerName}", _cInfo2.playerName);
                                ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase535 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                            }
                            else
                            {
                                Phrases.Dict.TryGetValue(534, out string _phrase534);
                                _phrase534 = _phrase534.Replace("{Value}", _bounty.ToString());
                                ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase534 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                            }
                        }
                    }
                    else
                    {
                        Phrases.Dict.TryGetValue(536, out string _phrase536);
                        ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase536 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                    }
                }
            }
            else
            {
                if (int.TryParse(_message, out int _id))
                {
                    ClientInfo _cInfo2 = ConnectionManager.Instance.Clients.ForEntityId(_id);
                    if (_cInfo2 != null)
                    {
                        int _currentCoins = Wallet.GetCurrentCoins(_cInfo.playerId);
                        if (_currentCoins >= Minimum_Bounty)
                        {
                            Wallet.SubtractCoinsFromWallet(_cInfo.playerId, Minimum_Bounty);
                            int _currentbounty = PersistentContainer.Instance.Players[_cInfo2.playerId].Bounty;
                            PersistentContainer.Instance.Players[_cInfo2.playerId].Bounty = _currentbounty + Minimum_Bounty;
                            PersistentContainer.Instance.Save();
                            using (StreamWriter sw = new StreamWriter(filepath, true))
                            {
                                sw.WriteLine(string.Format("{0}: {1} {2} added {3} bounty to {4} {5}.", DateTime.Now, _cInfo.playerId, _cInfo.playerName, Minimum_Bounty, _cInfo2.playerId, _cInfo2.playerName));
                                sw.WriteLine();
                                sw.Flush();
                                sw.Close();
                            }
                            Phrases.Dict.TryGetValue(535, out string _phrase535);
                            _phrase535 = _phrase535.Replace("{Value}", Minimum_Bounty.ToString());
                            _phrase535 = _phrase535.Replace("{PlayerName}", _cInfo2.playerName);
                            ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase535 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                        }
                        else
                        {
                            Phrases.Dict.TryGetValue(534, out string _phrase534);
                            _phrase534 = _phrase534.Replace("{Value}", Minimum_Bounty.ToString());
                            ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase534 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                        }
                    }
                }
            }
        }

        public static void PlayerKilled(EntityPlayer _player1, EntityPlayer _player2, ClientInfo _cInfo1, ClientInfo _cInfo2)
        {
            if (!_player1.IsFriendsWith(_player2) && !_player2.IsFriendsWith(_player1) && !_player1.Party.ContainsMember(_player2) && !_player2.Party.ContainsMember(_player1))
            {
                if (ClanManager.IsEnabled)
                {
                    if (ClanManager.ClanMember.Contains(_cInfo1.playerId) && ClanManager.ClanMember.Contains(_cInfo2.playerId))
                    {
                        string _clanName1 = PersistentContainer.Instance.Players[_cInfo1.playerId].ClanName;
                        string _clanName2 = PersistentContainer.Instance.Players[_cInfo2.playerId].ClanName;
                        if (string.IsNullOrEmpty(_clanName1) && string.IsNullOrEmpty(_clanName2))
                        {
                            if (_clanName1 == _clanName2)
                            {
                                return;
                            }
                        }
                    }
                }
                int _victimBounty = PersistentContainer.Instance.Players[_cInfo1.playerId].Bounty;
                int _victimBountyHunter = PersistentContainer.Instance.Players[_cInfo1.playerId].BountyHunter;
                if (_victimBounty > 0)
                {
                    int _killerWallet = PersistentContainer.Instance.Players[_cInfo2.playerId].PlayerWallet;
                    int _killerBounty = PersistentContainer.Instance.Players[_cInfo2.playerId].Bounty;
                    int _killerBountyHunter = PersistentContainer.Instance.Players[_cInfo2.playerId].BountyHunter;
                    if (Kill_Streak > 0)
                    {
                        int _victimBountyPlus = _victimBounty + Bonus;
                        if (_killerBountyHunter + 1 >= Kill_Streak)
                        {
                            PersistentContainer.Instance.Players[_cInfo2.playerId].PlayerWallet = _killerWallet + _victimBountyPlus;
                            PersistentContainer.Instance.Players[_cInfo2.playerId].BountyHunter = _killerBountyHunter + 1;
                            PersistentContainer.Instance.Players[_cInfo2.playerId].Bounty = _killerBounty + Bonus;
                            using (StreamWriter sw = new StreamWriter(filepath, true))
                            {
                                sw.WriteLine(string.Format("{0}: {1} {2} has collected {3} bounties without dying. Their bounty has increased.", DateTime.Now, _cInfo2.playerId, _cInfo2.playerName, _killerBountyHunter + 1));
                                sw.WriteLine();
                                sw.Flush();
                                sw.Close();
                            }
                            Phrases.Dict.TryGetValue(531, out string _phrase531);
                            _phrase531 = _phrase531.Replace("{PlayerName}", _cInfo2.playerName);
                            _phrase531 = _phrase531.Replace("{Value}", _killerBountyHunter + 1.ToString());
                            ChatHook.ChatMessage(null, LoadConfig.Chat_Response_Color + _phrase531 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Global, null);
                        }
                        else if (_killerBountyHunter + 1 < Kill_Streak)
                        {
                            PersistentContainer.Instance.Players[_cInfo2.playerId].PlayerWallet = _killerWallet + _victimBounty;
                            PersistentContainer.Instance.Players[_cInfo2.playerId].BountyHunter = _killerBountyHunter + 1;
                        }
                        if (_victimBountyHunter >= Kill_Streak)
                        {
                            using (StreamWriter sw = new StreamWriter(filepath, true))
                            {
                                sw.WriteLine(string.Format("{0}: {1} {2} has died. Their kill streak came to an end by {3}.", DateTime.Now, _cInfo1.playerId, _cInfo1.playerName, _cInfo2.playerName));
                                sw.WriteLine();
                                sw.Flush();
                                sw.Close();
                            }
                            Phrases.Dict.TryGetValue(532, out string _phrase532);
                            _phrase532 = _phrase532.Replace("{Victim}", _cInfo1.playerName);
                            _phrase532 = _phrase532.Replace("{Killer}", _cInfo2.playerName);
                            ChatHook.ChatMessage(null, LoadConfig.Chat_Response_Color + _phrase532 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Global, null);
                        }
                    }
                    else
                    {
                        PersistentContainer.Instance.Players[_cInfo2.playerId].PlayerWallet = _killerWallet + _victimBounty;
                    }
                    PersistentContainer.Instance.Players[_cInfo1.playerId].Bounty = 0;
                    PersistentContainer.Instance.Players[_cInfo1.playerId].BountyHunter = 0;
                    PersistentContainer.Instance.Save();
                    Phrases.Dict.TryGetValue(533, out string _phrase533);
                    _phrase533 = _phrase533.Replace("{Victim}", _cInfo1.playerName);
                    _phrase533 = _phrase533.Replace("{Killer}", _cInfo2.playerName);
                    ChatHook.ChatMessage(null, LoadConfig.Chat_Response_Color + _phrase533 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Global, null);
                }
            }
        }

        public static void ConsoleEdit(string _id, int _value)
        {
            int _oldBounty = PersistentContainer.Instance.Players[_id].Bounty;
            if (_value > 0)
            {
                PersistentContainer.Instance.Players[_id].Bounty = _oldBounty + _value;
                PersistentContainer.Instance.Save();
                SdtdConsole.Instance.Output(string.Format("Bounty edit was successful for {0}. The new value is set to {1}", _id, _oldBounty + _value));
            }
            else
            {
                if (_oldBounty - _value < 0)
                {
                    PersistentContainer.Instance.Players[_id].Bounty = 0;
                    SdtdConsole.Instance.Output(string.Format("Bounty edit was successful for {0}. The new value is {1}", _id, 0));
                }
                else
                {
                    PersistentContainer.Instance.Players[_id].Bounty = _oldBounty - _value;
                    SdtdConsole.Instance.Output(string.Format("Bounty edit was successful for {0}. The new value is {1}", _id, _oldBounty - _value));
                }
                PersistentContainer.Instance.Save();
            }
        }

        public static void ConsoleRemoveBounty(string _id)
        {
            PersistentPlayer p = PersistentContainer.Instance.Players[_id];
            if (p != null)
            {
                p.Bounty = 0;
                PersistentContainer.Instance.Save();
                SdtdConsole.Instance.Output(string.Format("Bounty was removed successfully for {0}", _id));
            }
        }

        public static void ConsoleBountyList()
        {
            List<ClientInfo> ClientInfoList = PersistentOperations.ClientList();
            for (int i = 0; i < ClientInfoList.Count; i++)
            {
                ClientInfo _cInfo = ClientInfoList[i];
                if (_cInfo != null)
                {
                    int _currentbounty = PersistentContainer.Instance.Players[_cInfo.playerId].Bounty;
                    if (_currentbounty > 0)
                    {
                        SdtdConsole.Instance.Output(string.Format("{0}, # {1}. Current bounty: {2}.", _cInfo.playerName, _cInfo.entityId, _currentbounty));
                    }
                }
            }
        }
    }
}
