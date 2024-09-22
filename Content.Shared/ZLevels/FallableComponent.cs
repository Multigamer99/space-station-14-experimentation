using Content.Shared.Gravity;
using Robust.Shared.GameStates;
using Robust.Shared.GameObjects;
using Content.Shared.ZLevel.Fallable.Systems;

namespace Content.Shared.ZLevel.Fallable.Components
{
    /*
    *   Vrell - So this class is where the shit for falling from one tile to another happens.
    *   Do note that this happens on nearly everything in the game, being attached to the base item and base mob definitions.
    *   That doesn't mean it always has an effect, but it is still important to consider.
    */
    [RegisterComponent, NetworkedComponent]
    public sealed partial class FallableComponent : Component
    {
        //Vrell - if set to true, the attached entity will NEVER fall.
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("preventFalls")]
        public bool PreventFalls = false;
    }
}
