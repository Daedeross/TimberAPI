using Timberborn.Persistence;

namespace TimberbornAPI.EntityLinkerSystem
{
    /// <summary>
    /// Defines how and instance of EntityLink should be serialized.
    /// Used when an EntityLinker which contains EntityLinks is saved/loaded
    /// </summary>
    public class EntityLinkSerializer : IObjectSerializer<EntityLink>
    {
        protected static readonly PropertyKey<EntityLinker> LinkerKey = new("Linker");
        protected static readonly PropertyKey<EntityLinker> LinkeeKey = new("Linkee");

        public virtual Obsoletable<EntityLink> Deserialize(IObjectLoader objectLoader)
        {
            EntityLinker linker = objectLoader.Get(LinkerKey);
            EntityLinker linkee = objectLoader.Get(LinkeeKey);
            EntityLink link = new(linker, linkee);
            return link;
        }

        public virtual void Serialize(EntityLink value, IObjectSaver objectSaver)
        {
            objectSaver.Set(LinkerKey, value.Linker);
            objectSaver.Set(LinkeeKey, value.Linkee);
        }
    }
}
