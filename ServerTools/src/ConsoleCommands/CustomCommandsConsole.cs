﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    class CustomCommandsConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable or disable custom commands.";
        }
        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. st-cc off\n" +
                   "  2. st-cc on\n" +
                   "1. Turn off custom commands\n" +
                   "2. Turn on custom commands\n";
        }
        public override string[] GetCommands()
        {
            return new string[] { "st-CustomCommands", "custom", "st-cc" };
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
                    if (CustomCommands.IsEnabled)
                    {
                        CustomCommands.IsEnabled = false;
                        Config.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Custom commands has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Custom commands is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!CustomCommands.IsEnabled)
                    {
                        CustomCommands.IsEnabled = true;
                        Config.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Custom commands has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Custom commands is already on"));
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
                Log.Out(string.Format("[SERVERTOOLS] Error in CustomCommandsConsole.Execute: {0}", e.Message));
            }
        }
    }
}