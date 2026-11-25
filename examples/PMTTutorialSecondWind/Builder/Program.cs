// Builds PMTTutorialSecondWind.esm with Mutagen and BuildKit (no Creation Kit needed). sf-mod-build runs it as:
//   dotnet run -c Release --project Builder -- <Starfield Data folder> <output folder> <script source folder>
// (the arguments come from builder.args in sfmod.json).
//
// Records (all new, prefix PTSW_; no vanilla record is edited; Starfield.esm is opened for lookups
// and becomes a master because the records point into it):
//   QUST PTSW_Quest              starts with the game, runs PTSW:SecondWindQuestScript
//   MGEF PTSW_RegenEffect        restores Health each second (Value Modifier, Fire and Forget, Self)
//   SPEL PTSW_SecondWindSpell    the spell the script casts on the player: 10 Health a second for 10 seconds
//   MESG PTSW_Msg_SecondWind     the HUD line "Second wind!" (a Message also shows on Xbox and PlayStation)
//   GMOP PTSW_Option_Debug       Settings > Gameplay Options: "Show Debug Messages (PC only)", off by default
//   GPOG PTSW_Options            the mod's own "Second Wind (PMT Tutorial)" group in Gameplay Options
// Vanilla records it only points to (looked up by EditorID, never changed):
//   AVIF Health (0002D4)         the actor value the effect restores and the script reads
//   EQUP EitherHand (013F44)     the equip type vanilla Fire and Forget self spells use
// FormIDs stay the same from build to build through ids.lock.json, next to sfmod.json (commit it).
using BuildKit;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

if (args.Length < 3)
{
    Console.Error.WriteLine("usage: PMTTutorialSecondWind.Builder <Starfield Data folder> <output folder> <script source folder>");
    return 2;
}
var (dataDir, outDir, scriptDir) = (args[0], args[1], args[2]);

// The mod folder is the first folder above the builder's output that holds sfmod.json.
var projectDir = new DirectoryInfo(AppContext.BaseDirectory);
while (projectDir != null && !File.Exists(Path.Combine(projectDir.FullName, "sfmod.json"))) projectDir = projectDir.Parent;
if (projectDir == null)
{
    Console.Error.WriteLine($"PMTTutorialSecondWind.Builder: no sfmod.json found above {AppContext.BaseDirectory}. Fix: keep the Builder folder inside the mod folder.");
    return 2;
}
var questScriptFile = Path.Combine(scriptDir, "PTSW", "SecondWindQuestScript.psc");
if (!File.Exists(questScriptFile))
{
    Console.Error.WriteLine($"PMTTutorialSecondWind.Builder: {questScriptFile} is missing. Fix: pass the script source folder (sfmod.json builder.args \"{{scripts}}\").");
    return 2;
}

using var game = new GameData(dataDir);
game.Open("Starfield.esm");
var mod = Plugin.New("PMTTutorialSecondWind.esm", PluginStyle.Light, "PMT Guide",
    "Second Wind (PMT tutorial): a short burst of health regeneration when your health runs low in combat.");

// ---- vanilla records we point to (GameData.Get stops the build if one is missing) ---------------
var health = game.Get<IActorValueInformationGetter>("Health");
var eitherHand = game.Get<IEquipTypeGetter>("EitherHand");
Console.WriteLine($"vanilla: Health = {health.FormKey}, EitherHand = {eitherHand.FormKey}");

// ---- quest -------------------------------------------------------------------------------------
var quest = Quests.New(mod, "PTSW_Quest");
quest.Name = "Second Wind";
quest.Data = new QuestData { Flags = Quest.Flag.StartGameEnabled | Quest.Flag.StartsEnabled };

// ---- the health regeneration effect and spell -------------------------------------------------
// Shaped like the game's own restore-health effects: a Value Modifier on Health, Fire and Forget, Self.
// For a Value Modifier with a duration, the magnitude is Health per second.
var regen = mod.MagicEffects.AddNew("PTSW_RegenEffect");
regen.Name = "Second Wind";
regen.Description = "Restores <mag> Health/s for <dur>";
regen.Archetype = new MagicEffectArchetype { Type = MagicEffectArchetype.TypeEnum.ValueModifier };
regen.SecondActorValue.SetTo(health);
regen.CastType = CastType.FireAndForget;
regen.TargetType = TargetType.Self;
regen.Flags = MagicEffect.Flag.NoArea;
regen.DualCastingScale = 1;

var spell = mod.Spells.AddNew("PTSW_SecondWindSpell");
spell.Name = "Second Wind";
spell.Type = Spell.SpellType.Spell;
spell.CastType = CastType.FireAndForget;
spell.TargetType = TargetType.Self;
spell.EquipmentType.SetTo(eitherHand);
spell.Effects.Add(Effects.New(regen.ToLink<IMagicEffectGetter>(), magnitude: 10, area: 0, duration: 10));

// ---- the HUD message ----------------------------------------------------------------------------
var msg = Messages.Hud(mod, "PTSW_Msg_SecondWind", "Second wind!", 3);

// ---- gameplay options (options first, group last) ----------------------------------------------
var optDebug = GameplayOptions.Debug(mod, "PTSW_Option_Debug", "Second Wind's log lines on screen");
GameplayOptions.Group(mod, "PTSW_Options", "Second Wind (PMT Tutorial)", [optDebug]);

// ---- the quest's script and its properties (names match the Property lines in the .psc) --------
var questScript = new ScriptEntry { Name = "PTSW:SecondWindQuestScript", Flags = ScriptEntry.Flag.Local };
void AddObject(string name, IFormLinkGetter<IStarfieldMajorRecordGetter> link) =>
    questScript.Properties.Add(new ScriptObjectProperty { Name = name, Object = link.FormKey.ToLink<IStarfieldMajorRecordGetter>() });
AddObject("PTSW_Option_Debug", optDebug.ToLink<IStarfieldMajorRecordGetter>());
AddObject("Health", health.ToLink<IStarfieldMajorRecordGetter>());
AddObject("PTSW_SecondWindSpell", spell.ToLink<IStarfieldMajorRecordGetter>());
AddObject("PTSW_Msg_SecondWind", msg.ToLink<IStarfieldMajorRecordGetter>());
quest.VirtualMachineAdapter = new QuestAdapter { Version = 6, ObjectFormat = 2, Scripts = { questScript } };

// ---- write (ID lock, ONAM like the Creation Kit, EFIF/MUID filled, master flags, one thread) --
Console.WriteLine(IdLock.Apply(mod, Path.Combine(projectDir.FullName, "ids.lock.json")));
var outPath = PluginWriter.Write(mod, outDir, game);
Console.WriteLine($"Wrote {outPath} ({new FileInfo(outPath).Length} bytes): {mod.EnumerateMajorRecords().Count()} records");
return 0;
