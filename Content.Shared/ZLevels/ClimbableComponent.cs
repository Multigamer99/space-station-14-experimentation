using System;
using Content.Shared.Gravity;
using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;
using Content.Shared.ZLevel.ZClimbable.Systems;

namespace Content.Shared.ZLevel.ZClimbable.Components
{
    /*
    *   Vrell - So this class is where the shit for climbing an entity onto the next z level.
    */
    [RegisterComponent, NetworkedComponent]
    public sealed partial class ZClimbableComponent : Component
    {
        //Vrell - if set to true, the attached entity will not be climbable.
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("canClimb")]
        public bool CanClimb = true;

        //Vrell - # of seconds needed to climb a structure
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("climbTime")]
        public float ClimbTime = 4;
    }


}
