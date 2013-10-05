using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FreeSrc.Utils.KeepTidy
{
   /// <summary>
   /// 
   /// </summary>
   /// <typeparam name="RefType"></typeparam>
   public class WeakRef<RefType> : WeakReference
      where RefType : class
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="info"></param>
      /// <param name="context"></param>
      protected WeakRef(SerializationInfo info, StreamingContext context)
         : base(info, context)
      {
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="r"></param>
      public WeakRef(RefType r)
         : base(r)
      {
      }
      /// <summary>
      /// 
      /// </summary>
      public WeakRef()
         : this(null)
      {
      }
      /// <summary>
      /// 
      /// </summary>
      public RefType RefTarget
      {
         get
         {
            return this.Target as RefType;
         }

         set
         {
            this.Target = value;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      public bool isNull
      {
         get
         {
            return !this.IsAlive;
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="w"></param>
      /// <returns></returns>
      public static implicit operator RefType(WeakRef<RefType> w)
      {
         return w.RefTarget;
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="r"></param>
      /// <returns></returns>
      public static implicit operator WeakRef<RefType>(RefType r)
      {
         return new WeakRef<RefType>(r);
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="w"></param>
      /// <param name="r"></param>
      /// <returns></returns>
      public static bool operator ==(WeakRef<RefType> w, RefType r)
      {
         if (!object.ReferenceEquals(w, null))
         {
            return w.Target == r;
         }
         else
         {
            return object.ReferenceEquals(w, r);
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="w"></param>
      /// <param name="r"></param>
      /// <returns></returns>
      public static bool operator !=(WeakRef<RefType> w, RefType r)
      {
         if (!object.ReferenceEquals(w, null))
         {
            return w.Target != r;
         }
         else
         {
            return !object.ReferenceEquals(w, r);
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="obj"></param>
      /// <returns></returns>
      public override bool Equals(object obj)
      {
         if (this.Target == null)
         {
            return base.Equals(obj);
         }
         else
         {
            WeakRef<RefType> r = obj as WeakRef<RefType>;

            if (r != null)
            {
               return this.Target.Equals(r.Target);
            }
            else
            {
               RefType r1 = obj as RefType;

               if (r1 != null)
               {
                  return this.Target.Equals(r1);
               }
               else
               {
                  return false;
               }
            }
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      public override int GetHashCode()
      {
         if (this.Target != null)
         {
            return this.Target.GetHashCode();
         }
         else
         {
            return base.GetHashCode();
         }
      }
      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
         if (this.Target != null)
         {
            return this.Target.ToString();
         }
         else
         {
            return base.ToString();
         }
      }
   }
}
