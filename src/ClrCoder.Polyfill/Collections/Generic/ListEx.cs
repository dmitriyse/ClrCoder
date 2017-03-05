// <copyright file="ListEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    /// <summary>
    /// Represents a strongly typed list of objects that can be accessed by index. Provides methods to search, sort, and
    /// manipulate lists. <br/>
    /// Polyfill of comming new features.
    /// </summary>
    /// <remarks>
    /// Implements https://github.com/dotnet/corefx/issues/16661 proposal.
    /// </remarks>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [PublicAPI]
    public class ListEx<T> : IListEx<T>, IList, IFreezable<ListEx<T>>
    {
        private readonly List<T> _list;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEx{T}"/> class.
        /// </summary>
        public ListEx()
        {
            _list = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEx{T}"/> class.
        /// </summary>
        /// <param name="collection">Collection for list initial fill.</param>
        public ListEx(IEnumerable<T> collection)
        {
            _list = new List<T>(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEx{T}"/> class.
        /// </summary>
        /// <param name="capacity">Initial list capacity.</param>
        public ListEx(int capacity)
        {
            _list = new List<T>(capacity);
        }

        /// <inheritdoc/>
        public int Count => _list.Count;

        bool IList.IsReadOnly => IsImmutable;

        bool ICollection<T>.IsReadOnly => IsImmutable;

        /// <inheritdoc/>
        public bool IsImmutable => IsFrozen;

        bool IList.IsFixedSize => ((IList)_list).IsFixedSize;

        bool ICollection.IsSynchronized => ((ICollection)_list).IsSynchronized;

        object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;

        /// <summary>
        /// Gets and sets the capacity of <c>this</c> list.  The capacity is the size of the <c>internal</c> array used to hold
        /// items.  When set,
        /// the <c>internal</c> array of the list is reallocated to the given capacity.
        /// </summary>
        public int Capacity => _list.Capacity;

        object IList.this[int index]
        {
            get
            {
                return ((IList)_list)[index];
            }

            set
            {
                VerifyNotFrozen();
                ((IList)_list)[index] = value;
            }
        }

        /// <inheritdoc/>
        public T this[int index]
        {
            get
            {
                return _list[index];
            }

            set
            {
                VerifyNotFrozen();
                _list[index] = value;
            }
        }

        /// <summary>
        /// Implicit conversion from polyfill to normal list.
        /// </summary>
        /// <param name="listEx">Polyfill list.</param>
        /// <returns>Normal BCL list.</returns>
        [CanBeNull]
        public static implicit operator List<T>([CanBeNull] ListEx<T> listEx)
        {
            return listEx?._list;
        }

        /// <summary>
        /// Implicit conversion from polyfill to normal list.
        /// </summary>
        /// <param name="list">Normal BCL list.</param>
        /// <returns>List polyfill.</returns>
        [CanBeNull]
        public static implicit operator ListEx<T>([CanBeNull] List<T> list)
        {
            if (list == null)
            {
                return null;
            }

            return new ListEx<T>(list);
        }

        /// <inheritdoc/>
        public void Add([CanBeNull] T item)
        {
            VerifyNotFrozen();
            _list.Add(item);
        }

        int IList.Add([CanBeNull] object value)
        {
            VerifyNotFrozen();
            return ((IList)_list).Add(value);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            VerifyNotFrozen();
            _list.Clear();
        }

        bool IList.Contains([CanBeNull] object value)
        {
            return ((IList)_list).Contains(value);
        }

        /// <inheritdoc/>
        public bool Contains([CanBeNull] T item)
        {
            return _list.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo([CanBeNull] T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo([CanBeNull] Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        int IList.IndexOf([CanBeNull] object value)
        {
            return ((IList)_list).IndexOf(value);
        }

        /// <inheritdoc/>
        public int IndexOf([CanBeNull] T item)
        {
            return _list.IndexOf(item);
        }

        void IList.Insert(int index, [CanBeNull] object value)
        {
            VerifyNotFrozen();
            ((IList)_list).Insert(index, value);
        }

        /// <inheritdoc/>
        public void Insert(int index, [CanBeNull] T item)
        {
            VerifyNotFrozen();
            _list.Insert(index, item);
        }

        void IList.Remove([CanBeNull] object value)
        {
            VerifyNotFrozen();
            ((IList)_list).Remove(value);
        }

        /// <inheritdoc/>
        public bool Remove([CanBeNull] T item)
        {
            VerifyNotFrozen();
            return _list.Remove(item);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            VerifyNotFrozen();
            _list.RemoveAt(index);
        }

        /// <summary>
        /// Adds range of elements to the list.
        /// </summary>
        /// <param name="collection">Elements to add.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            VerifyNotFrozen();
            _list.AddRange(collection);
        }

        /// <summary>
        /// Performs binary search of the specified <c>item</c> with default comparer.
        /// </summary>
        /// <param name="item">Item to search.</param>
        /// <returns>Index of found <c>item</c> or -1, if <c>item</c> was not found.</returns>
        public int BinarySearch(T item)
        {
            return _list.BinarySearch(item);
        }

        /// <summary>
        /// Performs binary search of the specified <c>item</c> with the specified comparer.
        /// </summary>
        /// <param name="item">Item to search.</param>
        /// <param name="comparer">Comparer for search.</param>
        /// <returns>Index of found <c>item</c> or -1, if <c>item</c> was not found.</returns>
        public int BinarySearch([CanBeNull] T item, IComparer<T> comparer)
        {
            return _list.BinarySearch(item, comparer);
        }

        /// <summary>
        /// Performs binary search of the specified <c>item</c> with the specified comparer in the specified range.
        /// </summary>
        /// <param name="index">Starting <c>index</c> of search range.</param>
        /// <param name="count">Length of a search range.</param>
        /// <param name="item">Item to search.</param>
        /// <param name="comparer">Comparer for search.</param>
        /// <returns>Index of found <c>item</c> or -1, if <c>item</c> was not found.</returns>
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return _list.BinarySearch(index, count, item, comparer);
        }

        /// <summary>
        /// Copyes items to the <c>array</c>.
        /// </summary>
        /// <param name="array">Target <c>array</c>.</param>
        public void CopyTo(T[] array)
        {
            _list.CopyTo(array);
        }

        /// <summary>
        /// Copies items to the specified array.
        /// </summary>
        /// <param name="index">Starting index in this list.</param>
        /// <param name="array">Target array.</param>
        /// <param name="arrayIndex">Starting index in the target array.</param>
        /// <param name="count">Amount of items to copy.</param>
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            _list.CopyTo(index, array, arrayIndex, count);
        }

        /// <summary>
        /// Verifies that at least one item exists that satisfy provided predicate.
        /// </summary>
        /// <param name="match">Predicate to match item.</param>
        /// <returns><see langword="true"/> - item was found, <see langword="false"/> otherwise.</returns>
        public bool Exists(Predicate<T> match)
        {
            return _list.Exists(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the first
        /// occurrence within the entire <see cref="List{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The first element that matches the conditions defined by the specified predicate, if found; otherwise, the
        /// default value for type T.
        /// </returns>
        [CanBeNull]
        public T Find(Predicate<T> match)
        {
            return _list.Find(match);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to search for.</param>
        /// <returns>
        /// A <see cref="List{T}"/> containing all the elements that match the conditions defined by the specified
        /// predicate, if found; otherwise, an empty <see cref="List{T}"/>.
        /// </returns>
        public List<T> FindAll(Predicate<T> match)
        {
            return _list.FindAll(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by a specified predicate, and returns the zero-based index
        /// of the first occurrence within the <see cref="List{T}"/> or a portion of it. This method returns -1 if an item that
        /// matches the conditions is not found.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by match, if
        /// found; otherwise, –1.
        /// </returns>
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return _list.FindIndex(startIndex, count, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by a specified predicate, and returns the zero-based index
        /// of the first occurrence within the <see cref="List{T}"/> or a portion of it. This method returns -1 if an item that
        /// matches the conditions is not found.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by match, if
        /// found; otherwise, –1.
        /// </returns>
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return _list.FindIndex(startIndex, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by a specified predicate, and returns the zero-based index
        /// of the first occurrence within the <see cref="List{T}"/> or a portion of it. This method returns -1 if an item that
        /// matches the conditions is not found.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by match, if
        /// found; otherwise, –1.
        /// </returns>
        public int FindIndex(Predicate<T> match)
        {
            return _list.FindIndex(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the last occurrence
        /// within the entire <see cref="List{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The last element that matches the conditions defined by the specified predicate, if found; otherwise, the
        /// default value for type T.
        /// </returns>
        [CanBeNull]
        public T FindLast(Predicate<T> match)
        {
            return _list.FindLast(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by a specified predicate, and returns the zero-based index
        /// of the last occurrence within the <see cref="Predicate{T}"/> or a portion of it.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by match, if
        /// found; otherwise, –1.
        /// </returns>
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return _list.FindLastIndex(startIndex, count, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by a specified predicate, and returns the zero-based index
        /// of the last occurrence within the <see cref="Predicate{T}"/> or a portion of it.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by match, if
        /// found; otherwise, –1.
        /// </returns>
        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return _list.FindLastIndex(startIndex, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by a specified predicate, and returns the zero-based index
        /// of the last occurrence within the <see cref="Predicate{T}"/> or a portion of it.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by match, if
        /// found; otherwise, –1.
        /// </returns>
        public int FindLastIndex(Predicate<T> match)
        {
            return _list.FindLastIndex(match);
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns>Performance optimized enumerator.</returns>
        public List<T>.Enumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Creates a shallow copy of a range of elements in the source <see cref="List{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based <see cref="List{T}"/> index at which the range starts.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns>A shallow copy of a range of elements in the source <see cref="List{T}"/>.</returns>
        public List<T> GetRange(int index, int count)
        {
            return _list.GetRange(index, count);
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of a value in the <see cref="List{T}"/> or in a portion of it.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the range of elements in the <see cref="List{T}"/>
        /// that starts at index and contains count number of elements, if found; otherwise, –1.
        /// </returns>
        public int IndexOf([CanBeNull] T item, int index)
        {
            return _list.IndexOf(item, index);
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of a value in the <see cref="List{T}"/> or in a portion of it.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the range of elements in the <see cref="List{T}"/>
        /// that starts at index and contains count number of elements, if found; otherwise, –1.
        /// </returns>
        public int IndexOf([CanBeNull] T item, int index, int count)
        {
            return 0;
        }

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="List{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">
        /// The collection whose elements should be inserted into the <see cref="List{T}"/>. The collection itself cannot be null,
        /// but it can contain elements that are null, if type T is a reference type.
        /// </param>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            VerifyNotFrozen();
            _list.InsertRange(index, collection);
        }

        /// <summary>
        /// Returns the zero-based index of the last occurrence of a value in the <see cref="List{T}"/> or in a portion of it.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of item within the range of elements in the <see cref="List{T}"/>
        /// that contains count number of elements and ends at index, if found; otherwise, –1.
        /// </returns>
        public int LastIndexOf([CanBeNull] T item)
        {
            return _list.LastIndexOf(item);
        }

        /// <summary>
        /// Returns the zero-based index of the last occurrence of a value in the <see cref="List{T}"/> or in a portion of it.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of item within the range of elements in the <see cref="List{T}"/>
        /// that contains count number of elements and ends at index, if found; otherwise, –1.
        /// </returns>
        public int LastIndexOf([CanBeNull] T item, int index)
        {
            return _list.LastIndexOf(item, index);
        }

        /// <summary>
        /// Returns the zero-based index of the last occurrence of a value in the <see cref="List{T}"/> or in a portion of it.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="List{T}"/>. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of item within the range of elements in the <see cref="List{T}"/>
        /// that contains count number of elements and ends at index, if found; otherwise, –1.
        /// </returns>
        public int LastIndexOf(T item, int index, int count)
        {
            return _list.LastIndexOf(item, index, count);
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the <see cref="List{T}"/>.</returns>
        public int RemoveAll(Predicate<T> match)
        {
            VerifyNotFrozen();
            return _list.RemoveAll(match);
        }

        /// <summary>
        /// Removes a range of elements from the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        public void RemoveRange(int index, int count)
        {
            VerifyNotFrozen();
            _list.RemoveRange(index, count);
        }

        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="List{T}"/>.
        /// </summary>
        public void Reverse()
        {
            VerifyNotFrozen();
            _list.Reverse();
        }

        /// <summary>
        /// Reverses the order of the elements in the specified range.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="count">The number of elements in the range to reverse.</param>
        public void Reverse(int index, int count)
        {
            VerifyNotFrozen();
            _list.Reverse(index, count);
        }

        /// <summary>
        /// Sorts the list with the default comparer.
        /// </summary>
        public void Sort()
        {
            VerifyNotFrozen();
            _list.Sort();
        }

        /// <summary>
        /// Sorts the list with the specified comparer.
        /// </summary>
        /// <param name="comparer">Comparer for sort.</param>
        public void Sort(IComparer<T> comparer)
        {
            VerifyNotFrozen();
            _list.Sort(comparer);
        }

        /// <summary>
        /// Sorts the list with the specified comparison delegate.
        /// </summary>
        /// <param name="comparison">Comprison for sort.</param>
        public void Sort(Comparison<T> comparison)
        {
            VerifyNotFrozen();
            _list.Sort(comparison);
        }

        /// <summary>
        /// Sorts range of elements.
        /// </summary>
        /// <param name="index">Range start.</param>
        /// <param name="count">Range length.</param>
        /// <param name="comparer">Comparer for sort.</param>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            VerifyNotFrozen();
            _list.Sort(index, count, comparer);
        }

        /// <summary>
        /// Copies all items to a new array.
        /// </summary>
        /// <returns>Array with items.</returns>
        public T[] ToArray()
        {
            return _list.ToArray();
        }

        /// <summary>
        /// Free up unused memory (do nothing when more that ~90% of capacity used).
        /// </summary>
        public void TrimExcess()
        {
            _list.TrimExcess();
        }

        /// <summary>
        /// Determines whether every element in the <see cref="List{T}"/> matches the conditions defined by the specified
        /// predicate.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions to check against the elements.</param>
        /// <returns>
        /// true if every element in the <see cref="List{T}"/> matches the conditions defined by the specified predicate;
        /// otherwise, false. If the list has no elements, the return value is true.
        /// </returns>
        public bool TrueForAll(Predicate<T> match)
        {
            return _list.TrueForAll(match);
        }

        private void VerifyNotFrozen()
        {
            if (IsFrozen)
            {
                throw new InvalidOperationException("Cannot modify frozen collection.");
            }
        }

        /// <inheritdoc/>
        public bool IsFrozen { get; private set; }
    }
}