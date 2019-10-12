using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Project.Scripts
{
    public class PriorityQueue<T> : IEnumerable<T>
        where T : IComparable<T>
    {
        [NotNull]
        private readonly List<T> _data = new List<T>();

        public int Count => _data.Count;

        public void Enqueue([NotNull] T item)
        {
            var childIndex = _data.Count;
            _data.Add(item);

            while (childIndex > 0)
            {
                // If the child has a higher or identical priority to the parent, we're good.
                // No further re-ordering is required in this case.
                var parentIndex = (childIndex - 1) / 2;
                if (_data[childIndex].CompareTo(_data[parentIndex]) >= 0)
                {
                    break;
                }

                // Swap parent and child.
                (_data[parentIndex], _data[childIndex]) = (item, _data[parentIndex]);

                childIndex = parentIndex;
            }
        }

        [NotNull]
        public T Dequeue()
        {
            var frontItem = _data[0];
            var lastIndex = _data.Count - 1;

            // Swap the top with the last item, then reduce the list size.
            _data[0] = _data[lastIndex];
            _data.RemoveAt(lastIndex);
            --lastIndex;

            var parentIndex = 0;
            while (true)
            {
                var childIndex = parentIndex * 2 + 1;
                var rightChildIndex = childIndex + 1;
                if (childIndex > lastIndex) break;

                // Find the more important child: If the right child is of higher priority (lower priority value),
                // select the right child to continue.
                if (rightChildIndex <= lastIndex && _data[rightChildIndex].CompareTo(_data[childIndex]) < 0)
                {
                    childIndex = rightChildIndex;
                }

                // If the parent has a higher priority (lower priority value) than the most important
                // child, the list is already correctly ordered and we're done.
                if (_data[parentIndex].CompareTo(_data[childIndex]) <= 0) break;

                // Swap the items.
                (_data[parentIndex], _data[childIndex]) = (_data[childIndex], _data[parentIndex]);

                parentIndex = childIndex;
            }

            return frontItem;
        }

        [NotNull]
        public T Peek() => _data[0];

        public bool Contains([NotNull] T item) => _data.Contains(item);

        [NotNull]
        public List<T> ToList() => new List<T>(_data);

        [NotNull]
        public IReadOnlyList<T> ToReadonlyList() => _data;

        public IEnumerator<T> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_data).GetEnumerator();
    }
}