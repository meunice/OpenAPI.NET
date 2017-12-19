﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Diagnostics;
using System.Linq;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// The base visitor class for Open Api elements.
    /// </summary>
    internal abstract class VisitorBase<T> : IVisitor where T : IOpenApiElement
    {
        /// <summary>
        /// Visit the element itself.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="item">The element.</param>
        public void Visit(ValidationContext context, object item)
        {
            Debug.Assert(item is T, "item should be " + typeof(T));

            var rules = context.RuleSet.Where(r => r.ElementType == typeof(T));
            foreach (var rule in rules)
            {
                rule.Evaluate(context, item);
            }

            T typedItem = (T)item;
            this.Next(context, typedItem);
        }

        /// <summary>
        /// Visit the children.
        /// </summary>
        /// <param name="context">The validation context.</param>
        /// <param name="element">The element.</param>
        protected virtual void Next(ValidationContext context, T element)
        {
            IOpenApiExtensible extensible = element as IOpenApiExtensible;
            if (extensible != null)
            {
                var rules = context.RuleSet.Where(r => r.ElementType == typeof(IOpenApiExtensible));
                foreach (var rule in rules)
                {
                    rule.Evaluate(context, extensible);
                }
            }
        }
    }
}
