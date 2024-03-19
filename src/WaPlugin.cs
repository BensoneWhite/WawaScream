namespace WawaScream;

[BepInPlugin(GUID: MOD_ID, Name: MOD_NAME, Version: VERSION)]
internal class WaPlugin : BaseUnityPlugin
{
    public const string MOD_ID = "wawascream";
    public const string AUTHORS = "BensoneWhite";
    public const string MOD_NAME = "WawaScream";
    public const string VERSION = "1.3.01";

    public bool IsInit;

    private WaOptionsMenu optionsMenuInstance;

    public void OnEnable()
    {
        On.RainWorld.OnModsInit += RainWorld_OnModsInit;
        Debug.LogWarning($"{MOD_NAME} is loading... {VERSION}");
    }

    private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsInit) return;
            IsInit = true;

            WaEnums.Init();

            WaLogic.Apply();

            MachineConnector.SetRegisteredOI(MOD_ID, optionsMenuInstance = new WaOptionsMenu());

        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError(e);
            Debug.Log($"Remix Menu: Hook_OnModsInit options failed init error {optionsMenuInstance}{e}");
            throw new Exception($"Failed to load OnModsInit MOD: {MOD_NAME}, VERSION {VERSION}, try to reload your game");
        }
    }
}
