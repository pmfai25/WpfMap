using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapWpfMVVM.Converters
{
    public class EventConvert : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            return new Tuple<object,object>(value,parameter);
        }
    }
}
