using EntityMapper;
using Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunSample
{
    class Program
    {
        static void Main(string[] args)
        {
          var r=  EMMapper.Map<StudentDemo, Student>(new StudentDemo() { Age = 4, Name = "jinyu" });
            Console.WriteLine(r);
        }
    }
}
