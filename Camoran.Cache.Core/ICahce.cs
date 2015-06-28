using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camoran.Cache.Core
{
    public interface ICahce<Key,Value>
    {
        Value Get(Key key);
        void Set(Key key, Value value,TimeSpan expireTime);
        void Remove(Key key);
    }
}
