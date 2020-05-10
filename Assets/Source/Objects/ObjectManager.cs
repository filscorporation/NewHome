using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Objects
{
    /// <summary>
    /// Holds all objects by their position x
    /// </summary>
    public class ObjectManager
    {
        private struct XObject : IComparable
        {
            public readonly float X;
            public readonly Transform Transform;
            public readonly float Size;

            public XObject(float x, Transform transform, float size)
            {
                X = x;
                Transform = transform;
                Size = size;
            }

            public int CompareTo(object obj)
            {
                if (obj is XObject b)
                    return X + Size / 2 < b.X - b.Size / 2 ? -1 : X - Size / 2 > b.X + b.Size / 2 ? 1 : 0;

                throw new NotImplementedException();
            }
        }

        private class XObjectComparer : IComparer<XObject>
        {
            public int Compare(XObject a, XObject b)
            {
                return a.X < b.X ? -1 : 1;
            }
        }

        private XObjectComparer comparer;

        private readonly List<XObject> objects = new List<XObject>();
        private List<XObject> objectsSorted = new List<XObject>();

        private void Start()
        {
            comparer = new XObjectComparer();
        }

        public void Sort()
        {
            objectsSorted.Clear();
            objectsSorted = objects
                .OrderBy(o => o.Transform.position.x)
                .Select(o => new XObject(o.Transform.position.x, o.Transform, o.Size))
                .ToList();
        }

        /// <summary>
        /// Add object to tracked list
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="size"></param>
        public void Add(Transform obj, float size)
        {
            objects.Add(new XObject(obj.position.x, obj, size));
        }

        /// <summary>
        /// Remove object from tracked list
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(Transform obj)
        {
            objects.RemoveAll(o => o.Transform == obj);
        }

        /// <summary>
        /// Returns far left object or null if non
        /// </summary>
        /// <returns></returns>
        public Transform FarLeft()
        {
            if (!objectsSorted.Any())
                return null;

            return objectsSorted.First().Transform;
        }

        /// <summary>
        /// Returns far right object or null if non
        /// </summary>
        /// <returns></returns>
        public Transform FarRight()
        {
            if (!objectsSorted.Any())
                return null;

            return objectsSorted.Last().Transform;
        }

        /// <summary>
        /// Returns first obj from x to the right in range
        /// </summary>
        /// <param name="x"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public Transform FirstToTheRight(float x, float range)
        {
            if (!objectsSorted.Any())
                return null;

            if (objectsSorted.First().X - objectsSorted.First().Size / 2 > x + range)
                // Closest object is too far, no need to check other
                return null;
            if (objectsSorted.First().X - objectsSorted.First().Size > x)
                // Closest left object is in range
                return objectsSorted.First().Transform;

            int i = objectsSorted.BinarySearch(new XObject(x, null, 0), comparer);
            if (i >= 0)
            {
                return objectsSorted[i].Transform;
            }
            else
            {
                int indexOfNearest = ~i;

                if (indexOfNearest == objectsSorted.Count)
                {
                    // number is greater than last item
                    return null;
                }
                else if (indexOfNearest == 0)
                {
                    // number is less than first item
                    return null;
                }
                else
                {
                    // number is between (indexOfNearest - 1) and indexOfNearest
                    if (objectsSorted[indexOfNearest].X > x + range)
                        // Too far
                        return null;
                    return objectsSorted[indexOfNearest].Transform;
                }
            }
        }

        /// <summary>
        /// Returns first obj from x to the left in range
        /// </summary>
        /// <param name="x"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public Transform FirstToTheLeft(float x, float range)
        {
            if (!objectsSorted.Any())
                return null;

            if (objectsSorted.Last().X < x - range)
                // Closest object is too far, no need to check other
                return null;
            if (objectsSorted.Last().X < x)
                // Closest right object is in range
                return objectsSorted.Last().Transform;

            int i = objectsSorted.BinarySearch(new XObject(x, null, 0), comparer);
            if (i >= 0)
            {
                return objectsSorted[i].Transform;
            }
            else
            {
                int indexOfNearest = ~i;

                if (indexOfNearest == objectsSorted.Count)
                {
                    // number is greater than last item
                    return null;
                }
                else if (indexOfNearest == 0)
                {
                    // number is less than first item
                    return null;
                }
                else
                {
                    // number is between (indexOfNearest - 1) and indexOfNearest
                    if (objectsSorted[indexOfNearest - 1].X < x - range)
                        // Too far
                        return null;
                    return objectsSorted[indexOfNearest - 1].Transform;
                }
            }
        }
    }
}
