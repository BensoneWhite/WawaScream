namespace WawaScream;

public class WaOptionsMenu : OptionInterface
{
    private OpCheckBox LizardsBox;
    private OpLabel LizardsLabel;

    private OpCheckBox CustomOptionsBox;
    private OpLabel CustomOptionsLabel;

    private OpListBox HearingList;
    private OpLabel HearingLabel;

    private OpListBox FadeOutList;
    private OpLabel FadeOutLabel;

    private OpListBox ScreamSoundList;
    private OpLabel ScreamSoundLabel;

    private OpKeyBinder ScreamBind;
    private OpLabel ScreamBindLabel;

    public static Configurable<KeyCode> Scream;

    public static Configurable<bool> Lizards;

    public static Configurable<bool> CustomOptions;

    public static Configurable<string> Hearing;

    public static Configurable<string> FadeOut;

    public static Configurable<string> ScreamSound;

    public WaOptionsMenu()
    {
        Scream = config.Bind<KeyCode>("scream", 0);
        Lizards = config.Bind("Lizards", false, new ConfigAcceptableList<bool>(false, true));
        CustomOptions = config.Bind("CustomOptions", false, new ConfigAcceptableList<bool>(false, true));
        Hearing = config.Bind("Hearing", "4. Normal");
        FadeOut = config.Bind("FadeOut", "2. Normal");
        ScreamSound = config.Bind("ScreamSound", "1. Wawa");
    }

    public override void Update()
    {
        base.Update();

        Color colorOff = new(0.1451f, 0.1412f, 0.1529f);
        Color colorOn = new(0.6627f, 0.6431f, 0.698f);

        LizardsBox.greyedOut = true;
        LizardsLabel.color = colorOff;

        CustomOptionsBox.greyedOut = true;
        CustomOptionsLabel.color = colorOff;
    
        HearingList.greyedOut = false;
        HearingLabel.color = colorOn;

        FadeOutList.greyedOut = false;
        FadeOutLabel.color = colorOn;

        ScreamSoundList.greyedOut = false;
        ScreamSoundLabel.color = colorOn;

        ScreamBind.greyedOut = false;
        ScreamBindLabel.color = colorOn;
    }

    public override void Initialize()
    {
        OpKeyBinder.BindController controllerNumber;
        controllerNumber = OpKeyBinder.BindController.AnyController;

        var opTab1 = new OpTab(this, "Options");
        var opTab2 = new OpTab(this, "Aggresiveness");
        var opTab3 = new OpTab(this, "Custom");

        Tabs = new[] { opTab1, opTab2, opTab3 };

        OpContainer tab1Container = new(new Vector2(0, 0));
        opTab1.AddItems(tab1Container);

        UIelement[] UIArrayElements1 = new UIelement[]
        {
            new OpLabel(0f, 580f, "Options", bigText: true),

            ScreamBind = new(Scream, new Vector2(10f, 530f), new Vector2(150f, 10f), false, controllerNumber),
            ScreamBindLabel = new(170f, 535f, "keybind for WAWA"),

            LizardsBox = new(Lizards, 10f, 490f) { description = Translate("Enables more agressiveness for some creatures when doing the WAWA") },
            LizardsLabel = new(45f, 490f, Translate("Plus Aggresiveness")),

            CustomOptionsBox = new(CustomOptions, 10f, 450f) { description = Translate("Enables the Custom Options WIP") },
            CustomOptionsLabel = new(45f, 450f, Translate("Custom Options")),

            FadeOutList = new(FadeOut, new Vector2(360f, 415f), 100f, new List<ListItem>{ new("1. No Fade out"), new("2. Normal"), new("3. Long"), new("4. Fast") }),
            FadeOutLabel = new(380f, 530f, Translate("FadeOut")),

            HearingList = new(Hearing, new Vector2(475f, 395f), 100f, new List<ListItem> { new("1. Disabled"), new("2. Deaf"), new("3. Bad"), new("4. Normal"), new("5. Good"), new("6. Precise") }) { description = Translate("Creature's listening level when shouting") },
            HearingLabel = new(475f, 530f, Translate("Creatures hearing")),

            ScreamSoundList = new(ScreamSound, new Vector2(475f, 275f), 100f, new List<ListItem> { new("1. Wawa"), new("2. Boing"), new("3. BRUMMMM") }) { description = Translate("Choose your scream!!") },
            ScreamSoundLabel = new(485f, 370f, Translate("Scream Sound")),
        };
        opTab1.AddItems(UIArrayElements1);

        UIelement[] UIArrayElements2 = new UIelement[]
        {
            new OpLabel(0f, 580f, "Aggresiveness", bigText: true),
        };
        opTab2.AddItems(UIArrayElements2);

        UIelement[] UIArrayElements3 = new UIelement[]
        {
            new OpLabel(0f, 580f, "Custom", bigText: true)
        };
        opTab3.AddItems(UIArrayElements3);
    }
}