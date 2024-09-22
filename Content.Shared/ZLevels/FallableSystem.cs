using System.Numerics;
using Content.Shared.Gravity;
using Content.Shared.ZLevel.Fallable.Components;
using Content.Shared.Maps;
using Robust.Shared.Map.Components;
using Robust.Shared.Map;

namespace Content.Shared.ZLevel.Fallable.Systems;

// Vrell - See FallableComponent.cs
public sealed class FallableSystem : EntitySystem
{
    [Dependency] private readonly SharedGravitySystem _gravity = default!;
    [Dependency] private readonly ITileDefinitionManager _tileDefinitionManager = default!;
    [Dependency] private readonly IMapManager _map = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    private EntityQuery<MapGridComponent> _gridQuery;
    private EntityQuery<MapComponent> _mapQuery;

    public override void Initialize()
    {
        base.Initialize();

        _gridQuery = GetEntityQuery<MapGridComponent>();
        _mapQuery = GetEntityQuery<MapComponent>();

        SubscribeLocalEvent<FallableComponent, MoveEvent>(OnMoveEvent);
    }

    void OnMoveEvent(EntityUid uid, FallableComponent component, ref MoveEvent args)
    {
        if(component.PreventFalls || _gravity.IsWeightless(uid)) return;

        if (!_gridQuery.TryGetComponent(args.Component.GridUid, out var grid)) return;

        if(!((ContentTileDefinition)_tileDefinitionManager[grid.GetTileRef(args.NewPosition).Tile.TypeId]).CanFall) return;

        if(!(args.Component.MapUid is EntityUid trueMapId)) return;
        if(!_entityManager.TryGetComponent<MapComponent>(trueMapId, out var mapComp)) return;
        if(!(mapComp is MapComponent trueMapComp)) return;
        if(trueMapComp.LowerZBound == args.Component.MapID.Value) return;

        var targetMapId = new MapId(args.Component.MapID.Value - 1);
        var position = args.Component.MapPosition.Position;

        _transform.SetMapCoordinates(uid, new MapCoordinates(position, targetMapId));
    }
}
