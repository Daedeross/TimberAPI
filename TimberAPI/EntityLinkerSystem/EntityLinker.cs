﻿using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.ConstructibleSystem;
using Timberborn.Persistence;
using TimberbornAPI.Internal;
using UnityEngine;

namespace TimberbornAPI.EntityLinkerSystem
{
    /// <summary>
    /// Defines the behaviour of Linkers
    /// </summary>
    public class EntityLinker : MonoBehaviour, IFinishedStateListener, IPersistentEntity
    {
        //Keys for saving/loading
        protected static readonly ComponentKey EntityLinkerKey = new ComponentKey("EntityLinker");
        protected static readonly ListKey<EntityLink> EntityLinksKey = new ListKey<EntityLink>(nameof(EntityLinks));

        internal readonly List<EntityLink> _entityLinks = new List<EntityLink>();
        public IReadOnlyCollection<EntityLink> EntityLinks { get; private set; }

        public virtual void Awake()
        {
            EntityLinks = _entityLinks.AsReadOnly();
        }

        /// <summary>
        /// Save existing Links
        /// </summary>
        /// <param name="entitySaver"></param>
        public virtual void Save(IEntitySaver entitySaver)
        {
            IObjectSaver component = entitySaver.GetComponent(EntityLinkerKey);
            component.Set(EntityLinksKey, EntityLinks);
        }

        /// <summary>
        /// Load saves Links
        /// </summary>
        /// <param name="entityLoader"></param>
        public virtual void Load(IEntityLoader entityLoader)
        {
            if (!entityLoader.HasComponent(EntityLinkerKey))
            {
                return;
            }
            IObjectLoader component = entityLoader.GetComponent(EntityLinkerKey);
            if (component.Has(EntityLinksKey))
            {
                var links = component.Get(EntityLinksKey);
                if (links == null)
                {
                    return;
                }
                var linkerLinks = links.Where(x => x.Linker == this).ToList();
                _entityLinks.AddRange(linkerLinks);
                foreach (var link in linkerLinks)
                {
                    PostCreateLink(link);
                }
            }
        }

        public virtual void OnEnterFinishedState()
        {
            //Nothing to do here?
        }

        /// <summary>
        /// When entity is deleted, remove all links
        /// </summary>
        public virtual void OnExitFinishedState()
        {
            RemoveAllLinks();
        }

        /// <summary>
        /// Creates a new link where this in the Linker
        /// </summary>
        /// <param name="linkee"></param>
        public virtual void CreateLink(EntityLinker linkee)
        {
            if(linkee == this)
            {
                TimberAPIPlugin.Log.LogWarning($"Tried to link entity with itself. Stopped linking.");
                return;
            }
            var link = new EntityLink(this, linkee);
            AddLink(link);
            PostCreateLink(link);
        }

        /// <summary>
        /// Adds a new Link where this is the Linkee
        /// </summary>
        /// <param name="link"></param>
        public virtual void AddLink(EntityLink link)
        {
            _entityLinks.Add(link);
        }

        /// <summary>
        /// Add the Link on the Linkee
        /// </summary>
        /// <param name="link"></param>
        public virtual void PostCreateLink(EntityLink link)
        {
            link.Linkee.AddLink(link);
        }

        /// <summary>
        /// Deletes a Link and calls to remove the Link from the Linkee
        /// </summary>
        /// <param name="link"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual void DeleteLink(EntityLink link)
        {
            if (!_entityLinks.Remove(link))
            {
                throw new InvalidOperationException($"Couldn't remove {link} from {this}, it wasn't added.");
            }
            PostDeleteLink(link);
        }


        /// <summary>
        /// Removes the Link from Linkee too
        /// </summary>
        /// <param name="link"></param>
        public virtual void PostDeleteLink(EntityLink link)
        {
            link.Linkee.RemoveLink(link);
        }

        /// <summary>
        /// Removes a Link from the list
        /// </summary>
        /// <param name="link"></param>
        public virtual void RemoveLink(EntityLink link)
        {
            _entityLinks.Remove(link);
        }

        /// <summary>
        /// Remvoes all existing Links
        /// </summary>
        public virtual void RemoveAllLinks()
        {
            foreach (var link in EntityLinks)
            {
                if(link.Linker == this)
                {
                    link.Linkee.RemoveLink(link);
                }
                else
                {
                    link.Linker.RemoveLink(link);
                }
            }
            _entityLinks.Clear();
        }
    }
}
