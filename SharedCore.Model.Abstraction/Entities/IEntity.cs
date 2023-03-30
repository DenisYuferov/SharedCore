namespace SharedCore.Model.Abstraction.Entities
{
    public interface IEntity<TId>
    {
        TId Id { get; }
    }
}
