using Camoran.Cache.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camoran.Cache.Redis
{
    public interface IRedisCacheStrategy<Key, Value> : ICahceStrategy<Key, Value>
    {
      
    }
}
