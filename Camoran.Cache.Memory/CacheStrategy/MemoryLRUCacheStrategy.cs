using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camoran.Cache.Memory.CacheStrategy
{
    public class MemoryLRUCacheStrategy<Key, Value> : IMemoryCacheStrategy<Key, Value>
    {
        private IMemoryCacheStrategy<Key, Value> _anotherStrategy;
        private DoubleLinkList<Key> _keys = new DoubleLinkList<Key>();
        private int _maxCount = 10;
        private int _deleteCount = 5;
        public MemoryLRUCacheStrategy(IMemoryCacheStrategy<Key, Value> anotherStrategy, int maxCount,int deleteCountWhenExpire)
        {
            if (anotherStrategy == null) throw new ArgumentNullException("LRU Cache Strategy need to store cache value by anohter memory cache strategy");
            this._anotherStrategy = anotherStrategy;
            this._maxCount = maxCount;
            this._deleteCount = deleteCountWhenExpire;
        }
        public void Set(Key key, Value value)
        {
            this._anotherStrategy.Set(key, value);
            _keys.Add(key);
        }

        public Value Get(Key key)
        {
            Value val = default(Value);
            this.RemoveKeysIfExpire();
            var currentKey = _keys.Find(key);
            if (currentKey != null)
            {
                val = this._anotherStrategy.Get(key);
                _keys.Add(currentKey);
            }
            return val;
        }

        public bool Remove(Key key)
        {
            this._keys.Remove(key);
            return this._anotherStrategy.Remove(key);
        }

        public void SetExpire(Key key, DateTime expireDate)
        {
            this._anotherStrategy.SetExpire(key, expireDate);
        }


        private void RemoveKeysIfExpire()
        {
            if (this._keys.Count >= this._maxCount)
            {
                _keys.Remove(_deleteCount);
            }
        }
    }


    public class DoubleLinkList<T>
    {
        DoubledLinkNode _head;
        DoubledLinkNode _tail;
        private int _count;

        public int Count { get { return this._count; } }

        public DoubleLinkList()
        {
            this._head = new DoubledLinkNode();
        }
        public void Add(T t)
        {
            DoubledLinkNode insertNode = new DoubledLinkNode();
            insertNode.SetValue(t);
            if (_head.HasNext)
            {
                this._head.Next = insertNode;
                insertNode.Prev = this._head;
            }
            else
            {
                var node = this._head;
                while (node.HasNext)
                {
                    node = node.Next;
                }
                node.Next = insertNode;
                insertNode.Prev = node;
            }

            this._tail = insertNode;
            this._count++;
        }

        public void Remove(int deletCount)
        {
            var node = this._tail; // delet from tail node
            while (deletCount > 0)
            {
                _tail = node.Prev;
                RemoveNode(node);
                node = _tail;
                deletCount--;
            }


        }


        public void Remove(T t)
        {
            var node = this.GetCurrentNode(t);
            RemoveNode(node);

        }

        public T Find(T t)
        {
            var currentNode = this.GetCurrentNode(t);
            return currentNode.Value;
        }

        private void RemoveNode(DoubledLinkNode node)
        {
            if (node.HasPrev)
                node.Prev.Next = node.Next;
            if (node.HasNext)
                node.Next.Prev = node.Prev;
            node = null;
            this._count--;
        }
        private DoubledLinkNode GetCurrentNode(T t)
        {
            var node = this._head;
            while (node.HasNext)
            {
                node = node.Next;
                if (node.Value.Equals(t)) break;
            }
            return node;
        }


        private class DoubledLinkNode
        {
            public DoubledLinkNode Next { get; set; }
            public DoubledLinkNode Prev { get; set; }

            public T Value { get; private set; }

            public bool HasNext { get { return this.Next != null; } }
            public bool HasPrev { get { return this.Prev != null; } }

            public void SetValue(T t)
            {
                this.Value = t;
            }
        }
    }
}