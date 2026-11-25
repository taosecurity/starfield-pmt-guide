Scriptname PTSW:SecondWindQuestScript extends Quest
{Second Wind: when the player's health drops below 25 percent in combat, the player regenerates health for
10 seconds and sees "Second wind!". It can happen once every 120 seconds.
Log lines start with "[PTSW]". To also see them on screen (PC only):
Settings > Gameplay Options > Second Wind (PMT Tutorial) > Show Debug Messages (PC only).}

GameplayOption Property PTSW_Option_Debug Auto Const Mandatory
{The "Show Debug Messages (PC only)" switch, off by default.}
ActorValue Property Health Auto Const Mandatory
{The game's Health actor value.}
Spell Property PTSW_SecondWindSpell Auto Const Mandatory
{The mod's own spell: restores health over 10 seconds.}
Message Property PTSW_Msg_SecondWind Auto Const Mandatory
{The mod's own HUD message, "Second wind!". A Message also shows on Xbox and PlayStation.}

Float Property ThresholdPercent = 0.25 Auto Const
{Health fraction below which the second wind starts.}
Float Property CooldownSeconds = 120.0 Auto Const
{Seconds before the second wind can start again.}

Int CooldownTimerID = 1
Bool bCoolingDown = false

Event OnQuestInit()
	Log("started; listening for hits on the player")
	ListenForHits()
EndEvent

; A hit event registration ends after one hit, so the script registers again after each hit.
Function ListenForHits()
	RegisterForHitEvent(Game.GetPlayer())
EndFunction

Event OnHit(ObjectReference akTarget, ObjectReference akAggressor, Form akSource, Projectile akProjectile, bool abPowerAttack, bool abSneakAttack, bool abBashAttack, bool abHitBlocked, string asMaterialName)
	Actor player = Game.GetPlayer()
	If !bCoolingDown && player.IsInCombat()
		Float healthLeft = player.GetValuePercentage(Health)
		If healthLeft < ThresholdPercent
			bCoolingDown = true
			Log("health at " + healthLeft + ", second wind starts; cooldown " + CooldownSeconds + " s")
			PTSW_Msg_SecondWind.Show()
			PTSW_SecondWindSpell.Cast(player, player)
			StartTimer(CooldownSeconds, CooldownTimerID)
		EndIf
	EndIf
	ListenForHits()
EndEvent

Event OnTimer(int aiTimerID)
	If aiTimerID == CooldownTimerID
		bCoolingDown = false
		Log("cooldown over; second wind is ready again")
	EndIf
EndEvent

; Every line goes to the Papyrus log (when Papyrus logging is on). With the debug option on, it also shows on
; screen. The Debug script does nothing on Xbox and PlayStation: text the player must see is the Message above.
Function Log(String asText)
	Debug.Trace("[PTSW] " + asText)
	If PTSW_Option_Debug.GetValue() != 0.0
		Debug.Notification("[PTSW] " + asText)
	EndIf
EndFunction
