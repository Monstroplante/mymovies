using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace MyMovies.Core.Test
{
    [TestFixture]
    public class TestMovie
    {
        [Test]
        public void TestHashCode()
        {
            var a = new Movie();
            var b = new Movie();
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());

            Action<PropertyInfo, Object, Object, bool> testSimpleProperty = (p, v1, v2, supportNull) => {
                var message = "property: " + p.Name;

                SetProperty(a, p, v1);
                SetProperty(b, p, v2);
                Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode(), message);

                SetProperty(b, p, v1);
                Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), message);

                if(!supportNull)
                    return;

                SetProperty(b, p, null);
                Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode(), message);

                SetProperty(a, p, null);
                Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), message);
            };

            foreach(var p in typeof(Movie).GetProperties())
            {
                if(!p.CanRead || !p.CanWrite)
                    continue;
                var message = "property: " + p.Name;

                if(p.PropertyType == typeof(String))
                {
                    testSimpleProperty(p, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true);
                }
                else if (p.PropertyType == typeof(int?))
                {
                    var r = new Random();
                    testSimpleProperty(p, r.Next(int.MinValue, int.MaxValue), r.Next(int.MinValue, int.MaxValue), true);
                }
                else if (p.PropertyType == typeof(double?))
                {
                    var r = new Random();
                    testSimpleProperty(p, r.NextDouble(), r.NextDouble(), true);
                }
                else if (p.PropertyType == typeof(DateTime))
                {
                    var r = new Random();
                    testSimpleProperty(p,
                        DateTime.Now.AddMilliseconds(r.Next(int.MinValue, int.MaxValue)),
                        DateTime.Now.AddMilliseconds(r.Next(int.MinValue, int.MaxValue)),
                        false);
                }
                else if (p.PropertyType == typeof(List<String>))
                {
                    var s1 = Guid.NewGuid().ToString();
                    var s2 = Guid.NewGuid().ToString();

                    var la = (List<String>)p.GetGetMethod().Invoke(a, new object[0]);
                    var lb = (List<String>)p.GetGetMethod().Invoke(b, new object[0]);

                    la.Add(s1);
                    Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode(), message);
                    lb.Add(s1);
                    Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), message);


                    la.Add(s1);
                    lb.Add(s2);
                    Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode(), message);

                    la.Add(s2);
                    lb.Add(s1);
                    Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode(), message);

                    la.Sort();
                    lb.Sort();
                    Assert.AreEqual(a.GetHashCode(), b.GetHashCode(), message);
                }
                else
                {
                    throw new Exception("Type " + p.PropertyType + " is not tested. " + message);
                }
            }

            var field = typeof (Movie).GetFields().FirstOrDefault();
            if(field != null)
                throw new Exception("Fields are not tested. Field name: " + field.Name);
        }

        public static void SetProperty(Object target, PropertyInfo property, params Object[] args)
        {
            property.GetSetMethod().Invoke(target, args ?? new[]{(object)null});
        }
    }
}
