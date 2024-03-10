namespace WawaScream;

public static class WaEnums
{
    public static void Init()
    {
        RuntimeHelpers.RunClassConstructor(typeof(Sound).TypeHandle);
    }

    public static void Unregister()
    {
        WaUtils.UnregisterEnums(typeof(Sound));
    }

    public static class Sound
    {
        public readonly static SoundID scream = new(nameof(scream), true);
        public readonly static SoundID boing = new(nameof(boing), true);
        public readonly static SoundID brummm = new(nameof(brummm), true);
    }
}