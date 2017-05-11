using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Song_Models;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;

namespace Song_Single
{

    /// <summary>
    /// 实现EF普通操作的封装
    /// </summary>
    class EFHelpler<T> where T : class
    {
       Song_2017Entities  m = new Song_2017Entities();

        /// <summary>
        /// 实体新增 EFHelpler<User> helper = new EFHelpler<User>(); helper.add(Userlist.ToArray());
        /// </summary>
        /// <param name="model"></param>
        public  void add(params T[] paramList)
        {
            foreach (var model in paramList)
            {
                m.Entry<T>(model).State = EntityState.Added;
            }
            m.SaveChanges();
        }
        /// <summary>
        /// 实体查询 var query = helper.getSearchList(item => item.UserName.Contains("keso"));
        /// </summary>
        public  IEnumerable<T> getSearchList(Expression<Func<T, bool>> where)
        {
            return m.Set<T>().Where(where);
        }
        /// <summary>
        /// 实体分页查询 var queryMulti = helper.getSearchListByPage<int>(item => item.UserName.Contains("FlyElehant"), order => order.PersonID, 2, 1);
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public  IEnumerable<T> getSearchListByPage<TKey>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderBy, int pageSize, int pageIndex)
        {
            return m.Set<T>().Where(where).OrderByDescending(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
        /// <summary>
        /// 实体删除 helper.getSearchList(item => item.UserName.Contains("keso"));helper.delete(query.ToArray());
        /// </summary>
        /// <param name="model"></param>
        public  void delete(params T[] paramList)
        {
            foreach (var model in paramList)
            {
                m.Entry<T>(model).State = EntityState.Deleted;
            }
            m.SaveChanges();
        }
        /// <summary>
        /// 按照条件修改数据 Dictionary<string,object> dic=new Dictionary<string,object>();dic.Add("PersonID",2); dic.Add("UserName","keso");helper.update(item => item.UserName.Contains("keso"), dic);
        /// </summary>
        /// <param name="where"></param>
        /// <param name="dic"></param>
        public  void update(Expression<Func<T, bool>> where, Dictionary<string, object> dic)
        {
            IEnumerable<T> result = m.Set<T>().Where(where).ToList(); //找到修改的数据列表【原数据】
            Type type = typeof(T); //找到数据对象类型
            List<PropertyInfo> propertyList = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).ToList(); //利用反射 修改赋值对象集合【新数据】
            //遍历结果集 并对比相同列进行赋值
            foreach (T entity in result)
            {
                foreach (PropertyInfo propertyInfo in propertyList)
                {
                    string propertyName = propertyInfo.Name;
                    if (dic.ContainsKey(propertyName))
                    {
                        //设置值
                        propertyInfo.SetValue(entity, dic[propertyName], null);
                    }
                }
            }

            m.SaveChanges();
        }


        /// <summary>
        /// 分页拓展 json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="select"></param>
        /// <param name="setting"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>

        //public static string SerializeQuery<T>(IQueryable<T> query, Func<T, List<string>> select, GridSettings setting, JsonRequestBehavior behavior)
        //{
        //    var json = new JsonResult();
        //    json.JsonRequestBehavior = behavior;
        //    //查询条件过滤
        //    if (setting.IsSearch && setting.Where.rules != null)
        //    {
        //        if (setting.Where.groupOp == "AND") // 且的关系，多个查询条件都满足
        //        {
        //            foreach (var rule in setting.Where.rules)
        //            {
        //                query = query.Where(rule.field, rule.data, rule.op);
        //            }
        //        }
        //        else if (setting.Where.groupOp == "OR") // 或的关系，至少满足其中一个条件
        //        {
        //            //var temp = (new List<T>()).AsQueryable();
        //            IQueryable<T> temp = null;
        //            foreach (var rule in setting.Where.rules)
        //            {
        //                if (temp != null)
        //                {
        //                    var t = query.Where(rule.field, rule.data, rule.op);
        //                    temp = temp.Concat(t);//连接查询条件
        //                }
        //                else
        //                { temp = query.Where(rule.field, rule.data, rule.op); }
        //            }
        //            query = temp.Distinct();//去掉重复
        //        }
        //    }
        //    //排序
        //    query = query.OrderBy(setting.SortColumn, setting.SortOrder);

        //    //统计行数
        //    var totalCount = query.Count();

        //    //分页
        //    var data = query.Skip((setting.PageIndex - 1) * setting.PageSize).Take(setting.PageSize);

        //    //格式转化
        //    json.Data = new
        //    {
        //        page = setting.PageIndex,
        //        records = totalCount,
        //        total = (int)Math.Ceiling((double)totalCount / setting.PageSize),
        //        rows = data.ToList().Select((d, id) => new { id, cell = select(d) }).ToArray()
        //    };
        //    return json;
        //}

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity"></param>
        public  void Add(T entity)
        {
            m.Set<T>().Add(entity);
            m.SaveChanges();
        }
        private  object obj = new object();
        /// <summary>
        /// 改变实体
        /// </summary>
        /// <param name="entity"></param>
        public  void Change(T entity)
        {
            m.Entry(entity);
            lock (obj)
            {
                m.SaveChanges();
            }

        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public  T Get(object id)
        {
            return m.Set<T>().Find(id);
        }

        /// <summary>
        /// 泛型获取列表
        /// </summary>
        /// <returns></returns>
        public  DbSet<T> GetAll()
        {
            return m.Set<T>();
        }
    }
}
