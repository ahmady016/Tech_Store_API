namespace Common;
public class UpdateCommand<T> : IdInput where T : class
{
    public T ModifiedEntity { get; set; }
}
