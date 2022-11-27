using DevourDev.Database.Interfaces;

namespace DevourDev.Database
{
    public abstract class EntityRequestSettings<Entity> where Entity : IEntity
    {
        public EntityRequestType RequestType;
        public GetEntityMode GetMode;
        public SetEntityMode SetMode;
        public DeleteEntityMode DeleteMode;
        public ReplaceEntityMode ReplaceMode;

        /// <summary>
        /// all that is not null - active
        /// </summary>
        public Entity Value;
    }

    public abstract class GetEntityRequestSettings<Entity> where Entity : IEntity
    {
        public GetEntityMode Mode;

        public GetEntityRequestSettings()
        {

        }

    }

    public abstract class SetEntityRequestSettings<Entity> where Entity : IEntity
    {
        public SetEntityMode Mode;

        public Entity Value;

    }

    public abstract class DeleteEntityRequestSettings<Entity> where Entity : IEntity
    {
        public DeleteEntityMode Mode;
    }





}
