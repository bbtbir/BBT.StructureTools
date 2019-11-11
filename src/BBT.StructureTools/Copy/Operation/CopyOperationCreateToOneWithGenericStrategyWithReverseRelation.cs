﻿namespace BBT.StructureTools.Copy.Operation
{
    using System;
    using System.Linq.Expressions;
    using BBT.StructureTools.Copy;
    using BBT.StructureTools.Copy.Strategy;
    using BBT.StructureTools.Extension;

    /// <inheritdoc/>
    internal class CopyOperationCreateToOneWithGenericStrategyWithReverseRelation<T, TStrategy, TChild> : ICopyOperationCreateToOneWithGenericStrategyWithReverseRelation<T, TStrategy, TChild>
        where T : class
        where TStrategy : class, ICopyStrategy<TChild>
        where TChild : class
    {
        private readonly ICopyStrategyProvider<TStrategy, TChild> strategyProvider;
        private Func<T, TChild> sourceFunc;
        private Expression<Func<T, TChild>> targetexpression;
        private Expression<Func<TChild, T>> reverseRelationExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyOperationCreateToOneWithGenericStrategyWithReverseRelation{T, TStrategy, TChild}"/> class.
        /// </summary>
        public CopyOperationCreateToOneWithGenericStrategyWithReverseRelation(ICopyStrategyProvider<TStrategy, TChild> strategyProvider)
        {
            strategyProvider.NotNull(nameof(strategyProvider));

            this.strategyProvider = strategyProvider;
        }

        /// <inheritdoc/>
        public void Copy(
            T source,
            T target,
            ICopyCallContext copyCallContext)
        {
            var sourceChild = this.sourceFunc.Invoke(source);

            // if the source is null, set the target also to null and exit copy process step.
            if (sourceChild == null)
            {
                target.SetPropertyValue(this.targetexpression, null);
                return;
            }

            var strategy = this.strategyProvider.GetStrategy(sourceChild);

            var copy = strategy.Create();
            strategy.Copy(sourceChild, copy, copyCallContext);
            copy.SetPropertyValue(this.reverseRelationExpression, target);

            target.SetPropertyValue(this.targetexpression, copy);
        }

        /// <inheritdoc/>
        public void Initialize(Func<T, TChild> sourceFunc, Expression<Func<T, TChild>> targetExpression, Expression<Func<TChild, T>> reverseRelationExpression)
        {
            sourceFunc.NotNull(nameof(sourceFunc));
            targetExpression.NotNull(nameof(targetExpression));
            reverseRelationExpression.NotNull(nameof(reverseRelationExpression));

            this.sourceFunc = sourceFunc;
            this.targetexpression = targetExpression;
            this.reverseRelationExpression = reverseRelationExpression;
        }
    }
}