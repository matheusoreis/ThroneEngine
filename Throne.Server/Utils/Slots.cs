namespace Throne.Server.Utils;

public class Slots<T>(int size)
{
  private readonly List<T?> slots = [..new T?[size]];

  private void CheckIndex(int index)
  {
    if (index < 0 || index >= slots.Count)
    {
      throw new ArgumentOutOfRangeException(nameof(index), $"Index out of range: {index}");
    }
  }

  public T? Get(int index)
  {
    CheckIndex(index);
    return slots[index];
  }

  public IEnumerable<int> GetFilledSlots()
  {
    for (int i = 0; i < slots.Count; i++)
    {
      if (slots[i] != null)
      {
        yield return i;
      }
    }
  }

  public IEnumerable<int> GetEmptySlots()
  {
    for (int i = 0; i < slots.Count; i++)
    {
      if (slots[i] == null)
      {
        yield return i;
      }
    }
  }

  public void Remove(int index)
  {
    CheckIndex(index);
    slots[index] = default;
  }

  public int Add(T value)
  {
    for (int i = 0; i < slots.Count; i++)
    {
      if (slots[i] != null) continue;
      slots[i] = value;
      return i;
    }
    throw new InvalidOperationException("No empty slots available");
  }

  public int? GetFirstEmptySlot()
  {
    int index = slots.IndexOf(default);
    return index != -1 ? index : (int?)null;
  }

  public void Update(int index, T value)
  {
    CheckIndex(index);
    slots[index] = value;
  }
}
