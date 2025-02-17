﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    class WalletConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable, disable, add, reduce, check wallet.";
        }

        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. st-wlt off\n" +
                   "  2. st-wlt on\n" +
                   "  3. st-wlt all <value>\n" +
                   "  4. st-wlt <steamId> <value>\n" +
                   "  5. st-wlt <steamId>\n" +
                   "1. Turn off wallet\n" +
                   "2. Turn on wallet\n" +
                   "3. Add to or reduce all online player's wallet value.\n" +
                   "4. Add to or reduce a player's wallet value.\n" +
                   "5. Check player's wallet value.\n";
                   
        }

        public override string[] GetCommands()
        {
            return new string[] { "st-Wallet", "wlt", "st-wlt" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count < 1 || _params.Count > 2)
                {
                    SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Wrong number of arguments, expected 1 or 2, found {0}", _params.Count));
                    return;
                }
                if (_params[0].ToLower().Equals("off"))
                {
                    if (Wallet.IsEnabled)
                    {
                        Wallet.IsEnabled = false;
                        Config.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Wallet has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Wallet is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!Wallet.IsEnabled)
                    {
                        Wallet.IsEnabled = true;
                        Config.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Wallet has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Wallet is already on"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("all"))
                {
                    bool _negative = false;
                    if (_params[1].Contains("-"))
                    {
                        _params[1].Replace("-", "");
                        _negative = true;
                    }
                    if (!int.TryParse(_params[1], out int _adjustCoins))
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Can not adjust wallet. Value {0} is invalid", _params[1]));
                    }
                    else
                    {
                        List<ClientInfo> _cInfoList = PersistentOperations.ClientList();
                        for (int i = 0; i < _cInfoList.Count; i++)
                        {
                            ClientInfo _cInfo = _cInfoList[i];
                            if (_cInfo != null)
                            {
                                if (_negative)
                                {
                                    Wallet.SubtractCoinsFromWallet(_cInfo.playerId, _adjustCoins);
                                    SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Subtracted {0} {1} from player id {2} wallet", _params[1], Wallet.Coin_Name, _params[0]));
                                }
                                else
                                {
                                    Wallet.AddCoinsToWallet(_cInfo.playerId, _adjustCoins);
                                    SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Added {0} {1} to player id {2} wallet", _params[1], Wallet.Coin_Name, _params[0]));
                                }
                            }
                        }
                    }
                }
                else if (_params.Count == 2)
                {
                    if (_params[0].Length < 1 || _params[0].Length > 17)
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Can not adjust wallet: Invalid Id {0}", _params[0]));
                        return;
                    }
                    if (_params[1].Length < 1 || _params[1].Length > 5)
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Can not adjust wallet. Value {0} is invalid", _params[1]));
                        return;
                    }
                    bool _negative = false;
                    if (_params[1].Contains("-"))
                    {
                        _params[1].Replace("-", "");
                        _negative = true;
                    }
                    if (!int.TryParse(_params[1], out int _adjustCoins))
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Can not adjust wallet. Value {0} is invalid", _params[1]));
                    }
                    else
                    {
                        PersistentPlayer p = PersistentContainer.Instance.Players[_params[0]];
                        if (p != null)
                        {
                            
                            if (_negative)
                            {
                                Wallet.SubtractCoinsFromWallet(_params[0], _adjustCoins);
                                SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Subtracted {0} {1} from wallet {2}", _params[1], Wallet.Coin_Name, _params[0]));
                            }
                            else
                            {
                                Wallet.AddCoinsToWallet(_params[0], _adjustCoins);
                                SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Added {0} {1} to wallet {2}", _params[1], Wallet.Coin_Name, _params[0]));
                            }
                        }
                        else
                        {
                            SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Player id not found: {0}", _params[0]));
                        }
                    }
                }
                else if (_params.Count == 1)
                {
                    if (_params[0].Length < 1 || _params[0].Length > 17)
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Can not check wallet value: Invalid Id {0}", _params[0]));
                        return;
                    }
                    int _currentWallet = Wallet.GetCurrentCoins(_params[0]);
                    string _playerName = PersistentContainer.Instance.Players[_params[0]].PlayerName;
                    if (string.IsNullOrEmpty(_playerName))
                    {
                        _playerName = "Unknown";
                    }
                    SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Player with id {0}, named {1}, has a wallet value of: {2}", _params[0], _playerName, _currentWallet));
                }
                else
                {
                    SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Invalid argument {0}", _params[0]));
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in WalletConsole.Execute: {0}", e.Message));
            }
        }
    }
}