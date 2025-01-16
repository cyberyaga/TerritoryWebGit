using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace TerritoryWeb.Common
{
    public static class Properties
    {
        public static string Culture {
            get 
            {
                return Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;            
            }
        }
    }
}