﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    class ScoutPlayerConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable or disable scout player.";
        }
        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. st-sp off\n" +
                   "  2. st-sp on\n" +
                   "1. Turn off scout player\n" +
                   "2. Turn on scout player\n";
        }
        public override string[] GetCommands()
        {
            return new string[] { "st-ScoutPlayer", "sp", "st-sp" };
        }
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 1)
                {
                    SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Wrong number of arguments, expected 1, found {0}", _params.Count));
                    return;
                }
                if (_params[0].ToLower().Equals("off"))
                {
                    if (ScoutPlayer.IsEnabled)
                    {
                        ScoutPlayer.IsEnabled = false;
                        Config.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Scout player has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Scout player is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!ScoutPlayer.IsEnabled)
                    {
                        ScoutPlayer.IsEnabled = true;
                        Config.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Scout player has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Scout player is already on"));
                        return;
                    }
                }
                else
                {
                    SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Invalid argument {0}", _params[0]));
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in ScoutPlayerConsole.Execute: {0}", e.Message));
            }
        }
    }
}