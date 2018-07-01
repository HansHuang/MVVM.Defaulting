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
    public class DefaultorTest
    {
        [TestMethod]
        public void TestMethod()
        {
            var alpha = new Alpha();
            alpha.ApplyDefaulting<Alpha, AlphaDefaultor>();

            Assert.AreEqual(null, alpha.Name);

            alpha.Beta1.Gamma1.Name = "A";
            Assert.AreEqual("A_Hans", alpha.Name);

            alpha.Beta1.Gamma1.Name = null;
            Assert.AreEqual("_Hans", alpha.Name);

            alpha.Beta1.Gamma2.Name = null;
            Assert.AreEqual(string.Empty, alpha.Name);
        }
    }

    class AlphaDefaultor : AbstaractDefaultor<Alpha>
    {
        public AlphaDefaultor()
        {
            When(s => s.Beta1.Gamma1.Name.HasText() || s.Beta1.Gamma2.Name.HasText(), () =>
            {
                RuleFor(s => s.Name).SetValue(s => s.Beta1.Gamma1.Name + "_Hans");
            }).Else(() =>
            {
                RuleFor(s => s.Name).SetValue(string.Empty);
            });

            //RuleFor(s=>s.Name).RelyOn(s=>s.Beta1.Gamma1.Name).SetValue()
            //RuleFor(s=>s.Name).When(s=>s.Beta1.Gamma1.Name.HasText()).SetValue()

            //RuleFor(s => s.Name).RelyOn(s => s.Beta1.Gamma1).SetAction((vm, prop) =>
            //{
            //    if (prop.HasText()) return;
            //    //could be some complex calcuation logic here
            //    //...
            //});
        }
    }

}
