﻿// Copyright 2014 The Rector & Visitors of the University of Virginia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Xamarin.Forms;
using SensusUI.UiProperties;

namespace SensusUI.Inputs
{
    public class NumberSliderInput : Input
    {
        private double _minimum;
        private double _maximum;
        private Slider _slider;

        [EntryDoubleUiProperty(null, true, 10)]
        public double Minimum
        {
            get
            {
                return _minimum;
            }
            set
            {
                _minimum = value;
            }
        }

        [EntryDoubleUiProperty(null, true, 11)]
        public double Maximum
        {
            get
            {
                return _maximum;
            }
            set
            {
                _maximum = value;
            }
        }

        public override bool Complete
        {
            get
            {
                return _slider != null;
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Number Slider";
            }
        }

        public NumberSliderInput(string name, string label, double minimum, double maximum)
            : base(name, label)
        {
            _minimum = minimum;
            _maximum = maximum;
        }

        public override View CreateView(out Func<object> valueRetriever)
        {
            _slider = new Slider
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Minimum = _minimum,
                Maximum = _maximum
            };

            Label sliderValueLabel = new Label
            {
                Text = _slider.Value.ToString(),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize = 20
            };

            _slider.ValueChanged += (o, e) =>
            {
                sliderValueLabel.Text = e.NewValue.ToString();
            };

            valueRetriever = new Func<object>(() => _slider.Value);

            return new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                { 
                    Label,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children = { _slider, sliderValueLabel }
                    }
                }
            };
        }
    }
}