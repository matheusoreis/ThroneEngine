namespace Throne.Shared.Slots;

public class Slots<T>(int size)
{
  private readonly List<T?> _slots = new List<T?>(new T?[size]);

  private void CheckIndex(int index)
  {
    if (index < 0 || index >= _slots.Count)
    {
      throw new ArgumentOutOfRangeException(nameof(index), $"Index out of range: {index}");
    }
  }

  public T? Get(int index)
  {
    CheckIndex(index);
    return _slots[index];
  }

  public IEnumerable<int> GetFilledSlots()
  {
    for (int i = 0; i < _slots.Count; i++)
    {
      if (_slots[i] != null)
      {
        yield return i;
      }
    }
  }

  public IEnumerable<int> GetEmptySlots()
  {
    for (int i = 0; i < _slots.Count; i++)
    {
      if (_slots[i] == null)
      {
        yield return i;
      }
    }
  }

  public void Remove(int index)
  {
    CheckIndex(index);
    _slots[index] = default;
  }

  public int Add(T value)
  {
    for (int i = 0; i < _slots.Count; i++)
    {
      if (_slots[i] == null)
      {
        _slots[i] = value;
        return i;
      }
    }
    throw new InvalidOperationException("No empty slots available");
  }

  public int? GetFirstEmptySlot()
  {
    int index = _slots.IndexOf(default);
    return index != -1 ? index : (int?)null;
  }

  public void Update(int index, T value)
  {
    CheckIndex(index);
    _slots[index] = value;
  }
}
