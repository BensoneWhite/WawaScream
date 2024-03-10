namespace WawaScream;

public static class WaUtils
{
    public static void UnregisterEnums(Type type)
    {
        IEnumerable<FieldInfo> extEnums = type.GetFields(BindingFlags.Static | BindingFlags.Public).Where(x => x.FieldType.IsSubclassOf(typeof(ExtEnumBase)));
        foreach (var (extEnum, obj) in from FieldInfo extEnum in extEnums
                                       let obj = extEnum.GetValue(null)
                                       where obj != null
                                       select (extEnum, obj))
        {
            _ = obj.GetType().GetMethod("Unregister")!.Invoke(obj, null);
            extEnum.SetValue(null, null);
        }
    }
}