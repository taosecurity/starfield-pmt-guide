// Builds PMTTutorialCourier.esm with Mutagen and BuildKit (no Creation Kit needed). sf-mod-build runs it as:
//   dotnet run -c Release --project Builder -- <Starfield Data folder> <output folder> <script source folder>
// (the arguments come from builder.args in sfmod.json).
//
// Records (all new, prefix PTCR_; no vanilla record is edited; Starfield.esm is opened for lookups
// and becomes a master because the records point into it):
//   QUST PTCR_Quest            "A Courier's Request": stages 10, 20, 30, objectives 10 and 20, one alias (Player)
//   BOOK PTCR_Note             the courier's note (a copy of a vanilla note's shape, with our own name and text)
//   MESG PTCR_Msg_Done         the HUD line at the end (a Message also shows on Xbox and PlayStation)
//   GMOP PTCR_Option_Debug     Settings > Gameplay Options: "Show Debug Messages (PC only)", off by default
//   GPOG PTCR_Options          the mod's own "A Courier's Request (PMT Tutorial)" group in Gameplay Options
// Vanilla records it only points to (looked up by EditorID, never changed):
//   LCTN CityNewAtlantisLocation (01295A)  where stage 10 starts
//   MISC InorgCommonIron (00556E)          the Iron resource for stage 20
//   NPC_ Player (000007)                   the Player alias's unique actor, as the game's own quests do it
//   KYWD QuestTypeActivities (0475F8)      puts the quest under Activities in the Missions menu
//   BOOK Trait_KidStuff_Backpack_Note      read only, to copy the shape (model, keywords) of a plain note
//
// BuildKit 0.5.0 has Quests.New for the quest shell but no helpers for stages, objectives or aliases
// (PMT gap G08), so those are written below with plain Mutagen calls. sf-mod-check gap runs after the write.
using BuildKit;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

if (args.Length < 3)
{
    Console.Error.WriteLine("usage: PMTTutorialCourier.Builder <Starfield Data folder> <output folder> <script source folder>");
    return 2;
}
var (dataDir, outDir, scriptDir) = (args[0], args[1], args[2]);

// The mod folder is the first folder above the builder's output that holds sfmod.json.
var projectDir = new DirectoryInfo(AppContext.BaseDirectory);
while (projectDir != null && !File.Exists(Path.Combine(projectDir.FullName, "sfmod.json"))) projectDir = projectDir.Parent;
if (projectDir == null)
{
    Console.Error.WriteLine($"PMTTutorialCourier.Builder: no sfmod.json found above {AppContext.BaseDirectory}. Fix: keep the Builder folder inside the mod folder.");
    return 2;
}
foreach (var script in new[] { "CourierQuestScript.psc", "CourierPlayerAliasScript.psc" })
{
    var file = Path.Combine(scriptDir, "PTCR", script);
    if (!File.Exists(file))
    {
        Console.Error.WriteLine($"PMTTutorialCourier.Builder: {file} is missing. Fix: pass the script source folder (sfmod.json builder.args \"{{scripts}}\").");
        return 2;
    }
}

using var game = new GameData(dataDir);
game.Open("Starfield.esm");
var mod = Plugin.New("PMTTutorialCourier.esm", PluginStyle.Light, "PMT Guide",
    "A Courier's Request (PMT tutorial): a small unvoiced quest in New Atlantis.");

// ---- vanilla records we point to (GameData.Get stops the build if one is missing) ---------------
var newAtlantis = game.Get<ILocationGetter>("CityNewAtlantisLocation");
var iron = game.Get<IMiscItemGetter>("InorgCommonIron");
var playerNpc = game.Get<INpcGetter>("Player");
var activities = game.Get<IKeywordGetter>("QuestTypeActivities");
var noteShape = game.Get<IBookGetter>("Trait_KidStuff_Backpack_Note");
Console.WriteLine($"vanilla: CityNewAtlantisLocation = {newAtlantis.FormKey} ({newAtlantis.Name}), " +
                  $"InorgCommonIron = {iron.FormKey} ({iron.Name}), Player = {playerNpc.FormKey}, " +
                  $"QuestTypeActivities = {activities.FormKey}");

// ---- the note -----------------------------------------------------------------------------------
// A new record of our own, copied from a plain vanilla note so it has a model, keywords and sounds;
// the vanilla note itself is not changed and not in our plugin.
var note = (Book)noteShape.Duplicate(mod.GetNextFormKey());
note.EditorID = "PTCR_Note";
note.Name = "A Courier's Request";
note.Text = "To whoever finds this,\n\nI run supplies for a workshop in the Commercial District, and my last " +
            "shipment never arrived. If you can bring together 5 Iron, the workshop will pay you for your trouble." +
            "\n\nThank you,\nA courier";
note.VirtualMachineAdapter = null;
mod.Books.Add(note);

// ---- the HUD message ----------------------------------------------------------------------------
var done = Messages.Hud(mod, "PTCR_Msg_Done", "A Courier's Request: done. The workshop paid you 100 credits.", 4);

// ---- the quest shell (BuildKit) -----------------------------------------------------------------
var quest = Quests.New(mod, "PTCR_Quest");
quest.Name = "A Courier's Request";
quest.Data = new QuestData { Flags = Quest.Flag.StartGameEnabled | Quest.Flag.StartsEnabled | Quest.Flag.RunOnce, Priority = 50 };
quest.QuestType.SetTo(activities);

// ---- stages, objectives and the alias (plain Mutagen: BuildKit has no helpers for these yet) -----
// The last stage's log entry carries the "Complete Quest" flag, as the game's own quests do (checked on
// Starfield.esm: 466 of its quests flag a log entry CompleteQuest, 272 of them on the highest stage, for
// example MQ101 stages 1800 and 2100). The script also calls CompleteQuest() and Stop() at stage 30.
QuestStage Stage(ushort index, string logEntry, bool completesQuest = false) => new QuestStage
{
    Index = index,
    LogEntries = { new QuestLogEntry { Flags = completesQuest ? QuestLogEntry.Flag.CompleteQuest : 0, Entry = logEntry } },
};
quest.Stages.Add(Stage(10, "A courier's note reached me in New Atlantis. Someone needs Iron."));
quest.Stages.Add(Stage(20, "The note asks for 5 Iron for a workshop in the Commercial District."));
quest.Stages.Add(Stage(30, "I collected the Iron, and the workshop paid me for it.", completesQuest: true));

quest.Objectives.Add(new QuestObjective { Index = 10, Flags = 0, DisplayText = "Read the courier's note" });
quest.Objectives.Add(new QuestObjective { Index = 20, Flags = 0, DisplayText = "Collect 5 Iron" });

const int PlayerAliasId = 0;
var playerAlias = new QuestReferenceAlias
{
    ID = PlayerAliasId,
    Name = "Player",
    Flags = 0,
    ALFG = 0,
};
playerAlias.UniqueActor.SetTo(playerNpc);
quest.Aliases.Add(playerAlias);

// ---- gameplay options (options first, group last) ----------------------------------------------
var optDebug = GameplayOptions.Debug(mod, "PTCR_Option_Debug", "A Courier's Request's log lines on screen");
GameplayOptions.Group(mod, "PTCR_Options", "A Courier's Request (PMT Tutorial)", [optDebug]);

// ---- scripts and their properties (names match the Property lines in the .psc files) -----------
static ScriptObjectProperty Prop(string name, FormKey target) =>
    new() { Name = name, Object = target.ToLink<IStarfieldMajorRecordGetter>() };

var questScript = new ScriptEntry { Name = "PTCR:CourierQuestScript", Flags = ScriptEntry.Flag.Local };
questScript.Properties.Add(Prop("PTCR_Option_Debug", optDebug.FormKey));
questScript.Properties.Add(Prop("PTCR_Note", note.FormKey));
questScript.Properties.Add(Prop("InorgCommonIron", iron.FormKey));
questScript.Properties.Add(Prop("PTCR_Msg_Done", done.FormKey));

var aliasScript = new ScriptEntry { Name = "PTCR:CourierPlayerAliasScript", Flags = ScriptEntry.Flag.Local };
aliasScript.Properties.Add(Prop("CityNewAtlantisLocation", newAtlantis.FormKey));
aliasScript.Properties.Add(Prop("InorgCommonIron", iron.FormKey));

// An alias script is stored in the quest's script data: an entry that points at the quest and the alias ID.
quest.VirtualMachineAdapter = new QuestAdapter
{
    Version = 6,
    ObjectFormat = 2,
    Scripts = { questScript },
    Aliases =
    {
        new QuestFragmentAlias
        {
            Property = new ScriptObjectProperty { Name = "", Object = quest.ToLink<IStarfieldMajorRecordGetter>(), Alias = PlayerAliasId },
            Version = 6,
            ObjectFormat = 2,
            Scripts = { aliasScript },
        },
    },
};

// ---- write (ID lock, ONAM like the Creation Kit, master flags, one thread) ---------------------
Console.WriteLine(IdLock.Apply(mod, Path.Combine(projectDir.FullName, "ids.lock.json")));
var outPath = PluginWriter.Write(mod, outDir, game);
Console.WriteLine($"Wrote {outPath} ({new FileInfo(outPath).Length} bytes): {mod.EnumerateMajorRecords().Count()} records");
return 0;
