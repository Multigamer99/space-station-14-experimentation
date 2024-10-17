using System.Numerics;
using Content.Shared.Gravity;
using Content.Shared.ZLevel.ZClimbable.Components;
using Content.Shared.Interaction;
using Content.Shared.DoAfter;
using Content.Shared.Maps;
using Robust.Shared.Map.Components;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.ZLevel.ZClimbable.Systems;

// Vrell - See ClimbableComponent.cs
public sealed partial class ZClimbableSystem : EntitySystem
{
    [Dependency] private readonly SharedGravitySystem _gravity = default!;
    [Dependency] private readonly ITileDefinitionManager _tileDefinitionManager = default!;
    [Dependency] private readonly IMapManager _map = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;

    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;

    private EntityQuery<MapGridComponent> _gridQuery;
    private EntityQuery<MapComponent> _mapQuery;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ZClimbableComponent, InteractHandEvent>(TryClimb);
        SubscribeLocalEvent<ZClimbableComponent, AddClimbDoAfterEvent>(DoClimb);
    }

    private void TryClimb(EntityUid uid, ZClimbableComponent comp, InteractHandEvent args)
    {
        if (args.Handled || !HasComp<ZClimbableComponent>(args.Target))
            return;

        if(!comp.CanClimb) return;

        //Checking if we can climb
        if (!TryComp<TransformComponent>(args.User, out var xform))
            return;
        if(!(xform.MapUid is EntityUid trueMapId)) return;
        if(!_entityManager.TryGetComponent<MapComponent>(trueMapId, out var mapComp)) return;
        if(!(mapComp is MapComponent trueMapComp)) return;
        if(trueMapComp.UpperZBound <= xform.MapID.Value) return;

        if(comp.NeedsSpaceAbove) {
            var overMapID = new MapId(xform.MapID.Value + 1);
            var overPlayerPos = new MapCoordinates(xform.MapPosition.Position, overMapID);
            if(!_mapSystem.TryGetMap(overMapID, out var mapUid)) return;
            if(!_entityManager.TryGetComponent<MapGridComponent>(mapUid, out var gridComp)) return;
            if(!(gridComp is MapGridComponent trueGridComp)) return;
            if(!((ContentTileDefinition)_tileDefinitionManager[trueGridComp.GetTileRef(overPlayerPos).Tile.TypeId]).CanFall) return;
        }



        var doAfterEventArgs = new DoAfterArgs(EntityManager, args.User, comp.ClimbTime, new AddClimbDoAfterEvent(), args.Target, args.Target)
        {
            BreakOnMove = true,
            BreakOnWeightlessMove = true,
            BreakOnDamage = false,
            NeedHand = true,
            DistanceThreshold = 0.5f
        };

        _doAfter.TryStartDoAfter(doAfterEventArgs);
        args.Handled = true;
    }

    private void DoClimb(EntityUid uid, ZClimbableComponent comp, AddClimbDoAfterEvent args)
    {
        if (!args.DoAfter.Completed) return;

        var user = args.Args.User;

        if (!TryComp<ZClimbableComponent>(args.Args.Target, out var climbable))
            return;

        if (!TryComp<TransformComponent>(args.Args.Target, out var xform)) return;
        if(!(xform.MapUid is EntityUid trueMapId)) return;
        if(!_entityManager.TryGetComponent<MapComponent>(trueMapId, out var mapComp)) return;
        if(!(mapComp is MapComponent trueMapComp)) return;
        if(trueMapComp.UpperZBound <= xform.MapID.Value) return;

        var targetMapID = xform.MapID.Value + 1;

        _transform.SetMapCoordinates(user, new MapCoordinates(xform.MapPosition.Position, new MapId(targetMapID)));
    }

    [Serializable, NetSerializable]
    private sealed partial class AddClimbDoAfterEvent : SimpleDoAfterEvent
    {
    }
}
