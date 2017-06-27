// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore.ChangeTracking.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class EntityGraphAttacher : IEntityGraphAttacher
    {
        private readonly IEntityEntryGraphIterator _graphIterator;
        private readonly IKeyPropagator _keyPropagator;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public EntityGraphAttacher(
            [NotNull] IEntityEntryGraphIterator graphIterator,
            [NotNull] IKeyPropagator keyPropagator)
        {
            _graphIterator = graphIterator;
            _keyPropagator = keyPropagator;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual void AttachGraph(InternalEntityEntry rootEntry, EntityState entityState)
            => _graphIterator.TraverseGraph(
                new EntityEntryGraphNode(rootEntry, null, null)
                {
                    NodeState = entityState
                },
                PaintAction);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public virtual Task AttachGraphAsync(
            InternalEntityEntry rootEntry,
            EntityState entityState,
            CancellationToken cancellationToken = default(CancellationToken))
            => _graphIterator.TraverseGraphAsync(
                new EntityEntryGraphNode(rootEntry, null, null)
                {
                    NodeState = entityState
                },
                PaintActionAsync,
                cancellationToken);

        private bool PaintAction(EntityEntryGraphNode node)
        {
            var internalEntityEntry = node.GetInfrastructure();
            if (internalEntityEntry.EntityState != EntityState.Detached)
            {
                return false;
            }

            var entityType = internalEntityEntry.EntityType;

            if (node.SourceEntry != null
                && entityType.IsOwned()
                && entityType.DefiningEntityType == node.SourceEntry.Metadata)
            {
                foreach (var property in entityType.FindPrimaryKey().Properties
                    .Where(p => p.ClrType.IsDefaultValue(internalEntityEntry[p])))
                {
                    _keyPropagator.PropagateValue(internalEntityEntry, property);
                }

                internalEntityEntry.SetEntityState(node.SourceEntry.State);
            }
            else
            {
                internalEntityEntry.SetEntityState(
                    internalEntityEntry.IsKeySet
                        ? (EntityState)node.NodeState
                        : EntityState.Added,
                    acceptChanges: true);
            }

            return true;
        }

        private static async Task<bool> PaintActionAsync(EntityEntryGraphNode node, CancellationToken cancellationToken)
        {
            var internalEntityEntry = node.GetInfrastructure();
            if (internalEntityEntry.EntityState != EntityState.Detached)
            {
                return false;
            }

            await internalEntityEntry.SetEntityStateAsync(
                internalEntityEntry.IsKeySet || internalEntityEntry.EntityType.IsOwned()
                    ? (EntityState)node.NodeState : EntityState.Added,
                acceptChanges: true,
                cancellationToken: cancellationToken);

            return true;
        }
    }
}
