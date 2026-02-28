FeatureScript 2837;
import(path : "onshape/std/common.fs", version : "2837.0");
import(path : "72008d674dc91a72981a4d68", version : "e8ba9d5709d4fe2183d6e9c8");

// export function CreatePlane(cid is CId, pln is Plane) returns Query
// {
//     const opId = nextId(cid);
//     opPlane(cid.ctx, opId, {
//             "plane" : pln
//     });
//     return qCreatedBy(opId);
// }

export function Delete(context is Context, operationId is Id, entities is Query)
{
    if (context->isQueryEmpty(entities))
        return;

    opDeleteBodies(context, operationId, {
            "entities" : entities
    });
}

// export function Enclose(cid is CId, entities is Query) returns Query
// precondition !isQueryEmpty(cid.ctx, entities);
// {
//     const opId = nextId(cid);
//     opEnclose(cid.ctx, opId, {
//             "entities" : entities
//     });
//     return qCreatedBy(opId);
// }

// =============================================================================================================================
//  EXTRUDE
//  =============================================================================================================================

export function Extrude(
    context is Context,
    operationId is Id,
    entities is Query,
    direction is Vector,
    frontDepth is ValueWithUnits,
    backDepth is ValueWithUnits
    ) returns Query
precondition
{
    is3dDirection(direction);
    isLength(frontDepth);
    isLength(backDepth);
}
{
    if (isQueryEmpty(context, entities)) 
        return qNothing();

    opExtrude(context, operationId, {
                "entities" : entities,
                "direction" : direction,
                "endBound" : BoundingType.BLIND,
                "endDepth" : frontDepth,
                "startBound" : BoundingType.BLIND,
                "startDepth" : backDepth
            });

    return qCreatedBy(operationId);
}

/** Extrude the input faces and/or edges along the specified direction.*/
export function Extrude(context is Context, operationId is Id, entities is Query, direction is Vector, frontDepth is ValueWithUnits) returns Query
{
    return Extrude(context, operationId, entities, direction, frontDepth, 0*millimeter);
}

// export function ExtrudeToSurface(context is Context, operationid is Id, forwardSurface is Query, reverseSurface is Query, extrudeDir is Vector, entities is Query) returns Query
// precondition is3dDirection(extrudeDir);
// {
//     if (isQueryEmpty(context, entities))
//         return qNothing();

//     var extrudeDef = {
//             "entities" : entities,
//             "direction" : extrudeDir,
//             "endBound" : BoundingType.UP_TO_SURFACE,
//             "endBoundEntity" : forwardSurface
//         };
//     if (!isQueryEmpty(cid.ctx, reverseSurface))
//     {
//         extrudeDef.startBound = BoundingType.;
//         extrudeDef.startBoundEntity = reverseSurface;
//     }
//     opExtrude(cid.ctx, opId, extrudeDef);
//     return qCreatedBy(opId);
// }

export function Fill(context is Context, operationId is Id, edges is Query) returns Query
{
    if (isQueryEmpty(context, edges))
        return qNothing();

    opFillSurface(context, operationId, {
                "edgesG0" : edges
            });

    return qCreatedBy(operationId);
}

export function Fillet(context is Context, operationId is Id, edges is Query, radius is ValueWithUnits, allowEdgeOverflow is boolean)
precondition isLength(radius, NONNEGATIVE_ZERO_INCLUSIVE_LENGTH_BOUNDS);
{
    if (isQueryEmpty(context, edges))
        return qNothing();

    opFillet(context, operationId, {
            "entities" : edges,
            "radius" : radius,
            "allowEdgeOverflow" : allowEdgeOverflow
    });

    return qCreatedBy(operationId);
}

/** Create helix oriented along zAxis of input coords.*/
export function Helix(context is Context, operationId is Id, rightHanded is boolean, baseRadius is ValueWithUnits, helicalPitch is ValueWithUnits, helixAngle is ValueWithUnits, rangeStart is number, rangeEnd is number, coords is CoordSystem) returns Query
precondition
{
    isLength(baseRadius, NONNEGATIVE_LENGTH_BOUNDS);
    isLength(helicalPitch, NONNEGATIVE_LENGTH_BOUNDS);
    rangeStart < rangeEnd;
}
{
    opHelix(context, operationId, {
            "direction" : coords.zAxis,
            "axisStart" : coords.origin,
            "startPoint" : coords.origin + coords.xAxis * baseRadius,
            "interval" : [rangeStart, rangeEnd],
            "clockwise" : rightHanded,
            "helicalPitch" : helicalPitch,
            "spiralPitch" : tolerantEquals(helixAngle, 0*degree) ? 0*millimeter : helicalPitch * tan(helixAngle)
    });

    return qCreatedBy(operationId);
}

export function Intersect(context is Context, operationId is Id, entities is Query, keepTools is boolean)
{
    if (isQueryEmpty(context, entities))
        return qNothing();

    opBoolean(context, operationId, {
            "tools" : entities,
            "operationType" : BooleanOperationType.INTERSECTION,
            "keepTools" : keepTools
    });

    return qCreatedBy(operationId);
}

export function IsoclineCurve(context is Context, operationId is Id, targetFace is Query, direction is Vector, angle is ValueWithUnits)
{
    opCreateIsocline(context, operationId, {
            "faces" : targetFace,
            "direction" : direction,
            "angle" : angle
    });
    
    return qCreatedBy(operationId);
}

export function IntersectionCurve(context is Context, operationId is Id, tools is Query, targets is Query) returns Query
{
    if (context->isQueryEmpty(tools) || context->isQueryEmpty(targets))
        return qNothing();
    
    opIntersectFaces(context, operationId, {
            "tools" : tools,
            "targets" : targets
    });
    
    return qCreatedBy(operationId);
}

export function Loft(context is Context, operationId is Id, profiles is array, bodyType is ToolBodyType) returns Query
precondition size(profiles) > 1;
{
    
    opLoft(context, operationId, {
                "profileSubqueries" : profiles,
                "bodyType" : bodyType
            });
 
    return qCreatedBy(operationId);
}

export function MateConnector(context is Context, operationId is Id, owner is Query, coords is CoordSystem) returns Query
{
    opMateConnector(context, operationId, {
            "coordSystem" : coords,
            "owner" : owner
    });
    return qCreatedBy(operationId);
}

export function Mirror(context is Context, operationId is Id, entities is Query, pln is Plane) returns Query
{
    if (isQueryEmpty(context, entities))
        return qNothing();

    opPattern(context, operationId, {
                "entities" : entities,
                "transforms" : [mirrorAcross(pln)],
                "instanceNames" : ["0"]
            });

    return qCreatedBy(operationId);
}

export function OffsetFace(context is Context, operationId is Id, faces is Query, distance is ValueWithUnits)
{
    if (isQueryEmpty(context, faces))
        return;

    opOffsetFace(context, operationId, {
            "moveFaces" : faces,
            "offsetDistance" : distance
    });
}

/** Returned array of instances does not include original entity.*/
export function Pattern(context is Context, operationId is Id, entities is Query, transformArray is array) returns array
precondition
{
    size(transformArray) > 0;
}
{
    if (isQueryEmpty(context, entities))
        return [];

    var names = [];
    for (var i = 0; i < size(transformArray); i += 1)
        names = append(names, toString(i));

    opPattern(context, operationId, {
                "entities" : entities,
                "transforms" : transformArray,
                "instanceNames" : names
            });

    var instances = [];
    for (var i = 0; i < size(transformArray); i += 1)
        instances = instances->append(qPatternInstances(operationId, names[i], EntityType.BODY));

    return instances;
}

export function PatternCircular(context is Context, operationId is Id, n is number, axis is Line, entities is Query) returns array
precondition
{
    isPositiveInteger(n);
    !isQueryEmpty(context, entities);
}
{
    if (isQueryEmpty(context, entities))
        return [];
    if (n == 1)
        return [entities];

    var transformArray = [];
    var names = [];
    for (var i = 1; i < n; i += 1)
    {
        transformArray = append(transformArray, rotationAround(axis, i * 360*degree / n));
        names = append(names, toString(i));
    }

    opPattern(context, operationId, {
                "entities" : entities,
                "transforms" : transformArray,
                "instanceNames" : names
            });
            
    var instances = [entities];
    for (var i = 1; i < n; i += 1)
        instances = instances->append(qPatternInstances(operationId, names[i - 1], EntityType.BODY));
    return instances;
}

export function PatternCopy(context is Context, operationId is Id, n is number, entities is Query) returns Query
precondition isNonNegativeInteger(n);
{
    if (isQueryEmpty(context, entities) || n == 0)
        return qNothing();

    var transformArray = [];
    var names = [];
    for (var i = 0; i < n; i += 1)
    {
        transformArray = append(transformArray, identityTransform());
        names = append(names, toString(i));
    }

    opPattern(context, operationId, {
            "entities" : entities,
            "transforms" : transformArray,
            "instanceNames" : names
    });

    var instances = [];
    for (var i = 0; i < n; i += 1)
        instances = instances->append(qPatternInstances(operationId, names[i], EntityType.BODY));

    return qCreatedBy(operationId);
}

export function PatternLinear(context is Context, operationId is Id, n is number, r is Vector, entities is Query) returns array
precondition
{
    isPositiveInteger(n);
    is3dLengthVector(r);
}
{
    if (isQueryEmpty(context, entities))
        return [];

    if (n == 1)
        return [entities];

    var transformArray = [];
    var names = [];
    for (var i = 1; i < n; i += 1)
    {
        transformArray = append(transformArray, transform(i * r));
        names = append(names, toString(i));
    }

    opPattern(context, operationId, {
                "entities" : entities,
                "transforms" : transformArray,
                "instanceNames" : names
            });
    
    var instances = [entities];
    for (var i = 1; i < n; i += 1)
        instances = instances->append(qPatternInstances(operationId, names[i - 1], EntityType.BODY));
    return instances;
}

export function Line3d(context is Context, operationId is Id, pointA is Vector, pointB is Vector) returns Query
{
    return context->Polyline3d(operationId, [pointA,pointB]);
}

export function Line3d(context is Context, operationId is Id, points is array) returns Query
precondition points->size() >= 2;
{
    return context->Polyline3d(operationId, points);
}

export function Polyline3d(context is Context, operationId is Id, points is array) returns Query
{
    context->opPolyline(operationId, {
            "points" : points
    });
    return qCreatedBy(operationId);
}

// =============================================================================================================================
//  REVOLVE
//  =============================================================================================================================
/** Revolve entities around axis by front/back angles. */
export function Revolve(context is Context, operationId is Id, entities is Query, axis is Line, frontAngle is ValueWithUnits, backAngle is ValueWithUnits) returns Query
precondition
{
    isAngle(frontAngle);
    isAngle(backAngle);
}
{
    if (isQueryEmpty(context, entities))
        return qNothing();

    opRevolve(context, operationId, {
                "entities" : entities,
                "axis" : axis,
                "angleForward" : frontAngle,
                "angleBack" : backAngle
            });

    return qCreatedBy(operationId);
}

/** Revolve entities around axis by angle. */
export function Revolve(context is Context, operationId is Id, entities is Query, axis is Line, frontAngle is ValueWithUnits) returns Query
{
    return Revolve(context, operationId, entities, axis, frontAngle, 0*degree);
}

/** Revolve entities around axis by full revolution. */
export function Revolve(context is Context, operationId is Id, entities is Query, axis is Line) returns Query
{
    return Revolve(context, operationId, entities, axis, 360*degree, 0*degree);
}

/** Rotate entities around axis by angle. */
export function Rotate(context is Context, operationId is Id, entities is Query, axis is Line, angle is ValueWithUnits) returns Query
precondition isAngle(angle);
{
    if (isQueryEmpty(context, entities))
        return qNothing();

    opTransform(context, operationId, {
                "bodies" : entities,
                "transform" : rotationAround(axis, angle)
            });
    return entities;
}

export function Spline(context is Context, operationId is Id, points is array) returns Query
precondition size(points) > 1;
{
    opFitSpline(context, operationId, {
                "points" : points
            });

    return qCreatedBy(operationId);
}

export function SplitFace(context is Context, operationId is Id, target is Query, tools is Query, keepTools is boolean) returns Query
{
    opSplitFace(context, operationId, {
            "faceTargets" : target,
            "faceTools" : tools,
            "keepToolSurfaces" : keepTools
    });
 
    return qUnion(qSplitBy(operationId, EntityType.FACE, false), qSplitBy(operationId, EntityType.FACE, true));
}

export function SplitPart(context is Context, operationId is Id, target is Query, tool is Query, keepTools is boolean) returns Query
{
    opSplitPart(context, operationId, {
                "targets" : target,
                "tool" : tool,
                "keepTools" : keepTools
            });
            
    return qUnion(qSplitBy(operationId, EntityType.BODY, false), qSplitBy(operationId, EntityType.BODY, true));
}

// export function SplitPart(context is Context, operationId is Id, target is Query, keepPlane is boolean, keepBackBody is boolean, pln is Plane) returns Query
// {
//     const splitPlane = CreatePlane(cid, pln);
//     const results = SplitPart(cid, keepPlane, splitPlane->faces(), target);
    
//     var deleteBodies = qNothing();
//     if (!keepPlane)
//         deleteBodies = qUnion(deleteBodies, splitPlane);
//     if (!keepBackBody)
//         deleteBodies = qUnion(deleteBodies, results->qNthElement(1));
//     Delete(cid, deleteBodies);
    
//     return results;
// }

export function Subtract(context is Context, operationId is Id, targets is Query, tools is Query, keepTools is boolean) returns Query
{
    if (isQueryEmpty(context, targets))
        return qNothing();
    if (isQueryEmpty(context, tools))
        return targets;

    opBoolean(context, operationId, {
                "tools" : tools,
                "targets" : targets,
                "operationType" : BooleanOperationType.SUBTRACTION,
                "keepTools" : keepTools
            }); 

    return qCreatedBy(operationId);
}

export function Sweep(context is Context, operationId is Id, profiles is Query, path is Query) returns Query
{
    if (isQueryEmpty(context, profiles) || isQueryEmpty(context, path))
        return qNothing();

    opSweep(context, operationId, {
            "profiles" : profiles,
            "path" : path
    });

    return qCreatedBy(operationId);
}

export function ApplyTransform(context is Context, operationid is Id, entities is Query, T is Transform) returns Query
{
    if (context->isQueryEmpty(entities) || T == WORLD_COORD_SYSTEM)
        return;

    opTransform(context, operationid + "transform", {
            "bodies" : entities,
            "transform" : T
    });
    
    return entities;
}

export function Translate(context is Context, operationId is Id, entities is Query, r is Vector) returns Query
precondition is3dLengthVector(r);
{
    if (isQueryEmpty(context, entities) || r->isZero())
        return qNothing();

    opTransform(context, operationId, {
                "bodies" : entities,
                "transform" : transform(r)
            });
    return entities;
}

export function Union(context is Context, operationId is Id, entities is Query) returns Query
{
    if (isQueryEmpty(context, entities))
        return qNothing();

    opBoolean(context, operationId, {
            "tools" : entities,
            "operationType" : BooleanOperationType.UNION
        });
    return entities;
}

export function Union(context is Context, operationId is Id, entityArray is array) returns Query
{
    return Union(context, operationId, qUnion(entityArray));
}

export function WrapPlaneToCylinder(context is Context, operationId is Id, entities is Query, srcPlane is Plane, destFace is Query) returns Query
precondition
{
    !isQueryEmpty(context, entities);
    !isQueryEmpty(context, destFace);
}
{
    var wrapSrc = makeWrapPlane(srcPlane, srcPlane.origin, srcPlane.x);
    var wrapDest = makeWrapSurface(context, destFace, srcPlane.origin, srcPlane.x);

    opWrap(context, operationId, {
                "wrapType" : WrapType.SIMPLE,
                "entities" : entities,
                "source" : wrapSrc,
                "destination" : wrapDest
            });

    return qCreatedBy(operationId);
}

export function GetAttribute(context is Context, entities is Query, attributeName is string)
{
    return isQueryEmpty(context, entities)
        ? undefined
        : getAttribute(context, {
                "entity" : entities,
                "name" : attributeName
            });
}

export function SetAttribute(context is Context, entities is Query, attributeName is string, attributeValue)
precondition !isQueryEmpty(context, entities);
{
    if (isQueryEmpty(context, entities))
        return;

    setAttribute(context, {
            "entities" : entities,
            "name" : attributeName,
            "attribute" : attributeValue
    });
}

export function SetFeatureComputedParameter(context is Context, operationId is Id, propertyName is string, propertyValue)
{
    setFeatureComputedParameter(context, operationId, {
                    "name" : propertyName,
                    "value" : propertyValue
    });
}

export function SetProperty(context is Context, propertyType is PropertyType, value, entities is Query)
{
    if (!isQueryEmpty(context, entities))
    {
        setProperty(context, {
                "entities" : entities,
                "propertyType" : PropertyType.NAME,
                "value" : value
        });
    }
}