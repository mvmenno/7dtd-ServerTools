﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    class SuicideConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable or disable suicide.";
        }
        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. st-scide off\n" +
                   "  2. st-scide on\n" +
                   "1. Turn off suicide\n" +
                   "2. Turn on suicide\n";
        }
        public override string[] GetCommands()
        {
            return new string[] { "st-Suicide", "scide", "st-scide" };
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
                    if (Suicide.IsEnabled)
                    {
                        Suicide.IsEnabled = false;
                        Config.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Suicide has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Suicide is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!Suicide.IsEnabled)
                    {
                        Suicide.IsEnabled = true;
                        Config.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Suicide has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Suicide is already on"));
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
                Log.Out(string.Format("[SERVERTOOLS] Error in SuicideConsole.Execute: {0}", e.Message));
            }
        }
    }
}