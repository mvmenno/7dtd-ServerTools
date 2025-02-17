﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    class AutoBackupConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable or disable auto backup.";
        }

        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. st-ab off\n" +
                   "  2. st-ab on\n" +
                   "  3. st-ab\n" +
                   "1. Turn off world auto backup\n" +
                   "2. Turn on world auto backup\n" +
                   "3. Start a backup manually\n";
        }

        public override string[] GetCommands()
        {
            return new string[] { "st-AutoBackup", "ab", "st-ab" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count > 1)
                {
                    SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Wrong number of arguments, expected 0 or 1, found {0}", _params.Count));
                    return;
                }
                if (_params.Count == 0)
                {
                    SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Starting backup"));
                    AutoBackup.Exec();
                    SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Backup complete"));
                    return;
                }
                else if (_params[0].ToLower().Equals("off"))
                {
                    if (AutoBackup.IsEnabled)
                    {
                        AutoBackup.IsEnabled = false;
                        Config.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Auto backup has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Auto backup is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!AutoBackup.IsEnabled)
                    {
                        AutoBackup.IsEnabled = true;
                        Config.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Auto backup has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("[SERVERTOOLS] Auto backup is already on"));
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
                Log.Out(string.Format("[SERVERTOOLS] Error in AutoBackup.Execute: {0}", e.Message));
            }
        }
    }
}
