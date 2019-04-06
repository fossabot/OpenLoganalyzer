﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenLoganalyzer.Core.Extensions
{
    public static class StringExtension
    {
        public static string GetTranslated(this string target)
        {
            try
            {
                target = Properties.Resources.ResourceManager.GetString(target);
            }
            catch (Exception)
            {
                return target;
            }
            
            if (target == null)
            {
                target = string.Empty;
            }

            return target;
        }
    }
}
