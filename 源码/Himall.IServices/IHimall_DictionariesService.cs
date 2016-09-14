using Himall.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Himall.IServices
{
    public interface IHimall_DictionariesService : IService, IDisposable
    {
        string GetValueBYKey(string key);
    }
}
