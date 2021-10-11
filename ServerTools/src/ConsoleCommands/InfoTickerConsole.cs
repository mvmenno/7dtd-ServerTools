﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    class InfoTickerConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools]- Enable or disable infoTicker.";
        }
        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. st-it off\n" +
                   "  2. st-it on\n" +
                   "1. Turn off info ticker\n" +
                   "2. Turn on info ticker\n";
        }
        public override string[] GetCommands()
        {
            return new string[] { "st-InfoTicker", "it", "st-it" };
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
                    if (InfoTicker.IsEnabled)
                    {
                        InfoTicker.IsEnabled = false;
                        Config.WriteXml();
                        Config.LoadXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] InfoTicker has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] InfoTicker is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!InfoTicker.IsEnabled)
                    {
                        InfoTicker.IsEnabled = true;
                        Config.WriteXml();
                        Config.LoadXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] InfoTicker has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] InfoTicker is already on"));
                        return;
                    }
                }
                else
                {
                    SdtdConsole.Instance.Output(string.Format("Invalid argument {0}", _params[0]));
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in InfoTickerConsole.Execute: {0}", e.Message));
            }
        }
    }
}