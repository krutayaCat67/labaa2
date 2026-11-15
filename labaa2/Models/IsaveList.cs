namespace labaa2.Models
{
    public interface ISaveList<T>
    {
        T Load(string path);
        void Save(T data, string path);
    }
}
