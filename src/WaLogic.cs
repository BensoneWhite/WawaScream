using MoreSlugcats;

namespace WawaScream;

public class WaLogic
{
    private static readonly ConditionalWeakTable<Player, ChunkDynamicSoundLoop> SoundsLoops = new();
    private static readonly ConditionalWeakTable<AbstractCreature, WaTimer> Timers = new();

    private static bool added;

    public static WorldCoordinate playerCoord;
    public static Player playerCreature;

    public static void Apply()
    {
        On.Player.ctor += Player_ctor;
        On.Player.Update += Player_Update;
        On.ArtificialIntelligence.Update += ArtificialIntelligence_Update;
        On.AbstractCreatureAI.AbstractBehavior += AbstractCreatureAI_AbstractBehavior;
    }

    private static void ArtificialIntelligence_Update(On.ArtificialIntelligence.orig_Update orig, ArtificialIntelligence self)
    {
        orig(self);
        if (!Timers.TryGetValue(self.creature, out WaTimer timer))
        {
            timer = new WaTimer();
            Timers.Add(self.creature, timer);
        }

        foreach (AbstractCreature player in self.creature.world.game.Players)
        {
            if (Input.GetKey(WaOptionsMenu.Scream.Value) && self != null && self.tracker != null && player.realizedCreature != null && !player.realizedCreature.dead && (timer.timerCooldown <= 0 || timer.timerCooldown <= 810) && self is not CicadaAI && self is not SnailAI && self is not YeekAI && !(WaOptionsMenu.Hearing.Value == "6. Precise"))
            {
                self.tracker.SeeCreature(player);
                timer.timerCooldown = 1080;
            }

            if (UnityEngine.Random.value > 0.5f && timer.timerCooldown != 0 && !(WaOptionsMenu.Hearing.Value == "6. Precise")) self.tracker.SeeCreature(player);

            if (Input.GetKey(WaOptionsMenu.Scream.Value) && self != null && self.tracker != null && player.realizedCreature != null && !player.realizedCreature.dead && (timer.timerCooldown <= 0 || timer.timerCooldown <= 810) && WaOptionsMenu.Hearing.Value == "6. Precise")
            {
                timer.timerCooldown = 1080;
            }

            if (self.tracker != null && self.creature.world.game.session != null && playerCreature != null && WaOptionsMenu.Hearing.Value == "6. Precise")
            {
                Tracker.CreatureRepresentation creatureRepresentation = self.tracker.RepresentationForObject(playerCreature, false);

                if (creatureRepresentation == null) self.tracker.SeeCreature(playerCreature.abstractCreature);
            }
        }

        if (timer.timerCooldown > 0) timer.timerCooldown--;
    }

    private static void AbstractCreatureAI_AbstractBehavior(On.AbstractCreatureAI.orig_AbstractBehavior orig, AbstractCreatureAI self, int time)
    {
        orig(self, time);

        if (!Timers.TryGetValue(self.parent, out WaTimer timer))
        {
            timer = new WaTimer();
            Timers.Add(self.parent, timer);
        }

        if (Input.GetKey(WaOptionsMenu.Scream.Value))
        {
            timer.timerCooldown = 12000;
        }

        if (timer.timerCooldown > 0) timer.timerCooldown--;

        Debug.LogWarning(timer.timerCooldown);

        if (self.parent.Room.realizedRoom != null && !(WaOptionsMenu.Hearing.Value == "6. Precise")) return;

        _ = playerCoord;

        AbstractRoom abstractRoom = self.world.GetAbstractRoom(playerCoord);
        if (abstractRoom == null) return;

        if (playerCoord.NodeDefined && self.parent.creatureTemplate.mappedNodeTypes[(int)abstractRoom.nodes[playerCoord.abstractNode].type] && timer.timerCooldown != 0)
        {
            self.SetDestination(playerCoord);
            return;
        }

        List<WorldCoordinate> availableCoords = new();
        for (int i = 0; i < abstractRoom.nodes.Length; i++)
        {
            if (self.parent.creatureTemplate.mappedNodeTypes[(int)abstractRoom.nodes[i].type] && timer.timerCooldown != 0) availableCoords.Add(new WorldCoordinate(playerCoord.room, -1, -1, i));
        }

        if (availableCoords.Count > 0 && timer.timerCooldown != 0) self.SetDestination(availableCoords[UnityEngine.Random.Range(0, availableCoords.Count)]);

        
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (self == null || self.room == null || self.room.world == null || self.room.world.game.session == null)
            return;

        if (self.room == null || self.room.world == null || self.room.world.game.session == null)
            added = true;

        if (added && !SoundsLoops.TryGetValue(self, out _))
        {
            var soundLoop = new ChunkDynamicSoundLoop(self.bodyChunks[0])
            {
                sound = WaOptionsMenu.ScreamSound.Value switch
                {
                    "1. Wawa" => WaEnums.Sound.scream,
                    "2. Boing" => WaEnums.Sound.boing,
                    "3. BRUMMMM" => WaEnums.Sound.brummm,
                    _ => WaEnums.Sound.scream
                },
                Volume = 0f
            };
            SoundsLoops.Add(self, soundLoop);
            added = false;
        }

        if (SoundsLoops.TryGetValue(self, out var sound) && self.room != null)
        {
            if (Input.GetKey(WaOptionsMenu.Scream.Value))
            {
                if (!self.room.game.GamePaused && !self.dead && sound.Volume != 0.4f)
                    sound.Volume = Custom.LerpAndTick(sound.Volume, 0.4f, 0.01f, 0.025f);
                sound.Update();
            }

            if (!Input.GetKey(WaOptionsMenu.Scream.Value) && WaOptionsMenu.FadeOut.Value == "1. No fade out")
                sound.Volume = 0f;

            if (!Input.GetKey(WaOptionsMenu.Scream.Value) && sound.Volume != 0f && !(WaOptionsMenu.FadeOut.Value == "1. No fade out"))
            {
                var lerp = WaOptionsMenu.FadeOut.Value switch
                {
                    "1. No Fade out" => 1f,
                    "2. Normal" => 0.01f,
                    "3. Long" => 0.001f,
                    "4. Fast" => 0.1f,
                    _ => 0.01f
                };

                if (!self.room.game.GamePaused && !self.dead)
                    sound.Volume = Custom.LerpAndTick(sound.Volume, 0f, lerp, 0.005f);
                sound.Update();
            }
        }

        if (self.room != null)
        {
            playerCreature = self;
            playerCoord = self.coord;
        }
    }

    private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);
        if (!SoundsLoops.TryGetValue(self, out _))
        {
            var soundLoop = new ChunkDynamicSoundLoop(self.bodyChunks[0])
            {
                sound = WaOptionsMenu.ScreamSound.Value switch
                {
                    "1. Wawa" => WaEnums.Sound.scream,
                    "2. Boing" => WaEnums.Sound.boing,
                    "3. BRUMMMM" => WaEnums.Sound.brummm,
                    _ => WaEnums.Sound.scream
                },
                Volume = 0f
            };
            SoundsLoops.Add(self, soundLoop);
        }
    }
}