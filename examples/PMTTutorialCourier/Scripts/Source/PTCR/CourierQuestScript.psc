Scriptname PTCR:CourierQuestScript extends Quest
{A Courier's Request (PMT tutorial). The quest's stages do their work here, in OnStageSet:
  10  the player is in New Atlantis: give the courier's note (a quest item, through the Note alias), show
      objective 10
  20  the note was read: show objective 20 (collect Iron)
  30  the player holds enough Iron: pay the reward, complete and stop the quest, then (after a short wait)
      show the HUD message
The player alias script (PTCR:CourierPlayerAliasScript) watches the player and sets stages 10 and 30.
Log lines start with "[PTCR]". To also see them on screen (PC only):
Settings > Gameplay Options > A Courier's Request (PMT Tutorial) > Show Debug Messages (PC only).}

GameplayOption Property PTCR_Option_Debug Auto Const Mandatory
{The "Show Debug Messages (PC only)" switch, off by default.}
Book Property PTCR_Note Auto Const Mandatory
{The mod's own note, "A Courier's Request".}
MiscObject Property InorgCommonIron Auto Const Mandatory
{The game's Iron resource.}
Message Property PTCR_Msg_Done Auto Const Mandatory
{The mod's own HUD message at the end. A Message also shows on Xbox and PlayStation.}
ReferenceAlias Property Alias_Note Auto Const Mandatory
{The quest's Note alias. Its Quest Item flag makes the note a quest item while the alias holds it, so it can't be
dropped, sold or stored before it is read.}

Int Property IronNeeded = 5 Auto Const
Int Property RewardCredits = 100 Auto Const
Float Property DoneMessageDelay = 3.0 Auto Const
{Seconds to wait before the final HUD line, so the pickup notices are gone.}

ObjectReference NoteRef

Event OnQuestInit()
	Log("started; waiting for the player to reach New Atlantis")
EndEvent

Event OnStageSet(int auiStageID, int auiItemID)
	Actor player = Game.GetPlayer()
	If auiStageID == 10
		; Place the note as a reference (disabled, so it never shows in the world), keep it in a variable so it
		; stays a persistent reference, put it in the player's inventory, and listen for it being read.
		NoteRef = player.PlaceAtMe(PTCR_Note, 1, true, true)
		player.AddItem(NoteRef, 1, false)
		; Put the note in the Note alias. The alias's Quest Item flag makes it a quest item, so the player can't
		; drop, sell or store it (a dropped copy would stay disabled and couldn't be read).
		Alias_Note.ForceRefTo(NoteRef)
		Log("stage 10: Note alias filled with " + Alias_Note.GetReference() + " (quest item)")
		RegisterForRemoteEvent(NoteRef, "OnRead")
		SetObjectiveDisplayed(10)
		Log("stage 10: note given (" + NoteRef + "), objective 10 shown")
	ElseIf auiStageID == 20
		SetObjectiveCompleted(10)
		SetObjectiveDisplayed(20)
		Log("stage 20: note read, objective 20 shown; the player has " + player.GetItemCount(InorgCommonIron) + " Iron")
		CheckIron()
	ElseIf auiStageID == 30
		SetObjectiveCompleted(20)
		player.AddItem(Game.GetCredits(), RewardCredits)
		Log("stage 30: " + RewardCredits + " credits paid, quest complete")
		CompleteQuest()
		; Empty the Note alias, so the note is a normal item again (stopping the quest clears its aliases too;
		; this makes it explicit and logs it).
		Alias_Note.Clear()
		Log("stage 30: Note alias cleared; the note is a normal item again")
		; Stop the quest too, as the game's own finished quests are stopped. Without Stop() the quest stays
		; "running". (The Missions menu never lists a finished Activity under Completed, so check the save.)
		Stop()
		Log("stage 30: quest stopped, HUD line in " + DoneMessageDelay + " seconds")
		; Wait a few seconds, so the Iron and credits pickup notices have cleared before the HUD line shows.
		Utility.Wait(DoneMessageDelay)
		PTCR_Msg_Done.Show()
		Log("stage 30: HUD line shown")
	EndIf
EndEvent

Event ObjectReference.OnRead(ObjectReference akSender)
	Log("OnRead from " + akSender)
	If akSender == NoteRef && GetStageDone(10) && !GetStageDone(20)
		SetStage(20)
	EndIf
EndEvent

; Called by the player alias script when the player arrives in New Atlantis.
Function PlayerArrived()
	If !GetStageDone(10)
		Log("the player is in New Atlantis")
		SetStage(10)
	EndIf
EndFunction

; Called after stage 20 starts and whenever the player picks up Iron.
Function CheckIron()
	If GetStageDone(20) && !GetStageDone(30)
		Int count = Game.GetPlayer().GetItemCount(InorgCommonIron)
		Log("Iron check: " + count + " of " + IronNeeded)
		If count >= IronNeeded
			SetStage(30)
		EndIf
	EndIf
EndFunction

; Every line goes to the Papyrus log (when Papyrus logging is on). With the debug option on, it also shows on
; screen. The Debug script does nothing on Xbox and PlayStation: text the player must see is a Message.
Function Log(String asText)
	Debug.Trace("[PTCR] " + asText)
	If PTCR_Option_Debug.GetValue() != 0.0
		Debug.Notification("[PTCR] " + asText)
	EndIf
EndFunction
