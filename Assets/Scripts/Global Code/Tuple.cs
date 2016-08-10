using UnityEngine;
using System.Collections;

public class Tuple<T, U> {

    public T item1;
    public U item2;

	public Tuple(T item1, U item2)
    {
        this.item1 = item1;
        this.item2 = item2;
    }
}
