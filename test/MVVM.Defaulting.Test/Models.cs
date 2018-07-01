using MVVM.Defaulting.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Defaulting.Test
{
    class Alpha : BindableBase
    {
        #region NotifyProperty Beta1
        private Beta _Beta1;

        public Beta Beta1
        {
            get { return _Beta1; }
            set { SetProperty(ref _Beta1, value, () => Beta1); }
        }
        #endregion

        #region NotifyProperty Beta2
        private Beta _Beta2;

        public Beta Beta2
        {
            get { return _Beta2; }
            set { SetProperty(ref _Beta2, value, () => Beta2); }
        }
        #endregion

        #region NotifyProperty Name
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value, () => Name); }
        }
        #endregion
        
        public Alpha()
        {
            InitMembers();
            BubblePropertyChanged(() => Beta1.Gamma1, () => Beta1.Gamma2);
        }

        private void InitMembers()
        {
            Beta1 = new Beta
            {
                Gamma1 = new Gamma { Name = Guid.NewGuid().ToString() },
                Gamma2 = new Gamma { Name = Guid.NewGuid().ToString() }
            };

            Beta2 = new Beta
            {
                Gamma1 = new Gamma { Name = Guid.NewGuid().ToString() },
                Gamma2 = new Gamma { Name = Guid.NewGuid().ToString() }
            };
        }
    }

    class Beta : BindableBase
    {
        #region NotifyProperty Gamma1
        private Gamma _Gamma1;

        public Gamma Gamma1
        {
            get { return _Gamma1; }
            set { SetProperty(ref _Gamma1, value, () => Gamma1); }
        }
        #endregion

        #region NotifyProperty Gamma2
        private Gamma _Gamma2;

        public Gamma Gamma2
        {
            get { return _Gamma2; }
            set { SetProperty(ref _Gamma2, value, () => Gamma2); }
        }
        #endregion
    }

    class Gamma : BindableBase
    {
        #region NotifyProperty Name
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value, () => Name); }
        }
        #endregion

    }
}
