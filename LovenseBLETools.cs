using LovenseLushController;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LovenseBLE
{
    class LovenseBLETools
    {

        #region  ########################################################## extern from C#############################
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void _NotifyCallbackDelegate(int result, [MarshalAs(UnmanagedType.LPWStr)] string id);
        [DllImport("LovenseBLE_Lib.dll")]
        private static extern void _RegisterNotifyCallback(_NotifyCallbackDelegate callback);
        private static _NotifyCallbackDelegate _notifyCallback;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void _ToyAddCallbackDelegate([MarshalAs(UnmanagedType.LPWStr)] string toyId, [MarshalAs(UnmanagedType.LPWStr)] string name, [MarshalAs(UnmanagedType.LPWStr)] string address, char c0, char c1);
        [DllImport("LovenseBLE_Lib.dll")]
        private static extern void _RegisterToyAddCallback(_ToyAddCallbackDelegate callback);
        private static _ToyAddCallbackDelegate _toyAddCallback;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void _CheckBLECallbackDelegate(int result);
        [DllImport("LovenseBLE_Lib.dll")]
        private static extern void _RegisterCheckBLECallback(_CheckBLECallbackDelegate callback);
        private static _CheckBLECallbackDelegate _checkBLECallback;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void _EventsAPICallbackDelegate([MarshalAs(UnmanagedType.LPWStr)] string toyId, int type, int hadrdwareType, int value);
        [DllImport("LovenseBLE_Lib.dll")]
        private static extern void _RegisterEventsAPICallback(_EventsAPICallbackDelegate callback);
        private static _EventsAPICallbackDelegate _eventsAPICallback;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void _ConnectChangedDelegate([MarshalAs(UnmanagedType.LPWStr)] string toyId, bool connected);
        [DllImport("LovenseBLE_Lib.dll")]
        private static extern void _RegisterConnectChangedCallback(_ConnectChangedDelegate callback);
        private static _ConnectChangedDelegate _connectChangedCallback;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void _BatteryDelegate([MarshalAs(UnmanagedType.LPWStr)] string toyId, [MarshalAs(UnmanagedType.LPWStr)] string battery);
        [DllImport("LovenseBLE_Lib.dll")]
        private static extern void _RegisterBatteryCallback(_BatteryDelegate callback);
        private static _BatteryDelegate _batteryCallback;


        [DllImport("LovenseBLE_Lib.dll")]
        public static extern void _Quit();

        [DllImport("LovenseBLE_Lib.dll")]
        public static extern void _CheckBLEStatus();

        [DllImport("LovenseBLE_Lib.dll")]
        public static extern void _StartBLEScan();

        [DllImport("LovenseBLE_Lib.dll")]
        public static extern void _StopBLEScan();

        [DllImport("LovenseBLE_Lib.dll")]
        public static extern void _ConnectToy([MarshalAs(UnmanagedType.LPWStr)] string deviceId);

        [DllImport("LovenseBLE_Lib.dll")]
        public static extern void _DisConnectToy([MarshalAs(UnmanagedType.LPWStr)] string deviceId);

        [DllImport("LovenseBLE_Lib.dll")]
        public static extern void _SendCommand([MarshalAs(UnmanagedType.LPWStr)] string deviceId, int commandType, int value);

        [DllImport("LovenseBLE_Lib.dll")]
        public static extern void _SendCommandToSolacePro([MarshalAs(UnmanagedType.LPWStr)] string deviceId, int speed, int strokeLow, int strokeHigh);

        [DllImport("LovenseBLE_Lib.dll")]
        public static extern void _OpenSolaceProEventsAPI([MarshalAs(UnmanagedType.LPWStr)] string deviceId, int mode);

        [DllImport("LovenseBLE_Lib.dll")]
        public static extern void _CloseSolaceProEventsAPI([MarshalAs(UnmanagedType.LPWStr)] string deviceId);

        [DllImport("LovenseBLE_Lib.dll")]
        public static extern void _OpenEventsAPI([MarshalAs(UnmanagedType.LPWStr)] string deviceId);

        [DllImport("LovenseBLE_Lib.dll")]
        public static extern void _CloseEventsAPI([MarshalAs(UnmanagedType.LPWStr)] string deviceId);
        #endregion

        private static LovenseBLETools instance;
        private List<LovenseToy> toys = new List<LovenseToy>();

        private Dictionary<string, ToySupport> supportedCommandDic = new Dictionary<string, ToySupport>();

        public delegate void ResultBLEEvent(CheckBLEResultStatus status);
        public event ResultBLEEvent resultBLEEvent;
        public delegate void AddToyEvent(LovenseToy toy);
        public event AddToyEvent addToyEvent;
        public delegate void ConnectChangedEvent(string id, bool connected);
        public event ConnectChangedEvent connectChangeEvent;
        public delegate void EventsAPIEvent(string id, LovenseDataReportingEventType type, int hardwareType, int value);
        public event EventsAPIEvent eventsAPIevent;

        public delegate void BatteryEvent(string id, string battery);
        public event BatteryEvent batteryEvent;

        public delegate void LovenseNotifyEventEvent(LovenseNotifyType type, string deviceId);
        public event LovenseNotifyEventEvent lovenseNotifyEvent;


        public static LovenseBLETools GetInstance()
        {
            if (instance == null)
            {
                instance = new LovenseBLETools();
            }
            return instance;
        }

        public void InitCallback()
        {
            InitCheckBleResultCallback();
            InitEventsApiCallback();
            InitNotifyResultCallback();
            InitToyAddCallback();
            InitConnectChangedCallback();
            InitBatteryCallback();

        }

        public void InitNotifyResultCallback()
        {
            if(_notifyCallback == null)
            {
                _notifyCallback = new _NotifyCallbackDelegate(OnNotifyResult);
                _RegisterNotifyCallback(_notifyCallback);
            }
        }

        private void OnNotifyResult(int result, string id)
        {
            if(lovenseNotifyEvent != null)
            {
                lovenseNotifyEvent((LovenseNotifyType)result, id);
            }
        }

        public void InitCheckBleResultCallback()
        {

            if (_checkBLECallback == null)
            {
                _checkBLECallback = new _CheckBLECallbackDelegate(OnCheckBLEResult);
                _RegisterCheckBLECallback(_checkBLECallback);
            }
        }

        private void OnCheckBLEResult(int result)
        {
            if (resultBLEEvent != null) {
                resultBLEEvent((CheckBLEResultStatus)result);
            }
        }

        public void InitToyAddCallback()
        {
            if (_toyAddCallback == null)
            {
                _toyAddCallback = new _ToyAddCallbackDelegate(OnToyAddResult);
                _RegisterToyAddCallback(_toyAddCallback);
            }
        }

        private void OnToyAddResult(string toyId, string name, string address, char c0, char c1)
        {
            if (addToyEvent != null)
            {
                string symbol = ("" + c0 + c1).Replace("0", "").ToLower();
                if(supportedCommandDic.ContainsKey(symbol))
                {
                    addToyEvent(new LovenseToy() { name = name, id = toyId, mac = address, type = supportedCommandDic[symbol].type });
                } 
                else
                {
                    addToyEvent(new LovenseToy() { name = name, id = toyId, mac = address, type = "unknown" });
                }
            }
        }

        public void InitEventsApiCallback()
        {
            if(_eventsAPICallback == null)
            {
                _eventsAPICallback = new _EventsAPICallbackDelegate(OnEventsApiResult);
                _RegisterEventsAPICallback(_eventsAPICallback);
            }
        }

        private void OnEventsApiResult(string toyId, int type, int hadrdwareType, int value)
        {
            if(eventsAPIevent != null)
            {
                eventsAPIevent(toyId, (LovenseDataReportingEventType)type, hadrdwareType, value);
            }
        }

        public void InitConnectChangedCallback()
        {
            if(_connectChangedCallback == null)
            {
                _connectChangedCallback = new _ConnectChangedDelegate(OnConnectChanged);
                _RegisterConnectChangedCallback(_connectChangedCallback);
            }
        }

        private void OnConnectChanged(string toyId, bool connected)
        {
            if(connectChangeEvent != null)
            {
                connectChangeEvent(toyId, connected);
            }
        }

        public void InitBatteryCallback()
        {
            if (_batteryCallback == null)
            {
                _batteryCallback = new _BatteryDelegate(OnGetBattery);
                _RegisterBatteryCallback(OnGetBattery);
            }
        }

        private void OnGetBattery(string toyId, string battery)
        {
            if (batteryEvent != null)
            {
                batteryEvent(toyId, battery);
            }
        }

        public void InitSupport(string supportTextData, Action<InitSupportCommandStatus> completed)
        {
            supportedCommandDic.Clear();
            try
            {
                List<LovenseCommandType> supported = new List<LovenseCommandType>();
                supportTextData = supportTextData.Replace("\"\"", "\"");
                string[] datas = supportTextData.Split('\n');
                for (int i=0;i<datas.Length;i++)
                {
                    if(i < 2)
                    {
                        continue;
                    }
                    string data = datas[i];
                    supported.Clear();
                    //^.??,|\"\[.*\]\"|[^,^\"]+|\"\{.*\}\"
                    //^.??,|\[.*\]|(?<=^|,)\w+|\{.*\}
                    string pattern = @"^.??,|\[.*\]|(?<=^|,)\w+|\{.*\}";
                    var matches = Regex.Matches(data, pattern);

                    List<string> result = new List<string>();
                    foreach (Match match in matches)
                    {
                        result.Add(match.Value);
                    }
                    if (result.Count == 7)
                    {
                        
                        string[] symbols = result[2].Replace("[", "").Replace("]", "").Replace("\\", "").Replace("\"", "").Split(',');
                        string type = result[4];
                        string showName = result[5];
                        string result6 = result[6].Replace(" ", "").Replace("\"", "");
                        var matches1 = Regex.Matches(result6, @"(?<=v:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.VIBRATE);
                            }
                        }
                        matches1 = Regex.Matches(result6, @"(?<=v1:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.VIBRATE1);
                            }
                        }
                        matches1 = Regex.Matches(result6, @"(?<=v2:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.VIBRATE2);
                            }
                        }
                        matches1 = Regex.Matches(result6, @"(?<=v3:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.VIBRATE3);
                            }
                        }
                        matches1 = Regex.Matches(result6, @"(?<=r:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.ROTATE);
                            }
                        }
                        matches1 = Regex.Matches(result6, @"(?<=p:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.PUMP);
                            }
                        }
                        matches1 = Regex.Matches(result6, @"(?<=t:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.THRUSTRING);
                            }
                        }
                        matches1 = Regex.Matches(result6, @"(?<=f:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.FINGERING);
                            }
                        }
                        matches1 = Regex.Matches(result6, @"(?<=s:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.SUCTION);
                            }
                        }
                        matches1 = Regex.Matches(result6, @"(?<=d:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.DEPTH);
                            }
                        }
                        matches1 = Regex.Matches(result6, @"(?<=pos:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.POSITION);
                            }
                        }
                        matches1 = Regex.Matches(result6, @"(?<=o:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.OSCILLATE);
                            }
                        }
                        foreach (string symbol in symbols)
                        {
                            ToySupport toySupport = new ToySupport();
                            toySupport.type = type;
                            toySupport.showName = showName;
                            toySupport.supported = supported.ToArray();
                            supportedCommandDic.Add(symbol.Trim(), toySupport);
                        }

                    } else if(result.Count == 5)
                    {
                        string[] symbols = result[1].Replace("[", "").Replace("]", "").Replace("\\", "").Replace("\"", "").Split(',');
                        string type = result[2];
                        string showName = result[3];
                        string result4 = result[4].Replace(" ", "").Replace("\"", "");
                        var matches1 = Regex.Matches(result4, @"(?<=v:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.VIBRATE);
                            }
                        }
                        matches1 = Regex.Matches(result4, @"(?<=v1:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.VIBRATE1);
                            }
                        }
                        matches1 = Regex.Matches(result4, @"(?<=v2:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.VIBRATE2);
                            }
                        }
                        matches1 = Regex.Matches(result4, @"(?<=v3:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.VIBRATE3);
                            }
                        }
                        matches1 = Regex.Matches(result4, @"(?<=r:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.ROTATE);
                            }
                        }
                        matches1 = Regex.Matches(result4, @"(?<=p:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.PUMP);
                            }
                        }
                        matches1 = Regex.Matches(result4, @"(?<=t:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.THRUSTRING);
                            }
                        }
                        matches1 = Regex.Matches(result4, @"(?<=f:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.FINGERING);
                            }
                        }
                        matches1 = Regex.Matches(result4, @"(?<=s:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.SUCTION);
                            }
                        }
                        matches1 = Regex.Matches(result4, @"(?<=d:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.DEPTH);
                            }
                        }
                        matches1 = Regex.Matches(result4, @"(?<=pos:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.POSITION);
                            }
                        }
                        matches1 = Regex.Matches(result4, @"(?<=o:)(false|true)");
                        if (matches1.Count > 0)
                        {
                            if (matches1[0].Value == "true")
                            {
                                supported.Add(LovenseCommandType.OSCILLATE);
                            }
                        }
                        foreach (string symbol in symbols)
                        {
                            ToySupport toySupport = new ToySupport();
                            toySupport.type = type;
                            toySupport.showName = showName;
                            toySupport.supported = supported.ToArray();
                            supportedCommandDic.Add(symbol.Trim(), toySupport);
                        }

                    }

                }
                if (completed != null)
                {
                    completed(InitSupportCommandStatus.SUCCESS);
                }
            }
            catch (Exception e)
            {
                if (completed != null)
                {
                    completed(InitSupportCommandStatus.FAILED);
                }
            }
        }

        public void CheckBLEStatus()
        {
            _CheckBLEStatus();
        }
        public void StartBLEScan()
        {
            _StartBLEScan();
        }

        public void StopBLEScan()
        {
            _StopBLEScan();
        }

        public void ConnectToy(string id)
        {
            _ConnectToy(id);
        }

        public void DisConnectToy(string id)
        {
            _DisConnectToy(id);
        }

        public void StartEventsAPI(string id)
        {
            _OpenEventsAPI(id);
        }
       
        public void CloseEventsAPI(string id)
        {
            _CloseEventsAPI(id);
        }

        public void StartSolaceProEventsAPI(string id,int mode)
        {
            _OpenSolaceProEventsAPI(id,mode);
        }

        public void CloseSolaceProEventsAPI(string id)
        {
            _CloseSolaceProEventsAPI(id);
        }



        public void SendCommands(string id,LovenseCommand[] commands)
        {
            foreach(LovenseCommand cmd in commands)
            {
                _SendCommand(id,(int)cmd.commandType, cmd.value);
            }
        }

        public void SendCommand(string id, LovenseCommand command)
        {
            Plugin.Logger.LogInfo($"Command {command} sent to {id}");
            _SendCommand(id, (int)command.commandType, command.value);
        }

        public void SendCommandsToSolacePro(string id, LovenseSolaceProCommand command)
        {
            _SendCommandToSolacePro(id, command.speedLevel, command.strokeLow, command.strokeHigh);
        }

        public void Stop(string id,string toyType)
        {
            LovenseCommandType[] commandTypes = GetSupportedCommandsByType(toyType);
            if(commandTypes.Length > 0)
            {
                foreach(LovenseCommandType commandType in commandTypes)
                {
                    LovenseCommand cmd = new LovenseCommand() { commandType = commandType, value = 0 };
                    SendCommand(id,cmd);
                }
            }
        }

        public LovenseCommandType[] GetSupportedCommandsByType(string toyType)
        {
            foreach(KeyValuePair<string,ToySupport> kv in supportedCommandDic)
            {
                if(kv.Value.type == toyType)
                {
                    return kv.Value.supported;
                }
            }
            
            return new LovenseCommandType[] { };
        }
    }



}
