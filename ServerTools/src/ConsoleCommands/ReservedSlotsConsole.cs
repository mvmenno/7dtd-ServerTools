﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    public class ReservedSlotConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable, disable, add, remove and view the reserved slots list.";
        }

        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. st-rs off\n" +
                   "  2. st-rs on\n" +
                   "  3. st-rs add <steamId> <playerName> <days to expire>\n" +
                   "  4. st-rs remove <steamId/entityId>\n" +
                   "  5. st-rs list\n" +
                   "1. Turn off reserved slots\n" +
                   "2. Turn on reserved slots\n" +
                   "3. Adds a steamID to the Reserved Slots list\n" +
                   "4. Removes a steamID from the Reserved Slots list\n" +
                   "5. Lists all steamIDs that have a Reserved Slot";
        }

        public override string[] GetCommands()
        {
            return new string[] { "st-ReservedSlots", "rs", "st-rs" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 1 && _params.Count != 2 && _params.Count != 4)
                {
                    SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Wrong number of arguments, expected 1, 2 or 4, found {0}", _params.Count));
                    return;
                }
                if (_params[0].ToLower().Equals("off"))
                {
                    if (ReservedSlots.IsEnabled)
                    {
                        ReservedSlots.IsEnabled = false;
                        Config.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Reserved slots has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Reserved slots is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!ReservedSlots.IsEnabled)
                    {
                        ReservedSlots.IsEnabled = true;
                        Config.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Reserved slots has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Reserved slots is already on"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("add"))
                {
                    if (_params.Count != 4)
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Wrong number of arguments, expected 4, found {0}", _params.Count));
                        return;
                    }
                    if (_params[1].Length != 17)
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Can not add Id: Invalid Id {0}", _params[1]));
                        return;
                    }
                    if (ReservedSlots.Dict.ContainsKey(_params[1]))
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Can not add Id. {0} is already in the Reserved slots list", _params[1]));
                        return;
                    }
                    if (!double.TryParse(_params[3], out double _daysToExpire))
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Invalid days to expire: {0}", _params[3]));
                        return;
                    }
                    DateTime _expireDate;
                    if (_daysToExpire > 0d)
                    {
                        _expireDate = DateTime.Now.AddDays(_daysToExpire);
                    }
                    else
                    {
                        _expireDate = DateTime.Now.AddDays(18250d);
                    }
                    ReservedSlots.Dict.Add(_params[1], _expireDate);
                    ReservedSlots.Dict1.Add(_params[1], _params[2]);
                    ReservedSlots.UpdateXml();
                    SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Added Id {0} with the name of {1} that expires on {2} to the Reserved slots list", _params[1], _params[2], _expireDate.ToString()));
                }
                else if (_params[0].ToLower().Equals("remove"))
                {
                    if (_params.Count != 2)
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Wrong number of arguments, expected 2, found {0}", _params.Count));
                        return;
                    }
                    if (_params[1].Length < 1 || _params[1].Length > 17)
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Can not remove Id: Invalid Id {0}", _params[1]));
                        return;
                    }
                    ClientInfo _cInfo = ConsoleHelper.ParseParamIdOrName(_params[1]);
                    if (_cInfo != null)
                    {
                        if (!ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                        {
                            SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Id {0} was not found on the Reserved slots list", _params[1]));
                            return;
                        }
                        ReservedSlots.Dict.Remove(_cInfo.playerId);
                        ReservedSlots.Dict1.Remove(_cInfo.playerId);
                        ReservedSlots.UpdateXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Removed Id {0} from Reserved slots list", _params[1]));
                    }
                    else
                    {
                        if (ReservedSlots.Dict.ContainsKey(_params[1]))
                        {
                            ReservedSlots.Dict.Remove(_params[1]);
                            ReservedSlots.Dict1.Remove(_params[1]);
                            SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Removed Id {0} from Reserved slots list", _params[1]));
                            ReservedSlots.UpdateXml();
                        }
                        else
                        {
                            SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Id {0} was not found on the Reserved slots list", _params[1]));
                        }
                    }
                }
                else if (_params[0].ToLower().Equals("list"))
                {
                    if (_params.Count != 1)
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Wrong number of arguments, expected 1, found {0}", _params.Count));
                        return;
                    }
                    if (ReservedSlots.Dict.Count == 0)
                    {
                        SdtdConsole.Instance.Output("[SERVERTOOLS] There are no players on the Reserved slots list");
                        return;
                    }
                    else
                    {
                        foreach (var _key in ReservedSlots.Dict)
                        {
                            if (ReservedSlots.Dict1.TryGetValue(_key.Key, out string _name))
                            {
                                SdtdConsole.Instance.Output(string.Format("{0} {1} {2}", _key.Key, _name, _key.Value));
                            }
                        }
                    }
                }
                else
                {
                    SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Invalid argument {0}", _params[0]));
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in ReservedSlotConsole.Execute: {0}", e.Message));
            }
        }
    }
}
