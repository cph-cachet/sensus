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
using System.Collections.Generic;

namespace SensusUI.Inputs
{
    public class ItemPickerInput : Input
    {
        private string _tipText;
        private List<string> _items;
        private Picker _picker;

        [EntryStringUiProperty("Tip Text:", true, 10)]
        public string TipText
        {
            get
            {
                return _tipText;
            }
            set
            {
                _tipText = value;
            }
        }

        [EditableListUiProperty(null, true, 11)]
        public List<string> Items
        {
            get
            {
                return _items;
            }
            // need set method so auto-binding can set the list via the EditableListUiProperty
            set
            {
                _items = value;
            }
        }

        public override bool Complete
        {
            get
            {
                return _picker != null && _picker.SelectedIndex >= 0;
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Picker";
            }
        }

        /// <summary>
        /// For JSON.NET deserialization.
        /// </summary>
        protected ItemPickerInput()
        {
            _items = new List<string>();
        }

        public ItemPickerInput(string name, string label, string tipText, List<string> items)
            : base(name, label)
        {
            _items = items;                
        }

        public override View CreateView(out Func<object> valueRetriever)
        {
            valueRetriever = null;

            if (_items.Count == 0)
                return null;
            
            _picker = new Picker
            {
                Title = _tipText,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            foreach (string item in _items)
                _picker.Items.Add(item);

            valueRetriever = new Func<object>(() => _picker.SelectedIndex >= 0 ? _picker.Items[_picker.SelectedIndex] : null);

            return new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { Label, _picker }
            };
        }
    }
}