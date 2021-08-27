using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlKata.Execution;
using ZeyjaFramework.Config;

namespace ZeyjaFramework.Interfaces
{
    public interface IDatabase
    {
        QueryFactory Db();
    }
}
