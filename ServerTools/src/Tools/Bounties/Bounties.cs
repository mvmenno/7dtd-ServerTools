﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ServerTools
{
    class Bounties
    {
        public static bool IsEnabled = false;
        public static int Minimum_Bounty = 5, Kill_Streak = 0, Bonus = 25;
        public static string Command_bounty = "bounty";
        public static Dictionary<int, int> KillStreak = new Dictionary<int, int>();

        private static readonly string file = string.Format("Bounty_{0}.txt", DateTime.Today.ToString("M-d-yyyy"));
        private static readonly string filepath = string.Format("{0}/Logs/BountyLogs/{1}", API.ConfigPath, file);

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
                        Phrases.Dict.TryGetValue("bounties8", out string _phrase);
                        _phrase = _phrase.Replace("{PlayerName}", _cInfo2.playerName);
                        _phrase = _phrase.Replace("{EntityId}", _cInfo2.entityId.ToString());
                        _phrase = _phrase.Replace("{CurrentBounty}", _currentbounty.ToString());
                        _phrase = _phrase.Replace("{Minimum}", Minimum_Bounty.ToString());
                        _phrase = _phrase.Replace("{CoinName}", Wallet.Coin_Name);
                        ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                    }
                }
            }
            Phrases.Dict.TryGetValue("bounties7", out string _phrase1);
            _phrase1 = _phrase1.Replace("{Command_Prefix1}", ChatHook.Chat_Command_Prefix1);
            _phrase1 = _phrase1.Replace("{Command_bounty}", Command_bounty);
            ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase1 + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
        }

        public static void NewBounty(ClientInfo _cInfo, string _message)
        {
            try
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
                                    PersistentContainer.DataChange = true;
                                    using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                                    {
                                        sw.WriteLine(string.Format("{0}: {1} {2} added {3} bounty to {4} {5}.", DateTime.Now, _cInfo.playerId, _cInfo.playerName, _bounty, _cInfo2.playerId, _cInfo2.playerName));
                                        sw.WriteLine();
                                        sw.Flush();
                                        sw.Close();
                                    }
                                    Phrases.Dict.TryGetValue("bounties5", out string _phrase);
                                    _phrase = _phrase.Replace("{Value}", _bounty.ToString());
                                    _phrase = _phrase.Replace("{PlayerName}", _cInfo2.playerName);
                                    ChatHook.ChatMessage(null, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Global, null);
                                }
                                else
                                {
                                    Phrases.Dict.TryGetValue("bounties4", out string _phrase);
                                    _phrase = _phrase.Replace("{Value}", _bounty.ToString());
                                    ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                                }
                            }
                        }
                        else
                        {
                            Phrases.Dict.TryGetValue("bounties6", out string _phrase);
                            ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
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
                                PersistentContainer.DataChange = true;
                                using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                                {
                                    sw.WriteLine(string.Format("{0}: {1} {2} added {3} bounty to {4} {5}.", DateTime.Now, _cInfo.playerId, _cInfo.playerName, Minimum_Bounty, _cInfo2.playerId, _cInfo2.playerName));
                                    sw.WriteLine();
                                    sw.Flush();
                                    sw.Close();
                                }
                                Phrases.Dict.TryGetValue("bounties5", out string _phrase);
                                _phrase = _phrase.Replace("{Value}", Minimum_Bounty.ToString());
                                _phrase = _phrase.Replace("{PlayerName}", _cInfo2.playerName);
                                ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                            }
                            else
                            {
                                Phrases.Dict.TryGetValue("bounties4", out string _phrase);
                                _phrase = _phrase.Replace("{Value}", Minimum_Bounty.ToString());
                                ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Whisper, null);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in Bounties.NewBounty: {0}", e.Message));
            }
        }

        public static void PlayerKilled(EntityPlayer _player1, EntityPlayer _player2, ClientInfo _cInfo1, ClientInfo _cInfo2)
        {
            try
            {
                if (_cInfo1.playerId != null && _player1 != null && _player2 != null && _cInfo2.playerId != null)
                {
                    PersistentPlayerData _ppd1 = PersistentOperations.GetPersistentPlayerDataFromSteamId(_cInfo1.playerId);
                    PersistentPlayerData _ppd2 = PersistentOperations.GetPersistentPlayerDataFromSteamId(_cInfo2.playerId);
                    if (_ppd1.ACL != null && !_ppd1.ACL.Contains(_cInfo2.playerId) && _ppd2.ACL != null && !_ppd2.ACL.Contains(_cInfo1.playerId))
                    {
                        if (_player1.Party != null && !_player1.Party.ContainsMember(_player2) && _player2.Party != null && !_player2.Party.ContainsMember(_player1))
                        {
                            ProcessPlayerKilled(_cInfo1, _cInfo2);
                        }
                    }
                    else if (_player1.Party != null && !_player1.Party.ContainsMember(_player2) && _player2.Party != null && !_player2.Party.ContainsMember(_player1))
                    {
                        ProcessPlayerKilled(_cInfo1, _cInfo2);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in Bounties.PlayerKilled: {0}", e.Message));
            }
        }

        private static void ProcessPlayerKilled(ClientInfo _cInfo1, ClientInfo _cInfo2)
        {
            try
            {
                int _victimBounty = PersistentContainer.Instance.Players[_cInfo1.playerId].Bounty;
                int _victimBountyHunter = PersistentContainer.Instance.Players[_cInfo1.playerId].BountyHunter; //victim kill streak
                if (_victimBounty > 0)
                {
                    int _killerWallet = PersistentContainer.Instance.Players[_cInfo2.playerId].PlayerWallet;
                    int _killerBounty = PersistentContainer.Instance.Players[_cInfo2.playerId].Bounty;
                    int _killerBountyHunter = PersistentContainer.Instance.Players[_cInfo2.playerId].BountyHunter; //killer kill streak
                    if (Kill_Streak > 0)
                    {
                        int _newKillerBountyHunter = _killerBountyHunter + 1;
                        if (_newKillerBountyHunter >= Kill_Streak)
                        {
                            PersistentContainer.Instance.Players[_cInfo2.playerId].PlayerWallet = _killerWallet + _victimBounty + Bonus;
                            PersistentContainer.Instance.Players[_cInfo2.playerId].BountyHunter = _newKillerBountyHunter;
                            PersistentContainer.Instance.Players[_cInfo2.playerId].Bounty = _killerBounty + Bonus;
                            PersistentContainer.DataChange = true;
                            using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                            {
                                sw.WriteLine(string.Format("{0}: {1} {2} has collected the bounty on {3} {4}.", DateTime.Now, _cInfo2.playerId, _cInfo2.playerName, _cInfo2.playerId, _cInfo2.playerName));
                                sw.WriteLine();
                                sw.WriteLine(string.Format("{0}: {1} {2} has collected {3} bounties without dying. Their kill streak and bounty have increased.", DateTime.Now, _cInfo2.playerId, _cInfo2.playerName, _newKillerBountyHunter));
                                sw.WriteLine();
                                sw.Flush();
                                sw.Close();
                            }
                            Phrases.Dict.TryGetValue("bounties1", out string _phrase);
                            _phrase = _phrase.Replace("{PlayerName}", _cInfo2.playerName);
                            _phrase = _phrase.Replace("{Value}", _newKillerBountyHunter.ToString());
                            ChatHook.ChatMessage(null, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Global, null);
                        }
                        else if (_killerBountyHunter + 1 < Kill_Streak)
                        {
                            PersistentContainer.Instance.Players[_cInfo2.playerId].PlayerWallet = _killerWallet + _victimBounty;
                            PersistentContainer.Instance.Players[_cInfo2.playerId].BountyHunter = _newKillerBountyHunter;
                            PersistentContainer.DataChange = true;
                        }
                        if (_victimBountyHunter >= Kill_Streak)
                        {
                            Phrases.Dict.TryGetValue("bounties2", out string _phrase);
                            _phrase = _phrase.Replace("{Victim}", _cInfo1.playerName);
                            _phrase = _phrase.Replace("{Killer}", _cInfo2.playerName);
                            ChatHook.ChatMessage(null, Config.Chat_Response_Color + _phrase + "[-]", -1, Config.Server_Response_Name, EChatType.Global, null);
                        }
                    }
                    else
                    {
                        PersistentContainer.Instance.Players[_cInfo2.playerId].PlayerWallet = _killerWallet + _victimBounty;
                        PersistentContainer.DataChange = true;
                        using (StreamWriter sw = new StreamWriter(filepath, true, Encoding.UTF8))
                        {
                            sw.WriteLine(string.Format("{0}: {1} {2} has collected the bounty on {3} {4}.", DateTime.Now, _cInfo2.playerId, _cInfo2.playerName, _cInfo2.playerId, _cInfo2.playerName));
                            sw.WriteLine();
                            sw.Flush();
                            sw.Close();
                        }
                    }
                    PersistentContainer.Instance.Players[_cInfo1.playerId].Bounty = 0;
                    PersistentContainer.Instance.Players[_cInfo1.playerId].BountyHunter = 0;
                    PersistentContainer.DataChange = true;
                    Phrases.Dict.TryGetValue("bounties3", out string _phrase1);
                    _phrase1 = _phrase1.Replace("{Victim}", _cInfo1.playerName);
                    _phrase1 = _phrase1.Replace("{Killer}", _cInfo2.playerName);
                    ChatHook.ChatMessage(null, Config.Chat_Response_Color + _phrase1 + "[-]", -1, Config.Server_Response_Name, EChatType.Global, null);
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in Bounties.ProcessPlayerKilled: {0}", e.Message));
            }
        }

        public static void ConsoleEdit(string _id, int _value)
        {
            int _oldBounty = PersistentContainer.Instance.Players[_id].Bounty;
            if (_value > 0)
            {
                int _newBounty = _oldBounty + _value;
                PersistentContainer.Instance.Players[_id].Bounty = _newBounty;
                PersistentContainer.DataChange = true;
                SdtdConsole.Instance.Output(string.Format("Bounty edit was successful for {0}. The new value is set to {1}", _id, _newBounty));
            }
            else
            {
                int _newBounty = _oldBounty - _value;
                if (_newBounty < 0)
                {
                    PersistentContainer.Instance.Players[_id].Bounty = 0;
                    PersistentContainer.DataChange = true;
                    SdtdConsole.Instance.Output(string.Format("Bounty edit was successful for {0}. The new value is {1}", _id, 0));
                }
                else
                {
                    PersistentContainer.Instance.Players[_id].Bounty = _newBounty;
                    PersistentContainer.DataChange = true;
                    SdtdConsole.Instance.Output(string.Format("Bounty edit was successful for {0}. The new value is {1}", _id, _newBounty));
                }
            }
        }

        public static void ConsoleRemoveBounty(string _id)
        {
            PersistentPlayer p = PersistentContainer.Instance.Players[_id];
            if (p != null)
            {
                p.Bounty = 0;
                PersistentContainer.DataChange = true;
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
                        SdtdConsole.Instance.Output(string.Format("Entity Id: {0} Name: {1} Current bounty: {2}", _cInfo.entityId, _cInfo.playerName, _currentbounty));
                    }
                }
            }
        }
    }
}
