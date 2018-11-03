using MVVM.Defaulting.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Defaulting.Test
{
    [TestClass]
    public class DeadLoopTest
    {
        [TestMethod]
        public void ChangedPropsAvoidDeadLoop()
        {
            var person = new Person();
            var defaultor = person.ApplyDefaulting<Person, PersonDefaultor>();
            //PreventDeadLoop default property is true

            person.FirstName = "A";

            Assert.AreEqual("A . ", person.FullName);
            Assert.AreEqual(".", person.MidName);
        }
        
        [TestMethod, ExpectedException(typeof(StackOverflowException), "Dead loop should cause stack overflow exception.")]
        public void ChangedPropsCauseDeadLoop()
        {
            var person = new Person();
            var defaultor = person.ApplyDefaulting<Person, PersonDefaultor>();
            defaultor.PreventDeadLoop = false;

            person.FirstName = "A";
        }
    }

    class PersonDefaultor: AbstaractDefaultor<Person>
    {
        public PersonDefaultor()
        {
            RuleFor(s => s.FullName).RelyOn(s => s.FirstName, s => s.LastName, s => s.MidName)
                .SetValue(s => string.Format("{0} {1} {2}", s.FirstName, s.MidName, s.LastName));

            //Generally below will cause dead loop
            RuleFor(s => s.MidName).RelyOn(s => s.FullName).SetValue(s => s.FullName.Split(' ')[1] + '.');
        }
    }
}