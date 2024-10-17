using System;
using Content.Shared.Gravity;
using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;
using Content.Shared.ZLevel.ZClimbable.Systems;

namespace Content.Shared.ZLevel.ZClimbable.Components
{
    /*
    *   Vrell - So this class is where the shit for climbing something onto the next z level.
    */
    [RegisterComponent, NetworkedComponent]
    public sealed partial class ZClimbableComponent : Component
    {
        //Vrell - if set to false, the attached entity will not be climbable.
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("canClimb")]
        public bool CanClimb = true;

        //Vrell - if set to false, the attached entity will not need open space above to be climbed. Useful for ladders.
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("needsSpaceAbove")]
        public bool NeedsSpaceAbove = true;

        //Vrell - # of seconds needed to climb a structure
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("climbTime")]
        public float ClimbTime = 4;
    }


}
