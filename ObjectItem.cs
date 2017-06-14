using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    public class ObjectItem<T, K> : Object
    {
        public T Id;
        public K Name;


        public ObjectItem(T id, K name)
        {
            this.Id = id;
            this.Name = name;
        }


        public ObjectItem()
        {

        }
        public override string ToString()
        {
            if (Id.GetType() == typeof(string))
            {
                return Id.ToString();
            }
            else if (Name.GetType() == typeof(string))
            {
                 return Name.ToString();
            }
            return Id.ToString() + Name.ToString();
        }
    }

    public class ObjectItem<T, K, V> : Object
    {
        public T Id;
        public K Name;
        public V Tag;


        public ObjectItem(T id, K name, V tag)
        {
            this.Id = id;
            this.Name = name;
            this.Tag = tag;
        }

        public ObjectItem()
        {

        }
        public override string ToString()
        {
            if (Id.GetType() == typeof(string))
            {
                return Id.ToString();
            }
            else if (Name.GetType() == typeof(string))
            {
                return Name.ToString();
            }
            
            return Id.ToString() + Name.ToString();
        }
    }


}
