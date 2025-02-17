﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ServerTools
{
    class LoginNotice
    {
        public static bool IsEnabled = false, IsRunning = false;
        public static Dictionary<string, string> Dict = new Dictionary<string, string>();

        private const string file = "LoginNotice.xml";
        private static readonly string FilePath = string.Format("{0}/{1}", API.ConfigPath, file);
        private static FileSystemWatcher FileWatcher = new FileSystemWatcher(API.ConfigPath, file);

        public static void Load()
        {
            LoadXml();
            InitFileWatcher();
        }

        public static void Unload()
        {
            Dict.Clear();
            FileWatcher.Dispose();
            IsRunning = false;
        }

        public static void LoadXml()
        {
            if (!Utils.FileExists(FilePath))
            {
                UpdateXml();
            }
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(FilePath);
            }
            catch (XmlException e)
            {
                Log.Error(string.Format("[SERVERTOOLS] Failed loading {0}: {1}", file, e.Message));
                return;
            }
            XmlNodeList _childNodes = xmlDoc.DocumentElement.ChildNodes;
            if (_childNodes != null && _childNodes.Count > 0)
            {
                Dict.Clear();
                bool upgrade = true;
                for (int i = 0; i < _childNodes.Count; i++)
                {
                    if (_childNodes[i].NodeType == XmlNodeType.Comment)
                    {
                        continue;
                    }
                    XmlElement _line = (XmlElement)_childNodes[i];
                    if (_line.HasAttributes)
                    {
                        if (_line.HasAttribute("Version") && _line.GetAttribute("Version") == Config.Version)
                        {
                            upgrade = false;
                        }
                        if (_line.HasAttribute("Id") && _line.HasAttribute("Message"))
                        {
                            string _id = _line.GetAttribute("Id");
                            string _message = _line.GetAttribute("Message");
                            if (!Dict.ContainsKey(_id))
                            {
                                Dict.Add(_id, _message);
                            }
                        }
                    }
                }
                if (upgrade)
                {
                    UpgradeXml(_childNodes);
                    return;
                }
            }
        }

        private static void UpdateXml()
        {
            try
            {
                FileWatcher.EnableRaisingEvents = false;
                using (StreamWriter sw = new StreamWriter(FilePath, false, Encoding.UTF8))
                {
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    sw.WriteLine("<LoginNotice>");
                    sw.WriteLine(string.Format("<ST Version=\"{0}\" />", Config.Version));
                    sw.WriteLine();
                    sw.WriteLine();
                    if (Dict.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> kvp in Dict)
                        {
                            if (Dict.TryGetValue(kvp.Key, out string _message))
                            {
                                sw.WriteLine(string.Format("    <Player Id=\"{0}\" Message=\"{1}\" />", kvp.Key, _message));
                            }
                        }
                    }
                    else
                    {
                        sw.WriteLine("    <Player Id=\"76561191234567891\" Message=\"Time to kick ass and chew bubble gum\" />");
                        sw.WriteLine("    <Player Id=\"76561199876543210\" Message=\"Head admin has arrived... shhh\" />");
                        sw.WriteLine("    <Player Id=\"76561191234509876\" Message=\"Run for your lives, {PlayerName} has logged on\" />");
                    }
                    sw.WriteLine("</LoginNotice>");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in LoginNotice.UpdateXml: {0}", e.Message));
            }
            FileWatcher.EnableRaisingEvents = true;
        }

        private static void InitFileWatcher()
        {
            FileWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
            FileWatcher.Created += new FileSystemEventHandler(OnFileChanged);
            FileWatcher.Deleted += new FileSystemEventHandler(OnFileChanged);
            FileWatcher.EnableRaisingEvents = true;
            IsRunning = true;
        }

        private static void OnFileChanged(object source, FileSystemEventArgs e)
        {
            if (!Utils.FileExists(FilePath))
            {
                UpdateXml();
            }
            LoadXml();
        }

        public static void PlayerNotice(ClientInfo _cInfo)
        {
            if (Dict.TryGetValue(_cInfo.playerId, out string _message))
            {
                _message = _message.Replace("{PlayerName}", _cInfo.playerName);
                ChatHook.ChatMessage(_cInfo, Config.Chat_Response_Color + _message + "[-]", -1, Config.Server_Response_Name, EChatType.Global, null);
            }
        }

        private static void UpgradeXml(XmlNodeList _oldChildNodes)
        {
            try
            {
                FileWatcher.EnableRaisingEvents = false;
                using (StreamWriter sw = new StreamWriter(FilePath, false, Encoding.UTF8))
                {
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    sw.WriteLine("<LoginNotice>");
                    sw.WriteLine(string.Format("<ST Version=\"{0}\" />", Config.Version));
                    sw.WriteLine();
                    sw.WriteLine();
                    for (int i = 0; i < _oldChildNodes.Count; i++)
                    {
                        if (_oldChildNodes[i].NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        XmlElement _line = (XmlElement)_oldChildNodes[i];
                        if (_line.HasAttributes && _line.Name == "Player")
                        {
                            string _id = "", _message = "";
                            if (_line.HasAttribute("Id"))
                            {
                                _id = _line.GetAttribute("Id");
                            }
                            if (_line.HasAttribute("Message"))
                            {
                                _message = _line.GetAttribute("Message");
                            }
                            sw.WriteLine(string.Format("    <Player Id=\"{0}\" Message=\"{1}\" />", _id, _message));
                        }
                    }
                    sw.WriteLine("</LoginNotice>");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in LoginNotice.UpgradeXml: {0}", e.Message));
            }
            FileWatcher.EnableRaisingEvents = true;
            LoadXml();
        }
    }
}
