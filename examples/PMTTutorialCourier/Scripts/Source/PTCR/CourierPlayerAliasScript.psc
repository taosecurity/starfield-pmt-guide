Scriptname PTCR:CourierPlayerAliasScript extends ReferenceAlias
{On the quest's one alias, Player. It watches where the player goes and what the player picks up, and tells
the quest script (PTCR:CourierQuestScript), which sets the stages.}

Location Property CityNewAtlantisLocation Auto Const Mandatory
{The game's New Atlantis location. Its districts (The Well, the Lodge, the embassies) are its children.}
MiscObject Property InorgCommonIron Auto Const Mandatory
{The game's Iron resource.}

Event OnAliasInit()
	; Only Iron pickups reach OnItemAdded.
	AddInventoryEventFilter(InorgCommonIron)
	CheckWhere(Game.GetPlayer().GetCurrentLocation())
EndEvent

Event OnLocationChange(Location akOldLoc, Location akNewLoc)
	CheckWhere(akNewLoc)
EndEvent

Event OnItemAdded(Form akBaseItem, int aiItemCount, ObjectReference akItemReference, ObjectReference akSourceContainer, int aiTransferReason)
	(GetOwningQuest() as PTCR:CourierQuestScript).CheckIron()
EndEvent

Function CheckWhere(Location akLoc)
	If akLoc && (akLoc == CityNewAtlantisLocation || akLoc.IsChild(CityNewAtlantisLocation))
		(GetOwningQuest() as PTCR:CourierQuestScript).PlayerArrived()
	EndIf
EndFunction
