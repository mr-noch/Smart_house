namespace SmartHouseUI.Interfaces;

public interface IRepository<T>
{
    void Add(T element);
    void Remove(T element);
    int Find(T element);
}

