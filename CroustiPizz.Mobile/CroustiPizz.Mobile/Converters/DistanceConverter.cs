﻿using System;
using System.Globalization;
using CroustiPizz.Mobile.Dtos.Pizzas;
using CroustiPizz.Mobile.ViewModels;
using Xamarin.Forms;

namespace CroustiPizz.Mobile.Converters
{
    public class DistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double distance = (double) value;

            if (distance > 1000)
            {
                distance = distance / 1000;
                return distance.ToString("0.0") + " km";
            }

            return distance.ToString("0") +" m";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}