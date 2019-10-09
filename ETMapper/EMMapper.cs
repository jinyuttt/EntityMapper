#region   文件版本注释

/************************************************************************
*CLR版本  ：4.0.30319.42000
*机器名称 ：DESKTOP-1IBOINI
*项目名称 ：EntityMapper
*项目描述 ：
*命名空间 ：EntityMapper
*文件名称 ：EMMapper.cs
*版本号   :   2019|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2019. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion



using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Collections.Concurrent;

namespace EntityMapper
{
    public delegate T ConvertMapper<V, T>(V obj);

    /*----------------------------------------------------------------
    * 功能描述 ：EMMapper
    * 作    者 ：jinyu  
    * 创建时间 ：2019/10/7 11:21:26
    * 更新时间 ：2019/10/7 11:21:26
    ----------------------------------------------------------------*/
    public class EMMapper
    {
        private static readonly ConcurrentDictionary<string, Delegate> dic = new ConcurrentDictionary<string, Delegate>();
        private static AssemblyName assemblyName;
        private static AssemblyBuilder assemblyBuilder;
        private static ModuleBuilder moduleBuilder;
        private static TypeBuilder typeBuilder;
        private static readonly object lock_obj = new object();
       

        /// <summary>
        /// 转换类型
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Map<V,T>(V obj)
        {
            string name = typeof(V).FullName + "To" + typeof(V).FullName;
            name = name.Replace(".", "");
            Delegate v = null;
            if (dic.TryGetValue(name, out v))
            {
                return (T)v.DynamicInvoke(obj);
            }
            else
            {
                lock (lock_obj)
                {
                    Create<V, T>(obj, name);
                }
                return (T)dic[name].DynamicInvoke(obj);
            }
           
       
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private static void Init()
        {
             assemblyName = new AssemblyName("Kitty");
             assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
             moduleBuilder = assemblyBuilder.DefineDynamicModule("KittyModule", "KittyEmit.dll");
             typeBuilder = moduleBuilder.DefineType("MapKitty", TypeAttributes.Public);
        }

        /// <summary>
        /// 生成动态方法
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        private static void Create<V,T>(V obj,string name)
        {
          if(typeBuilder==null)
            {
                Init();
            }
            var methodBuilder = typeBuilder.DefineMethod(
           name,
          MethodAttributes.Public | MethodAttributes.Static,
         typeof(T),
         new Type[] { typeof(V)});
            var il = methodBuilder.GetILGenerator();

                 il.DeclareLocal(typeof(T)); // create a local variable
                 il.DeclareLocal(typeof(T)); // create a local variable
               var lb= il.DefineLabel();
                il.Emit(OpCodes.Newobj,typeof(T).GetConstructors()[0]);
                il.Emit(OpCodes.Stloc_0);
            //
           
            var proprtys = typeof(V).GetRuntimeProperties();
            var vp = typeof(T).GetRuntimeProperties();
            var vf = typeof(T).GetRuntimeFields();
            foreach (var p in proprtys)
            {
                var r=   vp.Where(X => X.Name == p.Name).ToList();
                if(r.Count>0)
                {
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldarg_0);
                    if (p.PropertyType.IsAssignableFrom(r[0].PropertyType))
                    {
                      
                        il.Emit(OpCodes.Call, p.GetGetMethod());
                        il.Emit(OpCodes.Call, r[0].GetSetMethod());
                    }
                    else
                    {
                      
                        il.Emit(OpCodes.Call, p.GetGetMethod());
                        il.Emit(OpCodes.Call, typeof(EMMapper).GetMethod("ConvertType").MakeGenericMethod(new Type[] {p.PropertyType,r[0].PropertyType }));
                        il.Emit(OpCodes.Call, r[0].GetSetMethod());
                    }
                }
                else
                {
                    var fields = vf.Where(X => X.Name.ToLower() == p.Name.ToLower()).ToList();
                    if(fields.Count>0)
                    {
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldarg_0);
                        if (p.PropertyType.IsAssignableFrom(fields[0].FieldType))
                        {
                           
                            il.Emit(OpCodes.Call, p.GetGetMethod());
                            il.Emit(OpCodes.Stfld, fields[0]);
                        }
                        else
                        {
                        
                            il.Emit(OpCodes.Call, p.GetGetMethod());
                            il.Emit(OpCodes.Call, typeof(EMMapper).GetMethod("ConvertType").MakeGenericMethod(new Type[] { p.PropertyType, fields[0].FieldType }));
                            il.Emit(OpCodes.Stfld, fields[0]);
                        }
                    }
                      
                }
            }
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Br_S,lb);
            il.MarkLabel(lb);
            il.Emit(OpCodes.Ldloc_1);
            
            il.Emit(OpCodes.Ret);
            var result=typeBuilder.CreateType();
           // assemblyBuilder.Save("KittyEmit.dll");
            dic[name] = Delegate.CreateDelegate(typeof(ConvertMapper<V, T>), result.GetMethod(name));
        }

        /// <summary>
        /// 转换类型
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ConvertType<V, T>(V obj) 
        {
            if(typeof(V).IsClass)
            {
                object k = obj;
                if(k==null)
                {
                    return default(T);
                }
            }
            Object tmp = Convert.ChangeType(obj, typeof(T));
            T ret = default(T);
            if (tmp != null)
            {
                ret = (T)tmp;
            }
            return ret;
        }
    }
}
