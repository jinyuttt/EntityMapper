#region   文件版本注释

/************************************************************************
*CLR版本  ：4.0.30319.42000
*机器名称 ：DESKTOP-1IBOINI
*项目名称 ：Sample
*项目描述 ：
*命名空间 ：Sample
*文件名称 ：Mapper.cs
*版本号   :   2019|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2019. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{

    /*----------------------------------------------------------------
    * 功能描述 ：Mapper
    * 作    者 ：jinyu  
    * 创建时间 ：2019/10/8 23:07:06
    * 更新时间 ：2019/10/8 23:07:06
    ----------------------------------------------------------------*/
   public class Mapper
    {
       public static Student ConvertToStudent(StudentDemo  student)
        {
            Student tmp = new Student();
            tmp.Age = student.Age;
            tmp.Name = student.Name;
            //tmp.ID = (int)student.ID;
            tmp.ID = ConvertType<string,int>(student.ID);
            tmp.kk = student.Age;
            return tmp;
        }
       
        public static T ConvertType<V, T>(V obj)
        {
            Object tmp = Convert.ChangeType(obj, typeof(T));
            T ret = default(T);
            if(tmp!=null)
            {
                ret = (T)tmp;
            }
            return ret;
        }

    }
}
