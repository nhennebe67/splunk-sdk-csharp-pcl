﻿/*
 * Copyright 2014 Splunk, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"): you may
 * not use this file except in compliance with the License. You may obtain
 * a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 */

//// TODO:
//// [ ] Remove EntityCollection.args and put optional arguments on the GetAsync
////     method (?) args does NOT belong on the constructor. One difficulty:
////     not all collections take arguments. Examples: ConfigurationCollection
////     and IndexCollection.
//// [O] Contracts
//// [O] Documentation

namespace Splunk.Client
{
    using Splunk.Client.Refactored;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a base class for representing a collection of Splunk resources.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity in <typeparamref name="TCollection"/>.
    /// </typeparam>
    /// <remarks>
    /// <para><b>References:</b></para>
    /// <list type="number">
    /// <item><description>
    ///   <a href="http://goo.gl/TDthxd">Accessing Splunk resources</a>, 
    ///   especially "Other actions for Splunk REST API endpoints".
    /// </description></item>
    /// <item><description>
    ///   <a href="http://goo.gl/oc65Bo">REST API Reference</a>.
    /// </description></item>
    /// </list>
    /// </remarks>
    public class EntityCollection<TEntity> : Resource, IReadOnlyList<TEntity> where TEntity : Resource, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollection&lt;TEntity&gt;"/> 
        /// class.
        /// </summary>
        /// <param name="context">
        /// An object representing a Splunk server session.
        /// </param>
        /// <param name="ns">
        /// An object identifying a Splunk services namespace.
        /// </param>
        /// <param name="name">
        /// An object identifying a Splunk entity collection within <paramref name="ns"/>.
        /// </param>
        protected internal EntityCollection(Context context, Namespace ns, ResourceName name)
            : base(context, ns, name)
        { }

        protected internal EntityCollection(Context context, AtomFeed feed)
            : base(context, feed)
        { }

        protected internal EntityCollection(Context context, AtomEntry entry, Version generatorVersion)
            : base(context, entry, generatorVersion)
        { }

#if false
        /// <summary>
        /// Infrastructure. Initializes a new instance of the <see cref=
        /// "EntityCollection&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <remarks>
        /// This API supports the Splunk client infrastructure and is not 
        /// intended to be used directly from your code. 
        /// </remarks>
        public EntityCollection()
        { }
#endif

        #endregion

        #region Properties

        #region AtomFeed properties

        /// <summary>
        /// Gets the pagination attributes for the current <see cref=
        /// "EntityCollection&lt;TEntity&gt;"/>.
        /// </summary>
        public Pagination Pagination
        {
            get { return this.CurrentSnapshot.Pagination; }
        }

        #endregion

        #region IReadOnlyList<TEntity> properties

        /// <summary>
        /// Gets the entity at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the entity to get.
        /// </param>
        /// <returns>
        /// An object representing the entity at <paramref name="index"/>.
        /// </returns>
        public TEntity this[int index]
        {
            get { return (TEntity)this.Resources[index]; }
        }

        /// <summary>
        /// Gets the number of entities in the current <see cref=
        /// "EntityCollection&lt;TEntity&gt;"/>.
        /// </summary>
        public int Count
        {
            get { return this.Resources.Count; }
        }

        #endregion

        #endregion

        #region Methods

        #region Request-related methods

        /// <summary>
        /// Asynchronously retrieves a fresh copy of the full list of entities
        /// in the current <see cref="EntityCollection&lt;TEntity&gt;"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the operation.
        /// </returns>
        /// <remarks>
        /// Following completion of the operation the list of entites in the
        /// current <see cref="EntityCollection&lt;TEntity&gt;"/> will
        /// contain all changes since the list was last retrieved.
        /// </remarks>
        public virtual async Task GetAllAsync()
        {
            using (Response response = await this.Context.GetAsync(this.Namespace, this.ResourceName))
            {
                await response.EnsureStatusCodeAsync(System.Net.HttpStatusCode.OK);
                await this.UpdateSnapshotAsync(response);
            }
        }

        protected override Resource CreateResource(Context context, AtomEntry entry, Version generatorVersion)
        {
            var entity = new TEntity();
            
            entity.Initialize(context, entry, generatorVersion);
            return entity;
        }

        /// <summary>
        /// Asynchronously forces the Splunk server to reload data for the current
        /// <see cref="EntityCollection&lt;TEntity&gt;"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing this operation.
        /// </returns>
        public async Task ReloadAsync()
        {
            var reload = new ResourceName(this.ResourceName, "_reload");

            using (Response response = await this.Context.GetAsync(this.Namespace, reload))
            {
                await response.EnsureStatusCodeAsync(System.Net.HttpStatusCode.OK);
            }
        }

        #endregion

        #region IReadOnlyList<TEntity> methods

        /// <summary>
        /// Gets an enumerator that iterates through the current <see cref=
        /// "EntityCollection&lt;TEntity&gt;"/>.
        /// </summary>
        /// <returns>
        /// An object for iterating through the current <see cref=
        /// "EntityCollection&lt;TEntity&gt;"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates through the current <see cref=
        /// "EntityCollection&lt;TEntity&gt;"/>.
        /// </summary>
        /// <returns>
        /// An object for iterating through the current <see cref=
        /// "EntityCollection&lt;TEntity&gt;"/>.
        /// </returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            if (this.Resources == null)
            {
                throw new InvalidOperationException();
            }

            return this.Resources.Cast<TEntity>().GetEnumerator();
        }

        #endregion

        #endregion

        #region Privates

        #endregion
   }
}
