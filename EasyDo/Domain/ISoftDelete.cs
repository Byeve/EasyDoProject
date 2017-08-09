namespace EasyDo.Domain
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
