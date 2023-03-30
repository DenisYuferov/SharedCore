using SharedCore.Model.Abstraction.Entities;

namespace SharedCore.Model.Entities
{
    public class EntityBase<TId> : IEntity<TId>
        where TId : struct
    {
        public TId Id { get; set; }
    }
}
