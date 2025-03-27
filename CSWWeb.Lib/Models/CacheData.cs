using System.Net;
using System.Security.Principal;

namespace CSWWeb.Lib.Model
{
    /// <summary>
    /// cache data model
    /// </summary>
    public class CacheData
    {
        private readonly Dictionary<Type, object> _dataStore = new();

        /// <summary>
        /// 取得指定類型的 List<T>
        /// </summary>
        public List<T> GetList<T>()
        {
            if (_dataStore.TryGetValue(typeof(T), out var list))
            {
                return (List<T>)list;
            }
            var newList = new List<T>();
            _dataStore[typeof(T)] = newList;
            return newList;
        }

        /// <summary>
        /// 設定指定類型的 List<T>
        /// </summary>
        public void SetList<T>(List<T> list)
        {
            _dataStore[typeof(T)] = list;
        }

        /// <summary>
        /// 追加資料到指定類型的 List<T>
        /// </summary>
        public void AddItem<T>(T item)
        {
            var list = GetList<T>();
            list.Add(item);
        }
    }
}
