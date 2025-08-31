using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LovenseBLE;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace LovenseLushController;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    public void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Plugin.Logger = base.Logger;
        Harmony harmony = new Harmony("com.Harmony.RepoLib");
        harmony.PatchAll(Assembly.GetExecutingAssembly());



        var go = new GameObject("IMGUIOverlay");
        GameObject.DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.HideAndDontSave;
        go.AddComponent<ImGuiBehaviour>();


        GetSupportedCommand();
        LovenseBLETools.GetInstance().InitCallback();
        LovenseBLETools.GetInstance().CheckBLEStatus();

        LovenseBLETools.GetInstance().lovenseNotifyEvent += Form1_LovenseNotifyMessage;
        LovenseBLETools.GetInstance().addToyEvent += Form1_addToyEvent;
        LovenseBLETools.GetInstance().connectChangeEvent += Form1_connectChangeEvent;
        LovenseBLETools.GetInstance().eventsAPIevent += Form1_eventsAPIevent;
        LovenseBLETools.GetInstance().batteryEvent += Form1_eventsBattery;


    }
    private void Form1_addToyEvent(LovenseToy toy)
    {
        //stop bluetooth scan and connect toy
        try
        {
            LovenseBLETools.GetInstance().StopBLEScan();
            LovenseBLETools.GetInstance().ConnectToy(toy.id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message);
        }
    }

    private void Form1_LovenseNotifyMessage(LovenseNotifyType type, string id)
    {
        Logger.LogInfo(type.ToString());
    }
    private void Form1_eventsBattery(string id, string battery)
    {
        Logger.LogInfo("get Battery:" + id + "," + battery);
    }

    private void Form1_eventsAPIevent(string id, LovenseDataReportingEventType type, int hardwareType, int value)
    {
        Logger.LogInfo("get events->" + id + ",event type:" + type + ",hardwareType:" + hardwareType + ",value:" + value);
    }


    private void Form1_connectChangeEvent(string id, bool connected)
    {
        if (connected)
        {
            try
            {
                LovenseSettings.ToyId = id;
                //LovenseBLETools.GetInstance().SendCommand(id, new LovenseCommand() { commandType = LovenseCommandType.GET_BATTERY });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        Logger.LogInfo("ConnectChanged:" + id + "," + connected);
    }


    public async void GetSupportedCommand()
    {
        string supportText = @",symbol,type,showName,func
0,1.5,version,,
1,[  ""l""],Ambi,Ambi,""{ """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}
            ""
2,[  ""ca""],Mission2,Mission2,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
3,[  ""su""],c20,C20,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
4,[  ""t""],Calor,Calor,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
5,[  ""r""],Diamo,Diamo,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
6,[  ""j""],Dolce,Dolce,""{  """"p"""": false,  """"r"""": false,  """"v2"""": true,  """"v1"""": true,  """"v"""": true}""
7,[  ""w""],Domi,Domi,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
8,[  ""p""],Edge,Edge,""{  """"p"""": false,  """"r"""": false,  """"v2"""": true,  """"v1"""": true,  """"v"""": true}""
9,[  ""ef""],Exomoon,Exomoon,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
10,[  ""x""],Ferri,Ferri,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
11,[  ""ei""],Flexer,Flexer,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true,  """"f"""": true}""
12,[  ""n""],Gemini,Gemini,""{  """"p"""": false,  """"r"""": false,  """"v2"""": true,  """"v1"""": true,  """"v"""": true}""
13,[  ""ea""],Gravity,Gravity,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true,  """"t"""": true,  """"s"""": false}""
14,[  ""ed""],Gush,Gush,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
15,[  ""ba""],SolacePro,SolacePro,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": false,  """"t"""": true,  """"pos"""": true}""
16,[  ""z""],Hush,Hush,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
17,[  ""eb""],Hyphy,Hyphy,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
18,[  ""u""],Lapis,Lapis,""{  """"p"""": false,  """"r"""": false,  """"v2"""": true,  """"v1"""": true,  """"v3"""": true,  """"v"""": true}""
19,[  ""s""],Lush,Lush,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
20,""[  """"toyb"""",  """"b""""]"",Max,Max,""{  """"p"""": true,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
21,[  ""fs""],MiniXMachine,MiniXMachine,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true,  """"t"""": true}""
22,[  ""v""],Mission,Mission,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
23,""[  """"toyc"""",  """"toya"""",  """"c"""",  """"a""""]"",Nora,Nora,""{  """"p"""": false,  """"r"""": true,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
24,[  ""o""],Osci,Osci,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
25,[  ""qa""],QA01,QA01,""{  """"p"""": false,  """"r"""": false,  """"v2"""": true,  """"v1"""": true,  """"v"""": true}""
26,[  ""el""],Ridge,Ridge,""{  """"p"""": false,  """"r"""": true,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
27,[  ""h""],Solace,Solace,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": false,  """"t"""": true,  """"d"""": true}""
28,[  ""q""],Tenera,Tenera,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true,  """"s"""": true}""
29,[  ""sd""],Vulse,Vulse,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true}""
30,[  ""f""],XMachine,XMachine,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": true,  """"t"""": true}""
31,[  ""ez""],Gush2,Gush2,""{  """"p"""": false,  """"r"""": false,  """"v2"""": false,  """"v1"""": false,  """"v"""": false, """"o"""":true}""";

        //string url = "https://developer.lovense.com/LovenseToySupportConfig.csv";
        //HttpClient client = new HttpClient();
        //string supportText = await client.GetStringAsync(new Uri(url));

        LovenseBLETools.GetInstance().InitSupport(supportText, (InitSupportCommandStatus status) => {
            if (status == InitSupportCommandStatus.SUCCESS)
            {

            }
        });
    }

    public static int Map1to20(int value, int max)
    {
        if (max <= 0) throw new ArgumentException("max must be > 0");
        double p = Math.Clamp((double)value / max, 0.0, 1.0); // or manual clamp
        int n = 1 + (int)Math.Round(p * 19, MidpointRounding.AwayFromZero);
        return Math.Min(Math.Max(n, 1), 20);
    }

    public static List<int> vibrationQueue = new List<int>();

    public static void VibrateToy(int intensity)
    {
        if (!LovenseSettings.Testing)
        {
            if (LovenseSettings.HardCoreMode)
            {
                intensity = 20;
            }
            
        }
        if (LovenseSettings.isItemGrabbed)
        {
            vibrationQueue.Add(intensity);
            LovenseSettings.isItemGrabbedListEmpty = false;
        }
        else if(vibrationQueue.Count != 0)
        {
            vibrationQueue.Add(intensity);
            LovenseSettings.isItemGrabbedListEmpty = false;
        }
        else
        {
            if (!LovenseSettings.Testing)
            {
                LovenseBLETools.GetInstance().SendCommand(LovenseSettings.ToyId, new LovenseCommand() { commandType = LovenseCommandType.VIBRATE, value = intensity });
            }
        }
        Plugin.Logger.LogInfo($"Plugin.VibrateToy({intensity})");
    }
    //public static void VibrateToy(int intensity)
    //{
    //    // Force hardcore override if not in testing
    //    if (!LovenseSettings.Testing && LovenseSettings.HardCoreMode)
    //        intensity = 20;
    //
    //    // Decide if we should enqueue or send immediately
    //    bool shouldEnqueue = LovenseSettings.isItemGrabbed || vibrationQueue.Count > 0;
    //
    //    if (shouldEnqueue)
    //    {
    //        vibrationQueue.Add(intensity);
    //        LovenseSettings.isItemGrabbedListEmpty = false;
    //    }
    //    else if (!LovenseSettings.Testing)
    //    {
    //        // Send vibration directly
    //        LovenseBLETools.GetInstance().SendCommand(
    //            LovenseSettings.ToyId,
    //            new LovenseCommand { commandType = LovenseCommandType.VIBRATE, value = intensity }
    //        );
    //    }
    //
    //    Plugin.Logger.LogInfo($"Plugin.VibrateToy({intensity})");
    //}


    public static void VibrateStop()
    {
        if (!LovenseSettings.Testing)
        {
            LovenseBLETools.GetInstance().SendCommand(LovenseSettings.ToyId, new LovenseCommand() { commandType = LovenseCommandType.VIBRATE, value = 0 });
        }
    }
}


public class ImGuiBehaviour : MonoBehaviour
{
    private bool showMenu = false;
    private Rect windowRect = new Rect(20, 20, 380, 240);
    private bool resizing = false;
    private Vector2 resizeStartMouse;
    private Vector2 resizeStartSize;

    public void RunQueue()
    {
        if (LovenseSettings.isItemGrabbed)
            return; // nothing to do while item is grabbed

        LovenseSettings.isItemGrabbedTick++;

        if (LovenseSettings.isItemGrabbedTick <= LovenseSettings.isItemGrabbedTickMax)
            return; // wait until tick threshold is reached

        LovenseSettings.isItemGrabbedTick = 0; // reset tick

        // Case 1: vibration queue has something
        if (Plugin.vibrationQueue.Count > 0)
        {
            int vib = Plugin.vibrationQueue[0];
            Plugin.vibrationQueue.RemoveAt(0);

            if (LovenseSettings.Enabled)
            {
                Plugin.Logger.LogInfo($"Queued Vibrate {vib}");

                if (!LovenseSettings.Testing)
                {
                    LovenseBLETools.GetInstance().SendCommand(
                        LovenseSettings.ToyId,
                        new LovenseCommand { commandType = LovenseCommandType.VIBRATE, value = vib }
                    );
                }
            }

            return;
        }

        // Case 2: empty queue but "not yet marked empty"
        if (!LovenseSettings.isItemGrabbedListEmpty)
        {
            Plugin.Logger.LogInfo("Queued Vibrate 0");

            if (!LovenseSettings.Testing)
            {
                LovenseBLETools.GetInstance().SendCommand(
                    LovenseSettings.ToyId,
                    new LovenseCommand { commandType = LovenseCommandType.VIBRATE, value = 0 }
                );
            }

            LovenseSettings.isItemGrabbedListEmpty = true;
        }

    }

    public void OnGUI()
    {

        if (!LovenseSettings.isItemGrabbed)
        {
            if (LovenseSettings.isItemGrabbedTick > (int)(LovenseSettings.isItemGrabbedTickMax))
            {
                if (Plugin.vibrationQueue.Count >= 1)
                {
                    int vib = Plugin.vibrationQueue[0];
                    if (LovenseSettings.Enabled)
                    {
                        Plugin.Logger.LogInfo($"Queued Vibrate {vib}");
                        if (!LovenseSettings.Testing)
                        {
                            LovenseBLETools.GetInstance().SendCommand(LovenseSettings.ToyId, new LovenseCommand() { commandType = LovenseCommandType.VIBRATE, value = vib });
                        }
                    }
                    Plugin.vibrationQueue.RemoveAt(0);
                }
                else
                {
                    if (!LovenseSettings.isItemGrabbedListEmpty)
                    {
                        Plugin.Logger.LogInfo($"Queued Vibrate 0");
                        if (!LovenseSettings.Testing)
                        {
                            LovenseBLETools.GetInstance().SendCommand(LovenseSettings.ToyId, new LovenseCommand() { commandType = LovenseCommandType.VIBRATE, value = 0 });
                        }
                        LovenseSettings.isItemGrabbedListEmpty = true;
                    }
                }
                LovenseSettings.isItemGrabbedTick = 0;
            }
            LovenseSettings.isItemGrabbedTick++;
        }

        //RunQueue();


        if (PlayerController.instance)
        {
            if (PlayerController.instance.playerAvatarScript)
            {
                if (PlayerController.instance.playerAvatarScript.playerHealth)
                {
                    if (LovenseSettings.TestingMode)
                    {
                        var field = AccessTools.Field(typeof(PlayerHealth), "godMode");
                        field.SetValue(PlayerController.instance.playerAvatarScript.playerHealth, true);
                        PlayerController.instance.EnergyCurrent = 100.0f;
                    }
                    else
                    {
                        var field = AccessTools.Field(typeof(PlayerHealth), "godMode");
                        field.SetValue(PlayerController.instance.playerAvatarScript.playerHealth, false);
                    }
                }
            }
        }

        if (Event.current.type == EventType.KeyDown)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.F1:
                    showMenu = !showMenu;
                    break;
                case KeyCode.F2: //This should only ever be used for testing and if you do not actually own a toy (I do not)
                    LovenseSettings.ToyId = "testing toy";
                    LovenseSettings.Testing = true;
                    break;
                default:
                    if (LovenseSettings.DebugKeys)
                    {
                        Debug.Log("Key down: " + Event.current.keyCode);
                    }
                    break;
            }
        }

        if (showMenu)
        {
            Cursor.visible = true;
            //Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
        }

        GUILayout.TextArea($"F1 For Lovense Toy Control - Build {BuildInfo.BuildNumber}");


        if (!showMenu) return;

        windowRect = GUI.Window(123456, windowRect, DrawWindow, $"Lovense Toy Control - Build {BuildInfo.BuildNumber}");
    }

    public void DumpGameObject(string name)
    {
        var go = GameObject.Find(name);
        if (go == null)
        {
            Plugin.Logger.LogInfo($"GameObject '{name}' not found.");
            return;
        }

        Plugin.Logger.LogInfo($"--- Dumping Components of GameObject: {go.name} ---");

        var comps = go.GetComponents<Component>();
        foreach (var comp in comps)
        {
            if (comp == null) continue;

            Type t = comp.GetType();
            Plugin.Logger.LogInfo($"Component: {t.FullName}");

            // Dump all fields + properties (basic)
            foreach (var field in t.GetFields(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance))
            {
                try
                {
                    var val = field.GetValue(comp);
                    Plugin.Logger.LogInfo($"   Field {field.Name} = {val}");
                }
                catch { }
            }

            foreach (var prop in t.GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;
                try
                {
                    var val = prop.GetValue(comp, null);
                    Plugin.Logger.LogInfo($"   Prop {prop.Name} = {val}");
                }
                catch { }
            }
        }
        Plugin.Logger.LogInfo($"--- Done dumping {go.name} ---");
    }

    public bool ShouldShow()
    {
        return !LovenseSettings.RandomizedIntensity;
    }


    public void DrawWindow(int windowID)
    {

        GUILayout.BeginVertical();

        LovenseSettings.DebugKeys = GUILayout.Toggle(LovenseSettings.DebugKeys, "Enable Key Debug");

        LovenseSettings.Enabled = GUILayout.Toggle(LovenseSettings.Enabled, "Enable Lovense Usage");

        // You can react to its state
        if (LovenseSettings.Enabled)
        {
            if (string.IsNullOrEmpty(LovenseSettings.ToyId))
            {
                if (GUILayout.Button("Scan For Toys"))
                {
                    if (!LovenseSettings.Scanning)
                    {
                        LovenseBLETools.GetInstance().StartBLEScan();
                        LovenseSettings.Scanning = true;
                    }
                    else
                    {
                        Plugin.Logger.LogInfo("Already scanning bluetooth devices for Lovense Toys!");
                    }
                }

                if (LovenseSettings.Scanning)
                {
                    if (GUILayout.Button("Stop Scan"))
                    {
                        LovenseBLETools.GetInstance().StopBLEScan();
                    }
                }
            }
            else
            {
                GUILayout.Label($"Toy Connected: {LovenseSettings.ToyId}");

                LovenseSettings.TestingMode = GUILayout.Toggle(LovenseSettings.TestingMode, "Testing Mode");

                if (GUILayout.Button("Add 10 Health"))
                {
                    if (PlayerController.instance)
                    {
                        if (PlayerController.instance.playerAvatarScript)
                        {
                            if (PlayerController.instance.playerAvatarScript.playerHealth)
                            {
                                PlayerController.instance.playerAvatarScript.playerHealth.Heal(10, false);
                            }
                        }
                    }
                }

                if (GUILayout.Button("Find Class Parent Object"))
                {
                    DumpGameObject("Menu Page Settings(Clone)");
                    //var objs = GameObject.FindObjectsOfType<MenuPage>();
                    //foreach (var o in objs)
                    //{
                    //    Debug.Log($"Found {o.GetType().Name} on GameObject: {o.gameObject.name}");
                    //}
                }

                if (GUILayout.Button("Test Menu"))
                {
                    //MenuManager.instance.PagePopUp("Save file limit reached", Color.red, "You can only have 10 save files at a time. Please delete some save files to make room for new ones.", "OK", true);
                }

                LovenseSettings.HardCoreMode = GUILayout.Toggle(LovenseSettings.HardCoreMode, "Hardcore Mode");
                LovenseSettings.RandomizedIntensity = GUILayout.Toggle(LovenseSettings.RandomizedIntensity, "Randomized Intensity");

                LovenseSettings.OnBreakVibrate = GUILayout.Toggle(LovenseSettings.OnBreakVibrate, "OnBreak Vibrate");

                //LovenseSettings.OnHealOthersVibrate = GUILayout.Toggle(LovenseSettings.OnHealOthersVibrate, "Heal Others Vibrate");
                //if (LovenseSettings.OnHealOthersVibrate)
                //{
                //    GUILayout.Label($"Heal Others Intensity {Convert.ToInt32(LovenseSettings.HealOthersIntensity)}");
                //    LovenseSettings.HealOthersIntensity = GUILayout.HorizontalSlider(LovenseSettings.HealOthersIntensity, 0.0f, 20.0f);
                //}

                LovenseSettings.OnPlayerDeathVibrate = GUILayout.Toggle(LovenseSettings.OnPlayerDeathVibrate, "Death Vibrate");
                if (LovenseSettings.OnPlayerDeathVibrate)
                {
                    if (ShouldShow())
                    {
                        GUILayout.Label($"Death Intensity {Convert.ToInt32(LovenseSettings.PlayerDeathIntensity)}");
                        LovenseSettings.PlayerDeathIntensity = GUILayout.HorizontalSlider(LovenseSettings.PlayerDeathIntensity, 0.0f, 20.0f);
                    }
                }

                LovenseSettings.OnGrabStartedVibrate = GUILayout.Toggle(LovenseSettings.OnGrabStartedVibrate, "Grab Start Vibrate");
                if (LovenseSettings.OnGrabStartedVibrate)
                {
                    if (ShouldShow())
                    {
                        GUILayout.Label($"Grab Start Intensity {Convert.ToInt32(LovenseSettings.GrabStartedVibrateIntensity)}");
                        LovenseSettings.GrabStartedVibrateIntensity = GUILayout.HorizontalSlider(LovenseSettings.GrabStartedVibrateIntensity, 0.0f, 20.0f);
                    }
                }

                LovenseSettings.OnCrownGrabVibrate = GUILayout.Toggle(LovenseSettings.OnCrownGrabVibrate, "Crown Grab Vibrate");
                if (LovenseSettings.OnCrownGrabVibrate)
                {
                    if (ShouldShow())
                    {
                        GUILayout.Label($"Crown Grab Intensity {Convert.ToInt32(LovenseSettings.CrownGrabVibrateIntensity)}");
                        LovenseSettings.CrownGrabVibrateIntensity = GUILayout.HorizontalSlider(LovenseSettings.CrownGrabVibrateIntensity, 0.0f, 20.0f);
                    }
                }

                LovenseSettings.AllowChatBoops = GUILayout.Toggle(LovenseSettings.AllowChatBoops, "Chat Boop Vibrate");
                if (LovenseSettings.AllowChatBoops)
                {
                    if (ShouldShow())
                    {
                        GUILayout.Label($"Chat Boop Intensity {Convert.ToInt32(LovenseSettings.AllowChatBoopsIntensity)}");
                        LovenseSettings.AllowChatBoopsIntensity = GUILayout.HorizontalSlider(LovenseSettings.AllowChatBoopsIntensity, 0.0f, 20.0f);
                    }
                }


                GUILayout.Label($"Item Grab Tick {Convert.ToInt32(LovenseSettings.isItemGrabbedTickMax)}");
                LovenseSettings.isItemGrabbedTickMax = GUILayout.HorizontalSlider(LovenseSettings.isItemGrabbedTickMax, 1.0f, 10000.0f);
            }
        }

        #region .. Resizing ..
        // Handle resize area (bottom-right corner)
        Rect resizeHandle = new Rect(windowRect.width - 17, windowRect.height - 17, 15, 15);
        GUI.Box(resizeHandle, ""); // optional visual handle

        // Detect dragging
        if (Event.current.type == EventType.MouseDown && resizeHandle.Contains(Event.current.mousePosition))
        {
            resizing = true;
            resizeStartMouse = Event.current.mousePosition;
            resizeStartSize = new Vector2(windowRect.width, windowRect.height);
            Event.current.Use();
        }
        if (Event.current.type == EventType.MouseUp)
        {
            resizing = false;
        }
        if (resizing)
        {
            Vector2 delta = Event.current.mousePosition - resizeStartMouse;
            windowRect.width = Mathf.Max(100, resizeStartSize.x + delta.x);
            windowRect.height = Mathf.Max(50, resizeStartSize.y + delta.y);
        }
        #endregion

        GUILayout.EndVertical();
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

}

public class LovenseSettings
{
    public static string ToyId = "";
    public static bool Testing;

    public static bool DebugKeys;

    public static bool Enabled;
    public static bool Scanning;

    public static bool TeaseMode;
    public static bool HardCoreMode;
    public static bool SelfControlOnly;

    public static bool TestingMode;

    public static bool RandomizedIntensity;

    //Done
    public static bool OnBreakVibrate; //Automatically adjust intensity

    //Working on
    public static bool OnHealOthersVibrate;
    public static float HealOthersIntensity;

    //Done
    public static bool OnPlayerDeathVibrate;
    public static float PlayerDeathIntensity;

    //Done
    public static bool OnGrabStartedVibrate;
    public static float GrabStartedVibrateIntensity;
    public static bool isItemGrabbed;
    public static int isItemGrabbedTick;
    public static float isItemGrabbedTickMax = 1000;
    public static bool isItemGrabbedListEmpty;

    //Done
    public static bool OnCrownGrabVibrate;
    public static float CrownGrabVibrateIntensity;

    //Done
    public static bool AllowChatBoops;
    public static float AllowChatBoopsIntensity;


}
