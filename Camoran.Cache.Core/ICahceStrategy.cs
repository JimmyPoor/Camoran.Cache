using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camoran.Cache.Core
{
    public interface ICahceStrategy<Key,Value>
    {
        void Set(Key key, Value value);
        Value Get(Key key);
        bool Remove(Key key);
        void SetExpire(Key key, DateTime expireDate);

    }
}
